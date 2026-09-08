using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class AiProvider : ISettingListItem
    {
        public int Id { get; set; }
        public string Title;
        public string Format;
        public string ProviderType;
        public string ConnectionSetting;
        public bool? Disabled;

        public static class Types
        {
            public const string MarkdownFile = "MarkdownFile";
            public const string Dify = "Dify";
            public const string OpenAi = "OpenAi";
        }

        public const string DefaultProviderType = Types.MarkdownFile;

        public AiProvider()
        {
        }

        public AiProvider(Context context)
        {
            Format = Parameters.AiConnect?.Rag.FormatTemplate != null
                ? string.Join("\n", Parameters.AiConnect.Rag.FormatTemplate)
                : null;
            ProviderType = DefaultProviderType;
            ConnectionSetting = AiProviderUtilities.ConnectionSettingTemplate(
                providerType: DefaultProviderType);
        }

        public AiProvider(
            int id,
            string title,
            string format,
            string providerType,
            string connectionSetting,
            bool disabled)
        {
            Id = id;
            Title = title;
            Format = format;
            ProviderType = providerType;
            ConnectionSetting = connectionSetting;
            Disabled = disabled;
        }

        public void Update(
            string title,
            string format,
            string providerType,
            string connectionSetting,
            bool disabled)
        {
            Title = title;
            Format = format;
            ProviderType = providerType;
            ConnectionSetting = connectionSetting;
            Disabled = disabled;
        }

        public AiProvider GetRecordingData(Context context)
        {
            var aiProvider = new AiProvider();
            aiProvider.Id = Id;
            aiProvider.ProviderType = ProviderType;
            if (Title.IsNullOrEmpty() == false)
            {
                aiProvider.Title = Title;
            }
            if (Format.IsNullOrEmpty() == false)
            {
                aiProvider.Format = Format;
            }
            if (ConnectionSetting.IsNullOrEmpty() == false)
            {
                aiProvider.ConnectionSetting = ConnectionSetting;
            }
            if (Disabled == true)
            {
                aiProvider.Disabled = Disabled;
            }
            return aiProvider;
        }
    }
}
