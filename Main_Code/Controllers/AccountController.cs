using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Test.Models;
using MySqlConnector;
using Microsoft.AspNetCore.Http; 

namespace YourNamespace.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        public ActionResult Login()
        {

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(string option, string username)
        {
            if (ModelState.IsValid)
            {
                using (DrugSystemContext dbcontext = new DrugSystemContext())
                {
                    switch (option)
                    {
                        case "a":
                            // 管理员端逻辑
                            if (username == "admin")

                                return RedirectToAction("Index", "Manager");
                            else
                                ViewBag.UserDoesNotExist = true;
                            break;
                        case "b":
                            // 客户端逻辑
                            int drugCount = dbcontext.Customers.Count(c => c.Cid == username);
                            if (drugCount > 0){
                                HttpContext.Session.SetString("username", username);
                                return RedirectToAction("Index", "Customer");
                            }
                            else
                                ViewBag.UserDoesNotExist = true;
                            break;
                        case "c":
                            // 供应商端逻辑
                            drugCount = dbcontext.Suppliers.Count(c => c.Sid == username);
                            if (drugCount > 0){
                                HttpContext.Session.SetString("username", username);
                                return RedirectToAction("Index", "Supplier");
                            }
                            else
                                ViewBag.UserDoesNotExist = true;
                            break;
                        default:
                            // 默认逻辑
                            break;
                    }
                }
            }

            // 如果验证失败，返回到登录页面并显示错误信息
            return View();
        }   
    }
}