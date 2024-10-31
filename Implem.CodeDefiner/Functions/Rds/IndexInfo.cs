﻿using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds
{
    internal class IndexInfo
    {
        internal string TableName;
        internal Types Type;
        internal string Name;
        internal IEnumerable<Column> ColumnCollection;
        private ISqlObjectFactory factory;

        internal class Column
        {
            internal string ColumnName;
            internal int No;
            internal SqlOrderBy.Types OrderType;
            internal bool Unique;

            internal Column(string columnName, int no, string orderType, bool unique = false)
            {
                ColumnName = columnName;
                No = no;
                switch(orderType.ToLower())
                {
                    case "asc": OrderType = SqlOrderBy.Types.asc; break;
                    case "desc": OrderType = SqlOrderBy.Types.desc; break;
                    default: OrderType = SqlOrderBy.Types.asc; break;
                }
                Unique = unique;
            }
        }

        internal enum Types
        {
            Pk,
            Ix
        }

        internal IndexInfo(
            ISqlObjectFactory factory,
            string tableName,
            Types type,
            string name,
            IEnumerable<Column> columnCollection)
        {
            this.factory = factory;
            TableName = tableName;
            Type = type;
            Name = name;
            ColumnCollection = columnCollection;
        }

        internal string IndexName()
        {
            return Name + "_" + ColumnCollection.Where(o => o.No > 0).Select(o =>
                Name +
                TableName +
                o.ColumnName +
                o.No.ToString() +
                o.OrderType.ToString() +
                o.Unique.ToString())
                    .Join(string.Empty)
                    .Sha512Cng()
                    .MaxLength(factory.SqlDefinitionSetting.IdentifierPostfixLength);
        }

        internal string IndexInfoString()
        {
            //MySQLの場合のPk変更検知処理専用のメソッド
            return ColumnCollection
                .Where(o => o.No > 0)
                .Select(o => o.ColumnName + "," + o.OrderType.ToString())
                .Join(","); 
        }
    }
}
