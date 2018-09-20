using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry2.Services
{
    public class CodeBuilder
    {
        public CodeBuilder()
        {
            _builder = new StringBuilder();
            _currentPadding = 0;
            SinglePadding = 4;
        }

        private StringBuilder _builder;
        private int _currentPadding;

        public int SinglePadding { get; set; }

        public void Append(string value)
        {
            _builder.Append(value);
        }
        public void Append(CodeBuilder codeBuilder)
        {
            foreach (string line in codeBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                AppendLine(line);
        }

        public void AppendLine()
        {
            _builder.AppendLine();
        }
        public void AppendLine(string line)
        {
            _builder.AppendLine(new string(' ', _currentPadding * SinglePadding) + line);
        }

        public void StartBlock()
        {
            _builder.AppendLine(new string(' ', _currentPadding * SinglePadding) +  "{");
            _currentPadding++;
        }
        public void EndBlock(string endWith = "")
        {
            _currentPadding--;
            _builder.AppendLine(new string(' ', _currentPadding * SinglePadding) + $"}}{endWith}");
        }
        public void Else()
        {
            _builder.AppendLine(new string(' ', (_currentPadding - 1) * SinglePadding) + "} else {");
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
