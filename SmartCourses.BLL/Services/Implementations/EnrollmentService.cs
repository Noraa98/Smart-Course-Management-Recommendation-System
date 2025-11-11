using AutoMapper;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        // Query Methods
        public async Task<ServiceResult<EnrollmentDto>> GetByIdAsync(int id)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(
                    id,
                    e => e.Course,
                    e => e.User);

                if (enrollment == null)
                {
                    return ServiceResult<EnrollmentDto>.Failure("Enrollment not found");
                }

                var enrollmentDto = _mapper.Map<EnrollmentDto>(enrollment);
                return ServiceResult<EnrollmentDto>.Success(enrollmentDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<EnrollmentDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<EnrollmentDetailsDto>> GetEnrollmentDetailsAsync(int enrollmentId, string userId)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetEnrollmentWithProgressAsync(enrollmentId);

                if (enrollment == null)
                {
                    return ServiceResult<EnrollmentDetailsDto>.Failure("Enrollment not found");
                }

                // Verify user owns this enrollment
                if (enrollment.UserId != userId)
                {
                    return ServiceResult<EnrollmentDetailsDto>.Failure("You are not authorized to view this enrollment");
                }

                var enrollmentDto = _mapper.Map<EnrollmentDetailsDto>(enrollment);
                return ServiceResult<EnrollmentDetailsDto>.Success(enrollmentDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<EnrollmentDetailsDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<EnrollmentDto>>> GetUserEnrollmentsAsync(string userId)
        {
            try
            {
                var enrollments = await _unitOfWork.Enrollments.GetUserEnrollmentsAsync(userId);
                var enrollmentDtos = _mapper.Map<List<EnrollmentDto>>(enrollments);

                return ServiceResult<List<EnrollmentDto>>.Success(enrollmentDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<EnrollmentDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<EnrollmentDto>>> GetCourseEnrollmentsAsync(int courseId)
        {
            try
            {
                var enrollments = await _unitOfWork.Enrollments.GetCourseEnrollmentsAsync(courseId);
                var enrollmentDtos = _mapper.Map<List<EnrollmentDto>>(enrollments);

                return ServiceResult<List<EnrollmentDto>>.Success(enrollmentDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<EnrollmentDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<EnrollmentDto>>> GetCompletedEnrollmentsAsync(string userId)
        {
            try
            {
                var enrollments = await _unitOfWork.Enrollments.GetCompletedEnrollmentsAsync(userId);
                var enrollmentDtos = _mapper.Map<List<EnrollmentDto>>(enrollments);

                return ServiceResult<List<EnrollmentDto>>.Success(enrollmentDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<EnrollmentDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<EnrollmentDto>>> GetInProgressEnrollmentsAsync(string userId)
        {
            try
            {
                var enrollments = await _unitOfWork.Enrollments.GetInProgressEnrollmentsAsync(userId);
                var enrollmentDtos = _mapper.Map<List<EnrollmentDto>>(enrollments);

                return ServiceResult<List<EnrollmentDto>>.Success(enrollmentDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<EnrollmentDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> IsUserEnrolledAsync(string userId, int courseId)
        {
            try
            {
                var isEnrolled = await _unitOfWork.Enrollments.IsUserEnrolledAsync(userId, courseId);
                return ServiceResult<bool>.Success(isEnrolled);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"An error occurred: {ex.Message}");
            }
        }

        // Command Methods
        public async Task<ServiceResult<EnrollmentDto>> EnrollAsync(EnrollmentCreateDto enrollmentDto, string userId)
        {
            try
            {
                // Check if course exists
                var course = await _unitOfWork.Courses.GetByIdAsync(enrollmentDto.CourseId);
                if (course == null)
                {
                    return ServiceResult<EnrollmentDto>.Failure("Course not found");
                }

                // Check if course is published
                if (!course.IsPublished)
                {
                    return ServiceResult<EnrollmentDto>.Failure("Cannot enroll in unpublished course");
                }

                // Check if already enrolled
                var isEnrolled = await _unitOfWork.Enrollments.IsUserEnrolledAsync(userId, enrollmentDto.CourseId);
                if (isEnrolled)
                {
                    return ServiceResult<EnrollmentDto>.Failure("You are already enrolled in this course");
                }

                // Create enrollment
                var enrollment = new Enrollment
                {
                    UserId = userId,
                    CourseId = enrollmentDto.CourseId,
                    EnrolledAt = DateTime.UtcNow,
                    ProgressPercent = 0,
                    IsCompleted = false,
                    CreatedBy = userId,
                    LastModifiedBy = userId,
                    CreatedOn = DateTime.UtcNow,
                    LastModifiedOn = DateTime.UtcNow
                };

                await _unitOfWork.Enrollments.AddAsync(enrollment);
                await _unitOfWork.SaveChangesAsync();

                // Reload with relationships
                var createdEnrollment = await _unitOfWork.Enrollments.GetByIdAsync(
                    enrollment.Id,
                    e => e.Course,
                    e => e.User);

                var enrollmentDtoResult = _mapper.Map<EnrollmentDto>(createdEnrollment);
                return ServiceResult<EnrollmentDto>.Success(enrollmentDtoResult, "Enrolled successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<EnrollmentDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UnenrollAsync(int enrollmentId, string userId)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(enrollmentId);
                if (enrollment == null)
                {
                    return ServiceResult.Failure("Enrollment not found");
                }

                if (enrollment.UserId != userId)
                {
                    return ServiceResult.Failure("You are not authorized to unenroll from this course");
                }

                // Check if course is completed
                if (enrollment.IsCompleted)
                {
                    return ServiceResult.Failure("Cannot unenroll from completed course");
                }

                _unitOfWork.Enrollments.Delete(enrollment);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Unenrolled successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        // Progress Management
        public async Task<ServiceResult> UpdateLessonProgressAsync(LessonProgressUpdateDto progressDto, string userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Verify enrollment belongs to user
                var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(progressDto.EnrollmentId);
                if (enrollment == null)
                {
                    return ServiceResult.Failure("Enrollment not found");
                }

                if (enrollment.UserId != userId)
                {
                    return ServiceResult.Failure("You are not authorized to update this progress");
                }

                // Get or create lesson progress
                var lessonProgress = await _unitOfWork.LessonProgresses.GetProgressAsync(
                    progressDto.EnrollmentId,
                    progressDto.LessonId);

                if (lessonProgress == null)
                {
                    lessonProgress = new LessonProgress
                    {
                        EnrollmentId = progressDto.EnrollmentId,
                        LessonId = progressDto.LessonId,
                        IsCompleted = progressDto.IsCompleted,
                        WatchedSeconds = progressDto.WatchedSeconds,
                        CompletedAt = progressDto.IsCompleted ? DateTime.UtcNow : null,
                        CreatedBy = userId,
                        LastModifiedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        LastModifiedOn = DateTime.UtcNow
                    };
                    await _unitOfWork.LessonProgresses.AddAsync(lessonProgress);
                }
                else
                {
                    lessonProgress.IsCompleted = progressDto.IsCompleted;
                    lessonProgress.WatchedSeconds = progressDto.WatchedSeconds;
                    lessonProgress.CompletedAt = progressDto.IsCompleted ? DateTime.UtcNow : lessonProgress.CompletedAt;
                    lessonProgress.LastModifiedBy = userId;
                    lessonProgress.LastModifiedOn = DateTime.UtcNow;

                    _unitOfWork.LessonProgresses.Update(lessonProgress);
                }

                // Update enrollment progress
                var progress = await _unitOfWork.LessonProgresses.CalculateEnrollmentProgressAsync(progressDto.EnrollmentId);
                enrollment.ProgressPercent = progress;
                enrollment.LastModifiedBy = userId;
                enrollment.LastModifiedOn = DateTime.UtcNow;

                // Check if course is completed
                if (progress >= 100 && !enrollment.IsCompleted)
                {
                    enrollment.IsCompleted = true;
                    enrollment.CompletedAt = DateTime.UtcNow;
                }

                _unitOfWork.Enrollments.Update(enrollment);


                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ServiceResult.Success("Progress updated successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<decimal>> CalculateProgressAsync(int enrollmentId)
        {
            try
            {
                var progress = await _unitOfWork.LessonProgresses.CalculateEnrollmentProgressAsync(enrollmentId);
                return ServiceResult<decimal>.Success(progress);
            }
            catch (Exception ex)
            {
                return ServiceResult<decimal>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> MarkEnrollmentAsCompleteAsync(int enrollmentId, string userId)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(enrollmentId);
                if (enrollment == null)
                {
                    return ServiceResult.Failure("Enrollment not found");
                }

                if (enrollment.UserId != userId)
                {
                    return ServiceResult.Failure("You are not authorized to complete this enrollment");
                }

                enrollment.IsCompleted = true;
                enrollment.CompletedAt = DateTime.UtcNow;
                enrollment.ProgressPercent = 100;
                enrollment.LastModifiedBy = userId;
                enrollment.LastModifiedOn = DateTime.UtcNow;

                _unitOfWork.Enrollments.Update(enrollment);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Course marked as completed");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}