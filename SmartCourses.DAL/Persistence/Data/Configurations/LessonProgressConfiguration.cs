using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data.Configurations.Common;

namespace SmartCourses.DAL.Persistence.Data.Configurations
{
    public class LessonProgressConfiguration : BaseAuditableEntityConfigurations<int, LessonProgress>
    {
        public override void Configure(EntityTypeBuilder<LessonProgress> builder)
        {
            base.Configure(builder); 

            builder.Property(lp => lp.Id).UseIdentityColumn(1, 1);

            builder.HasOne(lp => lp.Enrollment)
                .WithMany(e => e.LessonProgresses)
                .HasForeignKey(lp => lp.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lp => lp.Lesson)
                .WithMany(l => l.LessonProgresses)
                .HasForeignKey(lp => lp.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(lp => new { lp.EnrollmentId, lp.LessonId })
                .IsUnique();
        }
    }
}
