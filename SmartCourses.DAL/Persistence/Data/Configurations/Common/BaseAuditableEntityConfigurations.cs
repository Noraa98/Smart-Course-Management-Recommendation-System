using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Common.Entities;

namespace SmartCourses.DAL.Persistence.Data.Configurations.Common
{
    public class BaseAuditableEntityConfigurations<TKey , TEntity> : BaseEntityConfigrations<TKey , TEntity>
        where TKey : IEquatable<TKey>
        where TEntity : BaseAuditableEntity<TKey>
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            base.Configure(builder);

            builder.Property(E => E.CreatedBy).
                HasColumnType("varchar(50)");

            builder.Property(E => E.LastModifiedBy)
                .HasColumnType("varchar(50)");

            builder.Property(E => E.CreatedOn)
                .HasDefaultValueSql("GETUTCDate()")
                .ValueGeneratedOnAdd();

            builder.Property(E => E.LastModifiedOn)
                 .IsRequired();

            // configure soft delete filter
            builder.HasQueryFilter(E => !E.IsDeleted);
        }
    }
}
