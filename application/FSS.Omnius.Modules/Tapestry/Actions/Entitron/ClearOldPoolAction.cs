using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    public class ClearOldPoolAction : Action
    {
        public override int Id => 1067;

        public override string[] InputVar => new string[] { };

        public override string[] OutputVar => new string[] { };

        public override int? ReverseActionId => null;

        public override string Name => "Clear old pool action";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;
            IDbCommand cmd = db.CommandSet.Command;

            cmd.CommandText =
                "DECLARE @v_spid INT " +
                "DECLARE c_Users CURSOR " +
                "   FAST_FORWARD FOR " +
                "   SELECT SPID " +
                "   FROM master..sysprocesses (NOLOCK) " +
                "   WHERE spid>50  " +
                "   AND status='sleeping'  " +
                "   AND (program_name = 'EntityFramework' OR program_name = 'Entitron') " +
                "   AND DATEDIFF(mi,last_batch,GETDATE())>=60 " +
                "   AND spid<>@@spid " +

                "OPEN c_Users " +
                "FETCH NEXT FROM c_Users INTO @v_spid " +
                "WHILE (@@FETCH_STATUS=0) " +
                "BEGIN " +
                "  PRINT 'KILLing '+CONVERT(VARCHAR,@v_spid)+'...' " +
                "  EXEC('KILL '+@v_spid) " +
                "  FETCH NEXT FROM c_Users INTO @v_spid " +
                "END " +

                "CLOSE c_Users " +
                "DEALLOCATE c_Users";

            db.ExecuteNonQuery(cmd);
        }
    }
}
