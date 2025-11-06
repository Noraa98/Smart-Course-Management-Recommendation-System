using Microsoft.AspNetCore.Http;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Services.Interfaces
{
    public interface IFileService
    {
        Task<ServiceResult<string>> UploadFileAsync(IFormFile file, string folder);
        Task<ServiceResult> DeleteFileAsync(string filePath);
        string GetFileUrl(string filePath);
        Task<ServiceResult<bool>> ValidateFileAsync(IFormFile file, string[] allowedExtensions, long maxSizeInBytes);

    }
}
