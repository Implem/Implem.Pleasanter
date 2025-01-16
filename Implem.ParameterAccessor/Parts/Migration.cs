using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Migration
    {
        public string Dbms;
        public string Provider;
        public string ServiceName;
        //「移行元」の接続文字列は環境変数非対応とする。（常時設定しないように運用回避とする。）
        public string SourceConnectionString;
        public List<string> ExcludeTables;
        public bool AbortWhenException;
        public bool InsertIfOverflowIfOverflow;
    }
}
