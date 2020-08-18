using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NomadMVC.Models;
using NomadMVC.Api;
using System.Data;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace NomadMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private ApiHelper _uow;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this._configuration = configuration;

            this._uow = new ApiHelper(_configuration.GetConnectionString("EliasDatabase"));
        }

        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string userName, string password) {
            if (this._uow.AuthenticateUser(userName, password)) {
                return RedirectToAction("List");
            }

            return View();
        }

        public IActionResult Register(string userName, string password, string confirmPassword) {
            if (userName != null && userName != "" && password != null && password != "") {
                if (this._uow.UserExits(userName)) {
                    ViewData["errorMessage"] = "Please register a with a different username.";
                    return View();
                }

                this._uow.InsertUser(userName, password);
                return View("Index");
            }

            return View();
        }

        public IActionResult List(int customItemId, int page, string filter)
        {   
            if (customItemId != 0) {
                this._uow.DeleteCustomItem(customItemId);
            }

            int count = this._uow.GetCustomItemCount(filter);
            int pageStart = 1;
            int pageEnd = (count - 1) / 10;
            int totalPages = 19;

            if (page > 9) {
                pageEnd = page + 10;
                if (pageEnd > (count - 1) / 10) {
                    pageEnd = (count - 1) / 10;
                }
                
                int pagesLeft = totalPages - (pageEnd - page);
                pageStart = page - pagesLeft;
                if (pageStart < 1) {
                    pageStart = 1;
                }
            } else {
                pageStart =  1;
                
                int pagesLeft = totalPages - Math.Abs(page - pageStart);
                if (page == 0)
                    pagesLeft += 2;

                pageEnd = page + pagesLeft;
                if (pageEnd > (count - 1) / 10) {
                    pageEnd = (count - 1) / 10;
                }
            }

            // if (pageStart < 1) {
            //     pageStart = 1;
            // }
            // if (count > 12 * 10) {
            //     if (page > 6) {
            //         ViewData["PageStart"] = page - 6;
            //     } else {
            //         ViewData["PageStart"] = 1;
            //     }
                
            //     if (page < ((count - 1) / 10) - 6)
            //         ViewData["PageEnd"] = page + 6;
            //     else
            //         ViewData["PageEnd"] = (count - 1) / 10;
            // } else {
            //     ViewData["PageStart"] = 1;
            //     ViewData["PageEnd"] = (count - 1) / 10;
            // }

            ViewData["PageStart"] = pageStart;
            ViewData["PageEnd"] = pageEnd;
            ViewData["CustomItemCount"] = count;
            ViewData["Page"] = page;
            ViewData["Filter"] = filter;

            return View(this._uow.GetCustomItems(page, filter));
        }
        
        public IActionResult Edit(int customItemId, int parentId)
        {
            EliasModel customItem = this._uow.GetCustomItem(customItemId);
            if (parentId != 0)
                customItem.ParentId = parentId;

            ViewData["CustomItemTypes"] = this._uow.GetCustomItemTypes();

            return View(customItem);
        }
        [HttpPost]
        public IActionResult Edit(EliasModel customItem, IFormFile image) {
            if (customItem != null && customItem.Name.Length > 0) {
                if (customItem.Id == 0 && image.FileName.Length > 0) {
                    this._uow.InsertCustomItem(customItem, image);
                } else {
                    this._uow.UpdateCustomItem(customItem);
                }

                return RedirectToAction("List");
            }

            return View(customItem);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
