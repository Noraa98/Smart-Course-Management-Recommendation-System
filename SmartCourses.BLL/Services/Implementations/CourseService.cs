using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Response_ResultDTOs;
using SmartCourses.BLL.Services.Contracts;
using SmartCourses.DAL.Common.Enums;
using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Entities.RelationshipsTables;
using SmartCourses.DAL.Persistence.Data;

namespace SmartCourses.BLL.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CourseService(IUnitOfWork unitOfWork, IMapper mapper ,ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        // Query Methods
        public async Task<ServiceResult<CourseDto>> GetByIdAsync(int id)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(
                    id,
                    c => c.Category,
                    c => c.Instructor,
                    c => c.CourseSkills,
                    c => c.Enrollments,
                    c => c.Reviews);

                if (course == null)
                {
                    return ServiceResult<CourseDto>.Failure("Course not found");
                }

                var courseDto = _mapper.Map<CourseDto>(course);
                return ServiceResult<CourseDto>.Success(courseDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<CourseDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CourseDto>> GetCourseWithDetailsAsync(int id)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);

                if (course == null)
                {
                    return ServiceResult<CourseDto>.Failure("Course not found");
                }

                var courseDto = _mapper.Map<CourseDto>(course);
                return ServiceResult<CourseDto>.Success(courseDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<CourseDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PaginatedResultDto<CourseListDto>>> GetPagedAsync(CourseFilterDto filter)
        {
            try
            {
                // ✅ استخدم IQueryable للفلترة في قاعدة البيانات
                IQueryable<Course> query = _context.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Include(c => c.CourseSkills)
                        .ThenInclude(cs => cs.Skill)
                    .Include(c => c.Enrollments)
                    .Include(c => c.Reviews)
                    .Where(c => c.IsPublished && !c.IsDeleted);

                // Apply filters
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(c =>
                        c.Title.Contains(filter.SearchTerm) ||
                        c.Description.Contains(filter.SearchTerm));
                }

                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(c => c.CategoryId == filter.CategoryId.Value);
                }

                if (filter.Level.HasValue)
                {
                    query = query.Where(c => (int)c.Level == filter.Level.Value);
                }

                if (filter.SkillId.HasValue)
                {
                    query = query.Where(c => c.CourseSkills.Any(cs => cs.SkillId == filter.SkillId.Value));
                }

                if (filter.MinPrice.HasValue)
                {
                    query = query.Where(c => c.Price >= filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue)
                {
                    query = query.Where(c => c.Price <= filter.MaxPrice.Value);
                }

                // Apply sorting
                query = filter.SortBy?.ToLower() switch
                {
                    "title" => filter.SortOrder == "desc"
                        ? query.OrderByDescending(c => c.Title)
                        : query.OrderBy(c => c.Title),
                    "price" => filter.SortOrder == "desc"
                        ? query.OrderByDescending(c => c.Price)
                        : query.OrderBy(c => c.Price),
                    _ => query.OrderByDescending(c => c.CreatedOn)
                };

                // Get total count BEFORE pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var pagedCourses = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync(); // ✅ فقط هنا نجلب البيانات

                var courseDtos = _mapper.Map<List<CourseListDto>>(pagedCourses);

                var paginatedResult = new PaginatedResultDto<CourseListDto>(
                    courseDtos,
                    totalCount,
                    filter.PageNumber,
                    filter.PageSize);

                return ServiceResult<PaginatedResultDto<CourseListDto>>.Success(paginatedResult);
            }
            catch (Exception ex)
            {
                return ServiceResult<PaginatedResultDto<CourseListDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
        public async Task<ServiceResult<List<CourseListDto>>> GetPublishedCoursesAsync()
        {
            try
            {
                var courses = await _unitOfWork.Courses.GetPublishedCoursesAsync();
                var courseDtos = _mapper.Map<List<CourseListDto>>(courses);
                
                return ServiceResult<List<CourseListDto>>.Success(courseDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CourseListDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<CourseListDto>>> GetInstructorCoursesAsync(string instructorId)
        {
            try
            {
                var courses = await _unitOfWork.Courses.GetCoursesByInstructorAsync(instructorId);
                var courseDtos = _mapper.Map<List<CourseListDto>>(courses);
                
                return ServiceResult<List<CourseListDto>>.Success(courseDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CourseListDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<CourseListDto>>> GetCoursesByCategoryAsync(int categoryId)
        {
            try
            {
                var courses = await _unitOfWork.Courses.GetCoursesByCategoryAsync(categoryId);
                var courseDtos = _mapper.Map<List<CourseListDto>>(courses);
                
                return ServiceResult<List<CourseListDto>>.Success(courseDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CourseListDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<CourseListDto>>> SearchCoursesAsync(string searchTerm)
        {
            try
            {
                var courses = await _unitOfWork.Courses.SearchCoursesAsync(searchTerm);
                var courseDtos = _mapper.Map<List<CourseListDto>>(courses);
                
                return ServiceResult<List<CourseListDto>>.Success(courseDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CourseListDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        // Command Methods - CRUD
        public async Task<ServiceResult<CourseDto>> CreateAsync(CourseCreateDto createDto, string instructorId)
        {
            try
            {
                // Validate category exists
                var category = await _unitOfWork.Categories.GetByIdAsync(createDto.CategoryId);
                if (category == null)
                {
                    return ServiceResult<CourseDto>.Failure("Category not found");
                }

                // Map DTO to entity
                var course = _mapper.Map<Course>(createDto);
                course.InstructorId = instructorId;
                course.IsPublished = false;
                course.CreatedBy = instructorId;
                course.LastModifiedBy = instructorId;
                course.CreatedOn = DateTime.UtcNow;
                course.LastModifiedOn = DateTime.UtcNow;

                // Add course
                await _unitOfWork.Courses.AddAsync(course);
                await _unitOfWork.SaveChangesAsync();

                // Add skills if provided
                if (createDto.SkillIds != null && createDto.SkillIds.Any())
                {
                    await AddCourseSkillsAsync(course.Id, createDto.SkillIds);
                }

                // Reload course with relationships
                var createdCourse = await _unitOfWork.Courses.GetCourseWithDetailsAsync(course.Id);
                var courseDto = _mapper.Map<CourseDto>(createdCourse);

                return ServiceResult<CourseDto>.Success(courseDto, "Course created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<CourseDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CourseDto>> UpdateAsync(CourseUpdateDto updateDto, string instructorId)
        {
            try
            {
                // Get existing course
                var course = await _unitOfWork.Courses.GetByIdAsync(updateDto.Id);
                if (course == null)
                {
                    return ServiceResult<CourseDto>.Failure("Course not found");
                }

                // Check if instructor owns the course
                if (course.InstructorId != instructorId)
                {
                    return ServiceResult<CourseDto>.Failure("You are not authorized to update this course");
                }

                // Validate category
                var category = await _unitOfWork.Categories.GetByIdAsync(updateDto.CategoryId);
                if (category == null)
                {
                    return ServiceResult<CourseDto>.Failure("Category not found");
                }

                // Update course properties
                course.Title = updateDto.Title;
                course.Description = updateDto.Description;
                course.ShortDescription = updateDto.ShortDescription;
                course.ThumbnailPath = updateDto.ThumbnailPath;
                course.Level = (CourseLevel)updateDto.Level;
                course.Price = updateDto.Price;
                course.DurationInHours = updateDto.DurationInHours;
                course.CategoryId = updateDto.CategoryId;
                course.IsPublished = updateDto.IsPublished;
                course.LastModifiedBy = instructorId;
                course.LastModifiedOn = DateTime.UtcNow;

                _unitOfWork.Courses.Update(course);

                // Update skills
                if (updateDto.SkillIds != null)
                {
                    await UpdateCourseSkillsAsync(course.Id, updateDto.SkillIds);
                }

                await _unitOfWork.SaveChangesAsync();

                // Reload with relationships
                var updatedCourse = await _unitOfWork.Courses.GetCourseWithDetailsAsync(course.Id);
                var courseDto = _mapper.Map<CourseDto>(updatedCourse);

                return ServiceResult<CourseDto>.Success(courseDto, "Course updated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<CourseDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id, string instructorId)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(id);
                if (course == null)
                {
                    return ServiceResult.Failure("Course not found");
                }

                // Check ownership
                if (course.InstructorId != instructorId)
                {
                    return ServiceResult.Failure("You are not authorized to delete this course");
                }

                // Check if course has enrollments
                var hasEnrollments = await _unitOfWork.Enrollments
                    .AnyAsync(e => e.CourseId == id);

                if (hasEnrollments)
                {
                    return ServiceResult.Failure("Cannot delete course with existing enrollments");
                }

                _unitOfWork.Courses.SoftDelete(course);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Course deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> PublishCourseAsync(int courseId, string instructorId)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(courseId);
                if (course == null)
                {
                    return ServiceResult.Failure("Course not found");
                }

                if (course.InstructorId != instructorId)
                {
                    return ServiceResult.Failure("You are not authorized to publish this course");
                }

                // Validate course has content
                if (!course.Sections.Any())
                {
                    return ServiceResult.Failure("Cannot publish course without sections");
                }

                if (!course.Sections.Any(s => s.Lessons.Any()))
                {
                    return ServiceResult.Failure("Cannot publish course without lessons");
                }

                course.IsPublished = true;
                course.LastModifiedBy = instructorId;
                course.LastModifiedOn = DateTime.UtcNow;

                _unitOfWork.Courses.Update(course);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Course published successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UnpublishCourseAsync(int courseId, string instructorId)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
                if (course == null)
                {
                    return ServiceResult.Failure("Course not found");
                }

                if (course.InstructorId != instructorId)
                {
                    return ServiceResult.Failure("You are not authorized to unpublish this course");
                }

                course.IsPublished = false;
                course.LastModifiedBy = instructorId;
                course.LastModifiedOn = DateTime.UtcNow;

                _unitOfWork.Courses.Update(course);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Course unpublished successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

    
        // Private Helper Methods
        private async Task AddCourseSkillsAsync(int courseId, List<int> skillIds)
        {
            var courseSkillRepo = _unitOfWork.Repository<CourseSkill, (int, int)>();

            foreach (var skillId in skillIds)
            {
                var skill = await _unitOfWork.Skills.GetByIdAsync(skillId);
                if (skill != null)
                {
                    await courseSkillRepo.AddAsync(new CourseSkill
                    {
                        CourseId = courseId,
                        SkillId = skillId
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task UpdateCourseSkillsAsync(int courseId, List<int> skillIds)
        {
            var courseSkillRepo = _unitOfWork.Repository<CourseSkill, (int, int)>();

            // Remove existing skills
            var existingSkills = await courseSkillRepo.FindAsync(cs => cs.CourseId == courseId);
            courseSkillRepo.DeleteRange(existingSkills);

            // Add new skills
            foreach (var skillId in skillIds)
            {
                var skill = await _unitOfWork.Skills.GetByIdAsync(skillId);
                if (skill != null)
                {
                    await courseSkillRepo.AddAsync(new CourseSkill
                    {
                        CourseId = courseId,
                        SkillId = skillId
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ServiceResult<SectionDto>> AddSectionAsync(SectionCreateDto sectionDto, string instructorId)
        {
            try
            {
                // Verify course exists and instructor owns it
                var course = await _unitOfWork.Courses.GetByIdAsync(sectionDto.CourseId);
                if (course == null)
                {
                    return ServiceResult<SectionDto>.Failure("Course not found");
                }

                if (course.InstructorId != instructorId)
                {
                    return ServiceResult<SectionDto>.Failure("You are not authorized to add sections to this course");
                }

                var section = _mapper.Map<Section>(sectionDto);
                section.CreatedBy = instructorId;
                section.LastModifiedBy = instructorId;
                section.CreatedOn = DateTime.UtcNow;
                section.LastModifiedOn = DateTime.UtcNow;

                var sectionRepo = _unitOfWork.Repository<Section, int>();
                await sectionRepo.AddAsync(section);
                await _unitOfWork.SaveChangesAsync();

                var createdSection = await sectionRepo.GetByIdAsync(section.Id, s => s.Lessons);
                var sectionDtoResult = _mapper.Map<SectionDto>(createdSection);

                return ServiceResult<SectionDto>.Success(sectionDtoResult, "Section added successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<SectionDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<LessonDto>> AddLessonAsync(LessonCreateDto lessonDto, string instructorId)
        {
            try
            {
                var sectionRepo = _unitOfWork.Repository<Section, int>();
                var section = await sectionRepo.GetByIdAsync(
                    lessonDto.SectionId,
                    s => s.Course);

                if (section == null)
                {
                    return ServiceResult<LessonDto>.Failure("Section not found");
                }

                // Check if instructor owns the course
                if (section.Course.InstructorId != instructorId)
                {
                    return ServiceResult<LessonDto>.Failure("You are not authorized to add lessons to this section");
                }

                var lesson = _mapper.Map<Lesson>(lessonDto);
                lesson.CreatedBy = instructorId;
                lesson.LastModifiedBy = instructorId;
                lesson.CreatedOn = DateTime.UtcNow;
                lesson.LastModifiedOn = DateTime.UtcNow;

                var lessonRepo = _unitOfWork.Repository<Lesson, int>();
                await lessonRepo.AddAsync(lesson);
                await _unitOfWork.SaveChangesAsync();

                var lessonDtoResult = _mapper.Map<LessonDto>(lesson);

                return ServiceResult<LessonDto>.Success(lessonDtoResult, "Lesson added successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<LessonDto>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteSectionAsync(int sectionId, string instructorId)
        {
            try
            {
                var sectionRepo = _unitOfWork.Repository<Section, int>();
                var section = await sectionRepo.GetByIdAsync(
                    sectionId,
                    s => s.Course,
                    s => s.Lessons);

                if (section == null)
                {
                    return ServiceResult.Failure("Section not found");
                }

                if (section.Course.InstructorId != instructorId)
                {
                    return ServiceResult.Failure("You are not authorized to delete this section");
                }

                // Check if section has lessons
                if (section.Lessons.Any())
                {
                    return ServiceResult.Failure("Cannot delete section with existing lessons. Please delete lessons first.");
                }

                sectionRepo.SoftDelete(section);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Section deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteLessonAsync(int lessonId, string instructorId)
        {
            try
            {
                var lessonRepo = _unitOfWork.Repository<Lesson, int>();
                var lesson = await lessonRepo.GetByIdAsync(
                    lessonId,
                    l => l.Section.Course);

                if (lesson == null)
                {
                    return ServiceResult.Failure("Lesson not found");
                }

                if (lesson.Section.Course.InstructorId != instructorId)
                {
                    return ServiceResult.Failure("You are not authorized to delete this lesson");
                }

                lessonRepo.SoftDelete(lesson);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult.Success("Lesson deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"An error occurred: {ex.Message}");
            }
        }

     
        // Statistics Methods
        public async Task<ServiceResult<List<CourseListDto>>> GetTopRatedCoursesAsync(int count = 10)
        {
            try
            {
                var courses = await _unitOfWork.Courses.GetTopRatedCoursesAsync(count);
                var courseDtos = _mapper.Map<List<CourseListDto>>(courses);

                return ServiceResult<List<CourseListDto>>.Success(courseDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CourseListDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<CourseListDto>>> GetMostEnrolledCoursesAsync(int count = 10)
        {
            try
            {
                var courses = await _unitOfWork.Courses.GetMostEnrolledCoursesAsync(count);
                var courseDtos = _mapper.Map<List<CourseListDto>>(courses);

                return ServiceResult<List<CourseListDto>>.Success(courseDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CourseListDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<CourseListDto>>> GetRecentCoursesAsync(int count = 10)
        {
            try
            {
                var courses = await _unitOfWork.Courses.GetRecentCoursesAsync(count);
                var courseDtos = _mapper.Map<List<CourseListDto>>(courses);

                return ServiceResult<List<CourseListDto>>.Success(courseDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CourseListDto>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}