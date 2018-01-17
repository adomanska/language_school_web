using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LanguageSchool.Models;
using Microsoft.Win32;
using SelectPdf;

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
                var responseTask = client.GetAsync("classes");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ClassDataDto>>();
                    readTask.Wait();

                    allClasses = readTask.Result;
                }
                else
                    return View("~/Views/Shared/Error.cshtml");

                responseTask = client.GetAsync("classes/top/3");
                responseTask.Wait();

                result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ClassBasicDataDto>>();
                    readTask.Wait();
                    topClasses = readTask.Result;
                }
                else
                    return View("~/Views/Shared/Error.cshtml");

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
                    else
                        return View("~/Views/Shared/Error.cshtml");

                    responseTask = client.GetAsync("student/classes");
                    responseTask.Wait();

                    result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<ClassBasicDataDto>>();
                        readTask.Wait();

                        studentClasses = readTask.Result;
                    }
                    else
                        return View("~/Views/Shared/Error.cshtml");

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
            HttpCookie cookie = Request.Cookies["token"];
            if (cookie != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchoolWeb/api/");
                    string accessToken = cookie.Value;
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
                            .Select(group => new ScheduleDay() { Day = group.Key, Items = group.ToList() });
                    }
                    else
                        return View("~/Views/Shared/Error.cshtml");
                }

                return View(scheduleDays);
            }
            else
                return RedirectToAction("ScheduleView");
        }

        public ActionResult SignFor(int classId)
        {
            HttpCookie cookie = Request.Cookies["token"];
            if (cookie != null)
            {
                string accessToken = cookie.Value;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchoolWeb/api/");
                    accessToken = cookie.Value;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var postTask = client.PostAsync("classes/" + classId.ToString(), null);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
                return Index();
            }
            else
                return View("~/Views/Shared/AccessDenied.cshtml");
        }

        public ActionResult GetPdf()
        {
            HttpCookie cookie = Request.Cookies["token"];
            if (cookie != null)
            {
                string accessToken = cookie.Value;
                HtmlToPdf converter = new HtmlToPdf();
                converter.Options.HttpCookies.Add("token", accessToken);
                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                converter.Options.MarginLeft = 0;
                converter.Options.MarginRight = 0;
                converter.Options.MarginTop = 0;
                converter.Options.MarginBottom = 0;
                PdfDocument doc = converter.ConvertUrl("http://localhost:53091/Classes/Schedule");
                DateTime now = DateTime.Now;
                string fileName = "/Schedule" + now.Year.ToString() + now.Month.ToString() + now.Day.ToString() + now.Hour.ToString() + now.Minute.ToString() + now.Second.ToString() + ".pdf";
                doc.Save(getDownloadFolderPath() + fileName);
                doc.Close();
                return RedirectToAction("ScheduleView");
            }
            else
                return View("~/Views/Shared/AccessDenied.cshtml");
        }

        public ActionResult ScheduleView()
        {
            HttpCookie cookie = Request.Cookies["token"];
            if (cookie != null)
                return View();
            else
                return View("~/Views/Shared/AccessDenied.cshtml");
        }

        string getDownloadFolderPath()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
        }
    }
}