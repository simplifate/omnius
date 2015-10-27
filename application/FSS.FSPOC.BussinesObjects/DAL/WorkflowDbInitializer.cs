using System.Collections.Generic;
using System.Data.Entity;
using FSS.FSPOC.BussinesObjects.Entities.Actions;

namespace FSS.FSPOC.BussinesObjects.DAL
{
    public class WorkflowDbInitializer : DropCreateDatabaseIfModelChanges<OmniusDbContext>
    {
        protected override void Seed(OmniusDbContext context)
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
                    Name = "Save entity"
                },
                new Action
                {
                    ActionCategory = category,
                    Name = "Send email"
                },
                new Action
                {
                    ActionCategory = category,
                    Name = "Move to another page"
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
                    Order=1,
                    ActionRule = actionRule[0],
                    Action = actions[0],
                    
                },
                new ActionActionRule
                {
                    Order= 2,
                    ActionRule = actionRule[0],
                    Action = actions[2],
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