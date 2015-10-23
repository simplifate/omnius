using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persona
{
    public class User
    {
        public Entitron.Entity.User _user;

        public User(Entitron.Entity.User user)
        {
            _user = user;
        }

        public bool isInGroup(string groupName)
        {
            return _user.Groups.Any(g => g.Name == groupName);
        }

        public bool canUseAction(Entitron.Entity.Action action, Entitron.Entity.DBEntities e)
        {
            var right = e.ActionRights.FirstOrDefault(ar => ar.ActionId == action.Id && _user.Groups.Contains(ar.Group));
            return right.Executable;
        }
    }
}
