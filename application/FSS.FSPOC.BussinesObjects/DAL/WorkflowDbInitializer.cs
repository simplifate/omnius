using System.Collections.Generic;
using System.Data.Entity;
using FSS.Omnius.Entitron.Entity.Tapestry;
using FSS.Omnius.Entitron.Entity;

namespace FSS.Omnius.BussinesObjects.DAL
{
    public class WorkflowDbInitializer : DropCreateDatabaseIfModelChanges<DBEntities>
    {
        protected override void Seed(DBEntities context)
        {

            var category = new ActionCategory
            {
                Name = "Default"
            };

            context.ActionCategories.Add(category);
            context.SaveChanges();
            var actions = new List<Action>
            {
                new Action
                {
                    ActionCategory = category,
                    Name = "Check Configuration Settings",
                    IdentifierAction = 1
                },
                new Action
                {
                    ActionCategory = category,
                    Name = "Send email",
                    IdentifierAction = 100

                },
                new Action
                {
                    ActionCategory = category,
                    Name = "Move to another page",
                    IdentifierAction = 101
                }
            };
            actions.ForEach(a=> context.Actions.Add(a));
            context.SaveChanges();
            var actionRule = new List<ActionRule>
            {
                new ActionRule
                {
                    Name = "Save entity"
                },
                new ActionRule
                {
                    Name = "Save entity with send email"
                }
            };
            actionRule.ForEach(a=> context.ActionRules.Add(a));
            context.SaveChanges();

            var actionActionRules = new List<ActionActionRule>
            {
                new ActionActionRule
                {
                    Order      = 1,
                    ActionRule = actionRule[0],
                    Action     = actions[0],
                    
                },
                new ActionActionRule
                {
                    Order= 2,
                    ActionRule = actionRule[0],
                    Action = actions[1],
                },
                new ActionActionRule
                {
                    Order= 1,
                    ActionRule = actionRule[1],
                    Action = actions[0],
                },
                new ActionActionRule
                {
                    Order= 2,
                    ActionRule = actionRule[1],
                    Action = actions[1],
                },
                new ActionActionRule
                {
                    Order= 3,
                    ActionRule = actionRule[1],
                    Action = actions[2]
                }
            };
            actionActionRules.ForEach(a=> context.ActionActionRules.Add(a));
            context.SaveChanges();
        }
    }
}