namespace SmartCourses.BLL.Models.DTOs.Response_ResultDTOs
{
    public class CourseFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? Level { get; set; }
        public int? SkillId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } // title, price, rating, enrolled
        public string? SortOrder { get; set; } = "asc"; // asc, desc
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
