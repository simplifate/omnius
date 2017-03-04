using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlInitScript : SqlQuery_withoutApp
    {
        public SqlInitScript(string connection) : base(connection)
        {
        }

        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            sqlString =
                $"if not exists(select * from sysobjects where name = '{DB_EntitronMeta}' and xtype = 'U')" +
                " BEGIN " +
                $"CREATE TABLE [dbo].[{DB_EntitronMeta}] " +
                "([Id] INT IDENTITY (1, 1) NOT NULL, " +
                " [Name] NVARCHAR(50) NOT NULL, " +
                " [ApplicationId] INT NULL, " +
                " [tableId] INT NOT NULL, " +
                " CONSTRAINT[PK_dbo.Entitron___META] PRIMARY KEY CLUSTERED([Id] ASC)); " +
                $"CREATE UNIQUE NONCLUSTERED INDEX[UNIQUE_{DB_EntitronMeta}_Name] ON[dbo].[{DB_EntitronMeta}]([ApplicationId] ASC, [Name] ASC);" +
                "END";

                //"CREATE PROCEDURE getTableRealName @applicationName NVARCHAR(50), @tableName NVARCHAR(50), @realTableName NVARCHAR(100) OUTPUT AS " +
                //"SET @realTableName = CONCAT('Entitron_', @applicationName, '_', @tableName);";

            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return "Initial query: creating procedure";
        }
    }
}
