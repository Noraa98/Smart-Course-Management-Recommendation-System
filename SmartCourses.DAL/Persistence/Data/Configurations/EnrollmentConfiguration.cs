using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data.Configurations.Common;

namespace SmartCourses.DAL.Persistence.Data.Configurations
{
    public class EnrollmentConfiguration : BaseAuditableEntityConfigurations<int, Enrollment>
    {
        public override void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            base.Configure(builder); 

            builder.Property(e => e.Id).UseIdentityColumn(1, 1);

            builder.Property(e => e.UserId)
                .HasColumnType("nvarchar(450)")
                .IsRequired();

            builder.Property(e => e.ProgressPercent)
                .HasColumnType("decimal(5,2)");

            builder.HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.UserId, e.CourseId })
                .IsUnique();
        }
    }
}