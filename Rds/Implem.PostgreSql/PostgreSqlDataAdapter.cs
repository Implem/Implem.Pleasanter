using Implem.IRds;
using Npgsql;
using System;
using System.Data;
namespace Implem.PostgreSql
{
    internal class PostgreSqlDataAdapter : ISqlDataAdapter
    {
        private NpgsqlDataAdapter instance;

        public PostgreSqlDataAdapter(ISqlCommand sqlCommand)
        {
            instance = new NpgsqlDataAdapter(
                selectCommand: ((PostgreSqlCommand)sqlCommand).InnerInstance);
        }

        public IDbCommand DeleteCommand
        {
            get
            {
                return instance.DeleteCommand;
            }
            set
            {
                instance.DeleteCommand = (NpgsqlCommand)value;
            }
        }

        public IDbCommand InsertCommand
        {
            get
            {
                return instance.InsertCommand;
            }
            set
            {
                instance.InsertCommand = (NpgsqlCommand)value;
            }
        }

        public IDbCommand SelectCommand
        {
            get
            {
                return instance.SelectCommand;
            }
            set
            {
                instance.SelectCommand = (NpgsqlCommand)value;
            }
        }

        public IDbCommand UpdateCommand
        {
            get
            {
                return instance.UpdateCommand;
            }
            set
            {
                instance.UpdateCommand = (NpgsqlCommand)value;
            }
        }

        public MissingMappingAction MissingMappingAction
        {
            get
            {
                return instance.MissingMappingAction;
            }
            set
            {
                instance.MissingMappingAction = value;
            }
        }

        public MissingSchemaAction MissingSchemaAction
        {
            get
            {
                return instance.MissingSchemaAction;
            }
            set
            {
                instance.MissingSchemaAction = value;
            }
        }

        public ITableMappingCollection TableMappings
        {
            get
            {
                return instance.TableMappings;
            }
        }

        public object Clone()
        {
            return ((ICloneable)instance).Clone();
        }

        public void Fill(DataTable dataTable)
        {
            instance.Fill(dataTable);
        }

        public int Fill(DataSet dataSet)
        {
            return instance.Fill(dataSet);
        }

        public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            throw new NotImplementedException();
        }

        public IDataParameter[] GetFillParameters()
        {
            return instance.GetFillParameters();
        }

        public int Update(DataSet dataSet)
        {
            return instance.Update(dataSet);
        }
    }
}
