using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Entities.RelationshipsTables;

namespace SmartCourses.DAL.Persistence.Data.Configurations.RelationshipsTablesConfigurations
{
    internal class UserSkillConfiguration : IEntityTypeConfiguration<UserSkill>
    {
        public void Configure(EntityTypeBuilder<UserSkill> builder)
        {
            builder.HasKey(us => new { us.UserId, us.SkillId });

            builder.HasOne(us => us.User)
                .WithMany(u => u.UserSkills)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(us => us.Skill)
                .WithMany(s => s.UserSkills)
                .HasForeignKey(us => us.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(us => us.ProficiencyLevel)
                .IsRequired()
                .HasDefaultValue(1);
        }
    }
}
