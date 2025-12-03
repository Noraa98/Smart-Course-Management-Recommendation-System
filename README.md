# 🎓 SmartCourses - Enterprise Learning Management System

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4)](https://asp.net/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4)](https://docs.microsoft.com/ef/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap)](https://getbootstrap.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)

> A comprehensive, enterprise-grade Learning Management System built with ASP.NET Core MVC, implementing clean architecture principles and modern web development best practices.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Architecture & Design Patterns](#-architecture--design-patterns)
- [Technical Stack](#-technical-stack)
- [System Modules](#-system-modules)
- [Database Design](#-database-design)
- [Security Implementation](#-security-implementation)
- [Setup & Installation](#-setup--installation)
- [Project Structure](#-project-structure)
- [API Endpoints](#-api-endpoints)
- [Screenshots](#-screenshots)
- [Future Enhancements](#-future-enhancements)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🌟 Overview

**SmartCourses** is a full-featured Learning Management System (LMS) designed to facilitate online education through a scalable, maintainable, and secure platform. The system supports multiple user roles, course management, student enrollment, progress tracking, and comprehensive administrative controls.

### 🎯 Project Objectives

- Create a scalable platform for online course delivery
- Implement role-based access control for Admin, Instructor, and Student roles
- Provide comprehensive course management with sections and lessons
- Enable student progress tracking and certification
- Build a responsive, user-friendly interface
- Follow industry-standard software architecture patterns

---

## ✨ Key Features

### 👥 Multi-Role System
- **Admin Dashboard**
  - Complete system overview with statistics
  - User management (CRUD operations)
  - Role assignment and permission management
  - Course approval and moderation
  - Category management
  - System-wide analytics

- **Instructor Portal**
  - Personal course creation and management
  - Section and lesson organization
  - Content upload (video, documents, images)
  - Student enrollment tracking
  - Course analytics and insights
  - Review and rating monitoring

- **Student Interface**
  - Course browsing and search
  - Course enrollment
  - Learning progress tracking
  - Certificate generation upon completion
  - Course reviews and ratings
  - Personal dashboard with enrolled courses

### 📚 Course Management
- **Hierarchical Structure**
  - Categories → Courses → Sections → Lessons
  - Flexible content organization
  - Drag-and-drop section/lesson ordering
  - Rich text description with formatting

- **Course Features**
  - Multiple difficulty levels (Beginner, Intermediate, Advanced)
  - Duration tracking
  - Pricing options (Free/Paid)
  - Thumbnail and preview images
  - Skill tagging system
  - Publish/Draft status control
  - Enrollment limit settings

- **Content Types**
  - Video lessons
  - Text-based content
  - Downloadable resources
  - Quizzes and assessments (planned)

### 🎓 Learning Features
- **Enrollment System**
  - One-click course enrollment
  - Enrollment history tracking
  - Progress percentage calculation
  - Completion date tracking

- **Progress Tracking**
  - Lesson completion marking
  - Section progress visualization
  - Overall course completion percentage
  - Time spent tracking

- **Certification**
  - Automatic certificate generation
  - PDF certificate download
  - Certificate verification system
  - Digital signature integration

### ⭐ Review & Rating System
- 5-star rating system
- Written reviews
- Review moderation
- Average rating calculation
- Review helpfulness voting

### 🔍 Search & Discovery
- Full-text search across courses
- Category-based filtering
- Level-based filtering
- Price range filtering
- Sort by: popularity, rating, newest, price
- Pagination support

---

## 🏗️ Architecture & Design Patterns

### Clean Architecture (N-Layer)
```
SmartCourses.PL (Presentation Layer)
    ↓
SmartCourses.BLL (Business Logic Layer)
    ↓
SmartCourses.DAL (Data Access Layer)
    ↓
Database (SQL Server)
```

### Design Patterns Implemented

#### 1. **Repository Pattern**
- Generic Repository for common CRUD operations
- Specific repositories for complex queries
- Abstraction of data access logic
```csharp
IGenericRepository<T>
ICourseRepository : IGenericRepository<Course>
```

#### 2. **Unit of Work Pattern**
- Coordinated transactions across repositories
- Single database context per request
- Automatic change tracking
```csharp
IUnitOfWork
{
    ICourseRepository Courses { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveChangesAsync();
}
```

#### 3. **Service Layer Pattern**
- Business logic encapsulation
- DTOs for data transfer
- Validation and business rules
```csharp
ICourseService, IUserService, IEnrollmentService
```

#### 4. **Dependency Injection**
- Constructor injection throughout application
- Service lifetime management (Scoped, Transient, Singleton)
- Loose coupling between layers

#### 5. **DTO (Data Transfer Objects)**
- Separation of domain models and view models
- Input validation attributes
- AutoMapper for object mapping

#### 6. **Result Pattern**
- Standardized response format
- Success/failure indication
- Error message collection
```csharp
Result<T> { bool IsSuccess, T Data, List<string> Errors }
```

#### 7. **Specification Pattern** (Partial)
- Complex query building
- Reusable query logic
- Include expressions for eager loading

---

## 🛠️ Technical Stack

### Backend
- **Framework:** ASP.NET Core 8.0 MVC
- **Language:** C# 12
- **ORM:** Entity Framework Core 8.0
- **Database:** SQL Server 2019+
- **Authentication:** ASP.NET Core Identity
- **Authorization:** Role-based & Policy-based
- **Mapping:** AutoMapper
- **Logging:** ILogger (built-in)
- **Validation:** Data Annotations + FluentValidation

### Frontend
- **UI Framework:** Bootstrap 5.3
- **Icons:** Bootstrap Icons
- **JavaScript:** Vanilla JS + jQuery
- **CSS:** Custom CSS + Bootstrap utilities
- **Forms:** Razor Tag Helpers
- **Client Validation:** jQuery Validation

### Development Tools
- **IDE:** Visual Studio 2022
- **Version Control:** Git & GitHub
- **Database Management:** SQL Server Management Studio (SSMS)
- **API Testing:** Postman (for future API endpoints)
- **Package Manager:** NuGet

### Architecture Principles
- **SOLID Principles**
- **DRY (Don't Repeat Yourself)**
- **Separation of Concerns**
- **Domain-Driven Design concepts**

---

## 📦 System Modules

### 1. Authentication & Authorization Module
**Features:**
- User registration with email confirmation
- Login with "Remember Me" functionality
- Password reset via email
- Password strength validation
- Account lockout after failed attempts
- Two-factor authentication (2FA) ready
- Role-based access control (RBAC)
- Claims-based authorization

**Technologies:**
- ASP.NET Core Identity
- Cookie Authentication
- Password hashing (PBKDF2)
- Anti-forgery tokens

---

### 2. User Management Module
**Admin Capabilities:**
- View all users (paginated)
- Filter by role (Admin, Instructor, Student)
- User details view
- Role assignment/removal
- Lock/unlock user accounts
- Delete users (soft delete)
- Search users

**User Profile:**
- Update personal information
- Change password
- Upload profile picture
- View enrollment history
- View certificates

---

### 3. Course Management Module

#### Course CRUD
- **Create:** Multi-step course creation wizard
- **Read:** Detailed course view with nested content
- **Update:** Edit course details, sections, lessons
- **Delete:** Soft delete with cascade handling

#### Section Management
- Add/edit/delete sections
- Reorder sections (drag-and-drop)
- Section descriptions

#### Lesson Management
- Video lessons with duration
- Text/document lessons
- Reorder lessons within sections
- Mark as preview (free preview lessons)

#### Course Publishing
- Draft/Published status
- Publish date scheduling
- Unpublish capability
- Publishing workflow approval

---

### 4. Category Management Module
- Category CRUD operations
- Category hierarchy (future: subcategories)
- Category icons/images
- Course count per category
- Category-based navigation

---

### 5. Enrollment Module
**Student Features:**
- Browse available courses
- Enroll in courses (free/paid)
- View enrolled courses dashboard
- Track learning progress
- Continue learning from last position

**Enrollment Logic:**
- Prevent duplicate enrollments
- Enrollment date tracking
- Progress calculation
- Completion detection
- Certificate generation trigger

---

### 6. Progress Tracking Module
- Lesson completion tracking
- Section progress percentage
- Overall course progress
- Time spent per lesson (planned)
- Learning path visualization
- Resume from last position

---

### 7. Review & Rating Module
- Submit course reviews
- 5-star rating system
- Review moderation (Admin)
- Average rating calculation
- Review sorting (newest, highest rated)
- Helpful/not helpful voting
- Edit/delete own reviews

---

### 8. Certificate Module
- Auto-generate upon course completion
- PDF certificate generation
- Certificate verification system
- Digital signature
- Download certificates
- Certificate revocation (Admin)
- Certificate templates

---

### 9. Dashboard Module

#### Admin Dashboard
- Total users count
- Total courses count
- Total enrollments
- Revenue statistics
- Recent activity feed
- System health indicators

#### Instructor Dashboard
- My courses statistics
- Total students enrolled
- Average course ratings
- Revenue from courses
- Recent reviews
- Course performance analytics

#### Student Dashboard
- Enrolled courses
- Learning progress overview
- Completed courses
- Certificates earned
- Recommended courses
- Continue learning section

---

### 10. Search & Filter Module
- Full-text search
- Multi-criteria filtering:
  - Category
  - Level
  - Price range
  - Rating
  - Duration
- Sort options
- Pagination
- Search suggestions (autocomplete)

---

## 🗄️ Database Design

### Core Entities

#### Identity Tables
```
- AspNetUsers (extended ApplicationUser)
  - Id, UserName, Email, FirstName, LastName, etc.
- AspNetRoles
- AspNetUserRoles
- AspNetUserClaims
- AspNetUserLogins
```

#### Application Tables
```
Categories
├── Id (PK)
├── Name
├── Description
├── IconPath
├── CreatedOn
└── IsDeleted

Courses
├── Id (PK)
├── Title
├── Description
├── ShortDescription
├── Level (enum: Beginner, Intermediate, Advanced)
├── Price (decimal?)
├── ThumbnailPath
├── DurationInHours
├── IsPublished
├── CategoryId (FK)
├── InstructorId (FK)
├── CreatedOn
├── CreatedBy
├── LastModifiedOn
├── LastModifiedBy
└── IsDeleted

Sections
├── Id (PK)
├── Title
├── Description
├── Order
├── CourseId (FK)
└── IsDeleted

Lessons
├── Id (PK)
├── Title
├── Content (text/video URL)
├── DurationInMinutes
├── Order
├── IsPreview
├── SectionId (FK)
└── IsDeleted

Enrollments
├── Id (PK)
├── CourseId (FK)
├── StudentId (FK)
├── EnrolledOn
├── CompletionPercentage
├── CompletedOn
└── IsDeleted

LessonProgress
├── Id (PK)
├── EnrollmentId (FK)
├── LessonId (FK)
├── IsCompleted
├── CompletedOn
└── TimeSpent

Reviews
├── Id (PK)
├── CourseId (FK)
├── StudentId (FK)
├── Rating (1-5)
├── Comment
├── ReviewDate
└── IsDeleted

Skills
├── Id (PK)
├── Name
└── Description

CourseSkills (Many-to-Many)
├── CourseId (FK)
└── SkillId (FK)

Certificates
├── Id (PK)
├── EnrollmentId (FK)
├── IssuedDate
├── CertificateNumber
└── CertificatePath
```

### Database Relationships
- **One-to-Many:**
  - Category → Courses
  - Course → Sections
  - Section → Lessons
  - Course → Enrollments
  - Course → Reviews
  - User (Instructor) → Courses

- **Many-to-Many:**
  - Courses ↔ Skills

- **One-to-One:**
  - Enrollment → Certificate

### Indexes
- Composite index on (CourseId, StudentId) for Enrollments
- Index on CategoryId in Courses
- Index on InstructorId in Courses
- Full-text index on Course.Title and Course.Description

---

## 🔒 Security Implementation

### Authentication Security
- **Password Policy:**
  - Minimum 8 characters
  - Requires uppercase, lowercase, digit, special character
  - Password history (prevent reuse)
  
- **Account Protection:**
  - Account lockout after 5 failed attempts
  - Lockout duration: 15 minutes
  - Email confirmation required
  - Password reset token expiration: 1 hour

### Authorization
- **Role-Based Access Control (RBAC)**
  - Admin: Full system access
  - Instructor: Manage own courses
  - Student: Enroll and learn

- **Resource-Based Authorization**
  - Instructors can only edit their own courses
  - Students can only access enrolled courses
  - Policy-based authorization for complex scenarios

### Data Protection
- **SQL Injection Prevention:** Parameterized queries (EF Core)
- **XSS Protection:** Razor encoding by default
- **CSRF Protection:** Anti-forgery tokens on all forms
- **Secure Headers:** Content-Security-Policy, X-Frame-Options
- **HTTPS Enforcement:** HSTS enabled
- **Sensitive Data:** Connection strings in User Secrets/Environment Variables

### Input Validation
- Server-side validation with Data Annotations
- Client-side validation with jQuery Validation
- FluentValidation for complex rules
- HTML sanitization for rich text inputs

---

## 🚀 Setup & Installation

### Prerequisites
```
- .NET 8.0 SDK or higher
- SQL Server 2019+ (or SQL Server Express/LocalDB)
- Visual Studio 2022 or VS Code
- Git
```

### Installation Steps

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/SmartCourses.git
cd SmartCourses
```

2. **Configure Database Connection**

Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SmartCoursesDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Or use User Secrets (recommended):
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YourConnectionString"
```

3. **Apply Database Migrations**
```bash
cd SmartCourses.DAL
dotnet ef database update --startup-project ../SmartCourses.PL
```

4. **Seed Initial Data (Optional)**
```bash
# Run the application once to seed roles and admin user
cd ../SmartCourses.PL
dotnet run
```

Default admin credentials:
- Email: admin@smartcourses.com
- Password: Admin@123

5. **Run the Application**
```bash
dotnet run
```

Navigate to: `https://localhost:7001`

---

## 📁 Project Structure

```
SmartCourses/
│
├── SmartCourses.PL (Presentation Layer)
│   ├── Areas/
│   │   ├── Admin/
│   │   │   ├── Controllers/
│   │   │   │   ├── DashboardController.cs
│   │   │   │   ├── UsersController.cs
│   │   │   │   ├── CoursesController.cs
│   │   │   │   └── CategoriesController.cs
│   │   │   └── Views/
│   │   │       ├── Dashboard/Index.cshtml
│   │   │       ├── Users/
│   │   │       ├── Courses/
│   │   │       └── Categories/
│   │   │
│   │   ├── Instructor/
│   │   │   ├── Controllers/
│   │   │   └── Views/
│   │   │
│   │   └── Student/
│   │       ├── Controllers/
│   │       └── Views/
│   │
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── AccountController.cs
│   │   ├── CourseController.cs
│   │   └── EnrollmentController.cs
│   │
│   ├── Views/
│   │   ├── Home/
│   │   ├── Account/
│   │   ├── Course/
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml
│   │   │   ├── _LoginPartial.cshtml
│   │   │   └── Error.cshtml
│   │   └── _ViewStart.cshtml
│   │
│   ├── wwwroot/
│   │   ├── css/
│   │   ├── js/
│   │   ├── images/
│   │   └── lib/
│   │
│   ├── Program.cs
│   └── appsettings.json
│
├── SmartCourses.BLL (Business Logic Layer)
│   ├── Services/
│   │   ├── Contracts/
│   │   │   ├── ICourseService.cs
│   │   │   ├── IUserService.cs
│   │   │   ├── IEnrollmentService.cs
│   │   │   └── IAuthService.cs
│   │   │
│   │   └── Implementations/
│   │       ├── CourseService.cs
│   │       ├── UserService.cs
│   │       ├── EnrollmentService.cs
│   │       └── AuthService.cs
│   │
│   ├── Models/
│   │   └── DTOs/
│   │       ├── CourseDTOs/
│   │       ├── UserDTOs/
│   │       ├── EnrollmentDTOs/
│   │       └── ResponseDTOs/
│   │
│   ├── Mapping/
│   │   └── MappingProfile.cs
│   │
│   └── Validators/
│       └── CourseValidator.cs
│
├── SmartCourses.DAL (Data Access Layer)
│   ├── Entities/
│   │   ├── Course.cs
│   │   ├── Category.cs
│   │   ├── Section.cs
│   │   ├── Lesson.cs
│   │   ├── Enrollment.cs
│   │   ├── Review.cs
│   │   └── Identity/
│   │       └── ApplicationUser.cs
│   │
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   │
│   ├── Repositories/
│   │   ├── Contracts/
│   │   │   ├── IGenericRepository.cs
│   │   │   ├── ICourseRepository.cs
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── ...
│   │   │
│   │   └── Implementations/
│   │       ├── GenericRepository.cs
│   │       ├── CourseRepository.cs
│   │       ├── UnitOfWork.cs
│   │       └── ...
│   │
│   ├── Configurations/
│   │   ├── CourseConfiguration.cs
│   │   ├── CategoryConfiguration.cs
│   │   └── ...
│   │
│   └── Migrations/
│       └── [EF Core Migrations]
│
└── SmartCourses.sln
```

---

## 🔌 API Endpoints

### Authentication
```
POST   /Account/Register          - Register new user
POST   /Account/Login             - User login
POST   /Account/Logout            - User logout
POST   /Account/ForgotPassword    - Request password reset
POST   /Account/ResetPassword     - Reset password
```

### Admin Area
```
GET    /Admin/Dashboard           - Admin dashboard
GET    /Admin/Users               - List all users
GET    /Admin/Users/Details/{id}  - User details
POST   /Admin/Users/ManageRoles   - Assign roles
POST   /Admin/Users/Lock/{id}     - Lock user account
POST   /Admin/Users/Delete/{id}   - Delete user

GET    /Admin/Courses             - List all courses
GET    /Admin/Courses/Details/{id}- Course details
POST   /Admin/Courses/Create      - Create course
POST   /Admin/Courses/Edit/{id}   - Edit course
POST   /Admin/Courses/Delete/{id} - Delete course

GET    /Admin/Categories          - List categories
POST   /Admin/Categories/Create   - Create category
POST   /Admin/Categories/Edit/{id}- Edit category
POST   /Admin/Categories/Delete/{id}- Delete category
```

### Instructor Area
```
GET    /Instructor/Dashboard      - Instructor dashboard
GET    /Instructor/Courses        - My courses
GET    /Instructor/Courses/Create - Create new course
POST   /Instructor/Courses/Create - Submit new course
GET    /Instructor/Courses/Edit/{id}- Edit my course
POST   /Instructor/Courses/Edit/{id}- Update my course
```

### Student/Public
```
GET    /                          - Home page
GET    /Course                    - Browse courses
GET    /Course/Details/{id}       - Course details
POST   /Enrollment/Enroll         - Enroll in course
GET    /Enrollment/MyCourses      - My enrolled courses
GET    /Enrollment/Learn/{id}     - Learning interface
POST   /Enrollment/CompleteLesson - Mark lesson complete
GET    /Certificate/{id}          - View certificate
```

---

## 📸 Screenshots

### Home Page
> Landing page with featured courses and categories

### Admin Dashboard
> System statistics and management overview

### Course Details
> Comprehensive course information with sections and lessons

### Learning Interface
> Video player, progress tracking, and lesson navigation

### Instructor Dashboard
> Course management and analytics

### Student Dashboard
> Enrolled courses and learning progress

---

## 🔮 Future Enhancements

### Planned Features
- [ ] **Payment Integration** (Stripe/PayPal)
- [ ] **Quizzes & Assessments** with automatic grading
- [ ] **Discussion Forums** per course
- [ ] **Live Classes** integration (Zoom/Teams)
- [ ] **Assignment Submission** system
- [ ] **Mobile App** (React Native/Flutter)
- [ ] **RESTful API** for third-party integrations
- [ ] **Real-time Notifications** (SignalR)
- [ ] **Advanced Analytics** with charts
- [ ] **Bulk Operations** for admins
- [ ] **Course Import/Export**
- [ ] **Multi-language Support** (i18n)
- [ ] **Dark Mode**
- [ ] **Gamification** (badges, achievements)
- [ ] **AI-powered Course Recommendations**

### Technical Improvements
- [ ] Implement CQRS pattern
- [ ] Add Redis caching layer
- [ ] Implement event sourcing
- [ ] Add comprehensive unit tests
- [ ] Integration tests with xUnit
- [ ] API versioning
- [ ] Microservices architecture migration
- [ ] Docker containerization
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Performance monitoring (Application Insights)

---

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Standards
- Follow C# coding conventions
- Write XML documentation for public APIs
- Include unit tests for new features
- Update README for significant changes

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Developer

**[Noura Ahmed]**
- LinkedIn: [your-profile](https://www.linkedin.com/in/noura-ahmed-36779b304)
- GitHub: [@yourusername](https://github.com/Noraa98)
- Email: noura.ahmed7258@gmail.com

---

## 🙏 Acknowledgments

- ASP.NET Core Documentation
- Entity Framework Core Documentation
- Bootstrap Documentation
- Stack Overflow Community
- Clean Architecture principles by Robert C. Martin

---

## 📊 Project Statistics

- **Total Lines of Code:** ~15,000+
- **Development Time:** [X months]
- **Entities:** 12+
- **Controllers:** 20+
- **Views:** 50+
- **Services:** 10+
- **Repositories:** 8+

---

<div align="center">

**⭐ Star this repository if you find it helpful!**

Made with ❤️ by [Noura Ahmed]

</div>
