namespace SmartCourses.DAL.Contracts
{
    public interface IDbInitializer
    {
        void Initialize();
        void  Seed();
    }
}