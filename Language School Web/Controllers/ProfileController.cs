﻿using LanguageSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;

namespace Language_School_Web.Controllers
{
    public class ProfileController : Controller
    {
        public ActionResult Info()
        {
            StudentDataDto model = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchool/api/");
                string accessToken = Request.Cookies["token"].Value;
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

            return View(model);
        }

        public ActionResult Save(StudentDataDto model, string submitButton)
        {
            if (submitButton == "No")
            {
                // delegate sending to another controller action
                return RedirectToAction("Info", "Profile"); 
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl/LanguageSchool/api/");
                string accessToken = Request.Cookies["token"].Value;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                //HTTP POST
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
    }
}
