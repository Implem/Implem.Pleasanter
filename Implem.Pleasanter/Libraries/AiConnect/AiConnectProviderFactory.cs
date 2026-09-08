using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public static class AiConnectProviderFactory
    {
        private static readonly IAiConnectProvider markdownFileProvider
            = new AiConnectMarkdownFileProvider();
        private static readonly IAiConnectProvider difyProvider
            = new AiConnectDifyProvider();
        private static readonly IAiConnectProvider openAiProvider
            = new AiConnectOpenAiProvider();

        public static IAiConnectProvider Get(string providerType)
        {
            switch (providerType)
            {
                case AiProvider.Types.MarkdownFile:
                    return markdownFileProvider;
                case AiProvider.Types.Dify:
                    return difyProvider;
                case AiProvider.Types.OpenAi:
                    return openAiProvider;
                default:
                    return null;
            }
        }
    }
}
