using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MovieManagement.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        void DeleteFile(string filePath);
    }
}