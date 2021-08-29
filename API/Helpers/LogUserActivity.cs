using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using API.Extensions;
using API.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace API.Helpers
{
    // cho phép chúng ta làm cái gì đó trước và sau khi Request  thực thi
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            
            // video 12 trong bài 13
            // trả về Context sau khi Request đã thực thi , dùng đê cập nhật một cí gì đó
            var resultContext = await next(); 
            // ta muốn kiểm tra xem neu user khong  
            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.getUserId();
            var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            // var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await uow.UserRepository.GetUserByIdAsync(userId);
            user.LastActive = DateTime.UtcNow;
            await uow.Complete();
            //await repo.SaveAllAsync();
            
        }
    }
}