using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Services.Interfaces;

namespace SmartCourses.BLL.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<ServiceResult<string>> UploadFileAsync(IFormFile file, string folder)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return ServiceResult<string>.Failure("No file uploaded");
                }

                // Create uploads folder if not exists
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path
                var relativePath = Path.Combine("uploads", folder, fileName).Replace("\\", "/");
                return ServiceResult<string>.Success(relativePath, "File uploaded successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Failure($"An error occurred while uploading file: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return ServiceResult.Success("No file to delete");
                }

                var fullPath = Path.Combine(_environment.WebRootPath, filePath);

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return ServiceResult.Success("File deleted successfully");
                }

                return ServiceResult.Success("File does not exist");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred while deleting file: {ex.Message}");
            }
        }

        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            return $"/{filePath.Replace("\\", "/")}";
        }

        public async Task<ServiceResult<bool>> ValidateFileAsync(
            IFormFile file,
            string[] allowedExtensions,
            long maxSizeInBytes)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return ServiceResult<bool>.Failure("No file uploaded");
                }

                // Check file size
                if (file.Length > maxSizeInBytes)
                {
                    var maxSizeInMB = maxSizeInBytes / (1024 * 1024);
                    return ServiceResult<bool>.Failure($"File size exceeds maximum allowed size of {maxSizeInMB}MB");
                }

                // Check file extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    return ServiceResult<bool>.Failure($"File type not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
                }

                await Task.CompletedTask;
                return ServiceResult<bool>.Success(true, "File validation passed");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"An error occurred during file validation: {ex.Message}");
            }
        }
    }
}