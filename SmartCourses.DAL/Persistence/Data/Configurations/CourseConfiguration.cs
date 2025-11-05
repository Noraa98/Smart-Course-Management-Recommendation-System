using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data.Configurations.Common;

namespace SmartCourses.DAL.Persistence.Data.Configurations
{
    internal class CourseConfiguration : BaseAuditableEntityConfigurations<int, Course>
    {
        public override void Configure(EntityTypeBuilder<Course> builder)
        {
            base.Configure(builder); 

            builder.Property(c => c.Id).UseIdentityColumn(1, 1);

            builder.Property(c => c.Title)
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(c => c.ShortDescription)
                .HasColumnType("varchar(300)")
                .IsRequired();

            builder.Property(c => c.ThumbnailPath)
                .HasMaxLength(500);

            builder.Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.InstructorId)
                .HasColumnType("varchar(450)") 
                .IsRequired();

            builder.HasOne(c => c.Category)
                .WithMany(cat => cat.Courses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Instructor)
                .WithMany(u => u.CoursesInstructed)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.Title);
            builder.HasIndex(c => c.IsPublished);
            builder.HasIndex(c => c.CategoryId);
            builder.HasIndex(c => c.InstructorId);
        }
    }
}
