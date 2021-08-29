using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FallBackController: Controller
    {
        //  trả về client trang Index
        public ActionResult Index() {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory() , 
                                            "wwwroot" , "index.html") , 
                                            "text/HTML");
        }
    }
}