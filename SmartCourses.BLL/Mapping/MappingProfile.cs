using AutoMapper;
using SmartCourses.BLL.Models.DTOs;
using SmartCourses.BLL.Models.DTOs.CourseDTOs;
using SmartCourses.BLL.Models.DTOs.Enrollment_ReviewDTOs;
using SmartCourses.BLL.Models.DTOs.User_AuthenticationDTOs;
using SmartCourses.DAL.Common.Enums;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Entities.Identity;
using SmartCourses.DAL.Entities.RelationshipsTables;

namespace SmartCourses.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()) // Will be populated separately
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.UserSkills.Select(us => us.Skill)));

            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UserProfileDto, ApplicationUser>()
                .ForMember(dest => dest.UserSkills, opt => opt.Ignore()); // Handle separately

            CreateMap<ApplicationUser, UserProfileDto>()
                .ForMember(dest => dest.SkillIds, opt => opt.MapFrom(src => src.UserSkills.Select(us => us.SkillId)));


            // Category Mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.Courses.Count));

            CreateMap<CategoryCreateDto, Category>();

            CreateMap<CategoryUpdateDto, Category>();


            // Skill Mappings
            CreateMap<Skill, SkillDto>()
                .ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.CourseSkills.Count))
                .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.UserSkills.Count));

            CreateMap<SkillCreateDto, Skill>();

            CreateMap<UserSkill, UserSkillDto>()
                .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.Name));



         
            // Course Mappings
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.ThumbnailPath, opt => opt.MapFrom(src => src.ThumbnailPath))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => $"{src.Instructor.FirstName} {src.Instructor.LastName}"))
                .ForMember(dest => dest.EnrollmentCount, opt => opt.MapFrom(src => src.Enrollments.Count))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.TotalLessons, opt => opt.MapFrom(src =>
                    src.Sections.SelectMany(s => s.Lessons).Count()))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src =>
                    src.CourseSkills.Select(cs => cs.Skill)))
                .ForMember(dest => dest.Sections, opt => opt.MapFrom(src =>
                    src.Sections.OrderBy(s => s.Order)));

            CreateMap<Course, CourseListDto>()
                .ForMember(dest => dest.ThumbnailPath, opt => opt.MapFrom(src => src.ThumbnailPath))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => $"{src.Instructor.FirstName} {src.Instructor.LastName}"))
                .ForMember(dest => dest.EnrollmentCount, opt => opt.MapFrom(src => src.Enrollments.Count))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                    src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.SkillNames, opt => opt.MapFrom(src =>
                    src.CourseSkills.Select(cs => cs.Skill.Name)));

            CreateMap<CourseCreateDto, Course>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => (CourseLevel)src.Level))
                .ForMember(dest => dest.InstructorId, opt => opt.Ignore()) // Set by service
                .ForMember(dest => dest.CourseSkills, opt => opt.Ignore()); // Handle separately

            CreateMap<CourseUpdateDto, Course>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => (CourseLevel)src.Level))
                .ForMember(dest => dest.CourseSkills, opt => opt.Ignore()); // Handle separately



            // Section Mappings
            CreateMap<Section, SectionDto>()
                .ForMember(dest => dest.Lessons, opt => opt.MapFrom(src =>
                    src.Lessons.OrderBy(l => l.Order)));

            CreateMap<SectionCreateDto, Section>();

         
            // Lesson Mappings
            CreateMap<Lesson, LessonDto>()
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType.ToString()))
                .ForMember(dest => dest.ExternalUrl, opt => opt.MapFrom(src => src.ExternalUrl))
                .ForMember(dest => dest.ContentPath, opt => opt.MapFrom(src => src.ContentPath));

            CreateMap<LessonCreateDto, Lesson>()
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => (ContentType)src.ContentType));

          
            // Enrollment Mappings
            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.CourseThumbnail, opt => opt.MapFrom(src => src.Course.ThumbnailPath))
                .ForMember(dest => dest.TotalLessons, opt => opt.MapFrom(src =>
                    src.Course.Sections.SelectMany(s => s.Lessons).Count()))
                .ForMember(dest => dest.CompletedLessons, opt => opt.MapFrom(src =>
                    src.LessonProgresses.Count(lp => lp.IsCompleted)));

            CreateMap<Enrollment, EnrollmentDetailsDto>()
                .IncludeBase<Enrollment, EnrollmentDto>()
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.LessonProgresses, opt => opt.MapFrom(src => src.LessonProgresses));

            CreateMap<EnrollmentCreateDto, Enrollment>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Set by service
                .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ProgressPercent, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false));

            
            // Lesson Progress Mappings
            CreateMap<LessonProgress, LessonProgressDto>()
                .ForMember(dest => dest.LessonTitle, opt => opt.MapFrom(src => src.Lesson.Title))
                .ForMember(dest => dest.TotalSeconds, opt => opt.MapFrom(src => src.Lesson.DurationInMinutes * 60));

            CreateMap<LessonProgressUpdateDto, LessonProgress>()
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom((src, dest) =>
                    src.IsCompleted ? DateTime.UtcNow : (DateTime?)null));

           

            // Review Mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.UserProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicturePath))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

            CreateMap<ReviewCreateDto, Review>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore()); // Set by service

            CreateMap<ReviewUpdateDto, Review>();
        }
    }
}