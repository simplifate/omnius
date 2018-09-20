using FSS.Omnius.Modules.CORE;
using NCrontab;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace FSS.Omnius.Modules.Entitron.Entity.Cortex
{
    public partial class CrontabTask
    {
        public void Start()
        {
            if (!IsActive)
                return;

            _threads[Id] = new Thread(new ThreadStart(Loop));
            _threads[Id].Start();
        }

        public void End()
        {
            if (!_threads.ContainsKey(Id))
                return;

            _threads[Id].Abort();
        }

        private void Loop()
        {
            while (true)
            {
                /// Sleep
                TimeSpan sleepTime = GetSleepTime();
                // negative -> never again
                if (sleepTime < TimeSpan.Zero)
                    return;
                // sleep
                Thread.Sleep(sleepTime);

                /// Run
                lock(_lock)
                {
                    Modules.Watchtower.OmniusInfo.Log($"Task[{Id}] start", Modules.Watchtower.OmniusLogSource.Cortex, Application);
                    LastStartTask = DateTime.UtcNow;
                    COREobject.i.Context.SaveChanges();

                    if (ScheduleStart == ScheduleStartAt.taskStart)
                    {
                        Thread thread = new Thread(new ThreadStart(Run));
                        thread.Start();
                    }
                    else
                    {
                        Run();
                    }
                }
            }
        }

        private void Run()
        {
            try
            {
                // System - call method
                if (Application.IsSystem)
                {
                    string typeName = BlockName;
                    string[] splitted = Executor.Split('/');
                    /// Post/string:ahoj;int:4/string:aho
                    string methodName = splitted[0];
                    object[] objectArguments = splitted.Length < 2
                        ? new object[0]
                        : getArguments(splitted[1]);
                    object[] callArguments = splitted.Length < 3
                        ? new object[0]
                        : getArguments(splitted[2]);

                    Type type = Assembly.GetCallingAssembly().GetType(typeName);
                    MethodInfo method = type.GetMethod(methodName);

                    if (method.IsStatic)
                        method.Invoke(null, callArguments);
                    else
                        method.Invoke(Activator.CreateInstance(type, objectArguments), callArguments);

                    return;
                }

                var core = COREobject.i;
                core.User = core.Context.Users.Single(u => u.UserName == "system");
                core.Application = core.Context.Applications.Find(ApplicationId);
                var block = core.Context.Blocks.Single(b => b.WorkFlow.ApplicationId == ApplicationId && b.Name.ToLower() == BlockName.ToLower());

                var tapestry = new Modules.Tapestry.Tapestry(core);
                var result = tapestry.innerRun(block, Executor ?? "INIT", ModelId ?? -1, null, -1);

                if (result.Item1.Type != MessageType.Error)
                {
                    LastEndTask = DateTime.UtcNow;
                    COREobject.i.Context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                Modules.Watchtower.OmniusException.Log(ex, source: Modules.Watchtower.OmniusLogSource.Cortex, application: Application);
            }
        }

        private TimeSpan GetSleepTime()
        {
            try
            {
                /// after
                if (Schedule.StartsWith("after "))
                {
                    DateTime now = DateTime.UtcNow;
                    DateTime next = now;

                    foreach (string part in Schedule.Substring("after ".Length).Split(' '))
                    {
                        var match = Regex.Match(part, "(\\d+)(\\D+)");
                        string unit = match.Groups[2].Value;
                        int count = Convert.ToInt32(match.Groups[1].Value);

                        if (part.EndsWith("sec") || part.EndsWith("s"))
                            next = next.AddSeconds(count);

                        else if (part.EndsWith("min") || part.EndsWith("m"))
                            next = next.AddMinutes(count);

                        else if (part.EndsWith("hour") || part.EndsWith("h"))
                            next = next.AddHours(count);

                        else if (part.EndsWith("day") || part.EndsWith("d"))
                            next = next.AddDays(count);

                        else if (part.EndsWith("week") || part.EndsWith("w"))
                            next = next.AddDays(count * 7);

                        else if (part.EndsWith("month") || part.EndsWith("M"))
                            next = next.AddMonths(count);

                        else if (part.EndsWith("year") || part.EndsWith("y"))
                            next = next.AddYears(count);
                    }
                    
                    return next - now;
                }

                /// At
                if (Schedule.StartsWith("at "))
                {
                    return
                        DateTime.Parse(Schedule.Substring("at ".Length)) - DateTime.UtcNow;
                }

                /// crontab
                else
                {
                    return
                        CrontabSchedule.Parse(Schedule).GetNextOccurrence(DateTime.UtcNow) - DateTime.UtcNow;
                }
            }
            catch(Exception ex)
            {
                Modules.Watchtower.OmniusException.Log(ex, Modules.Watchtower.OmniusLogSource.Cortex, Application);
                return TimeSpan.FromSeconds(-1);
            }
        }
        
        /// <summary>
        /// int:5;string:ahoj
        /// </summary>
        private object[] getArguments(string argName)
        {
            List<object> result = new List<object>();

            foreach (string arg in argName.Split(';'))
            {
                var splitted = arg.Split(':');
                if (splitted.Length != 2)
                    throw new InvalidOperationException("");

                switch(splitted[0])
                {
                    case "int":
                        result.Add(Convert.ToInt32(splitted[1]));
                        break;
                    case "string":
                        result.Add(splitted[1]);
                        break;
                    case "char":
                        result.Add(splitted[1][0]);
                        break;
                    case "double":
                        result.Add(Convert.ToDouble(splitted[1]));
                        break;
                }
            }

            return result.ToArray();
        }

        private static object _lock = new object();

        public static void StartAll()
        {
            var tasks = COREobject.i.Context.CrontabTask.Where(ct => !ct.IsDeleted && ct.IsActive).ToList();
            foreach (CrontabTask task in tasks)
            {
                task.Start();
            }
        }
        public static int CountRunning()
        {
            return _threads.Count(t => t.Value.IsAlive);
        }
        public static int CountAll()
        {
            return COREobject.i.Context.CrontabTask.Count(ct => !ct.IsDeleted);
        }

        [NotMapped]
        private static Dictionary<int, Thread> _threads = new Dictionary<int, Thread>();
    }
}
