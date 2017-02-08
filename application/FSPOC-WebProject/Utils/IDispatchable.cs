using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Utils
{
    public class BasicDispatchableObject
    {
        public event EventHandler<BasicDispatchArgs> OnDispatch;

        protected void Dispatch(object data)
        {
            OnDispatch(this, new BasicDispatchArgs(data));
        }
    }

    public class BasicDispatchArgs : EventArgs
    {
        public object Data;

        public BasicDispatchArgs(object data)
        {
            this.Data = data;
        }
    }
}
