using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface IEnrollmentService 
    {
        Task<ServiceResult<EnrollmentDto>> GetByIdAsync(int id);
        Task<ServiceResult<EnrollmentDetailsDto>> GetEnrollmentDetailsAsync(int enrollmentId, string userId);
        Task<ServiceResult<List<EnrollmentDto>>> GetUserEnrollmentsAsync(string userId);
        Task<ServiceResult<List<EnrollmentDto>>> GetCourseEnrollmentsAsync(int courseId);
        Task<ServiceResult<EnrollmentDto>> EnrollAsync(EnrollmentCreateDto enrollmentDto, string userId);
        Task<ServiceResult> UnenrollAsync(int enrollmentId, string userId);
        Task<ServiceResult<bool>> IsUserEnrolledAsync(string userId, int courseId);

        // Progress Management
        Task<ServiceResult> UpdateLessonProgressAsync(LessonProgressUpdateDto progressDto, string userId);
        Task<ServiceResult<decimal>> CalculateProgressAsync(int enrollmentId);
        Task<ServiceResult> MarkEnrollmentAsCompleteAsync(int enrollmentId, string userId);

        // Statistics
        Task<ServiceResult<List<EnrollmentDto>>> GetCompletedEnrollmentsAsync(string userId);
        Task<ServiceResult<List<EnrollmentDto>>> GetInProgressEnrollmentsAsync(string userId);

    }
}
