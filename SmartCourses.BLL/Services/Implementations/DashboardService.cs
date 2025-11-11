using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SmartCourses.BLL.Models.DTOs;
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
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                stats.StudentsCount = students.Count;
                stats.InstructorsCount = instructors.Count;
                stats.AdminsCount = admins.Count;

                // Get course counts by publish status
                var allCourses = await _unitOfWork.Courses.GetAllAsync();
                stats.PublishedCoursesCount = allCourses.Count(c => c.IsPublished);
                stats.UnpublishedCoursesCount = allCourses.Count(c => !c.IsPublished);

                //  repository method  GetAllAsync
                var recentCourses = await _unitOfWork.Courses.GetRecentCoursesAsync(5);
                stats.RecentCourses = _mapper.Map<List<CourseListDto>>(recentCourses);

                var topRatedCourses = await _unitOfWork.Courses.GetTopRatedCoursesAsync(5);
                stats.TopRatedCourses = _mapper.Map<List<CourseListDto>>(topRatedCourses);

               
                var recentEnrollments = await _unitOfWork.Enrollments.GetRecentEnrollmentsAsync(10);
                stats.RecentEnrollments = _mapper.Map<List<EnrollmentDto>>(recentEnrollments);

                // Calculate total revenue (if needed)
                var paidCourses = allCourses.Where(c => c.Price.HasValue && c.Price > 0);
                stats.TotalRevenue = paidCourses.Sum(c => c.Price ?? 0) * stats.TotalEnrollments; // تقدير

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
                dashboard.UnpublishedCourses = courses.Count(c => !c.IsPublished);

                var courseIds = courses.Select(c => c.Id).ToList();

                
                dashboard.TotalEnrollments = await _unitOfWork.Enrollments
                    .GetEnrollmentCountByCourseIdsAsync(courseIds);

                // Get enrollments for detailed stats
                var enrollments = await _unitOfWork.Enrollments
                    .FindAsync(e => courseIds.Contains(e.CourseId));
                dashboard.ActiveEnrollments = enrollments.Count(e => !e.IsCompleted);
                dashboard.CompletedEnrollments = enrollments.Count(e => e.IsCompleted);

                
                dashboard.TotalReviews = await _unitOfWork.Reviews
                    .GetReviewCountByCourseIdsAsync(courseIds);

                dashboard.AverageRating = await _unitOfWork.Reviews
                    .GetAverageRatingByCourseIdsAsync(courseIds);

                // Calculate revenue
                var paidCourses = courses.Where(c => c.Price.HasValue && c.Price > 0);
                dashboard.TotalRevenue = paidCourses.Sum(c => c.Price ?? 0) * dashboard.TotalEnrollments;

                // Map courses
                dashboard.MyCourses = _mapper.Map<List<CourseListDto>>(courses);

                // Get recent reviews
                var instructorReviews = await _unitOfWork.Reviews
                    .GetReviewsByCourseIdsAsync(courseIds);

                dashboard.RecentReviews = _mapper.Map<List<ReviewDto>>(
                    instructorReviews.Take(10));

                // Get recent enrollments
                var recentEnrollments = enrollments
                    .OrderByDescending(e => e.EnrolledAt)
                    .Take(10);
                dashboard.RecentEnrollments = _mapper.Map<List<EnrollmentDto>>(recentEnrollments);

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
                dashboard.RecentEnrollments = _mapper.Map<List<EnrollmentDto>>(
                    enrollments.OrderByDescending(e => e.EnrolledAt).Take(5));

                // In progress courses
                var inProgressEnrollments = await _unitOfWork.Enrollments
                    .GetInProgressEnrollmentsAsync(studentId);
                dashboard.InProgressCourses = _mapper.Map<List<EnrollmentDto>>(
                    inProgressEnrollments.Take(5));

                // Completed courses
                var completedEnrollments = await _unitOfWork.Enrollments
                    .GetCompletedEnrollmentsAsync(studentId);
                dashboard.CompletedCourses = _mapper.Map<List<CourseListDto>>(
                    completedEnrollments.Take(5).Select(e => e.Course));

                // Get recommended courses (simple implementation)
                var publishedCourses = await _unitOfWork.Courses.GetPublishedCoursesAsync();
                var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToList();
                var recommendedCourses = publishedCourses
                    .Where(c => !enrolledCourseIds.Contains(c.Id))
                    .OrderByDescending(c => c.Enrollments.Count)
                    .Take(6);
                dashboard.RecommendedCourses = _mapper.Map<List<CourseListDto>>(recommendedCourses);

                // Optional: Calculate watched hours
                dashboard.TotalWatchedHours = enrollments
                    .Where(e => e.Course?.DurationInHours != null)
                    .Sum(e => (int)(e.Course!.DurationInHours * (e.ProgressPercent / 100)));

                return ServiceResult<StudentDashboardDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                return ServiceResult<StudentDashboardDto>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}