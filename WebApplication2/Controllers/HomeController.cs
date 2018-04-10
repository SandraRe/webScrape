using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Controllers.Functionality;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["UrlsCount"] = Scrapper.countDBDocs();
            return View();
        }

        [HttpPost]
        public ActionResult GetUrl(FormCollection collection)
        {
            try
            {          
                Scrapper scrape = new Scrapper();
                string url = collection["Name"];
                string[] results= { };
                scrape.ScrapeWebPage(url, out results);
                ViewData["Message"] = "Success";
                ViewData["Divs"] = results[1];
                ViewData["Spans"] = results[2];
                ViewData["Links"] = results[3]; 
                ViewData["Last URL"] = results[0];

                ViewData["UrlsCount"] = Scrapper.countDBDocs();


                return View("~/Views/Home/index.cshtml");



            }
            
            catch
            {
                ViewData["Message"] = "Failed";

                return View("~/Views/Home/index.cshtml");
            }
        }




    }
}