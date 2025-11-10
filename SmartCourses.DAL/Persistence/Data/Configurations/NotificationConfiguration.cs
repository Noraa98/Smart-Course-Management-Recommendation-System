using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCourses.DAL.Entities;
using SmartCourses.DAL.Persistence.Data.Configurations.Common;

namespace SmartCourses.DAL.Persistence.Data.Configurations
{
    public class NotificationConfiguration : BaseAuditableEntityConfigurations<int, Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder); 

            builder.Property(n => n.Id).UseIdentityColumn(1, 1);

            builder.Property(n => n.UserId)
                .HasColumnType("nvarchar(450)")
                .IsRequired();

            builder.Property(n => n.Message)
                .HasColumnType("varchar(500)")
                .IsRequired();

            builder.Property(n => n.Link)
                .HasMaxLength(255);

            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(n => new { n.UserId, n.IsRead });

        }
    }
}
