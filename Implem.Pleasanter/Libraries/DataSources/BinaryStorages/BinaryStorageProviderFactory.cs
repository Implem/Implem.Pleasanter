using Implem.DefinitionAccessor;
using Implem.ParameterAccessor.Parts;

namespace Implem.Pleasanter.Libraries.DataSources.BinaryStorages
{
    public static class BinaryStorageProviderFactory
    {
        public static IBinaryStorageProvider Create(string provider)
        {
            return provider switch
            {
                BinaryStorageProviderNames.LocalFolder => new LocalFolderBinaryStorageProvider(),
                BinaryStorageProviderNames.Local => new LocalFolderBinaryStorageProvider(),
                BinaryStorageProviderNames.AzureBlob => new AzureBlobBinaryStorageProvider(),
                _ => null
            };
        }

        public static IBinaryStorageProvider Current()
        {
            return Create(Parameters.BinaryStorage.Provider);
        }
    }
}
