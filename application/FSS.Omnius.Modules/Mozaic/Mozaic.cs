namespace FSS.Omnius.Modules.Mozaic
{
    using CORE;
    
    public class Mozaic : IModule
    {
        private CORE _CORE;
        public Mozaic(CORE core)
        {
            _CORE = core;
        }

        //public string Render(int pageId, ActionResultCollection results)
        //{
        //    Page page = _CORE.Entitron.GetStaticTables().Pages.FirstOrDefault(p => p.Id == pageId);

        //    return Render(page, results);
        //}
        //public string Render(Page page, ActionResultCollection results)
        //{
        //    if (page == null)
        //        return "Page not found";

        //    return page.Render(results, _CORE.Entitron.GetStaticTables());
        //}
    }
}
