using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry2
{
    using CORE;
    using Entitron.Entity;
    using System.Text.RegularExpressions;
    
    public class Tapestry : IModule
    {
        private COREobject _core;

        public Tapestry(COREobject core)
        {
            _core = core;
        }
        
        public JObject jsonRun(string blockName, string executor = null, Action<int, Block> symbolAction = null)
        {
            innerRun(blockName, executor, symbolAction);
            return JObject.FromObject(_core.Results);
        }

        /// <summary>
        /// Loads Tapestry DLL and run it
        /// </summary>
        /// <returns>Target block name</returns>
        public string innerRun(string blockName, string executor = null, Action<int, Block> symbolAction = null)
        {
            DBEntities masterContext = _core.Context;
            string[] userRoleNames = masterContext.Users_Roles.Where(ur => ur.UserId == _core.User.Id && ur.ApplicationId == _core.Application.Id).Select(ur => ur.RoleName).ToArray();
            if (symbolAction == null)
                symbolAction = (WFItemId, block) => { };
            if (blockName == null)
                blockName = "__INIT__";

            /// RUN
            
            /// AppDomain
            //AppDomain domain = GetAppDomain(_core.Application.Name);
            //Block blockInstance = (Block)domain.CreateInstance(
            //    _core.Application.Name,
            //    $"{BlockNamespace(_core.Application.Name)}.{blockName ?? "__INIT__"}",
            //    false,
            //    BindingFlags.Default,
            //    null,
            //    new object[] { new ConstructTransaction {
            //        httpRequest = RequestTransaction.Create(HttpContext.Current.Request),
            //        efConnectionString = Entitron.EntityConnectionString,
            //        Username = _core.User.UserName,
            //        ApplicationName = _core.Application.Name,
            //        ModelId = _core.ModelId,
            //        DeleteId = _core.DeleteId,
            //        Data = _core.Data
            //    } },
            //    CultureInfo.CurrentCulture,
            //    new object[] { }).Unwrap();

            //try
            //{
            //    // run
            //    blockInstance._Run(methodName, userRoleNames, symbolAction);

            //    // get results
            //    _core.Update(blockInstance._GetDestructData());

            //    // return block
            //    return blockInstance.TargetName;
            //}

            // assembly
            Assembly appDll = GetAssembly(_core.Application.Name);
            if (appDll == null)
                throw new TapestryLoadOmniusException($"Could not load assembly file for Application[{_core.Application.Name}]. Try rebuild it. Path: {FullDllName(PathOutput, _core.Application.Name)}", TapestryLoadOmniusException.LoadTarget.Assembly, null);

            // block
            string fullBlockName = $"{BlockNamespace(_core.Application.Name)}.{blockName}";
            Type blockType = appDll.GetType(fullBlockName);
            if (blockType == null)
                throw new TapestryLoadOmniusException($"Block[{fullBlockName}] not found", TapestryLoadOmniusException.LoadTarget.Block, null);

            // method
            string methodName = MethodName(executor);
            MethodInfo method = blockType.GetMethod(methodName);
            if (method == null)
                throw new TapestryLoadOmniusException($"Rule[{methodName}] not found in block[{fullBlockName}]", TapestryLoadOmniusException.LoadTarget.Rule, null);
                
            _core.BlockAttribute = blockType.GetCustomAttribute<BlockAttribute>();
            Block blockInstance = (Block)Activator.CreateInstance(blockType, _core);
                
            try
            {
                method.Invoke(blockInstance, new object[] { userRoleNames, symbolAction });
                return blockInstance.TargetName;
            }
            catch (TapestryAuthenticationOmniusException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TapestryRunOmniusException($"Error while running Tapestry[B:{blockName},M:{methodName}]", ex);
            }
        }

        public static Assembly GetAssembly(string ApplicationName)
        {
            lock (_assemblyCacheLock)
            {
                if (!_assemblyCache.ContainsKey(ApplicationName))
                {
                    string dllPath = FullDllName(PathOutput, ApplicationName);
                    if (dllPath == null)
                        return null;

                    Assembly assembly = Assembly.LoadFile(dllPath);
                    _assemblyCache.Add(ApplicationName, assembly);
                }
            }

            return _assemblyCache[ApplicationName];
        }
        public static string GetAssemblyVersion(string ApplicationName)
        {
            Version version = GetAssembly(ApplicationName)?.GetName().Version;
            if (version == null)
                return null;

            return $"{version.Major.ToString().PadLeft(4, '0')}.{version.Minor.ToString().PadLeft(2, '0')}.{version.Build.ToString().PadLeft(2, '0')}.{version.Revision.ToString().PadLeft(4, '0')}";
        }
        public static void RefreshAssembly(string applicationName)
        {
            _assemblyCache.Remove(applicationName);
        }

        private static object _assemblyCacheLock = new object();
        private static Dictionary<string, Assembly> _assemblyCache = new Dictionary<string, Assembly>();

        private static string _path => Assembly.GetExecutingAssembly().CodeBase; // file:///C:/.../FSPOC2/application/FSS.Omnius.FrontEnd/bin/FSS.Omnius.Modules.DLL
        public static string PathRun => Path.GetDirectoryName(_path.Substring(8, _path.LastIndexOf('/') + 9)); // C:\\...\\FSPOC2\\application\\FSS.Omnius.FrontEnd\\bin
        public static string PathSolutionRoot => Path.Combine(PathRun, "..", ".."); // C:\\...\\FSPOC2\\application
        public static string PathOutput => Path.Combine(PathModules, "Tapestry2", "bin"); // C:\\...\\FSPOC2\\application\\FSS.Omnius.Modules\\Tapestry2\\bin
        public static string PathOutputTemp => Path.Combine(PathOutput, "temp"); // C:\\...\\FSPOC2\\application\\FSS.Omnius.Modules\\Tapestry2\\bin\\temp
        public static string PathModules => Path.Combine(PathSolutionRoot, "FSS.Omnius.Modules");  // C:\\...\\FSPOC2\\application\\FSS.Omnius.Modules
        public static string BlockNamespace(string appName)
        {
            return $"FSS.Omnius.App.{appName}";
        }
        public static string FullDllName(string path, string appName, string version = null)
        {
            // last version
            if (version == null)
                return Directory.EnumerateFiles(path).Where(f => Path.GetFileName(f).StartsWith($"App_{appName}_v") && f.EndsWith(".dll")).OrderByDescending(f => f).FirstOrDefault();

            return Path.Combine(path, DllName(appName, version));
        }
        public static string DllName(string appName, string version)
        {
            return $"App_{appName}_v{version.Replace('.', '_')}.dll";
        }
        public static string PdbName(string dllPath)
        {
            return
                Regex.Replace(dllPath, "[.]dll$", ".pdb");
        }
        public static string MethodName(string executor)
        {
            if (executor != null)
                return $"Exec_{executor.RemoveDiacritics()}";

            return "INIT";
        }
        
        /// AppDomain
        //public static AppDomain GetAppDomain(string ApplicationName)
        //{
        //    if (!_appDomainCache.ContainsKey(ApplicationName))
        //    {
        //        lock (_appDomainCacheLock)
        //        {
        //            AppDomain domain = AppDomain.CreateDomain(ApplicationName, null, AppDomain.CurrentDomain.SetupInformation);
        //            domain.AssemblyResolve += new ResolveEventHandler((sender, args) =>
        //            {
        //                AppDomain domainSender = (AppDomain)sender;
        //                string dllPath = Path.Combine(PathOutput, DllName(args.Name));
        //                Assembly assembly = domainSender.Load(File.ReadAllBytes(dllPath));

        //                return assembly;
        //            });
        //            _appDomainCache.Add(ApplicationName, domain);
        //        }
        //    }

        //    return _appDomainCache[ApplicationName];
        //}
        //public static void UnloadAppDomain(string ApplicationName)
        //{
        //    if (_appDomainCache.ContainsKey(ApplicationName))
        //    {
        //        lock (_appDomainCacheLock)
        //        {
        //            AppDomain.Unload(_appDomainCache[ApplicationName]);
        //            _appDomainCache.Remove(ApplicationName);
        //        }
        //    }
        //}

        //private static object _appDomainCacheLock = new object();
        //private static Dictionary<string, AppDomain> _appDomainCache = new Dictionary<string, AppDomain>();
    }
}
