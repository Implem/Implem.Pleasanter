using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public interface IBackgroundJobHandler
    {
        Task ExecuteAsync(
            Context context,
            BackgroundJobModel backgroundJobModel);
    }
}
