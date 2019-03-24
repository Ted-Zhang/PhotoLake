using System.IO;
using System.Threading.Tasks;

namespace PhotoStorage.Service
{
    public interface IAzureFileStorageService
    {
        Task SaveFileToAzureStorage(string username, string fileName, Stream fileStream);
    }
}
