using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data.Configurations.Common;


namespace SmartCourses.DAL.Persistence.Data.Configurations
{
    internal class SectionConfiguration : BaseAuditableEntityConfigurations<int, Section>
    {
        public override void Configure(EntityTypeBuilder<Section> builder)
        {
            base.Configure(builder); 

            builder.Property(s => s.Id).UseIdentityColumn(1, 1);

            builder.Property(s => s.Title)
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder.Property(s => s.Description)
                .HasMaxLength(500);

            builder.HasOne(s => s.Course)
                .WithMany(c => c.Sections)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => new { s.CourseId, s.Order });
        }
    }
}