using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelView
    {
        public readonly int Id;
        public List<string> AlwaysGetColumns = new List<string>();
        public string OnSelectingWhere;
        public string OnSelectingOrderBy;
        public Dictionary<string, string> ColumnPlaceholders;
        public readonly ExpandoObject Filters = new ExpandoObject();
        public readonly Dictionary<string, bool> FilterNegatives = new Dictionary<string, bool>();
        public readonly ExpandoObject SearchTypes = new ExpandoObject();
        public readonly ExpandoObject Sorters = new ExpandoObject();
        // ClearFilters()呼んだ時trueになる。サーバスクリプトからは変更禁止にしたいのでprivate setにしてある。
        public bool FiltersCleared { private set; get; }
        private List<string> ChangedFilters = new List<string>();
        private bool Initialized { get; set; }

        public ServerScriptModelView(int id = 0)
        {
            Id = id;
            ((INotifyPropertyChanged)Filters).PropertyChanged += FiltersChanged;
        }

        public void AddColumnPlaceholder(string key, string value)
        {
            if (ColumnPlaceholders == null)
            {
                ColumnPlaceholders = new Dictionary<string, string>();
            }
            ColumnPlaceholders.AddOrUpdate(key, value);
        }

        public void ClearFilters()
        {
            //ExpandoObjectにはクリア系メソッドが無いのでキャストしてクリアする
            ((IDictionary<string, object>)Filters).Clear();
            FiltersCleared = true;
        }

        public void FilterNegative(string name, bool negative = true)
        {
            FilterNegatives.AddOrUpdate(name, negative);
        }

        /// <summary>
        /// サーバスクリプト実行前にViewからの転記が完了した後に初期化完了フラグをオンにする
        /// </summary>
        internal void SetInitialized()
        {
            Initialized = true;
        }

        /// <summary>
        /// サーバスクリプトでフィルタ(Filters)が変更された際に呼び出されるイベント
        /// </summary>
        private void FiltersChanged(object sender, PropertyChangedEventArgs e)
        {
            FiltersChanged(
                filters: Filters,
                name: e.PropertyName);
        }

        private void FiltersChanged(
            IDictionary<string, object> filters,
            string name)
        {
            // サーバスクリプトで変更したカラム名は記憶しておきColumnFilterNegativesから除外する
            // 初期化が完了（Initialized）していない場合には初期値の設定のため記憶しない
            if (Initialized && !ChangedFilters.Contains(name))
            {
                ChangedFilters.Add(name);
                // or_やand_フィルタが存在する場合には再帰的に条件ツリーの処理を行う
                if (name.StartsWith("or_") || name.StartsWith("and_"))
                {
                    var childFilters = filters[name].ToString()
                        .Deserialize<Dictionary<string, object>>()
                        ?.ToDictionary(
                            o => name + "\\" + o.Key,
                            o => o.Value);
                    if (childFilters != null)
                    {
                        foreach (var childName in childFilters.Keys)
                        {
                            FiltersChanged(
                                filters: childFilters,
                                name: childName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// view.Filters指定されたものをColumnFilterNegativesから削除する
        /// </summary>
        internal void ClearColumnFilterNegatives(View view)
        {
            view.ColumnFilterNegatives?.RemoveAll(o => ChangedFilters.Contains(o));
        }
    }
}