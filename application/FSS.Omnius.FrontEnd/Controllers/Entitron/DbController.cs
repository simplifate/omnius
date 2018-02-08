using E = FSS.Omnius.Modules.Entitron;
using System;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.DB;
using System.Data;
using System.Collections.Generic;
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
                DBConnection db = E.Entitron.i;
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

                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                Modules.Watchtower.OmniusException.Log(ex, Modules.Watchtower.OmniusLogSource.Entitron);
                return -1;
            }
        }
        //public string TestEntitron(int id)
        //{
        //    /// INIT
        //    E.Entitron.Create(id);
        //    DBConnection db = E.Entitron.i;

        //    /// Create schema
        //    DBTable table = db.Table("Test_ueoaueoa_aueoa");
        //    table.Columns.Add(new DBColumn(db) { Tabloid = table, Name = "id", Type = DbType.Int32 });
        //    table.Columns.Add(new DBColumn(db) { Tabloid = table, Name = "Name", Type = DbType.String, IsNullable = true, MaxLength = 200, DefaultValue = "'YEY'" });
        //    table.Indexes.Add(new DBIndex(db) { Table = table, Columns = new List<string> { "Name" }, isUnique = true });
        //    db.TableCreate(table);
        //    db.SaveChanges();

        //    DBTable table2 = db.Table("Test2_ueoaueoa_aueoa");
        //    DBColumn name = new DBColumn(db) { Tabloid = table2, Name = "Name", Type = DbType.String, MaxLength = 400 };
        //    table2.Columns.Add(new DBColumn(db) { Tabloid = table2, Name = "id", Type = DbType.Int32 });
        //    table2.Columns.Add(name);
        //    table2.Indexes.Add(new DBIndex(db) { Table = table2, Columns = new List<string> { "id", "Name" } });
        //    db.TableCreate(table2);
        //    db.SaveChanges();

        //    DBTable table3 = db.Table("Test3_ueoaueoa_aueoa");
        //    table3.Columns.Add(new DBColumn(db) { Tabloid = table3, Name = "id", Type = DbType.Int32 });
        //    table3.Columns.Add(new DBColumn(db) { Tabloid = table3, Name = "Name", Type = DbType.String, IsNullable = true, MaxLength = 200, DefaultValue = "'YEY'" });
        //    table3.Columns.Add(new DBColumn(db) { Tabloid = table3, Name = "TestId", Type = DbType.Int32, IsNullable = true });
        //    db.TableCreate(table3);
        //    table3.ForeignKeys.Add(new DBForeignKey(db) { SourceTable = table3, SourceColumn = "TestId", TargetTable = table, TargetColumn = "id" });
        //    db.SaveChanges();

        //    table2.Columns.Add(new DBColumn(db) { Tabloid = table2, Name = "TestId", Type = DbType.Int32 });
        //    table2.Indexes.Add(new DBIndex(db) { Table = table2, Columns = new List<string> { "TestId" }, isUnique = true });
        //    table2.ForeignKeys.Add(new DBForeignKey(db) { SourceTable = table2, SourceColumn = "TestId", TargetTable = table, TargetColumn = "id" });
        //    table3.Columns.Add(new DBColumn(db) { Tabloid = table3, Name = "hebleble", Type = DbType.String });
        //    db.SaveChanges();
            
        //    TestEntitronDataCreate(id);
        //    TestEntitronData(id);

        //    /// Drop schema
        //    table2.Columns.Remove(name);
        //    db.SaveChanges();

        //    db.TableDrop(table);
        //    db.SaveChanges();

        //    table2.Drop();
        //    table3.Drop();
        //    db.SaveChanges();

        //    return "";
        //}

        //public string TestEntitronDataCreate(int id)
        //{
        //    /// INIT
        //    if (E.Entitron.i.Application == null)
        //        E.Entitron.Create(id);
        //    DBConnection db = E.Entitron.i;

        //    DBTable table = db.Table("Test_ueoaueoa_aueoa");
        //    DBTable table2 = db.Table("Test2_ueoaueoa_aueoa");
        //    DBTable table3 = db.Table("Test3_ueoaueoa_aueoa");

        //    int idA = table.Add(new { Name = "A" });
        //    int idQ = table.Add(new { Name = "Q" });
        //    int idB = table.Add(new { Name = "B" });
        //    int idC = table.Add(new { Name = "C" });
        //    int idD = table.Add(new { Name = "D" });

        //    int idA2 = table2.Add(new { Name = "A2", TestId = idA });
        //    int idB2 = table2.Add(new { Name = "B2", TestId = idB });
        //    int idD2 = table2.Add(new { Name = "D2", TestId = idD });
        //    int idA2b = table2.Add(new { Name = "A2", TestId = idQ });
        //    int idA2c = table2.Add(new { Name = "A2", TestId = idC });

        //    int idZ = table3.Add(new { Name = "Z", TestId = idA, hebleble = "zz" });
        //    int idY = table3.Add(new { Name = "Y", TestId = idB, hebleble = "yy" });
        //    int idX = table3.Add(new { Name = "X", hebleble = "xx" });
        //    int idW = table3.Add(new { Name = "W", hebleble = "ww" });

        //    return "";
        //}

        //public string TestEntitronData(int id)
        //{
        //    /// INIT
        //    if (E.Entitron.i.Application == null)
        //        E.Entitron.Create(id);
        //    DBConnection db = E.Entitron.i;
            
        //    DBTable table = db.Table("Test_ueoaueoa_aueoa");
        //    DBTable table2 = db.Table("Test2_ueoaueoa_aueoa");
        //    DBTable table3 = db.Table("Test3_ueoaueoa_aueoa");


        //    var a = table.SelectById(1);
        //    var b = table.SelectById(4);

        //    var tcount = table.Select().Count();
        //    var all = table.Select().Order(AscDesc.Asc, "Name").ToList();
        //    var first = table.Select().First();
        //    var dropLast = table.Select().DropStep(tcount - 3, ESqlFunction.LAST, AscDesc.Asc, "id").ToList();
        //    var dropFirst = table.Select().DropStep(tcount - 3, ESqlFunction.FIRST, AscDesc.Asc, "id").ToList();
        //    var dropMax = table.Select().DropStep(tcount - 3, ESqlFunction.MAX, AscDesc.Asc, "id").ToList();
        //    var join = table.Select().Join(table2.Name, "TestId", "id").ToList();
        //    var groupFirst = table2.Select().Order(AscDesc.Asc, "id").Group(ESqlFunction.FIRST, null, "Name").ToList();
        //    var groupNone = table2.Select().Order(AscDesc.Asc, "Name").Group(ESqlFunction.none, null, "Name").ToList();
        //    var groupAVG = table2.Select().Order(AscDesc.Asc, "id").Group(ESqlFunction.AVG, null, "Name").ToList();
        //    var page1 = table3.Select().Order(AscDesc.Asc, "id").Page(3, 0).ToList();
        //    var page0 = table3.Select().Order(AscDesc.Asc, "id").Page(3, 1).ToList();
        //    var page2 = table3.Select().Order(AscDesc.Asc, "id").Page(1, 3).ToList();
        //    var limit = table3.Select().Limit(2).ToList();

        //    var having = table2.Select().Group(ESqlFunction.none, c => c.Column("TestId").Greater(1), "Name").ToList();
        //    var havingFirst = table2.Select().Group(ESqlFunction.FIRST, c => c.Column("TestId").Greater(1), "Name").ToList();
        //    var havingCount = table2.Select().Group(ESqlFunction.COUNT, c => c.Column("TestId").Greater(1), "Name").ToList();
        //    var havingAvg = table2.Select().Group(ESqlFunction.AVG, c => c.Column("TestId").Greater(1), "Name").ToList();

        //    var allFunc = table2
        //        .Select($"{table.Name}.id", $"{table2.Name}.id", $"{table2.Name}.Name", $"{table2.Name}.TestId")
        //        .Join(table.Name, "id", "TestId")
        //        .Group(ESqlFunction.SUM, null, $"{table2.Name}.Name")
        //        .Order(AscDesc.Asc, $"{table2.Name}.Name")
        //        .Where(c => c.Column($"{table2.Name}.TestId").Between(12, 15))
        //        .DropStep(3, ESqlFunction.LAST, AscDesc.Asc, $"{table2.Name}.id")
        //        .ToList();

        //    return "";
        //}
    }
}