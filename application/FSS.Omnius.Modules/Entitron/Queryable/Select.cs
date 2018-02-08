using System;
using System.Collections.Generic;
using System.Text;
using FSS.Omnius.Modules.Entitron.Queryable.Cond;
using FSS.Omnius.Modules.Entitron.DB;
using System.Linq;
using System.Data;

namespace FSS.Omnius.Modules.Entitron.Queryable
{
    public class Select : IQueryable, IWhere<Select>, IJoin<Select>, IOrder<Select>, IGroup<Select>, IPage<Select>, IDropStep<Select>
    {
        public Select(DBConnection db, string tabloidName, string[] columns, bool isSystem = false)
        {
            _tabloidName = tabloidName;
            _isSystem = isSystem;
            _columns = columns;
            _db = db;

            _condition = new Manager<Condition>(_db);
            _condition.i.tabloidName = tabloidName;
            _join = new Manager<Join>(_db);
        }

        private DBConnection _db;
        private string _tabloidName;
        private bool _isSystem;
        private string[] _columns;
        private int? _limit;
        private Manager<Condition> _condition;
        private Manager<Join> _join;
        private Order _order;
        private GroupBy _groupColumns;
        private Page _page;
        private DropStep _dropStep;

        public ListJson<DBItem> ToList()
        {
            Tabloid newTabloid = new Tabloid(_db) { Name = _tabloidName };
            return _db.ExecuteRead(_db.CommandSet.SELECT(_db, newTabloid, _isSystem, _columns, _condition, _join, _order, _groupColumns, _limit, _page, _dropStep), newTabloid);
        }
        public DBItem First()
        {
            Tabloid newTabloid = new Tabloid(_db) { Name = _tabloidName };
            return _db.ExecuteRead(_db.CommandSet.SELECT(_db, newTabloid, _isSystem, _columns, _condition, _join, _order, _groupColumns, 1, _page, _dropStep), newTabloid).First();
        }
        public DBItem FirstOrDefault()
        {
            Tabloid newTabloid = new Tabloid(_db) { Name = _tabloidName };
            return _db.ExecuteRead(_db.CommandSet.SELECT(_db, newTabloid, _isSystem, _columns, _condition, _join, _order, _groupColumns, 1, _page, _dropStep), newTabloid).FirstOrDefault();
        }
        public DBItem Single()
        {
            Tabloid newTabloid = new Tabloid(_db) { Name = _tabloidName };
            return _db.ExecuteRead(_db.CommandSet.SELECT(_db, newTabloid, _isSystem, _columns, _condition, _join, _order, _groupColumns, 2, _page, _dropStep), newTabloid).Single();
        }
        public DBItem SingleOrDefault()
        {
            Tabloid newTabloid = new Tabloid(_db) { Name = _tabloidName };
            return _db.ExecuteRead(_db.CommandSet.SELECT(_db, newTabloid, _isSystem, _columns, _condition, _join, _order, _groupColumns, 2, _page, _dropStep), newTabloid).SingleOrDefault();
        }
        public int Count()
        {
            Tabloid newTabloid = new Tabloid(_db) { Name = _tabloidName };
            using (DBReader reader = _db.ExecuteCommand(_db.CommandSet.SELECT_count(_db, newTabloid, _isSystem, _condition, _join, _groupColumns)))
            {
                reader.Read();

                return Convert.ToInt32(reader["count"]);
            }
        }

        public Select Where(Func<ConditionColumn, ConditionConcat> condition)
        {
            _condition.Start();
            condition(new ConditionColumn(_condition));

            return this;
        }

        public Select Join(string joinTableName, string joinColumnName, string originColumnName)
        {
            _join.i.joinTableName = joinTableName;
            _join.i.joinColumnName = joinColumnName;
            _join.i.originTableName = _tabloidName;
            _join.i.originColumnName = originColumnName;
            _join.Next();

            return this;
        }

        public Select Order(AscDesc ascDesc = AscDesc.Asc, params string[] columnNames)
        {
            if (columnNames.Any(c => !string.IsNullOrEmpty(c)))
                _order = new Order
                {
                    AscDesc = ascDesc,
                    Columns = columnNames.Select(c => c.Contains('.') ? c : $"{_tabloidName}.{c}")
                };

            return this;
        }

        public Select Group(ESqlFunction function = ESqlFunction.none, Func<ConditionColumn, ConditionConcat> having = null, params string[] columns)
        {
            _groupColumns = new GroupBy(_db)
            {
                Columns = columns.Select(c => c.Contains('.') ? c : $"{_tabloidName}.{c}"),
                Function = function,
                Having = new Manager<Condition>(_db)
            };
            if (having != null)
            {
                _groupColumns.Having.i.tabloidName = _tabloidName;
                _groupColumns.Having.Start();
                having(new ConditionColumn(_groupColumns.Having));
            }

            return this;
        }

        public Select Limit(int count)
        {
            _limit = count;

            return this;
        }

        public Select Page(int rowsPerPage, int pageIndex)
        {
            _page = new Page
            {
                RowsPerPage = rowsPerPage,
                PageIndex = pageIndex
            };

            return this;
        }
        
        public Select DropStep(int finalCount, ESqlFunction function = ESqlFunction.LAST, AscDesc ascDesc = AscDesc.Asc, params string[] orderColumnNames)
        {
            if (!orderColumnNames.Any())
                throw new ArgumentException("orderedColumnNames needs at least one column");

            return DropStep(finalCount, new Order { AscDesc = ascDesc, Columns = orderColumnNames.Select(c => c.Contains('.') ? c : $"{_tabloidName}.{c}") }, function);
        }

        public Select DropStep(int finalCount, Order order, ESqlFunction function = ESqlFunction.LAST)
        {
            if (function == ESqlFunction.none)
                throw new ArgumentException("DropStep with no function return no data!");

            _dropStep = new DropStep
            {
                FinalCount = finalCount,
                Function = function,
                Order = order
            };

            return this;
        }
    }
}
