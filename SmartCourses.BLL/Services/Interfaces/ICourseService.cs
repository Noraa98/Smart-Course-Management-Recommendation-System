using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface ICourseService 
    {
        Task<ServiceResult<CourseDto>> GetByIdAsync(int id);
        Task<ServiceResult<CourseDto>> GetCourseWithDetailsAsync(int id);
        Task<ServiceResult<PaginatedResultDto<CourseListDto>>> GetPagedAsync(CourseFilterDto filter);
        Task<ServiceResult<List<CourseListDto>>> GetPublishedCoursesAsync();
        Task<ServiceResult<List<CourseListDto>>> GetInstructorCoursesAsync(string instructorId);
        Task<ServiceResult<List<CourseListDto>>> GetCoursesByCategoryAsync(int categoryId);
        Task<ServiceResult<List<CourseListDto>>> SearchCoursesAsync(string searchTerm);
        Task<ServiceResult<CourseDto>> CreateAsync(CourseCreateDto createDto, string instructorId);
        Task<ServiceResult<CourseDto>> UpdateAsync(CourseUpdateDto updateDto, string instructorId);
        Task<ServiceResult> DeleteAsync(int id, string instructorId);
        Task<ServiceResult> PublishCourseAsync(int courseId, string instructorId);
        Task<ServiceResult> UnpublishCourseAsync(int courseId, string instructorId);

        // Section & Lesson Management
        Task<ServiceResult<SectionDto>> AddSectionAsync(SectionCreateDto sectionDto, string instructorId);
        Task<ServiceResult<LessonDto>> AddLessonAsync(LessonCreateDto lessonDto, string instructorId);
        Task<ServiceResult> DeleteSectionAsync(int sectionId, string instructorId);
        Task<ServiceResult> DeleteLessonAsync(int lessonId, string instructorId);

        // Statistics
        Task<ServiceResult<List<CourseListDto>>> GetTopRatedCoursesAsync(int count = 10);
        Task<ServiceResult<List<CourseListDto>>> GetMostEnrolledCoursesAsync(int count = 10);
        Task<ServiceResult<List<CourseListDto>>> GetRecentCoursesAsync(int count = 10);

    }
}
