using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitron.Entity;

namespace Tapestry
{
    public class Action
    {
        public ActionRole entity;
        public Action(ActionRole actionRule)
        {
            entity = actionRule;
        }

        public void run()
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
