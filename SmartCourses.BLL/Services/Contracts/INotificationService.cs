using SmartCourses.DAL.Entities;

namespace SmartCourses.BLL.Services.Contracts
{
    public interface INotificationService : IGenericService<Notification>
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
    }
}