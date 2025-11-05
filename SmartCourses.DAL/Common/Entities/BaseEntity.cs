namespace SmartCourses.DAL.Common.Entities
{
    public class BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

    }
}
