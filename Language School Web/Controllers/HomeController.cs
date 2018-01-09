using LanguageSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace Language_School_Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            StudentDataDto model = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchoolWeb/api/");
                HttpCookie cookie = Request.Cookies["token"];
                if (cookie != null)
                {
                    string accessToken = cookie.Value;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    //HTTP GET
                    var responseTask = client.GetAsync("student/info");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<StudentDataDto>();
                        readTask.Wait();

                        model = readTask.Result;
                    }
                    else //web api sent error response 
                    {
                        //log response status here..

                        model = new StudentDataDto();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            return View(model);
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