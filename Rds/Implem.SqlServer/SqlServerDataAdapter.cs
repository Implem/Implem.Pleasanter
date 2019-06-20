using Implem.IRds;
using System;
using System.Data;
using System.Data.SqlClient;
namespace Implem.SqlServer
{
    class SqlServerDataAdapter : ISqlDataAdapter
    {
        SqlDataAdapter _instance;

        public SqlServerDataAdapter(ISqlCommand sqlCommand)
        {
            _instance = new SqlDataAdapter(((SqlServerCommand)sqlCommand).InnerInstance);
        }

        public IDbCommand DeleteCommand { get => _instance.DeleteCommand; set => _instance.DeleteCommand = (SqlCommand)value; }
        public IDbCommand InsertCommand { get => _instance.InsertCommand; set => _instance.InsertCommand = (SqlCommand)value; }
        public IDbCommand SelectCommand { get => _instance.SelectCommand; set => _instance.SelectCommand = (SqlCommand)value; }
        public IDbCommand UpdateCommand { get => _instance.UpdateCommand; set => _instance.UpdateCommand = (SqlCommand)value; }
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
