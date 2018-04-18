using LibraryAngular.Models;
using System.Linq;
using System.Web.Mvc;

namespace AngularLib.Controllers
{
    public class HomeController : Controller
    {
        
        private ContextBooks db;
        public HomeController()
        {
            db = new ContextBooks();
        }

        

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}