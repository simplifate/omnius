using NCrontab;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
                Modules.Watchtower.OmniusInfo.Log($"Task[{Id}] start", Modules.Watchtower.OmniusLogSource.Cortex, Application);
                LastStartTask = DateTime.UtcNow;
                DBEntities.instance.SaveChanges();
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

        private void Run()
        {
            try
            {
                var core = new Modules.CORE.CORE();
                var context = DBEntities.instance;
                core.User = context.Users.Single(u => u.UserName == "scheduler");
                var block = context.Blocks.Single(b => b.WorkFlow.ApplicationId == ApplicationId && b.Name == BlockName);

                var tapestry = new Modules.Tapestry.Tapestry(core);
                var result = tapestry.innerRun(core.User, block, Executor ?? "INIT", ModelId ?? -1, null, -1);

                if (result.Item1.Type != Modules.Tapestry.ActionResultType.Error)
                {
                    LastEndTask = DateTime.UtcNow;
                    DBEntities.instance.SaveChanges();
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

        public static void StartAll()
        {
            foreach (CrontabTask task in DBEntities.instance.CrontabTask.Where(ct => !ct.IsDeleted && ct.IsActive))
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
            return DBEntities.instance.CrontabTask.Count(ct => !ct.IsDeleted);
        }

        [NotMapped]
        private static Dictionary<int, Thread> _threads = new Dictionary<int, Thread>();
    }
}
