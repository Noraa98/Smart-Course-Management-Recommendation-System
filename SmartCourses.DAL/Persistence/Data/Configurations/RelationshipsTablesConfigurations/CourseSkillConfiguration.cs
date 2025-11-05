using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Entities.RelationshipsTables;

namespace SmartCourses.DAL.Persistence.Data.Configurations.RelationshipsTablesConfigurations
{
    public class CourseSkillConfiguration :IEntityTypeConfiguration<CourseSkill>
    {
        public void Configure(EntityTypeBuilder<CourseSkill> builder)
        {
            builder.HasKey(cs => new { cs.CourseId, cs.SkillId });

            builder.HasOne(cs => cs.Course)
                .WithMany(c => c.CourseSkills)
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cs => cs.Skill)
                .WithMany(s => s.CourseSkills)
                .HasForeignKey(cs => cs.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
