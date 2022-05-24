using Implem.PleasanterTest.Utilities;
using Microsoft.Extensions.DependencyInjection;
namespace Implem.PleasanterTest
{
    /// <summary>
    /// テスト起動時に1度だけ初期化処理を呼び出します。
    /// </summary>
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Initializer.Initialize();
        }
    }
}
