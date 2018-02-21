using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public class DBReader : IDisposable, IDataRecord
    {
        public DBReader(IDbConnection connection, IDataReader reader)
        {
            _conn = connection;
            _reader = reader;
        }

        private IDbConnection _conn;
        private IDataReader _reader;

        public int FieldCount => _reader.FieldCount;

        public object this[string name] => _reader[name];

        public object this[int i] => _reader[i];

        public void Dispose()
        {
            _conn.Dispose();
            _reader.Dispose();
        }

        public bool Read()
        {
            return _reader.Read();
        }

        public string GetName(int i)
        {
            return _reader.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            return _reader.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            return _reader.GetFieldType(i);
        }

        public object GetValue(int i)
        {
            return _reader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return _reader.GetValues(values);
        }

        public int GetOrdinal(string name)
        {
            return _reader.GetOrdinal(name);
        }

        public bool GetBoolean(int i)
        {
            return _reader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _reader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _reader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            return _reader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _reader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _reader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _reader.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            return _reader.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            return _reader.GetDouble(i);
        }

        public string GetString(int i)
        {
            return _reader.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            return _reader.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _reader.GetDateTime(i);
        }

        public IDataReader GetData(int i)
        {
            return _reader.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            return _reader.IsDBNull(i);
        }
    }
}
