using System;
using System.Collections.Generic;
using System.Text;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public enum AscDesc
    {
        Asc = 0,
        Desc = 1
    }

    public enum ESqlType
    {
        MSSQL,
        MySQL
    }

    public enum ESqlFunction
    {
        none,
        MAX,
        MIN,
        AVG,
        COUNT,
        SUM,
        FIRST,
        LAST
    }

    [Flags]
    public enum ETabloid
    {
        ApplicationTables = 1,
        SystemTables = 2,
        Views = 4
    }

    public static class EnumExtend
    {
        public static bool RequireNumeric(this ESqlFunction function)
        {
            switch (function)
            {
                case ESqlFunction.none:
                case ESqlFunction.MIN:
                case ESqlFunction.MAX:
                case ESqlFunction.COUNT:
                case ESqlFunction.FIRST:
                case ESqlFunction.LAST:
                    return false;
                case ESqlFunction.AVG:
                case ESqlFunction.SUM:
                    return true;
                default:
                    throw new InvalidOperationException();
            }
        }
        public static bool NeedsInnerQuery(this ESqlFunction function)
        {
            switch (function)
            {
                case ESqlFunction.none:
                case ESqlFunction.MIN:
                case ESqlFunction.MAX:
                case ESqlFunction.COUNT:
                case ESqlFunction.AVG:
                case ESqlFunction.SUM:
                    return false;
                case ESqlFunction.FIRST:
                case ESqlFunction.LAST:
                    return true;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static bool Contains(this ETabloid tabloid, ETabloid item)
        {
            return (tabloid & item) > 0;
        }
    }
}
