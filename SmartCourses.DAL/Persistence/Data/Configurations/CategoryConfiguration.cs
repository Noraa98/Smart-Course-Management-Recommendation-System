using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data.Configurations.Common;

namespace SmartCourses.DAL.Persistence.Data.Configurations
{
    internal class CategoryConfiguration : BaseAuditableEntityConfigurations<int, Category>
    {
       
        public override void  Configure(EntityTypeBuilder<Category> builder)
        {
            base.Configure(builder);

            
            builder.Property(E => E.Id).UseIdentityColumn(1, 1);

            builder.Property(c => c.Name)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.IconPath)
                .HasMaxLength(255);

            builder.HasIndex(c => c.Name)
                .IsUnique();

        }

    }
}
