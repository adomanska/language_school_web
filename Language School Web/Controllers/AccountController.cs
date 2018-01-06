﻿using Language_School_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using System.Web;
using Newtonsoft.Json;
using System.Text;

namespace Language_School_Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = new HttpClient())
            {
                
                TokenModel token = new TokenModel();

                //HTTP POST
                //var postTask = client.PostAsJsonAsync<LoginViewModel>("token", model);
                //postTask.Wait();
                client.BaseAddress = new Uri("http://projektnet.mini.pw.edu.pl");
                var request = new HttpRequestMessage(HttpMethod.Post, "/LanguageSchool/token");

                var requestContent = string.Format("grant_type={0}&username={1}&password={2}", Uri.EscapeDataString("password"),
                    Uri.EscapeDataString(model.Username), Uri.EscapeDataString(model.Password));
                request.Content = new StringContent(requestContent, Encoding.UTF8, "application/x-www-form-urlencoded");

              
                var postTask = client.SendAsync(request);
                postTask.Wait();

                var res = postTask.Result;
                if (res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var response = res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    token = JsonConvert.DeserializeObject<TokenModel>(response);
                    Response?.SetCookie(new HttpCookie("token", token.Access_Token));
                    return RedirectToAction("../Home/Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(model);
        }

        public ActionResult Register()
        {
            ViewBag.Message = "Your register page.";

            return View();
        }
    }
}
