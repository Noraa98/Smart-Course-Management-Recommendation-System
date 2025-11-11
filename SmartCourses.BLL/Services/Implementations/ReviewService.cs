using AutoMapper;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ReviewDto>> GetByIdAsync(int id)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(
                    id,
                    r => r.User,
                    r => r.Course);

                if (review == null)
                {
                    return ServiceResult<ReviewDto>.Failure("Review not found");
                }

                var reviewDto = _mapper.Map<ReviewDto>(review);
                return ServiceResult<ReviewDto>.Success(reviewDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<ReviewDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<ReviewDto>>> GetCourseReviewsAsync(int courseId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetCourseReviewsAsync(courseId);
                var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);

                return ServiceResult<List<ReviewDto>>.Success(reviewDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<ReviewDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<ReviewDto>>> GetUserReviewsAsync(string userId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetUserReviewsAsync(userId);
                var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);

                return ServiceResult<List<ReviewDto>>.Success(reviewDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<ReviewDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ReviewDto>> CreateAsync(ReviewCreateDto reviewDto, string userId)
        {
            try
            {
                // ✅ Validate rating
                if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
                {
                    return ServiceResult<ReviewDto>.Failure("Rating must be between 1 and 5");
                }
                // Check if course exists
                var course = await _unitOfWork.Courses.GetByIdAsync(reviewDto.CourseId);
                if (course == null)
                {
                    return ServiceResult<ReviewDto>.Failure("Course not found");
                }

                // Check if user is enrolled in the course
                var isEnrolled = await _unitOfWork.Enrollments.IsUserEnrolledAsync(userId, reviewDto.CourseId);
                if (!isEnrolled)
                {
                    return ServiceResult<ReviewDto>.Failure("You must be enrolled in the course to leave a review");
                }

                // Check if user already reviewed this course
                var hasReviewed = await _unitOfWork.Reviews.HasUserReviewedCourseAsync(userId, reviewDto.CourseId);
                if (hasReviewed)
                {
                    return ServiceResult<ReviewDto>.Failure("You have already reviewed this course");
                }

                var review = _mapper.Map<Review>(reviewDto);
                review.UserId = userId;
                review.CreatedBy = userId;
                review.LastModifiedBy = userId;
                review.CreatedOn = DateTime.UtcNow;
                review.LastModifiedOn = DateTime.UtcNow;

                await _unitOfWork.Reviews.AddAsync(review);
                await _unitOfWork.SaveChangesAsync();

                // Reload with relationships
                var createdReview = await _unitOfWork.Reviews.GetByIdAsync(
                    review.Id,
                    r => r.User,
                    r => r.Course);

                var reviewDtoResult = _mapper.Map<ReviewDto>(createdReview);
                return ServiceResult<ReviewDto>.Success(reviewDtoResult, "Review submitted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<ReviewDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ReviewDto>> UpdateAsync(ReviewUpdateDto reviewDto, string userId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(reviewDto.Id);
                if (review == null)
                {
                    return ServiceResult<ReviewDto>.Failure("Review not found");
                }

                if (review.UserId != userId)
                {
                    return ServiceResult<ReviewDto>.Failure("You are not authorized to update this review");
                }

                review.Rating = reviewDto.Rating;
                review.Comment = reviewDto.Comment;
                review.LastModifiedBy = userId;
                review.LastModifiedOn = DateTime.UtcNow;

                _unitOfWork.Reviews.Update(review);
                await _unitOfWork.SaveChangesAsync();

                // Reload with relationships
                var updatedReview = await _unitOfWork.Reviews.GetByIdAsync(
                    review.Id,
                    r => r.User,
                    r => r.Course);

                var reviewDtoResult = _mapper.Map<ReviewDto>(updatedReview);
                return ServiceResult<ReviewDto>.Success(reviewDtoResult, "Review updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<ReviewDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id, string userId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(id);
                if (review == null)
                {
                    return ServiceResult.Failure("Review not found");
                }

                if (review.UserId != userId)
                {
                    return ServiceResult.Failure("You are not authorized to delete this review");
                }

                _unitOfWork.Reviews.SoftDelete(review);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Review deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<double>> GetCourseAverageRatingAsync(int courseId)
        {
            try
            {
                var averageRating = await _unitOfWork.Reviews.GetCourseAverageRatingAsync(courseId);
                return ServiceResult<double>.Success(averageRating);
            }
            catch (Exception ex)
            {
                return ServiceResult<double>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> HasUserReviewedCourseAsync(string userId, int courseId)
        {
            try
            {
                var hasReviewed = await _unitOfWork.Reviews.HasUserReviewedCourseAsync(userId, courseId);
                return ServiceResult<bool>.Success(hasReviewed);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
