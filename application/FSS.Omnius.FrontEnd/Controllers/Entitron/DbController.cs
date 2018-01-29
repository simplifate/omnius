using E = FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.FrontEnd.Controllers.Entitron
{
    public class DbController : Controller
    {
        // GET: Db
        public int ClearOldPool()
        {
            try
            {
                int count = 0;
                SqlQuery_Command query = new SqlQuery_Command(E.Entitron.connectionString)
                {
                    Sql = "DECLARE @v_spid INT " +
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
                        "DEALLOCATE c_Users"
                };
                query.Execute(E.Entitron.connectionString);

                return count;
            }
            catch(Exception ex)
            {
                return -1;
            }
        }
    }
}