using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry2.Actions;

namespace FSS.Omnius.Modules.Tapestry2.Services
{
    public class TapestryGenerateService
    {
        public TapestryGenerateService(DBEntities context, Application application, ModalProgressHandler<EModule> progressHandler, bool rebuild)
        {
            _rebuild = rebuild;
            _context = context;
            _application = application;
            _tapestryNameSpace = "FSS.Omnius.Modules.Tapestry2.Block";
            _usings = new string[] { "System", "System.Collections.Generic", "System.Linq", "System.Threading.Tasks", "System.Web", "FSS.Omnius.Modules.CORE", "FSS.Omnius.Modules.Tapestry2" };
            _resourceLinks = new string[] { "FSS.Omnius.Modules.dll", "Newtonsoft.Json.dll", "System.ValueTuple.dll" }.Select(r => Path.Combine(Tapestry.PathRun, r))
                .Concat(new string[] { Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location,
                                       Assembly.Load("EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089").Location,
                                       Assembly.Load("Microsoft.AspNet.Identity.EntityFramework, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").Location }).ToArray();
            _progressHandler = progressHandler;
            
            Directory.CreateDirectory(Tapestry.PathOutput);
            Directory.CreateDirectory(Tapestry.PathOutputTemp);
        }

        private string[] _usings;
        private string[] _resourceLinks;
        private string _tapestryNameSpace;
        private string _currentVersion;

        private bool _rebuild;
        private DBEntities _context;
        private Application _application;

        private HashSet<string> _varNames;
        private CodeBuilder _threadMethods;
        private string _methodName;

        private TapestryDesignerBlockCommit _currentBlockCommit;
        private TapestryDesignerWorkflowRule _currentWFRule;

        private ModalProgressHandler<EModule> _progressHandler;

        public void Generate()
        {
            _progressHandler.SetMessage("T2_block", "Create blocks", MessageType.Info);
            _progressHandler.SetMessage("T2_wfRule", "Create workflow rules", MessageType.Info);
            _progressHandler.SetMessage("T2_dll", "Generate dll", MessageType.Info);

            _currentVersion = GetCurrentVersion(_application.Name);

            GenerateCsFiles();
            GenerateDll();
        }

        /// <summary>
        /// Generate workflow cs files
        /// </summary>
        private void GenerateCsFiles()
        {
            List<TapestryDesignerBlock> designerBlocksToBuild = _rebuild
                ? _application.TapestryDesignerMetablocks.SelectMany(mb => mb.Blocks).Where(b => !b.IsDeleted).ToList()
                : _application.TapestryDesignerMetablocks.SelectMany(mb => mb.Blocks).Where(b => !b.IsDeleted && b.IsChanged).ToList();

            _progressHandler.SetMessage("T2_block", "Creating blocks", MessageType.InProgress, designerBlocksToBuild.Count(b => b.BlockCommits.Any()));
            _progressHandler.SetMessage("T2_wfRule", "Creating workflow rules", MessageType.InProgress);

            foreach (TapestryDesignerBlock designerBlock in designerBlocksToBuild)
            {
                TapestryDesignerBlockCommit blockCommit = designerBlock.BlockCommits.OrderByDescending(bc => bc.Timestamp).FirstOrDefault();
                if (blockCommit == null)
                    continue;

                try
                {
                    // init block
                    if (designerBlock.IsInitial)
                        GenerateInitBlock(blockCommit.Name.RemoveDiacritics());

                    // 
                    _currentBlockCommit = blockCommit;
                    GenerateBlock();
                    designerBlock.IsChanged = false;
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    _progressHandler.Error(
                        (ex is TapestrySyntacticOmniusException)
                            ? $"[Block:{(ex as TapestrySyntacticOmniusException).BlockName},WF:{(ex as TapestrySyntacticOmniusException).WorkflowName},WFItemId:{(ex as TapestrySyntacticOmniusException).WFItemId}] {ex.Message}"
                            : ex.Message
                    );
                    designerBlock.IsChanged = true;
                    _context.SaveChanges();
                }
                finally
                {
                    _progressHandler.IncrementProgress("T2_block");
                }
            }

            GenerateAssemblyInfoFile();

            _progressHandler.SetMessage("T2_block", "Create blocks - complete", MessageType.Success);
            _progressHandler.SetMessage("T2_wfRule", "Create workflow rules - complete", MessageType.Success, 0);
        }
        private void GenerateBlock()
        {
            string blockName = _currentBlockCommit.Name.RemoveDiacritics();
            string DisplayName = _currentBlockCommit.Name;
            string ModelName = _currentBlockCommit.ModelTableName;
            
            _threadMethods = new CodeBuilder();
            _varNames = new HashSet<string>();

            CodeBuilder result = new CodeBuilder();
            // usings
            foreach (string use in _usings)
                result.AppendLine($"using {use};");
            // class
            result.AppendLine($"namespace {Tapestry.BlockNamespace(_application.Name)}");
            result.StartBlock();
            string attribute = $"Name = \"{blockName}\", DisplayName = \"{_currentBlockCommit.Name}\"";
            if (!string.IsNullOrEmpty(_currentBlockCommit.ModelTableName))
                attribute += $", ModelTableName = \"{_currentBlockCommit.ModelTableName}\"";
            else if (!string.IsNullOrEmpty(_currentBlockCommit.AssociatedTableName))
                attribute += $", ModelTableName = \"{_currentBlockCommit.AssociatedTableName.Split(',').First()}\"";
            if (!string.IsNullOrEmpty(_currentBlockCommit.AssociatedBootstrapPageIds))
                attribute += $", BootstrapPageId = {_currentBlockCommit.AssociatedBootstrapPageIds.Split(',').First()}";
            if (!string.IsNullOrEmpty(_currentBlockCommit.AssociatedPageIds))
                attribute += $", MozaicPageId = {_currentBlockCommit.AssociatedPageIds.Split(',').Where(pId => _context.MozaicEditorPages.Find(Convert.ToInt32(pId))?.IsModal == false).First()}";
            result.AppendLine($"[Block({attribute})]");
            result.AppendLine($"public class {blockName} : {_tapestryNameSpace}");
            result.StartBlock();
            result.AppendLine($"public {blockName}(COREobject core) : base(core)");
            result.AppendLine("{ }");
            /// AppDomain
            //result.AppendLine($"~{blockName}()");
            //result.StartBlock();
            //result.AppendLine("_core.Destroy();");
            //result.EndBlock();

            // ActionRule
            _progressHandler.SetMessage("T2_wfRule", progressSteps: _currentBlockCommit.WorkflowRules.Count);
            foreach (TapestryDesignerWorkflowRule workflowRule in _currentBlockCommit.WorkflowRules)
            {
                try
                {
                    _currentWFRule = workflowRule;
                    DrainWorkflowRule(result, workflowRule);
                    _progressHandler.IncrementProgress("T2_wfRule");
                }
                catch(TapestrySyntacticOmniusException ex)
                {
                    ex.ApplicationName = _application.Name;
                    ex.BlockName = _currentBlockCommit.Name;
                    ex.WorkflowName = workflowRule.Name;
                    throw;
                }
                catch(Exception ex)
                {
                    throw new TapestrySyntacticOmniusException(ex.Message, _application.Name, _currentBlockCommit.Name, workflowRule.Name, ex);
                }
            }

            // thread method
            result.Append(_threadMethods);
            // variables
            foreach(var varName in _varNames)
            {
                result.AppendLine("[IsVariable]");
                result.AppendLine($"public object {varName} {{ get; set; }}");
            }

            result.EndBlock(); // end class
            result.EndBlock(); // end namespace

            string filePath = Path.Combine(Tapestry.PathOutputTemp, $"App_{_application.Name}_{blockName}.cs");
            File.WriteAllText(filePath, result.ToString());
        }
        private void GenerateAssemblyInfoFile()
        {
            CodeBuilder builder = new CodeBuilder();
            builder.AppendLine("using System.Reflection;");
            builder.AppendLine("using System.Runtime.CompilerServices;");
            builder.AppendLine("using System.Runtime.InteropServices;");
            builder.AppendLine();
            builder.AppendLine("// General Information about an assembly is controlled through the following ");
            builder.AppendLine("// set of attributes. Change these attribute values to modify the information");
            builder.AppendLine("// associated with an assembly.");
            builder.AppendLine($"[assembly: AssemblyTitle(\"OmniusApp: {_application.DisplayName}\")]");
            builder.AppendLine("[assembly: AssemblyDescription(\"\")]");
            builder.AppendLine("[assembly: AssemblyConfiguration(\"\")]");
            builder.AppendLine("[assembly: AssemblyCompany(\"Simplifate\")]");
            builder.AppendLine("[assembly: AssemblyProduct(\"Omnius\")]");
            builder.AppendLine("[assembly: AssemblyCopyright(\"Copyright © 2018\")]");
            builder.AppendLine("[assembly: AssemblyTrademark(\"\")]");
            builder.AppendLine("[assembly: AssemblyCulture(\"\")]");
            builder.AppendLine("");
            builder.AppendLine("// Setting ComVisible to false makes the types in this assembly not visible ");
            builder.AppendLine("// to COM components.  If you need to access a type in this assembly from ");
            builder.AppendLine("// COM, set the ComVisible attribute to true on that type.");
            builder.AppendLine("[assembly: ComVisible(false)]");
            builder.AppendLine("");
            builder.AppendLine("// The following GUID is for the ID of the typelib if this project is exposed to COM");
            builder.AppendLine("[assembly: Guid(\"d3f17b6a-a0d7-4f4f-8026-296ff8b34713\")]");
            builder.AppendLine("");
            builder.AppendLine("// Version information for an assembly consists of the following four values:");
            builder.AppendLine("//");
            builder.AppendLine("//      Major Version");
            builder.AppendLine("//      Minor Version ");
            builder.AppendLine("//      Build Number");
            builder.AppendLine("//      Revision");
            builder.AppendLine("//");
            builder.AppendLine("// You can specify all the values or you can default the Build and Revision Numbers ");
            builder.AppendLine("// by using the '*' as shown below:");
            builder.AppendLine("// [assembly: AssemblyVersion(\"1.0.*\")]");
            builder.AppendLine($"[assembly: AssemblyVersion(\"{_currentVersion}\")]");
            builder.AppendLine($"[assembly: AssemblyFileVersion(\"{_currentVersion}\")]");
            
            string filePath = Path.Combine(Tapestry.PathOutputTemp, $"App_{_application.Name}__AssemblyInfo__.cs");
            File.WriteAllText(filePath, builder.ToString());
        }
        private void GenerateInitBlock(string blockName)
        {
            CodeBuilder result = new CodeBuilder();

            foreach (string use in _usings)
                result.AppendLine($"using {use};");
            result.AppendLine($"namespace {Tapestry.BlockNamespace(_application.Name)}");
            result.StartBlock();
            result.AppendLine($"public class __INIT__ : {blockName}");
            result.StartBlock();
            result.AppendLine("public __INIT__(COREobject core) : base(core)");
            result.AppendLine("{ }");
            /// AppDomain
            //result.AppendLine("~__INIT__()");
            //result.StartBlock();
            //result.AppendLine("_core.Destroy();");
            //result.EndBlock();
            result.EndBlock();
            result.EndBlock();

            string filePath = Path.Combine(Tapestry.PathOutputTemp, $"App_{_application.Name}__INIT__.cs");
            File.WriteAllText(filePath, result.ToString());
        }
        private void DrainWorkflowRule(CodeBuilder result, TapestryDesignerWorkflowRule workflowRule)
        {
            /// INIT
            IEnumerable<TapestryDesignerWorkflowItem> startingItems = _context.TapestryDesignerWorkflowItems.Where(i => i.ParentSwimlane.ParentWorkflowRule.Id == workflowRule.Id && (i.TypeClass == "uiItem" || i.SymbolType == "circle-single" || i.SymbolType == "envelope-start" || i.SymbolType == "circle-event"));
            // swimlane has starting item
            if (startingItems.Count() != workflowRule.Swimlanes.Count || startingItems.Select(wfi => wfi.ParentSwimlaneId).Except(workflowRule.Swimlanes.Select(sw => sw.Id)).Any())
                throw new TapestrySyntacticOmniusException("Each swimlane requires one starting item!");
            // starting item should be the same
            TapestryDesignerWorkflowItem defaultStartingItem = startingItems.FirstOrDefault();
            foreach (TapestryDesignerWorkflowItem item in startingItems)
            {
                if (item.SymbolType != defaultStartingItem.SymbolType
                    || item.ComponentName != defaultStartingItem.ComponentName)
                    throw new TapestrySyntacticOmniusException("Starting items has to be same for WorkflowRule");
            }

            // method name
            _methodName = workflowRule.Name.ToUpper() == "INIT"
                ? Tapestry.MethodName(null)
                : Tapestry.MethodName(defaultStartingItem.ComponentName);

            /// start build
            result.AppendLine($"public {(_methodName == Tapestry.MethodName(null) ? "override " : "")}void {_methodName}(string[] userRoleNames, Action<int, Block> _symbolAction)");
            result.StartBlock();

            /// swimlanes
            bool firstSwimlane = true;
            foreach(TapestryDesignerSwimlane swimlane in workflowRule.Swimlanes)
            {
                // roles
                if (!firstSwimlane)
                    result.Append("else ");
                result.AppendLine($"if ({(string.IsNullOrEmpty(swimlane.Roles) ? "true" : string.Join("||", swimlane.Roles.Split(',').Select(r => $"userRoleNames.Contains(\"{r}\")")))})");
                result.StartBlock();

                /// WF
                (var item, int count) = DrainBranch(result, startingItems.Single(si => si.ParentSwimlaneId == swimlane.Id), BranchType.Method);
                if (item != null || count != 0)
                    _progressHandler.Warning("WF ends too soon");

                result.EndBlock(); // end if - roles
                firstSwimlane = false;
            }
            // has no required role
            result.AppendLine("else");
            result.AppendLine($"    throw new TapestryAuthenticationOmniusException(\"Authentication error\", \"{string.Join(",", workflowRule.Swimlanes.Select(sw => sw.Roles))}\" , string.Join(\",\", userRoleNames));");

            result.EndBlock(); // end method
        }
        private (TapestryDesignerWorkflowItem, int) DrainBranch(CodeBuilder result, TapestryDesignerWorkflowItem startItem, BranchType branchType)
        {
            /// INIT
            int branchGoesThrought = 1;
            TapestryDesignerWorkflowItem currentItem = startItem;

            /// foreach in branch
            while (currentItem != null)
            {
                try
                {
                    // join
                    int targetToCount = currentItem.TargetToConnection.Count(c => c.Source.TypeClass != "integrationItem" && c.Source.TypeClass != "templateItem");
                    if (targetToCount > branchGoesThrought)
                        return (currentItem, branchGoesThrought);
                    else
                        branchGoesThrought = 1;

                    // compile symbol
                    result.AppendLine($"_symbolAction({currentItem.Id}, this);");

                    // lock
                    if (currentItem.HasParallelLock)
                    {
                        result.AppendLine($"lock (_core.Lock({currentItem.Id}))");
                        result.StartBlock();
                    }

                    string inputVariables = currentItem.InputVariables ?? "";
                    switch (currentItem.TypeClass)
                    {
                        // action
                        // action with params
                        case "actionItem":
                            // params
                            var paramItem = currentItem.TargetToConnection.SingleOrDefault(c => c.Source.TypeClass == "integrationItem" || c.Source.TypeClass == "templateItem")?.Source;
                            if (paramItem != null)
                            {
                                switch (paramItem.TypeClass)
                                {
                                    case "integrationItem":
                                        inputVariables = $"{currentItem.InputVariables.Trim().TrimEnd(';')};WSName=s${paramItem.Label.Substring(4)}"; // remove 'WS: ' from beginning
                                        break;
                                    case "templateItem":
                                        inputVariables = $"{currentItem.InputVariables.Trim().TrimEnd(';')};Template=s${paramItem.Label}";
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }

                            }

                            // Action
                            if (!ActionManager.All.ContainsKey(currentItem.ActionId.Value))
                                throw new TapestrySyntacticOmniusException($"Action[Id:{currentItem.ActionId.Value},Name:{currentItem.Label}] not found");
                            TapestryAction action = ActionManager.All[currentItem.ActionId.Value];
                            result.AppendLine($"{ForOutput(currentItem.OutputVariables?.Trim(), action)}{action.Repository}.{action.Method.Name}(_core{string.Join("", transformInputParams(currentItem, inputVariables, action.Method).Select(p => $",{p}"))});");
                            break;
                        // target
                        case "targetItem":
                            result.AppendLine($"this.TargetName = \"{currentItem.Target.Name.RemoveDiacritics()}\";");
                            result.AppendLine("return;");
                            break;
                        // gw x
                        // gw +
                        case "symbol":
                            switch (currentItem.SymbolType)
                            {
                                case "gateway-x":
                                    branchGoesThrought--; // removes current
                                    result.AppendLine($"if ({currentItem.ConditionGroups.SingleOrDefault()?.ToString(this) ?? "true"})");
                                    result.StartBlock();
                                    (var joinItem, int returnedBranchCount) = DrainBranch(result, currentItem.SourceToConnection.SingleOrDefault(c => c.SourceSlot == 0)?.Target, branchType);
                                    branchGoesThrought += returnedBranchCount; // add if
                                    result.Else();
                                    (currentItem, returnedBranchCount) = DrainBranch(result, currentItem.SourceToConnection.SingleOrDefault(c => c.SourceSlot == 1)?.Target, branchType);
                                    branchGoesThrought += returnedBranchCount; // add else
                                    result.EndBlock();
                                    // wrong syntactic - branch has to meet, if it not end
                                    if (joinItem != null && currentItem != null && joinItem != currentItem)
                                        throw new TapestrySyntacticOmniusException($"Gateway ends on different items[{joinItem.Id}:{joinItem.Label}, {currentItem.Id}:{currentItem.Label}]");
                                    // 'else' ends with return -> continue with 'if'
                                    if (currentItem == null)
                                        currentItem = joinItem;
                                    continue;
                                case "gateway-plus":
                                    branchGoesThrought--; // removes current
                                    TapestryDesignerWorkflowItem gatewayItem = currentItem;
                                    result.AppendLine($"_ParallelRun({gatewayItem.Id}, () => ThreadMethod_{gatewayItem.Id}(_symbolAction));");
                                    (currentItem, returnedBranchCount) = DrainBranch(result, gatewayItem.SourceToConnection.Single(c => c.SourceSlot == 0).Target, branchType);
                                    branchGoesThrought += returnedBranchCount;

                                    _threadMethods.AppendLine($"private void ThreadMethod_{gatewayItem.Id}(Action<int, Block> _symbolAction)");
                                    _threadMethods.StartBlock();
                                    (joinItem, returnedBranchCount) = DrainBranch(_threadMethods, gatewayItem.SourceToConnection.Single(c => c.SourceSlot == 1).Target, BranchType.Method);
                                    branchGoesThrought += returnedBranchCount;
                                    _threadMethods.EndBlock();

                                    // wrong syntactic - branch has to meet, if it not end
                                    if (joinItem != null && currentItem != null && joinItem != currentItem)
                                        throw new TapestrySyntacticOmniusException($"Gateway ends on different items[{joinItem.Id}:{joinItem.Label}, {currentItem.Id}:{currentItem.Label}]");

                                    // merge
                                    if (joinItem != null && currentItem != null)
                                        result.AppendLine($"_WaitForParallel({gatewayItem.Id});");
                                    continue;
                                case "circle-single":
                                    // wf beginning
                                    // DONE
                                    break;
                            }
                            break;
                        case "stateItem":
                            break;
                        // foreach
                        case "virtualAction":
                            string itemName = currentItem.ParentForeach.ItemName ?? "__item__";
                            switch (currentItem.SymbolType)
                            {
                                case "foreach":
                                    if (currentItem.ParentForeach.IsParallel)
                                    {
                                        result.AppendLine($"Parallel.ForEach((IEnumerable<dynamic>){_realGlobalVarName(currentItem.ParentForeach.DataSource)}, ({itemName}) => ");
                                        result.StartBlock();
                                        _varNames.Add("__item__");
                                        DrainBranch(result, _context.TapestryDesignerWorkflowItems.Single(i => i.ParentForeachId == currentItem.ParentForeachId && i.IsForeachStart == true), BranchType.Method);
                                        _varNames.Remove("__item__");
                                        result.EndBlock(");"); // end foreach
                                    }
                                    else
                                    {
                                        result.AppendLine($"foreach (object {itemName} in (IEnumerable<dynamic>){_realGlobalVarName(currentItem.ParentForeach.DataSource)})");
                                        result.StartBlock();
                                        _varNames.Add(itemName);
                                        DrainBranch(result, _context.TapestryDesignerWorkflowItems.Single(i => i.ParentForeachId == currentItem.ParentForeachId && i.IsForeachStart == true), BranchType.ForeachLoop);
                                        _varNames.Remove(itemName);
                                        result.EndBlock(); // end foreach
                                    }
                                    break;
                            }
                            break;


                        case "uiItem": // button - wf beginning
                        case "integrationItem": // integration - param
                        case "templateItem": // email template - param
                            break;

                        case "attributeItem":
                        case "circle-single":
                        default:
                            throw new NotImplementedException();
                    }

                    // lock
                    if (currentItem.HasParallelLock)
                        result.EndBlock();

                    /// next
                    // foreach
                    if (currentItem?.IsForeachEnd == true)
                        return (null, 0);

                    // ok
                    currentItem = currentItem?.SourceToConnection.SingleOrDefault()?.Target;
                }
                catch(Exception ex)
                {
                    throw new TapestrySyntacticOmniusException(ex.Message, currentItem?.Id, ex);
                }
            }

            /// END of thread, foreach or WF
            switch (branchType)
            {
                case BranchType.ForeachLoop:
                    // compile end symbol
                    result.AppendLine($"_symbolAction(-2, this);");
                    // next
                    result.AppendLine("continue;");
                    break;
                case BranchType.Method:
                    // compile end symbol
                    result.AppendLine($"_symbolAction(-1, this);");
                    // end
                    result.AppendLine("return;");
                    break;
                default:
                    throw new Exception("Unknown branchType");
            }
            return (null, 0);
        }

        private IEnumerable<string> transformInputParams(TapestryDesignerWorkflowItem item, string inputVariables, MethodInfo method)
        {
            /// parse
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            List<string> split = inputVariables.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
            foreach (string pair in split)
            {
                string[] splittedPair = pair.Split('=');
                if (splittedPair.Length != 2)
                {
                    _progressHandler.Warning($"[Block:{_currentBlockCommit.Name},WF:{_currentWFRule.Name},WFItemId:{item.Id},Action:{method.Name}] WorkFlow item has incorrect InputVariables!");
                    continue;
                }

                string key = splittedPair[0];
                string value = splittedPair[1];

                /// dict
                if (value.Length > 1 && value[1] != '$' && value.Contains("::"))
                {
                    var newDict = new Dictionary<string, string>();
                    var newPairs = value.Split(',');
                    foreach (string newPair in newPairs)
                    {
                        var splitted = newPair.Split(':');
                        if (splitted.Length != 2)
                        {
                            _progressHandler.Warning($"[Block:{_currentBlockCommit.Name},WF:{_currentWFRule.Name},WFItemId:{item.Id},Action:{method.Name}] WorkFlow item has incorrect InputVariables for Dictionary!");
                            continue;
                        }

                        newDict.Add(splitted[0], splitted[1]);
                    }

                    // DBItem
                    if (method.GetParameters().Single(p => p.Name == key).ParameterType == typeof(Entitron.DB.DBItem))
                        value = $"new FSS.Omnius.Modules.Entitron.DB.DBItem(_core.Entitron, dict: {_dictToBuildString(item, newDict, method)})";
                    else
                        value = _dictToBuildString(item, newDict, method);
                }

                // key already exists
                if (pairs.ContainsKey(key))
                {
                    string realValue = ForInput(item, key, value, method);
                    if (pairs[key] == realValue)
                    {
                        _progressHandler.Warning($"[Block:{_currentBlockCommit.Name},WF:{_currentWFRule.Name},WFItemId:{item.Id},Name:{item.Label}] Item has variable '{key}' defined multiple times");
                        continue;
                    }
                    else
                        throw new TapestrySyntacticOmniusException($"Single variable has multiple values - WFItem[Id:{item.Id}, Name:{item.Label}, Value1:{pairs[key]}, Value2:{realValue}]");
                }

                // skip empty
                string resultParam = ForInput(item, key, value, method);
                if (resultParam != null)
                    pairs.Add(key, resultParam);
            }

            /// array
            // select params witch is array
            foreach (ParameterInfo param in method.GetParameters().Where(p => p.ParameterType.IsArray))
            {
                // param items
                var currentMatches = pairs.ToDictionary(pair => pair, pair => Regex.Match(pair.Key, RegexIsArray(param.Name))).Where(pair => pair.Value.Success);

                // count
                string[] list = currentMatches.Any()
                    ? new string[currentMatches.Max(p => Convert.ToInt32(p.Value.Groups[2].Value != "" ? p.Value.Groups[2].Value : "0") + 1)]
                    : new string[0];

                // fill list & remove from pairs
                foreach (var pair in currentMatches)
                {
                    list[Convert.ToInt32(pair.Value.Groups[2].Value != "" ? pair.Value.Groups[2].Value : "0")] = pair.Key.Value;
                    pairs.Remove(pair.Key.Key);
                }

                // add to pair
                pairs.Add(param.Name, $"new {param.ParameterType.Name} {{ {string.Join(",", list.Select(i => i != null ? (param.ParameterType.GetElementType() != typeof(object) ? $"Extend.ConvertTo<{_typeName(param.ParameterType.GetElementType())}>(_core, {i})" : i) : "null"))} }}");
            }

            /// return
            return pairs.Select(pair => $"{pair.Key}:{pair.Value}");
        }
        
        public string ForOutput(string paramNames, TapestryAction action)
        {
            if (!action.OutputVars.Any() || string.IsNullOrWhiteSpace(paramNames))
                return "";

            /// split
            Dictionary<string, string> allParams;
            try
            {
                allParams = paramNames.Split(';').Select(i => i.Split('=')).ToDictionary(i => i[1], i => i[0]);
            }
            catch(Exception ex)
            {
                throw new TapestrySyntacticOmniusException($"Wrong item output format! [{paramNames}]", ex);
            }

            /// order
            List<string> result = new List<string>();
            for(int i = 0; i < action.OutputVars.Length; i++)
            {
                string outVarName = action.OutputVars[i];
                /// value -> /dev/null
                if (!allParams.ContainsKey(outVarName))
                {
                    result.Add("_core.Data[\"\"]");
                    continue;
                }

                string globalVarName = allParams[outVarName];

                /// system
                if (globalVarName[0] == '_')
                {
                    if (globalVarName == "__ModelId__")
                    {
                        result.Add("_core.ModelId");
                        continue;
                    }

                    if (globalVarName.StartsWith("__Result["))
                    {
                        result.Add($"_core.Results[\"{globalVarName.Substring(9).TrimEnd(']')}\"]");
                        continue;
                    }

                    result.Add($"_core.Data[\"{globalVarName}\"]");
                    continue;
                }

                /// add
                string realGlobalVarName = _realGlobalVarName(globalVarName);
                if (!_varNames.Contains(realGlobalVarName))
                    _varNames.Add(realGlobalVarName);

                result.Add(realGlobalVarName);
            }

            if (result.Count == 1)
            {
                if (result.First() == "_core.ModelId")
                    return "_core.ModelId = (int)";

                return $"{result.First()} = ";
            }

            return $"({string.Join(",", result)}) = ";
        }
        public string ForInput(TapestryDesignerWorkflowItem item, string innerParamName, string paramName, MethodInfo method)
        {
            ParameterInfo originParam = method.GetParameters().SingleOrDefault(p => p.Name == innerParamName && !p.ParameterType.IsArray);
            
            if (originParam == null)
            {
                ParameterInfo arrayParam = method.GetParameters().SingleOrDefault(p => p.ParameterType.IsArray && Regex.Match(innerParamName, RegexIsArray(p.Name)).Success);
                if (arrayParam == null)
                {
                    _progressHandler.Warning($"[Block:{_currentBlockCommit.Name},WF:{_currentWFRule.Name},WFItemId:{item?.Id},Label:{item?.Label}] Method[{method.Name}] has no param[{innerParamName}], skipping...");
                    return null;
                }
            }

            return ForInput(paramName, originParam?.ParameterType);
        }
        public string ForInput(string paramName, Type type)
        {
            /// null
            if (paramName == null)
                return null;

            /// static
            if (paramName.Length >= 2 && paramName[1] == '$')
            {
                switch (paramName[0])
                {
                    case 's':
                        return $"\"{paramName.Substring(2).Replace("\"", "\\\"")}\"";
                    case 'b':
                        return Convert.ToBoolean(paramName.Substring(2)).ToString().ToLower();
                    case 'd':
                        return $"DateTime.Parse({paramName.Substring(2)})";
                    case 'f':
                    case 'c':
                        return paramName.Substring(2).Replace(',', '.');
                    // i, l
                    default:
                        return paramName.Substring(2);
                }
            }

            // split
            string[] splitted = paramName.Split(new string[] { ".", "[\"", "\"]" }, StringSplitOptions.RemoveEmptyEntries);
            string baseName = splitted[0];
            splitted = splitted.Skip(1).ToArray();

            /// system
            if (baseName == "__ModelId__")
                baseName = "_core.ModelId";
            else if (baseName == "__CORE__")
                baseName = $"_core";
            else if (baseName.StartsWith("__Result["))
                baseName = $"_core.Results[\"{baseName.Substring("__Result[".Length).TrimEnd(']')}\"]";
            /// not system, but unknown
            else if (!_varNames.Contains(_realGlobalVarName(baseName)))
                baseName = $"_core.Data[\"{baseName}\"]";

            /// chain
            baseName = _realGlobalVarName(baseName);
            paramName = splitted.Length > 0
                ? paramName = $"Extend.GetChained({baseName}, {string.Join(", ", splitted.Select(i => $"\"{i}\""))})"
                : baseName;
            
            /// var
            if (type != null && type.FullName != null && type != typeof(object))
                return $"Extend.ConvertTo<{_typeName(type)}>(_core, {paramName})";

            /// value[0] - has no type
            return paramName;
        }
        private string _typeName(Type type)
        {
            if (type.FullName.Contains('`'))
            {
                string result = type.ToString().Replace('[', '<').Replace(']', '>');
                return Regex.Replace(result, "`\\d+", "");
            }

            return type.FullName;
        }

        private string _realGlobalVarName(string varName)
        {
            // system
            if (varName[0] == '_')
                return varName;

            // other
            return $"{_methodName}__{varName}";
        }

        private string _dictToBuildString(TapestryDesignerWorkflowItem item, Dictionary<string, string> dictionary, MethodInfo method)
        {
            List<string> result = new List<string>();
            foreach (var pair in dictionary)
            {
                string resultValue = ForInput(item, pair.Key, pair.Value, method);
                if (resultValue != null)
                {
                    result.Add($"{{ {pair.Key}, {resultValue} }}");
                }
            }

            return $"new Dictionary<string, object> {{ {string.Join(",", result)}";
        }
        private static string RegexIsArray(string paramName)
        {
            return $"^{paramName}(\\[?(\\d+)\\]?)?$";
        }

        /// <summary>
        /// Compile workflow to dll file
        /// </summary>
        private void GenerateDll()
        {
            _progressHandler.SetMessage("T2_dll", "Generating dll", MessageType.InProgress);

            /// init
            string dllPath = Tapestry.FullDllName(Tapestry.PathOutput, _application.Name, _currentVersion);
            string tempDllPath = Tapestry.FullDllName(Tapestry.PathOutputTemp, _application.Name, _currentVersion);
            List<string> errors = new List<string>();
            /// delete old file
            File.Delete(tempDllPath);

            /// files to compile
            IEnumerable<string> pathGeneratedfiles = Directory.EnumerateFiles(Tapestry.PathOutputTemp).Where(p => Path.GetFileName(p).StartsWith($"App_{_application.Name}_") && p.EndsWith(".cs"));

            /// compile config
            Process compileProcess = new Process();
            compileProcess.StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(Tapestry.PathSolutionRoot, "packages", "Microsoft.Net.Compilers.2.7.0", "tools", "csc.exe"),
                Arguments = $"/t:library /debug:pdbonly /out:{tempDllPath} {string.Join(" ", _resourceLinks.Select(r => $"/r:\"{r}\""))} {string.Join(" ", pathGeneratedfiles)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            compileProcess.EnableRaisingEvents = true;
            compileProcess.OutputDataReceived += (sender, args) => { if (args.Data?.Contains("error ") ?? false) errors.Add(args.Data); else Debug.WriteLine(args.Data); };
            compileProcess.ErrorDataReceived += (sender, args) => { if (args.Data?.Contains("error ") ?? false) errors.Add(args.Data); else Debug.WriteLine(args.Data); };

            /// compile
            compileProcess.Start();
            compileProcess.BeginOutputReadLine();
            compileProcess.BeginErrorReadLine();
            compileProcess.WaitForExit();

            // OK
            if (File.Exists(tempDllPath))
            {
                /// AppDomain
                //Tapestry.UnloadAppDomain(_application.Name);
                //File.Delete(dllPath);

                File.Move(tempDllPath, dllPath);
                Tapestry.RefreshAssembly(_application.Name);

                // debug
                try
                {
                    string pdbPath = Tapestry.PdbName(dllPath);
                    string tempPdbPath = Tapestry.PdbName(tempDllPath);
                    
                    File.Move(tempPdbPath, pdbPath);
                }
                catch(Exception)
                {
                    _progressHandler.Warning("Can't override symbol file!");
                }

                _progressHandler.SetMessage("T2_dll", "Generate dll - completed", MessageType.Success);
            }
            else
                throw new OmniusMultipleException(errors.Select(e => new Exception(e)).ToList());
        }

        private static string GetCurrentVersion(string ApplicationName)
        {
            string lastVersion = Tapestry.GetAssemblyVersion(ApplicationName);
            string todayVersionBegining = DateTime.UtcNow.ToString("yyyy.MM.dd.");

            // no version today
            if (lastVersion == null || !lastVersion.StartsWith(todayVersionBegining))
                return $"{todayVersionBegining}0000";

            int lastInt = Convert.ToInt32(lastVersion.Substring(lastVersion.LastIndexOf(".") + 1));

            return $"{todayVersionBegining}{(lastInt + 1).ToString().PadLeft(4, '0')}";
        }

        public enum BranchType
        {
            Method,
            ForeachLoop
        }
    }
}
