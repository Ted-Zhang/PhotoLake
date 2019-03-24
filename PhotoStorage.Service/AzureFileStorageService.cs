using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace PhotoStorage.Service
{
    public class AzureFileStorageService : IAzureFileStorageService
    {
        public readonly IConfiguration _configuration;
        public AzureFileStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SaveFileToAzureStorage(string username, string fileName, Stream fileStream)
        {
            using (fileStream)
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                // Read this from configuration or azure vault.
                var connectionString = _configuration["AzureStorageConnectionString"];
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var fileClient = storageAccount.CreateCloudFileClient();

                // Folder name should be account name or user login name.
                var folderReference = fileClient.GetShareReference(username);
                await folderReference.CreateIfNotExistsAsync();

                // Depends on app, might have a different name.
                const string folderName = "PhotoBackup";
                var dictionaryFolder =
                    folderReference.GetRootDirectoryReference().GetDirectoryReference(folderName);
                await dictionaryFolder.CreateIfNotExistsAsync();

                var cloudFile = dictionaryFolder.GetFileReference(fileName);

                // For exists file, make a duplicate one.
                if (await cloudFile.ExistsAsync())
                {
                    cloudFile = dictionaryFolder.GetFileReference($"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.UtcNow.Ticks}{Path.GetExtension(fileName)}");
                }
                await cloudFile.UploadFromStreamAsync(fileStream);
                fileStream.Dispose();
            }
        }
    }
}
