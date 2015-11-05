using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE
{
    public abstract class Module
    {
        public string Name { get; }

        public Module(string name)
        {
            Name = name;
        }
    }

    public abstract class RunableModule : Module
    {
        public RunableModule(string name) : base(name)
        {
        }
        public abstract void run(string url);
        public abstract string GetHtmlOutput();
        public abstract string GetJsonOutput();
        public abstract string GetMailOutput();
    }
}
