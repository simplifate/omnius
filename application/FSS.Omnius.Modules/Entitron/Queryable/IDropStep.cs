using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Entitron.Queryable
{
    interface IDropStep<T> where T : IQueryable
    {
        T DropStep(int finalCount, ESqlFunction function = ESqlFunction.LAST, AscDesc ascDesc = AscDesc.Asc, params string[] orderColumnNames);
        T DropStep(int finalCount, Order order, ESqlFunction function = ESqlFunction.LAST);
    }

    public class DropStep
    {
        public int FinalCount { get; set; }
        public ESqlFunction Function { get; set; }
        public Order Order { get; set; }
    }
}
