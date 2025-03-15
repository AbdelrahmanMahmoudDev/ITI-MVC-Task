namespace Task.Services
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile File, string Directory, string[] AllowedExtensions);
        void DeleteFile(string FileName, string Directory);
    }
    public class FileService : IFileService
    {
        private IWebHostEnvironment _WebHostEnvironment;
        public FileService(IWebHostEnvironment WebHostEnvironment)
        {
            _WebHostEnvironment = WebHostEnvironment;
        }
        public async Task<string> SaveFile(IFormFile File, string Directory, string[] AllowedExtensions)
        {
            var StaticFilePath = _WebHostEnvironment.WebRootPath;
            var Path = System.IO.Path.Combine(StaticFilePath, Directory);

            if(!System.IO.Directory.Exists(Path))
            {
                System.IO.Directory.CreateDirectory(Path);
            }

            var extension = System.IO.Path.GetExtension(File.FileName);
            if(!AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException($"Only {string.Join(", ", AllowedExtensions)} extensions are allowed");
            }

            var UniqueFileName = $"{Guid.NewGuid()}{extension}";
            var OutputFilePath = System.IO.Path.Combine(Path, UniqueFileName);

            using var FileStream = new FileStream(OutputFilePath, FileMode.Create);
            await File.CopyToAsync(FileStream);

            return UniqueFileName;
        }

        public void DeleteFile(string FileName, string Directory)
        {
            var FullPath = Path.Combine(_WebHostEnvironment.WebRootPath, Directory, FileName);

            if(!System.IO.Path.Exists(FullPath))
            {
                throw new FileNotFoundException($"File: {FileName} not found");
            }
            File.Delete(FullPath);
        }
    }
}
