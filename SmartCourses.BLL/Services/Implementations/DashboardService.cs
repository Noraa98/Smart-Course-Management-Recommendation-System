using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Services.Interfaces;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities.Identity;

namespace SmartCourses.BLL.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public DashboardService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ServiceResult<DashboardStatsDto>> GetAdminDashboardAsync()
        {
            try
            {
                var stats = new DashboardStatsDto();

                // Get total counts
                stats.TotalUsers = await _unitOfWork.Repository<ApplicationUser, string>().CountAsync();
                stats.TotalCourses = await _unitOfWork.Courses.CountAsync();
                stats.TotalEnrollments = await _unitOfWork.Enrollments.CountAsync();
                stats.TotalCategories = await _unitOfWork.Categories.CountAsync();

                // Get role-specific counts
                var students = await _userManager.GetUsersInRoleAsync("Student");
                var instructors = await _userManager.GetUsersInRoleAsync("Instructor");
                stats.StudentsCount = students.Count;
                stats.InstructorsCount = instructors.Count;

                // Get recent courses
                var recentCourses = await _unitOfWork.Courses.GetRecentCoursesAsync(5);
                stats.RecentCourses = _mapper.Map<List<CourseListDto>>(recentCourses);

                // Get top rated courses
                var topRatedCourses = await _unitOfWork.Courses.GetTopRatedCoursesAsync(5);
                stats.TopRatedCourses = _mapper.Map<List<CourseListDto>>(topRatedCourses);

                // Get recent enrollments (you'll need to add this to repository)
                var allEnrollments = await _unitOfWork.Enrollments.GetAllAsync();
                var recentEnrollments = allEnrollments
                    .OrderByDescending(e => e.EnrolledAt)
                    .Take(10);
                stats.RecentEnrollments = _mapper.Map<List<EnrollmentDto>>(recentEnrollments);

                return ServiceResult<DashboardStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                return ServiceResult<DashboardStatsDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<InstructorDashboardDto>> GetInstructorDashboardAsync(string instructorId)
        {
            try
            {
                var dashboard = new InstructorDashboardDto();

                // Get instructor courses
                var courses = await _unitOfWork.Courses.GetCoursesByInstructorAsync(instructorId);
                dashboard.TotalCourses = courses.Count();
                dashboard.PublishedCourses = courses.Count(c => c.IsPublished);

                // Get total enrollments across all instructor courses
                var courseIds = courses.Select(c => c.Id).ToList();
                var enrollments = await _unitOfWork.Enrollments.GetAllAsync();
                dashboard.TotalEnrollments = enrollments.Count(e => courseIds.Contains(e.CourseId));

                // Get reviews
                var reviews = await _unitOfWork.Reviews.GetAllAsync();
                var instructorReviews = reviews.Where(r => courseIds.Contains(r.CourseId)).ToList();
                dashboard.TotalReviews = instructorReviews.Count;
                dashboard.AverageRating = instructorReviews.Any()
                    ? instructorReviews.Average(r => r.Rating)
                    : 0;

                // Map courses
                dashboard.MyCourses = _mapper.Map<List<CourseListDto>>(courses);

                // Get recent reviews
                var recentReviews = instructorReviews
                    .OrderByDescending(r => r.CreatedOn)
                    .Take(10);
                dashboard.RecentReviews = _mapper.Map<List<ReviewDto>>(recentReviews);

                return ServiceResult<InstructorDashboardDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return ServiceResult<InstructorDashboardDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<StudentDashboardDto>> GetStudentDashboardAsync(string studentId)
        {
            try
            {
                var dashboard = new StudentDashboardDto();

                // Get enrollments
                var enrollments = await _unitOfWork.Enrollments.GetUserEnrollmentsAsync(studentId);
                dashboard.EnrolledCoursesCount = enrollments.Count();
                dashboard.CompletedCoursesCount = enrollments.Count(e => e.IsCompleted);
                dashboard.InProgressCoursesCount = enrollments.Count(e => !e.IsCompleted);

                // Calculate overall progress
                if (enrollments.Any())
                {
                    dashboard.OverallProgress = enrollments.Average(e => e.ProgressPercent);
                }

                // Recent enrollments
                var recentEnrollments = enrollments
                    .OrderByDescending(e => e.EnrolledAt)
                    .Take(5);
                dashboard.RecentEnrollments = _mapper.Map<List<EnrollmentDto>>(recentEnrollments);

                // In progress courses
                var inProgressEnrollments = await _unitOfWork.Enrollments.GetInProgressEnrollmentsAsync(studentId);
                dashboard.InProgressCourses = _mapper.Map<List<EnrollmentDto>>(inProgressEnrollments.Take(5));

                // Get recommended courses (simple implementation)
                var publishedCourses = await _unitOfWork.Courses.GetPublishedCoursesAsync();
                var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToList();
                var recommendedCourses = publishedCourses
                    .Where(c => !enrolledCourseIds.Contains(c.Id))
                    .OrderByDescending(c => c.Enrollments.Count)
                    .Take(6);
                dashboard.RecommendedCourses = _mapper.Map<List<CourseListDto>>(recommendedCourses);

                return ServiceResult<StudentDashboardDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return ServiceResult<StudentDashboardDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}