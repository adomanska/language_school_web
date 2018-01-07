using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using LanguageSchool.Models;

namespace Language_School_Web.Controllers
{
    public class ClassesController : Controller
    {
        // GET: Classes
        public ActionResult Index()
        {
            IEnumerable<ClassDataDto> allClasses = null;
            IEnumerable<ClassBasicDataDto> topClasses = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchool/api/");
                
                //HTTP GET
                var responseTask = client.GetAsync("classes");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ClassDataDto>>();
                    readTask.Wait();

                    allClasses = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    allClasses = Enumerable.Empty<ClassDataDto>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }

                responseTask = client.GetAsync("classes/top/3");
                responseTask.Wait();

                result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ClassBasicDataDto>>();
                    readTask.Wait();

                    topClasses = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    topClasses = Enumerable.Empty<ClassBasicDataDto>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
                //string accessToken = Request.Cookies["token"].Value;
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return View(Tuple.Create(allClasses, topClasses));
        }
    }
}