namespace Web.API.Helpers
{
    public class QueryObject
    {
        public string? CompanyName { get; set; } = null;
        public string? Symbol { get; set; } = null;
        public int? Id { get; set; } = null;
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
