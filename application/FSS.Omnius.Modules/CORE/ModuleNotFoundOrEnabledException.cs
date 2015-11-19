namespace System
{
    class ModuleNotFoundOrEnabledException : Exception
    {
        public string moduleName { get; set; }

        public ModuleNotFoundOrEnabledException(string moduleName)
        {
            this.moduleName = moduleName;
        }
    }
}
