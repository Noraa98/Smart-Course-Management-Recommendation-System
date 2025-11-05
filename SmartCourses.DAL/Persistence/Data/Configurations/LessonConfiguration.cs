using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data.Configurations.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCourses.DAL.Persistence.Data.Configurations
{
    public class LessonConfiguration : BaseAuditableEntityConfigurations<int, Lesson>
    {
        public override void Configure(EntityTypeBuilder<Lesson> builder)
        {
            base.Configure(builder); 

            builder.Property(l => l.Id).UseIdentityColumn(1, 1);

            builder.Property(l => l.Title)
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder.Property(l => l.Description)
                .HasMaxLength(1000);

            builder.Property(l => l.ContentPath)
                .HasMaxLength(500);

            builder.Property(l => l.ExternalUrl)
                .HasMaxLength(500);

            builder.HasOne(l => l.Section)
                .WithMany(s => s.Lessons)
                .HasForeignKey(l => l.SectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(l => new { l.SectionId, l.Order });
            

        }
    }
}