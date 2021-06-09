namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelContextServerScript
    {
        public readonly bool OnTesting;
        public readonly long ScriptDepth;

        public ServerScriptModelContextServerScript(
            bool onTesting,
            long scriptDepth)
        {
            OnTesting = onTesting;
            ScriptDepth = scriptDepth;
        }
    }
}