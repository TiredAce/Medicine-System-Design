using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core;
using Microsoft.AspNetCore.Mvc;
using MVC_Test.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace MVC_Test.Controllers
{
    public class CustomerController : Controller
    {
        private DrugSystemContext db = new DrugSystemContext();
        public IActionResult Index()
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            var medicines = db.Medicines.ToList();
            return View(medicines);
        }

        public IActionResult Profile()
        {
            // 获取当前用户的用户名
            var username = HttpContext.Session.GetString("username");

            var customer = db.Customers.FirstOrDefault(s => s.Cid == username);

            var viewModel = new Customer
            {
                Cid = customer?.Cid, // 设置供应商ID
                Cname = customer?.Cname, // 设置供应商名称
                Telephone = customer?.Telephone, // 设置供应商电话
                Email = customer?.Email, // 设置供应商邮箱
                Province = customer?.Province, // 设置供应商省份
                City = customer?.City, // 设置供应商城市
                District = customer?.District, // 设置供应商区县
                DetailedAddress = customer?.DetailedAddress, // 设置供应商详细地址
                Age = customer?.Age ?? 0
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult UpdateCustomer(string cname, string telephone, int age, string province, string city, string district, string detailedAddress, string email)
        {
            
                var cid = HttpContext.Session.GetString("username");
                // 根据sid从数据库中查找对应的供应商
                var customer = db.Customers.FirstOrDefault(s => s.Cid == cid);

                if (customer == null)
                {
                    return NotFound(); // 如果找不到供应商，返回404错误
                }

                // 更新供应商信息
                customer.Cname = cname;
                customer.Telephone = telephone;
                customer.Email = email;
                customer.Province = province;
                customer.City = city;
                customer.District = district;
                customer.DetailedAddress = detailedAddress;
                customer.Age = age;

                // 保存更改到数据库
                db.SaveChanges();

                // 可以根据需要返回不同的结果，比如重定向到供应商详情页或返回一个成功消息
                return RedirectToAction("profile");
            
            
        }

        public IActionResult PendingP()
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            var orders = db.Orders.Where(m => m.Cid == username1 && m.State == "Pending payment").ToList();
            return View(orders);
        }

        public IActionResult PendingD()
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            var orders = db.Orders.Where(m => m.Cid == username1 && m.State == "Pending delivery").ToList();
            return View(orders);
        }

        public IActionResult Transitting()
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            var orders = db.Orders.Where(m => m.Cid == username1 && m.State == "Transitting delivery").ToList();
            return View(orders);
        }

        public IActionResult Received()
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            var orders = db.Orders.Where(m => m.Cid == username1 && m.State == "Received delivery").ToList();
            return View(orders);
        }

        [HttpPost]
        public IActionResult searchmedicine(string searchText)
        {
            var query = db.Medicines.AsQueryable();
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c =>  (c.Mid.Contains(searchText) ||
                                  c.Mname.Contains(searchText) ||
                                  c.Details.Contains(searchText)));
            }

            var medicines = query.ToList();
            return View("index", medicines);
        }
        [HttpPost]
        public IActionResult DeleteSelectedOrder(List<string> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    if (int.TryParse(id, out int intId))
                    {
                        var order = db.Orders.Find(intId);
                        if (order != null)
                        {
                            db.Orders.Remove(order);
                        }
                    }
                }
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult SignFor(List<string> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    if (int.TryParse(id, out int intId))
                    {
                        var order = db.Orders.Find(intId);
                        if (order != null)
                        {
                            order.State = "Received delivery"; // 修改订单状态
                        }
                    }
                }
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult SearchPendingPOrder(string searchText)
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            var query = db.Orders.AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c => (c.State == "Pending payment" && c.Cid == username1 && (c.Oid.ToString() == searchText ||
                                  c.Mid.Contains(searchText) ||
                                  c.Mname.Contains(searchText) ||
                                  c.Sid.Contains(searchText))) );
            }
            else
            {
                query = query.Where(c => (c.State == "Pending payment" && c.Cid == username1));
            }

            var medicines = query.ToList();
            return View("PendingP", medicines);
        }

        [HttpPost]
        public IActionResult SearchPendingDOrder(string searchText)
        {
            var query = db.Orders.AsQueryable();
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c => (c.State == "Pending delivery" && c.Cid == username1 &&( c.Oid.ToString() == searchText ||
                                  c.Mid.Contains(searchText) ||
                                  c.Mname.Contains(searchText) ||
                                  c.Sid.Contains(searchText))));
            }
            else
            {
                query = query.Where(c => (c.State == "Pending delivery" && c.Cid == username1));
            }

            var medicines = query.ToList();
            return View("PendingD", medicines);
        }

        [HttpPost]
        public IActionResult SearchTransitting(string searchText)
        {
            var query = db.Orders.AsQueryable();
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c => (c.State == "Transitting delivery" && c.Cid == username1 && (c.Oid.ToString() == searchText ||
                                  c.Mid.Contains(searchText) ||
                                  c.Mname.Contains(searchText) ||
                                  c.Sid.Contains(searchText))));
            }
            else
            {
                query = query.Where(c => (c.State == "Transitting delivery" && c.Cid == username1));
            }

            var medicines = query.ToList();
            return View("Transitting", medicines);
        }

        [HttpPost]
        public IActionResult SearchReceived(string searchText)
        {
            var query = db.Orders.AsQueryable();
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c => (c.State == "Received delivery" && c.Cid == username1 && (c.Oid.ToString() == searchText ||
                                  c.Mid.Contains(searchText) ||
                                  c.Mname.Contains(searchText) ||
                                  c.Sid.Contains(searchText))));
            }
            else
            {
                query = query.Where(c => (c.State == "Received delivery" && c.Cid == username1));
            }

            var medicines = query.ToList();
            return View("Received", medicines);
        }


        [HttpPost]
        public IActionResult Buymedicine(string Mid, string Mname, decimal Price, int Quantity, string Sid, string Details, int BuyQuantity)
        {
            try
            {
                var username1 = HttpContext.Session.GetString("username");
                var medicine = db.Medicines.FirstOrDefault(m => m.Mid == Mid);
                if (medicine == null)
                {
                    return Json(new { success = false, message = "Medicine not found" });
                }

                if (BuyQuantity <= 0)
                {
                    return Json(new { success = false, message = "Buy quantity must be greater than zero" });
                }

                if (BuyQuantity > medicine.Quantity)
                {
                    return Json(new { success = false, message = "Insufficient stock" });
                }
                var customer = db.Customers.FirstOrDefault(c => c.Cid == username1);
                // Create new order
                var order = new Order
                {
                    Cid = username1,
                    Mid = Mid,
                    Sid = Sid,
                    Mname = medicine.Mname,
                    Number = BuyQuantity,
                    Time = DateTime.Now,
                    Price = Price * BuyQuantity,
                    Province = customer.Province,
                    City = customer.City,
                    District = customer.District,
                    DetailedAddress = customer.DetailedAddress,
                    State = "Pending payment"
                };

                db.Orders.Add(order);

                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // int oid, string mid, string sid, int number, decimal price, string state
        [HttpPost]
        public IActionResult PayRequest(string Oid, 
            string Cid,
            string Mid, 
            string Sid,
            string Number,
            string Time,
            string Price,
            string Province,
            string City,
            string District,
            string DetailedAddress)
        {
            DefaultAopClient client = new DefaultAopClient(Config.Gatewayurl, Config.AppId, Config.PrivateKey, "json", "2.0",
                Config.SignType, Config.AlipayPublicKey, Config.CharSet, false);
            
                // 查找 Oid 为 1 的订单
                var order = db.Orders.FirstOrDefault(o => o.Oid == int.Parse(Oid));

                if (order != null)
                {
                    // 更新状态
                    order.State = "Pending delivery";
                    db.SaveChanges();
                }

            // 组装业务参数model
            AlipayTradePagePayModel model = new AlipayTradePagePayModel();
            model.Body = "46565422525254";
            model.Subject = "5656562252525";
            model.TotalAmount = Price; 
            model.OutTradeNo = Oid;
            model.ProductCode = "FAST_INSTANT_TRADE_PAY";

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            request.SetReturnUrl("https://localhost:7101/Customer/PendingP");
            // 设置异步通知接收地址
            request.SetNotifyUrl("");
            // 将业务model载入到request
            request.SetBizModel(model);

            var response = client.SdkExecute(request);
            //跳转支付宝支付
            Response.Redirect(Config.Gatewayurl + "?" + response.Body);
            return Json(new { success = true });
        }


    }
}
