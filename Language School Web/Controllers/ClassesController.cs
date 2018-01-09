using System;
using System.Collections.Generic;
using System.Globalization;
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
            IEnumerable<ClassBasicDataDto> suggestedClasses = null;
            IEnumerable<ClassBasicDataDto> studentClasses = null;
            bool isLoggedInUser = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchoolWeb/api/");

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

                HttpCookie cookie = Request.Cookies["token"];
                if (cookie != null)
                {
                    string accessToken = cookie.Value;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    responseTask = client.GetAsync("classes/suggested");
                    responseTask.Wait();

                    result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<ClassBasicDataDto>>();
                        readTask.Wait();

                        suggestedClasses = readTask.Result;
                    }
                    else //web api sent error response 
                    {
                        //log response status here..

                        suggestedClasses = Enumerable.Empty<ClassBasicDataDto>();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }

                    responseTask = client.GetAsync("student/classes");
                    responseTask.Wait();

                    result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<ClassBasicDataDto>>();
                        readTask.Wait();

                        studentClasses = readTask.Result;
                    }
                    else //web api sent error response 
                    {
                        //log response status here..

                        studentClasses = Enumerable.Empty<ClassBasicDataDto>();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }

                    var studentClassesIds = studentClasses.Select(c => c.Id);
                    allClasses = allClasses.Where(c => !studentClassesIds.Contains(c.Id));
                    isLoggedInUser = true;
                }
            }
            return View(Tuple.Create(allClasses, topClasses, suggestedClasses, isLoggedInUser));
        }

        public ActionResult Schedule()
        {
            IEnumerable<ClassDataDto> userClasses = null;
            IEnumerable<ScheduleDay> scheduleDays = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchoolWeb/api/");
                string accessToken = Request.Cookies["token"].Value;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var responseTask = client.GetAsync("student/classes");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ClassDataDto>>();
                    readTask.Wait();

                    userClasses = readTask.Result;
                    scheduleDays = userClasses.GroupBy(c => c.DayOfWeek)
                        .Select(group => new ScheduleDay(){ Day = group.Key, Items = group.ToList() });
                }
                else //web api sent error response 
                {
                    //log response status here..

                    userClasses = Enumerable.Empty<ClassDataDto>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(scheduleDays);
        }

        public ActionResult SignFor(int classId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchoolWeb/api/");
                string accessToken = Request.Cookies["token"].Value;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var postTask = client.PostAsync("classes/" + classId.ToString(), null);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return Index();
        }
    }
}