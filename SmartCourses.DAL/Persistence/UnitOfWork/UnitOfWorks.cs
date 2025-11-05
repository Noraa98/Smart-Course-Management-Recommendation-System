using SmartCourses.DAL.Contracts;
using SmartCourses.DAL.Contracts.Repositories;
using SmartCourses.DAL.Persistence.Data;
using SmartCourses.DAL.Persistence.Repositories;

namespace SmartCourses.DAL.Persistence.UnitOfWork
{
    public class UnitOfWorks : IUnitOfWork, IAsyncDisposable
    {
        private readonly ApplicationDbContext _dbContext;

        // Lazy Repositories
        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<ISkillRepository> _skillRepository;
        private readonly Lazy<ICourseRepository> _courseRepository;
        private readonly Lazy<ISectionRepository> _sectionRepository;
        private readonly Lazy<ILessonRepository> _lessonRepository;
        private readonly Lazy<ILessonProgressRepository> _lessonProgressRepository;
        private readonly Lazy<IEnrollmentRepository> _enrollmentRepository;
        private readonly Lazy<IReviewRepository> _reviewRepository;
        private readonly Lazy<INotificationRepository> _notificationRepository;
        private readonly Lazy<IUserSkillRepository> _userSkillRepository;
        private readonly Lazy<ICourseSkillRepository> _courseSkillRepository;

        public UnitOfWorks(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(_dbContext));
            _skillRepository = new Lazy<ISkillRepository>(() => new SkillRepository(_dbContext));
            _courseRepository = new Lazy<ICourseRepository>(() => new CourseRepository(_dbContext));
            _sectionRepository = new Lazy<ISectionRepository>(() => new SectionRepository(_dbContext));
            _lessonRepository = new Lazy<ILessonRepository>(() => new LessonRepository(_dbContext));
            _lessonProgressRepository = new Lazy<ILessonProgressRepository>(() => new LessonProgressRepository(_dbContext));
            _enrollmentRepository = new Lazy<IEnrollmentRepository>(() => new EnrollmentRepository(_dbContext));
            _reviewRepository = new Lazy<IReviewRepository>(() => new ReviewRepository(_dbContext));
            _notificationRepository = new Lazy<INotificationRepository>(() => new NotificationRepository(_dbContext));
            _userSkillRepository = new Lazy<IUserSkillRepository>(() => new UserSkillRepository(_dbContext));
            _courseSkillRepository = new Lazy<ICourseSkillRepository>(() => new CourseSkillRepository(_dbContext));
        }

        // Properties
        public ICategoryRepository Categories => _categoryRepository.Value;
        public ISkillRepository Skills => _skillRepository.Value;
        public ICourseRepository Courses => _courseRepository.Value;
        public ISectionRepository Sections => _sectionRepository.Value;
        public ILessonRepository Lessons => _lessonRepository.Value;
        public ILessonProgressRepository LessonProgress => _lessonProgressRepository.Value;
        public IEnrollmentRepository Enrollments => _enrollmentRepository.Value;
        public IReviewRepository Reviews => _reviewRepository.Value;
        public INotificationRepository Notifications => _notificationRepository.Value;
        public IUserSkillRepository UserSkills => _userSkillRepository.Value;
        public ICourseSkillRepository CourseSkills => _courseSkillRepository.Value;


        // Save Changes
        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        

        // Dispose
        public async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }
    }
}

