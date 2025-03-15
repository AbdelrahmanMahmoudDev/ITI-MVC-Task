using System.IO;

namespace Task.Utilities
{
    public static class FileUtility
    {
        public static string WebRootPath { get; set; } = "";
        public static async Task<string> SaveFile(IFormFile File, string Directory, string[] AllowedExtensions)
        {
            if(string.IsNullOrEmpty(WebRootPath))
            {
                throw new InvalidOperationException("WebRootPath is not set");
            }

            var Path = System.IO.Path.Combine(WebRootPath, Directory);
            if(!System.IO.Directory.Exists(Path))
            {
                System.IO.Directory.CreateDirectory(Path);
            }

            var extension = System.IO.Path.GetExtension(File.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException($"Only {string.Join(", ", AllowedExtensions)} extensions are allowed.");
            }

            var UniqueFileName = $"{Guid.NewGuid()}{extension}";
            var FullPath = System.IO.Path.Combine(Path, UniqueFileName);

            using (var Stream = new FileStream(FullPath, FileMode.Create))
            {
                await File.CopyToAsync(Stream);
            }

            return $"/{Directory}/{UniqueFileName}";
        }

        public static void DeleteFile(string FilePath)
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                return;
            }

            var FullPath = Path.Combine(WebRootPath, FilePath.TrimStart('/').Replace('/', '\\'));
            if (File.Exists(FullPath))
            {
                File.Delete(FullPath);
            }
        }
    }
}
