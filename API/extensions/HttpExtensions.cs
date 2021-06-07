// using API.Helpers;
// using Microsoft.AspNetCore.Http;
// using System.Text.Json;
// namespace API.extensions
// {
//     public static class HttpExtensions
//     {
//         public static void  AddPaginationHeader(this HttpResponse response , 
//                             int currentPage,
//                             int itemsPerPage , 
//                             int totalItems , 
//                             int totalPages) {
//         var paginationHeader = new PaginationHeader(currentPage ,itemsPerPage , totalItems , totalPages);
//         var option = new JsonSerializerOptions {
//             PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//         };
//         response.Headers.Add("Pagination" , JsonSerializer.Serialize(paginationHeader , option));
//         response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
//         }
        
//     }
// }


  
using System.Text.Json;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions // gán value vào Header để client nhận đc = response.header.get('Pagination')
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, 
            int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}