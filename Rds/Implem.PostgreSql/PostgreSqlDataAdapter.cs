using Implem.IRds;
using Npgsql;
using System;
using System.Data;
namespace Implem.PostgreSql
{
    class PostgreSqlDataAdapter : ISqlDataAdapter
    {
        NpgsqlDataAdapter _instance;

        public PostgreSqlDataAdapter(ISqlCommand sqlCommand)
        {
            _instance = new NpgsqlDataAdapter(((PostgreSqlCommand)sqlCommand).InnerInstance);
        }

        public IDbCommand DeleteCommand { get => _instance.DeleteCommand; set => _instance.DeleteCommand = (NpgsqlCommand)value; }
        public IDbCommand InsertCommand { get => _instance.InsertCommand; set => _instance.InsertCommand = (NpgsqlCommand)value; }
        public IDbCommand SelectCommand { get => _instance.SelectCommand; set => _instance.SelectCommand = (NpgsqlCommand)value; }
        public IDbCommand UpdateCommand { get => _instance.UpdateCommand; set => _instance.UpdateCommand = (NpgsqlCommand)value; }
        public MissingMappingAction MissingMappingAction { get => _instance.MissingMappingAction; set => _instance.MissingMappingAction = value; }
        public MissingSchemaAction MissingSchemaAction { get => _instance.MissingSchemaAction; set => _instance.MissingSchemaAction = value; }

        public ITableMappingCollection TableMappings => _instance.TableMappings;

        public object Clone()
        {
            return ((ICloneable)_instance).Clone();
        }

        public void Fill(DataTable dataTable)
        {
            _instance.Fill(dataTable);
        }

        public int Fill(DataSet dataSet)
        {
            return _instance.Fill(dataSet);
        }

        public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            throw new NotImplementedException();
        }

        public IDataParameter[] GetFillParameters()
        {
            return _instance.GetFillParameters();
        }

        public int Update(DataSet dataSet)
        {
            return _instance.Update(dataSet);
        }
    }
}
