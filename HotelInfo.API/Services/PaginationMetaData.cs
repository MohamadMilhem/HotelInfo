using Microsoft.AspNetCore.Identity;

namespace CityInfo.API.Services
{
    public class PaginationMetaData
    {
        public int CurrentPage { get; set; }
        public int TotalPageCount { get; set; }
        public int TotalItemCount { get; set; } 
        public int PageSize { get; set; }

        public PaginationMetaData(int currentPage, int totalItemCount, int pageSize)
        {
            CurrentPage = currentPage;
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        }
    }
}
