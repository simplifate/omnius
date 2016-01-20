using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry.Service
{
    public class TapestryGeneratorService
    {
        private CORE.CORE _core;
        private Dictionary<int, Block> _blockMapping = new Dictionary<int, Block>();

        public void GenerateTapestry(CORE.CORE core, TapestryDesignerApp designApp)
        {
            _core = core;
            _core.Entitron.AppName = designApp.Name;
            DBEntities e = core.Entitron.GetStaticTables();
            
            WorkFlow wf = saveMetaBlock(e, designApp.RootMetablock, true);
            e.WorkFlows.Add(wf);

            e.SaveChanges();
        }

        private WorkFlow saveMetaBlock(DBEntities e, TapestryDesignerMetablock block, bool init = false)
        {
            WorkFlow resultWF = new WorkFlow
            {
                ApplicationId = _core.Entitron.AppId,
                Type = init ? e.WorkFlowTypes.Single(t => t.Name == "Init") : e.WorkFlowTypes.Single(t => t.Name == "Partial"),
            };

            // child meta block
            foreach (TapestryDesignerMetablock childMetaBlock in block.Metablocks)
            {
                WorkFlow wf = saveMetaBlock(e, childMetaBlock);
                resultWF.Children.Add(wf);
            }

            // child block
            foreach(TapestryDesignerBlock childBlock in block.Blocks)
            {
                Block bl = saveBlock(e, childBlock);
                resultWF.Blocks.Add(bl);

                _blockMapping.Add(childBlock.Id, bl);
            }

            // rules
            foreach (TapestryDesignerRule rule in e.TapestryDesignerRules.Where(r => _blockMapping.Keys.Contains(r.ParentBlockCommit.ParentBlock.Id)))
            {
                saveActionRule(e, rule);
            }

            // DONE :)
            return resultWF;
        }

        private Block saveBlock(DBEntities e, TapestryDesignerBlock block)
        {
            Block resultBlock = new Block
            {
                Name = block.Name,
                ModelName = block.AssociatedTableName
            };
            
            return resultBlock;
        }

        private void saveActionRule(DBEntities e, TapestryDesignerRule rule)
        {
            //-- attribute --//
            TapestryDesignerItem view = rule.Items.FirstOrDefault(i => i.TypeClass == "view");
            if (view != null) // TODO: opravit. tohle sežere všechny rules
            {
                AttributeRule attrRule = new AttributeRule
                {
                    InputName = view.Label,
                    AttributeName = rule.Items.Single(i => i.Id != view.Id).Label,
                    // TODO: datatype
                    Block = _blockMapping[rule.ParentBlockCommit.ParentBlock.Id]
                };
                e.AttributeRules.Add(attrRule);
                return;
            }

            //-- Action Rule --//
            // find virtual block
            Dictionary<TapestryDesignerOperator, Block> operatorMapping = new Dictionary<TapestryDesignerOperator, Block>();
            foreach(TapestryDesignerOperator opr in rule.Operators)
            {
                // TODO
            }
            // create actionRules
            // TODO
        }
    }
}
