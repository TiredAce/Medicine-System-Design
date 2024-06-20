using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Test.Models;
using MySqlConnector;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;

namespace MVC_Test.Controllers
{
    public class ManagerController : Controller
    {

        private DrugSystemContext db = new DrugSystemContext();
        public IActionResult Index()
        {
            ViewBag.OrderCnt = db.Orders.Count();
            ViewBag.OrderSum = db.Orders.Sum(o => o.Price * o.Number);
            ViewBag.UnPay = db.Orders.Count(o => o.State == "Pending payment");
            ViewBag.UnDeliver = db.Orders.Count(o => o.State == "Pending delivery");
            ViewBag.Delivered = db.Orders.Count(o => o.State == "Transitting delivery");
            ViewBag.Received = db.Orders.Count(o => o.State == "Received delivery");


            ViewBag.AgeGroup0_18Count = db.Customers.Count(c => c.Age >= 0 && c.Age <= 18);
            ViewBag.AgeGroup19_30Count = db.Customers.Count(c => c.Age >= 19 && c.Age <= 30);
            ViewBag.AgeGroup31_45Count = db.Customers.Count(c => c.Age >= 31 && c.Age <= 45);
            ViewBag.AgeGroup46_60Count = db.Customers.Count(c => c.Age >= 46 && c.Age <= 60);
            ViewBag.AgeGroup61PlusCount = db.Customers.Count(c => c.Age >= 61);


            var topMedicines = db.Orders
            .GroupBy(m => m.Mname) // 按照药品名称聚合
            .Select(g => new
            {
                Sname = g.Key, // 药品名称
                TotalSales = g.Sum(m => m.Number) // 对销售数量求和
            })
            .OrderByDescending(g => g.TotalSales) // 根据销售数量倒序排序
            .Take(10) // 获取指定数量的药品
            .ToList();
            Console.Write(topMedicines.Count());
            
            return View(topMedicines);
        }




        public IActionResult Supplier_Information()
        {
            var customers = db.Suppliers.ToList();
            return View(customers);
        }

        public IActionResult Import_Supplier_Information()
        {
            return View();
        }
        public IActionResult Customer_Information()
        {
            var customers = db.Customers.ToList();
            return View(customers);
        }

        [HttpPost]
        public IActionResult search(string searchText)
        {
            var query = db.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c => c.Cid.Contains(searchText) ||
                                         c.Cname.Contains(searchText) ||
                                         c.Telephone.Contains(searchText) ||
                                         c.Email.Contains(searchText) ||
                                         c.Province.Contains(searchText) ||
                                         c.City.Contains(searchText) ||
                                         c.District.Contains(searchText) ||
                                         c.DetailedAddress.Contains(searchText));
            }

            var customers = query.ToList();
            return View("Customer_Information", customers);
        }

        public IActionResult search2(string searchText)
        {
            var query = db.Suppliers.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c => c.Sid.Contains(searchText) ||
                                         c.Sname.Contains(searchText) ||
                                         c.Telephone.Contains(searchText) ||
                                         c.Email.Contains(searchText) ||
                                         c.Province.Contains(searchText) ||
                                         c.City.Contains(searchText) ||
                                         c.District.Contains(searchText) ||
                                         c.DetailedAddress.Contains(searchText) ||
                                         c.ContactPerson.Contains(searchText));
            }

            var suppliers = query.ToList();
            return View("Supplier_Information", suppliers);
        }

        [HttpPost]
        public IActionResult DeleteSelected(List<string> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var customer = db.Customers.Find(id);
                    if (customer != null)
                    {
                        db.Customers.Remove(customer);
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
        public IActionResult DeleteSelected2(List<string> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var supplier = db.Suppliers.Find(id);
                    if (supplier != null)
                    {
                        db.Suppliers.Remove(supplier);
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
        public IActionResult UpdateCustomer(Customer customer)
        {
                var existingCustomer = db.Customers.FirstOrDefault(c => c.Cid == customer.Cid);
                if (existingCustomer != null)
                {
                    existingCustomer.Cname = customer.Cname;
                    existingCustomer.Telephone = customer.Telephone;
                    existingCustomer.Age = customer.Age;
                    existingCustomer.Province = customer.Province;
                    existingCustomer.City = customer.City;
                    existingCustomer.District = customer.District;
                    existingCustomer.DetailedAddress = customer.DetailedAddress;
                    existingCustomer.Email = customer.Email;

                    db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdateSupplier(Supplier supplier)
        {
            var existingCustomer = db.Suppliers.FirstOrDefault(c => c.Sid == supplier.Sid);
            if (existingCustomer != null)
            {
                existingCustomer.Sname = supplier.Sname;
                existingCustomer.Telephone = supplier.Telephone;
                existingCustomer.ContactPerson = supplier.ContactPerson;
                existingCustomer.Province = supplier.Province;
                existingCustomer.City = supplier.City;
                existingCustomer.District = supplier.District;
                existingCustomer.DetailedAddress = supplier.DetailedAddress;
                existingCustomer.Email = supplier.Email;

                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = true });
        }

        public IActionResult Import_Customer_Information()
        {
            return View();
        }


        [HttpPost]
        public IActionResult AddCustomer(Customer customer)
        {
            try
            {
                var existingCustomer = db.Customers.FirstOrDefault(c => c.Cid == customer.Cid);
                if (existingCustomer != null)
                {
                    return Json(new { success = false, message = "Cid already exists" });
                }

                db.Customers.Add(customer);
                db.SaveChanges();
                return Json(new { success = true});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddSupplier(Supplier model)
        {
            try
            {
                var existingSupplier = db.Suppliers.FirstOrDefault(c => c.Sid == model.Sid);
                if (existingSupplier != null)
                {
                    return Json(new { success = false, message = "Sid already exists" });
                }

                db.Suppliers.Add(model);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Please select a valid Excel file.";
                return View();
            }

            var customers = new List<Customer>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        ViewBag.Error = "No worksheet found in the Excel file.";
                        return View();
                    }

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        string cid = worksheet.Cells[row, 1].Text;

                        // 检查数据库中是否已存在相同的Cid
                        if (!db.Customers.Any(c => c.Cid == cid))
                        {
                            var customer = new Customer
                            {
                                Cid = cid,
                                Cname = worksheet.Cells[row, 2].Text,
                                Telephone = worksheet.Cells[row, 3].Text,
                                Age = int.Parse(worksheet.Cells[row, 4].Text),
                                Province = worksheet.Cells[row, 5].Text,
                                City = worksheet.Cells[row, 6].Text,
                                District = worksheet.Cells[row, 7].Text,
                                DetailedAddress = worksheet.Cells[row, 8].Text,
                                Email = worksheet.Cells[row, 9].Text
                            };
                            customers.Add(customer);
                        }
                        else
                        {
                            // 如果Cid已存在，则可以记录日志或进行其他处理
                            Console.WriteLine($"Skipping Customer with Cid {cid} as it already exists in the database.");
                        }
                    }
                }
            }

            db.Customers.AddRange(customers);
            db.SaveChanges();

            ViewBag.Message = "Customers uploaded successfully!";
            return View("Import_Customer_Information");
        }


        [HttpPost]
        public IActionResult Upload2(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Please select a valid Excel file.";
                return View();
            }

            var suppliers = new List<Supplier>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        ViewBag.Error = "No worksheet found in the Excel file.";
                        return View();
                    }

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        string sid = worksheet.Cells[row, 1].Text;

                        if (!db.Suppliers.Any(s => s.Sid == sid))
                        {
                            var supplier = new Supplier
                            {
                                Sid = sid,
                                Sname = worksheet.Cells[row, 2].Text,
                                Telephone = worksheet.Cells[row, 3].Text,
                                Email = worksheet.Cells[row, 4].Text,
                                Province = worksheet.Cells[row, 5].Text,
                                City = worksheet.Cells[row, 6].Text,
                                District = worksheet.Cells[row, 7].Text,
                                DetailedAddress = worksheet.Cells[row, 8].Text,
                                ContactPerson = worksheet.Cells[row, 9].Text
                            };
                            suppliers.Add(supplier);
                        }
                        else
                        {
                            // 如果Cid已存在，则可以记录日志或进行其他处理
                            Console.WriteLine($"Skipping Customer with Cid {sid} as it already exists in the database.");
                        }
                    }
                }
            }

            db.Suppliers.AddRange(suppliers);
            db.SaveChanges();

            ViewBag.Message = "Supplier uploaded successfully!";
            return View("Import_Supplier_Information");
        }

        /// 发起支付请求
        /// </summary>
        /// <param name="tradeno">外部订单号，商户网站订单系统中唯一的订单号</param>
        /// <param name="subject">订单名称</param>
        /// <param name="totalAmout">付款金额</param>
        /// <param name="itemBody">商品描述</param>
        /// <returns></returns>
        [HttpPost]
        public void PayRequest(string tradeno, string subject, string totalAmout, string itemBody)
        {
            DefaultAopClient client = new DefaultAopClient(Config.Gatewayurl, Config.AppId, Config.PrivateKey, "json", "2.0",
                Config.SignType, Config.AlipayPublicKey, Config.CharSet, false);


            // 组装业务参数model
            AlipayTradePagePayModel model = new AlipayTradePagePayModel();
            model.Body = itemBody;
            model.Subject = subject;
            model.TotalAmount = totalAmout;
            model.OutTradeNo = tradeno;
            model.ProductCode = "FAST_INSTANT_TRADE_PAY";

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            request.SetReturnUrl("http://localhost:5000/Pay/Callback");
            // 设置异步通知接收地址
            request.SetNotifyUrl("");
            // 将业务model载入到request
            request.SetBizModel(model);

            var response = client.SdkExecute(request);
            Console.WriteLine($"订单支付发起成功，订单号：{tradeno}");
            //跳转支付宝支付
            Response.Redirect(Config.Gatewayurl + "?" + response.Body);
        }


    }


}
