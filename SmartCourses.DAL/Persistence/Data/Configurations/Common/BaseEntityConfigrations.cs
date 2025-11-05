using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Common.Entities;

namespace SmartCourses.DAL.Persistence.Data.Configurations.Common
{
    public class BaseEntityConfigrations<TKey , TEntity> : IEntityTypeConfiguration<TEntity>
        where TKey : IEquatable<TKey>
        where TEntity : BaseEntity<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(E => E.Id);

        }
    }
}
