using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data.Configurations.Common;

namespace SmartCourses.DAL.Persistence.Data.Configurations
{
    internal class SkillConfiguration : BaseAuditableEntityConfigurations<int, Skill>
    {
        public override void Configure(EntityTypeBuilder<Skill> builder)
        {
            base.Configure(builder); 

            builder.Property(E => E.Id).UseIdentityColumn(1, 1);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .HasMaxLength(300);

            builder.HasIndex(s => s.Name)
                .IsUnique();

            // builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}