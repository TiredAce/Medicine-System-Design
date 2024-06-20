using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Test.Models;
using OfficeOpenXml;

namespace MVC_Test.Controllers
{
    public class SupplierController : Controller
    {
        private DrugSystemContext db = new DrugSystemContext();
       
        public IActionResult Index()
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;

            var medicines = db.Medicines.Where(m => m.Sid == username1).ToList();
            return View(medicines);
        }
        public IActionResult import_medicine_information()
        {
            return View();
        }
        public IActionResult PendingD()
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            var orders = db.Orders.Where(m => m.Sid == username1 && m.State == "Pending delivery").ToList();
            return View(orders);
        }

        public IActionResult Transitting()
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            var orders = db.Orders.Where(m => m.Sid == username1 && m.State == "Transitting delivery").ToList();
            return View(orders);
        }

        public IActionResult Received()
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            var orders = db.Orders.Where(m => m.Sid == username1 && m.State == "Received delivery").ToList();
            return View(orders);
        }

        public IActionResult Delivery(List<string> ids)
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
                            order.State = "Transitting delivery"; // 修改订单状态
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
        public IActionResult SearchPendingDOrder(string searchText)
        {
            var query = db.Orders.AsQueryable();
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c => (c.State == "Pending delivery" && c.Sid == username1 && (c.Oid.ToString() == searchText ||
                                  c.Mid.Contains(searchText) ||
                                  c.Mname.Contains(searchText) ||
                                  c.Sid.Contains(searchText))));
            }
            else
            {
                query = query.Where(c => (c.State == "Pending delivery" && c.Sid == username1));
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
                query = query.Where(c => (c.State == "Transitting delivery" && c.Sid == username1 && (c.Oid.ToString() == searchText ||
                                  c.Mid.Contains(searchText) ||
                                  c.Mname.Contains(searchText) ||
                                  c.Sid.Contains(searchText))));
            }
            else
            {
                query = query.Where(c => (c.State == "Transitting delivery" && c.Sid == username1));
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
                query = query.Where(c => (c.State == "Received delivery" && c.Sid == username1 && (c.Oid.ToString() == searchText ||
                                  c.Mid.Contains(searchText) ||
                                  c.Mname.Contains(searchText) ||
                                  c.Sid.Contains(searchText))));
            }
            else
            {
                query = query.Where(c => (c.State == "Received delivery" && c.Sid == username1));
            }

            var medicines = query.ToList();
            return View("Received", medicines);
        }

        public IActionResult Profile()
        {
            // 获取当前用户的用户名
            var username = HttpContext.Session.GetString("username");

            var supplier = db.Suppliers.FirstOrDefault(s => s.Sid == username);

            var viewModel = new Supplier
            {
                Sid = supplier?.Sid, // 设置供应商ID
                Sname = supplier?.Sname, // 设置供应商名称
                Telephone = supplier?.Telephone, // 设置供应商电话
                Email = supplier?.Email, // 设置供应商邮箱
                Province = supplier?.Province, // 设置供应商省份
                City = supplier?.City, // 设置供应商城市
                District = supplier?.District, // 设置供应商区县
                DetailedAddress = supplier?.DetailedAddress, // 设置供应商详细地址
                ContactPerson = supplier?.ContactPerson
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult UpdateSupplier(string sname, string telephone, string email, string province, string city, string district, string detailedAddress, string contactPerson)
        {
            try
            {
                var sid = HttpContext.Session.GetString("username");
                // 根据sid从数据库中查找对应的供应商
                var supplier = db.Suppliers.FirstOrDefault(s => s.Sid == sid);

                if (supplier == null)
                {
                    return NotFound(); // 如果找不到供应商，返回404错误
                }

                // 更新供应商信息
                supplier.Sname = sname;
                supplier.Telephone = telephone;
                supplier.Email = email;
                supplier.Province = province;
                supplier.City = city;
                supplier.District = district;
                supplier.DetailedAddress = detailedAddress;
                supplier.ContactPerson = contactPerson;

                // 保存更改到数据库
                db.SaveChanges();

                // 可以根据需要返回不同的结果，比如重定向到供应商详情页或返回一个成功消息
                return RedirectToAction("profile");
            }
            catch (Exception ex)
            {
                // 如果更新过程中发生异常，可以处理异常并返回适当的错误信息或错误页面
                return RedirectToAction("profile");
            }
        }
        [HttpPost]
        public IActionResult searchmedicine(string searchText)
        {
            var username1 = HttpContext.Session.GetString("username");
            ViewBag.Username = username1;
            var query = db.Medicines.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(c => c.Sid == username1 &&
                                 (c.Mid.Contains(searchText) ||
                                  c.Mname.Contains(searchText) ||
                                  c.Details.Contains(searchText)));
            }

            var medicines = query.ToList();
            return View("index", medicines);
        }

        [HttpPost]
        public IActionResult Addmedicine(Medicine medicine)
        {
            try
            {
                if (medicine.Price < 0)
                {
                    return Json(new { success = false, message = "Price cannot be less than 0" });
                }
                if (medicine.Quantity < 0)
                {
                    return Json(new { success = false, message = "Quantity cannot be less than 0" });
                }
                var existingCustomer = db.Medicines.FirstOrDefault(c => c.Mid == medicine.Mid);
                if (existingCustomer != null)
                {
                    return Json(new { success = false, message = "Mid already exists" });
                }

                db.Medicines.Add(medicine);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult DeleteSelectedmedince(List<string> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var medicine = db.Medicines.Find(id);
                    if (medicine != null)
                    {
                        db.Medicines.Remove(medicine);
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
        public IActionResult Updatemedicine(Medicine medicine)
        {
            if (medicine.Price < 0)
            {
                return Json(new { success = false, message = "Price cannot be less than 0" });
            }
            if (medicine.Quantity < 0)
            {
                return Json(new { success = false, message = "Quantity cannot be less than 0" });
            }
            var existingCustomer = db.Medicines.FirstOrDefault(c => c.Mid == medicine.Mid);
            if (existingCustomer != null)
            {
                existingCustomer.Mname = medicine.Mname;
                existingCustomer.Price = medicine.Price;
                existingCustomer.Quantity = medicine.Quantity;
                existingCustomer.Details = medicine.Details;

                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message="error" });
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Please select a valid Excel file.";
                return View();
            }
            var username1 = HttpContext.Session.GetString("username");
            var medicines = new List<Medicine>();

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
                        string mid = worksheet.Cells[row, 1].Text;

                        // 检查数据库中是否已存在相同的Cid
                        if (!db.Medicines.Any(c => c.Mid == mid))
                        {
                            var medicine = new Medicine
                            {
                                Mid = mid,
                                Mname = worksheet.Cells[row, 2].Text,
                                Price = decimal.Parse(worksheet.Cells[row, 3].Text),
                                Quantity = int.Parse(worksheet.Cells[row, 4].Text),
                                Sid = username1,
                                Details = worksheet.Cells[row, 5].Text,
                            };
                            medicines.Add(medicine);
                        }
                        else
                        {
                            // 如果Cid已存在，则可以记录日志或进行其他处理
                            Console.WriteLine($"Skipping Customer with Cid {mid} as it already exists in the database.");
                        }
                    }
                }
            }

            db.Medicines.AddRange(medicines);
            db.SaveChanges();

            ViewBag.Message = "medicine uploaded successfully!";
            return View("import_medicine_information");
        }
    }
}
