
using Microsoft.AspNetCore.Http;

namespace Pharmacy_Backend.Helpers
{
    public class FileUploadHelper
    {
        private readonly string _uploadPath;

        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions =
        {
            ".pdf", ".jpg", ".jpeg", ".png"
        };

        public FileUploadHelper(IWebHostEnvironment environment)
        {
            _uploadPath = Path.Combine(
                environment.ContentRootPath,
                "PrivateUploads",
                "Prescriptions"
            );

            Directory.CreateDirectory(_uploadPath);
        }

        public async Task<(string FileName, string StoredFileName)>
            SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Please select a file.");

            if (file.Length > MaxFileSize)
                throw new ArgumentException("Maximum file size is 5 MB.");

            string extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException(
                    "Only PDF, JPG, JPEG and PNG files are allowed."
                );

            string storedFileName = $"{Guid.NewGuid()}{extension}";

            string fullPath = Path.Combine(_uploadPath, storedFileName);

            await using var stream = new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write
            );

            await file.CopyToAsync(stream);

            return (Path.GetFileName(file.FileName), storedFileName);
        }

        public void DeleteFile(string? storedFileName)
        {
            if (string.IsNullOrWhiteSpace(storedFileName))
                return;

            string fullPath = Path.Combine(_uploadPath, storedFileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public string GetFilePath(string storedFileName)
        {
            return Path.Combine(_uploadPath, storedFileName);
        }
    }
}
