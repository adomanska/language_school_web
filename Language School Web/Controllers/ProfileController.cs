using LanguageSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Language_School_Web.Controllers
{
    public class ProfileController : Controller
    {
        public ActionResult Info()
        {
            StudentDataDto model = null;
            IEnumerable<ClassBasicDataDto> studentClasses = null;
            string charge = "?";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchoolWeb/api/");
                string accessToken = null;
                try
                {
                    accessToken = Request.Cookies["token"].Value;
                }
                catch
                {
                    return View("~/Views/Shared/AccessDenied.cshtml");
                }
                
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

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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

                responseTask = client.GetAsync("student/charge");
                responseTask.Wait();

                result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<string>();
                    readTask.Wait();

                    charge = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    charge = "?";

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View(Tuple.Create(model, GetClassesLists(studentClasses), charge));
        }

        public ActionResult Save([Bind(Prefix = "Item1")] StudentDataDto model, string submitButton)
        {
            if (submitButton == "No")
            {
                // delegate sending to another controller action
                return RedirectToAction("Info", "Profile"); 
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchoolWeb/api/");
                string accessToken = null;
                try
                {
                    accessToken = Request.Cookies["token"].Value;
                }
                catch
                {
                    return View("~/Views/Shared/AccessDenied.cshtml");
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                //HTTP PUT
                if (model.PhoneNumber == null)
                    model.PhoneNumber = "";
                var postTask = client.PutAsJsonAsync<StudentDataDto>("student/info", model);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Info");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(model);
        }

        public ActionResult DeleteClass(string deleteId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchoolWeb/api/");
                string accessToken = null;
                try
                {
                    accessToken = Request.Cookies["token"].Value;
                }
                catch
                {
                    return View("~/Views/Shared/AccessDenied.cshtml");
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                //HTTP Delete
                var postTask = client.DeleteAsync("classes/" + deleteId);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Info");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View();
        }

        private IEnumerable<Tuple<ClassBasicDataDto, IEnumerable<StudentBasicDataDto>>> GetClassesLists(IEnumerable<ClassBasicDataDto> classes)
        {
            List<Tuple<ClassBasicDataDto, IEnumerable<StudentBasicDataDto>>> classesData = new List<Tuple<ClassBasicDataDto, IEnumerable<StudentBasicDataDto>>>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchoolWeb/api/");
                string accessToken = Request.Cookies["token"].Value;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                Task<HttpResponseMessage> responseTask;
                HttpResponseMessage result;

                foreach (var c in classes)
                {

                    responseTask = client.GetAsync("classes/"+c.Id.ToString()+"/students");
                    responseTask.Wait();

                    result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IEnumerable<StudentBasicDataDto>>();
                        readTask.Wait();
                        var students = readTask.Result;
                        students = students.OrderBy(s => s.LastName);
                        classesData.Add(Tuple.Create( c, students));
                    }
                    else //web api sent error response 
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        return classesData;
                    }
                }
            }

            return classesData;
        }
    }
}
