namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptJsLibraries
    {
        public static string ScriptInit()
        {
            return """
                let $ps = {};
                $ps._utils = {
                    _f0: (f) =>
                    {
                        var err = null;
                        var ret = f((type, data) => {err = data});
                        if (err) throw new Error(err);
                        return ret;
                    },
                };
                $ps.utilities = {};
                """;
        }

        public static string Scripts()
        {
            return """
                $ps.JSON = {
                        stringify: function() { },
                        parse: function() { }};
                (function() {
                    {
                        let js = JSON.stringify;
                        let clr = JsonConvert.SerializeObject;
                        $ps.JSON.stringify = (v, r, s) => js(v, r, s) || clr(v);
                    }
                    {
                        let js = JSON.parse;
                        let clr = JsonConvert.Parse;
                        $ps.JSON.parse = (v, r) => js(v, r) || clr(v);
                    }
                })();
                $p = {};
                $p.JSON = {
                    stringify: (v, r, s) => $ps.JSON.stringify(v, r, s)
                };
                """;
        }
    }
}
