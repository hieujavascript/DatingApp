using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{

    // Khai báo các thuôc tính và trả về danh sách của tập kết quả với [fromquery] phù hợp
    public class PagedList<T>: List<T> 
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }
        // tính toán các kết quả và trả về giá trị trong Respontory
        public static async Task<PagedList<T>> CreateAysnc(
        IQueryable<T> source , int pageNumber , int pageSize) 
        {
            //  goi database đếm số record
            var  count = await source.CountAsync();
            var items = await source.Skip( (pageNumber - 1 ) * pageSize ).Take(pageSize).ToListAsync();
            return new PagedList<T>(items , count , pageNumber , pageSize);
        }


    }
}