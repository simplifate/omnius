using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public abstract class ActionManager
    {
        private static Dictionary<int, TapestryAction> _all;

        public static Dictionary<int, TapestryAction> All
        {
            get
            {
                if (_all == null)
                {
                    _all = new Dictionary<int, TapestryAction>();
                    List<string> errors = new List<string>();
                    foreach (Type manager in Assembly.GetAssembly(typeof(ActionManager)).GetTypes().Where(t => t.IsSubclassOf(typeof(ActionManager)) && !t.IsAbstract))
                    {
                        foreach (MethodInfo method in manager.GetMethods().Where(m => m.IsStatic))
                        {
                            ActionAttribute attribute = method.GetCustomAttribute<ActionAttribute>();
                            if (attribute == null)
                                continue;

                            if (_all.ContainsKey(attribute.Id))
                            {
                                TapestryAction first = _all[attribute.Id];
                                errors.Add($"Method duplicity [Id:{attribute.Id},Name:{attribute.Name},Repository:{manager.FullName}] X [Id:{first.Id},Name:{first.Name},Repository:{first.Repository}]");
                                continue;
                            }

                            _all.Add(attribute.Id, new TapestryAction { Id = attribute.Id, Name = attribute.Name, Repository = manager.FullName, InputVars = method.GetParameters().Select(p => p.Name).Where(p => p != "core").ToArray(), OutputVars = attribute.ReturnValues, Method = method });
                        }
                    }

                    if (errors.Any())
                        throw new Exception(string.Join("<br/>", errors));
                }

                return _all;
            }
        }
    }

    public class TapestryAction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Repository { get; set; }
        public string[] InputVars { get; set; }
        public string[] OutputVars { get; set; }
        public MethodInfo Method { get; set; }
    }
}
