using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface IReviewService 
    {
        Task<ServiceResult<ReviewDto>> GetByIdAsync(int id);
        Task<ServiceResult<List<ReviewDto>>> GetCourseReviewsAsync(int courseId);
        Task<ServiceResult<List<ReviewDto>>> GetUserReviewsAsync(string userId);
        Task<ServiceResult<ReviewDto>> CreateAsync(ReviewCreateDto reviewDto, string userId);
        Task<ServiceResult<ReviewDto>> UpdateAsync(ReviewUpdateDto reviewDto, string userId);
        Task<ServiceResult> DeleteAsync(int id, string userId);
        Task<ServiceResult<double>> GetCourseAverageRatingAsync(int courseId);
        Task<ServiceResult<bool>> HasUserReviewedCourseAsync(string userId, int courseId);

    }
}