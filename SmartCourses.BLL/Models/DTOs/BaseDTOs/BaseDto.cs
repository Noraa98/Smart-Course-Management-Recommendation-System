namespace SmartCourses.BLL.Models.DTOs.BaseDTOs
{
    public class BaseDto<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; } = default!;
    }
}
