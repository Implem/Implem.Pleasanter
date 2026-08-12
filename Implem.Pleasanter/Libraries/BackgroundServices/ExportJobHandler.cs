using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class ExportJobHandler : IBackgroundJobHandler
    {
        public Task ExecuteAsync(
            Context context,
            BackgroundJobModel backgroundJobModel)
        {
            var jobParameters = backgroundJobModel.JobParameters
                .Deserialize<ExportJobParameters>();
            if (jobParameters == null)
            {
                throw new InvalidDataException(Displays.Get(
                    context: context,
                    id: "BackgroundJobInvalidParameters"));
            }
            var exportContext = CreateContext(
                backgroundJobModel: backgroundJobModel,
                jobParameters: jobParameters);
            var outputDirectory = Path.Combine(
                Directories.BackgroundJobExport(),
                backgroundJobModel.BackgroundJobId.ToString());
            Directory.CreateDirectory(outputDirectory);
            backgroundJobModel.File = ExportByGui(
                context: exportContext,
                siteId: backgroundJobModel.SiteId,
                outputDirectory: outputDirectory);
            return Task.CompletedTask;
        }

        private static Context CreateContext(
            BackgroundJobModel backgroundJobModel,
            ExportJobParameters jobParameters)
        {
            var lookupContext = new Context(
                tenantId: backgroundJobModel.TenantId,
                request: false);
            var user = SiteInfo.User(
                context: lookupContext,
                userId: backgroundJobModel.UserId);
            var context = new Context(
                tenantId: backgroundJobModel.TenantId,
                deptId: user.DeptId,
                userId: backgroundJobModel.UserId,
                language: jobParameters.Language,
                request: false,
                setAuthenticated: true)
            {
                ContentType = jobParameters.ContentType,
                Forms = new Forms(),
                SessionData = jobParameters.SessionData ?? new Dictionary<string, string>(),
                UserSessionData = jobParameters.UserSessionData
                    ?? new Dictionary<string, string>(),
                AbsoluteUri = Parameters.Service.AbsoluteUri
            };
            foreach (var item in jobParameters.Forms ?? new Dictionary<string, string>())
            {
                context.Forms[item.Key] = item.Value;
            }
            context.SetTenantProperties(force: true);
            return context;
        }

        private static string ExportByGui(
            Context context,
            long siteId,
            string outputDirectory)
        {
            var responseFile = new ItemModel(
                context: context,
                referenceId: siteId).Export(context: context);
            if (responseFile == null
                || responseFile.IsError())
            {
                throw new InvalidDataException(
                    responseFile?.ErrorMessage
                        ?? Displays.Get(
                            context: context,
                            id: "BackgroundJobExportFailed",
                            data: BackgroundJobQueue.GetJobTypeLabel(
                                context: context,
                                jobType: "Export")));
            }
            var fileName = Path.GetFileName(responseFile.FileDownloadName);
            var filePath = Path.Combine(outputDirectory, fileName);
            if (responseFile.FileInfo != null)
            {
                responseFile.FileInfo.CopyTo(
                    destFileName: filePath,
                    overwrite: true);
                return filePath;
            }
            if (responseFile.FileContentsStream != null)
            {
                using var stream = new FileStream(
                    path: filePath,
                    mode: FileMode.Create,
                    access: FileAccess.Write);
                responseFile.FileContentsStream.CopyTo(stream);
                return filePath;
            }
            var encoding = responseFile.Encoding == "Shift-JIS"
                ? Encoding.GetEncoding("Shift_JIS")
                : Encoding.UTF8;
            File.WriteAllText(
                path: filePath,
                contents: responseFile.FileContents,
                encoding: encoding);
            return filePath;
        }

    }

    public class ExportJobParameters
    {
        public string ContentType { get; set; }
        public string Language { get; set; }
        public Dictionary<string, string> Forms { get; set; }
        public Dictionary<string, string> SessionData { get; set; }
        public Dictionary<string, string> UserSessionData { get; set; }

        public static ExportJobParameters FromContext(Context context)
        {
            return new ExportJobParameters
            {
                ContentType = context.ContentType,
                Language = context.Language,
                Forms = new Dictionary<string, string>(context.Forms),
                SessionData = new Dictionary<string, string>(context.SessionData),
                UserSessionData = new Dictionary<string, string>(context.UserSessionData)
            };
        }
    }
}
