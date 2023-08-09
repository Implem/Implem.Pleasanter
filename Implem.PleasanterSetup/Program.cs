namespace Implem.PleasanterSetup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = ConsoleApp.Create(args);
            app.AddCommands<PleasanterSetup>();
            app.Run();
        }
    }
}
