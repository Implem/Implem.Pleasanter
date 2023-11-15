namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptJsLibraries
    {
        public static string Scripts()
        {
            return  @"
                let $p = {
                    JSON: {
                        stringify: function() { }}};
                    (function() {
                        let js = JSON.stringify;
                        let clr = JsonConvert.SerializeObject;
                        $p.JSON.stringify = (v, r, s) => js(v, r, s) || clr(v);
                    })()
                ";
        }
    }
}
