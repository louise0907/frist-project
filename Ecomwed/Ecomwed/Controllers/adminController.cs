using Ecomwed.Models;
using Ecomwed.Models.admin;
using Microsoft.AspNetCore.Mvc;
using Ecomwed.viewmodel;
using Microsoft.EntityFrameworkCore;
using Ecomwed.Models.custermor;
using Ecomwed.Models.seller;
using Ecomwed.Models.product;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Ecomwed.Controllers
{
    public class adminController : Controller
    {
        private readonly Cappdbcontext db;

        public adminController(Cappdbcontext context)
        {
            db = context;
        }
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult login(string username, string password)
        {
            var result = db.admins.FirstOrDefault(x => x.username == username && x.password == password);
            if (result == null)
            {
                ViewBag.ErrorMsg = "Wrong UserName or Password";
                return View();
            }
            HttpContext.Session.SetString("adminsId", result.Id.ToString());
            return RedirectToAction(nameof(Chart));
        }
        public IActionResult homepage()
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));

            return View(nameof(homepage));
          
        }
        public IActionResult custormerlist()
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));

            var result = db.customers.ToList();
            return View(result);
        }
        public IActionResult customerdetail(int? id)
        {

            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));

            if (id == null)
                return RedirectToAction(nameof(custormerlist));

            // Fetch the customer with profile and transaction history
            var customer = db.customers.Include(c => c.profile).FirstOrDefault(c => c.ID == id);
            if (customer == null)
                return RedirectToAction(nameof(custormerlist));

            var transHistory = db.tanshhistory.Where(t => t.customersID == id).ToList();

            var viewModel = new adminseecustormerdetail
            {
                customers = customer,
                profile = customer.profile,
                tanshhistory = transHistory // Assuming you want the latest or a specific transaction
            };

            return View(viewModel);
        }
            //public IActionResult customerdetail(int? id)
            //{
            //    var loginuser = LoginUser().Result;
            //    if (loginuser == null)
            //        return RedirectToAction(nameof(login));
            //    var profile = db.customers.Include(x => x.profile).Select(x => new customer_profile
            //    {
            //        customers = x,
            //        profile = x.profile,


            //    });

            //    var getcustormer = db.customers.FirstOrDefault(x => x.ID == id);
            //    var product = db.customerprofile.FirstOrDefault(x => x.ID == getcustormer.customerprofileID);
            //    if (product == null)
            //        return RedirectToAction(nameof(custormerlist));

            //    return View(product);

            //}
            public IActionResult sellerlist()
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));

            var result = db.sellers.ToList();
            return View(result);
        }
        public IActionResult sellerdetail(int? id)
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));
            var profile = db.sellers.Include(x => x.profile).Select(x => new seller_profilecs
            {
                seller = x,
                profile = x.profile
            });

            var getcustormer = db.sellers.FirstOrDefault(x => x.ID == id);
            var product = db.sellerprofile.FirstOrDefault(x => x.ID == getcustormer.sellerprofileID);
            if (product == null)
                return RedirectToAction(nameof(custormerlist));

            return View(product);
        }
        public IActionResult productlist()
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));

            var result = db.products.ToList();
            return View(result);
        }
        public IActionResult productDeactive(int id)
        {
           
            var finproduct = db.products.FirstOrDefault(x => x.Id == id);
            if (finproduct != null)
            {
                finproduct.status = productstatus.Inactive;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public IActionResult productactive(int id)
        {
            var findproduct = db.products.FirstOrDefault(x => x.Id == id);
            if (findproduct != null)
            {
                findproduct.status = productstatus.Active;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
          
        }
		[HttpGet]
		public IActionResult GetproductStatus(int id)
		{
			var finpproduct = db.products.FirstOrDefault(x => x.Id == id);
			if (finpproduct != null)
			{
				return Json(new { status = finpproduct.status.ToString() });
			}
			return Json(new { status = "Unknown" });
		}
		[HttpPost]
        public IActionResult CustomerDeactive(int id)
        {
            var finpcustomer = db.customers.FirstOrDefault(x => x.ID == id);
            if (finpcustomer != null)
            {
                finpcustomer.status = customerstatus.Inactive;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult CustomerActive(int id)
        {
            var finpcustomer = db.customers.FirstOrDefault(x => x.ID == id);
            if (finpcustomer != null)
            {
                finpcustomer.status = customerstatus.Active;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpGet]
        public IActionResult GetCustomerStatus(int id)
        {
            var finpcustomer = db.customers.FirstOrDefault(x => x.ID == id);
            if (finpcustomer != null)
            {
                return Json(new { status = finpcustomer.status.ToString() });
            }
            return Json(new { status = "Unknown" });
        }
        [HttpPost]
        public IActionResult sellerDective(int id)
        {

            var findseller = db.sellers.FirstOrDefault(x => x.ID == id);
            if (findseller != null)
            {
                findseller.status = sellersstatus.Inactive;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
         
        }
        [HttpPost]
        public IActionResult selleractive(int id)
        {
            var findseller = db.sellers.FirstOrDefault(x => x.ID == id);
            if (findseller != null)
            {
                findseller.status = sellersstatus.Active;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpGet]
        public IActionResult GetsellerStatus(int id)
        {
            var finpseller = db.sellers.FirstOrDefault(x => x.ID == id);
            if (finpseller != null)
            {
                return Json(new { status = finpseller.status.ToString() });
            }
            return Json(new { status = "Unknown" });
        }
        public IActionResult createcategoreis()
		{
			var loginuser = LoginUser().Result;
			if (loginuser == null)
				return RedirectToAction(nameof(login));

			return View();
		}
        [HttpPost]
        public IActionResult createcategoreis(IFormFile? file, categories? newcategories)
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
            return RedirectToAction(nameof(login));

			var result = db.categories.FirstOrDefault(x => x.Categoryname == newcategories.Categoryname);
            if (result != null)
            {
                ViewBag.ErrorMsg = "cannot same name";
                return View();
            }
            if(string.IsNullOrEmpty(newcategories.description)||string.IsNullOrEmpty(newcategories.Categoryname))
            {
				ViewBag.ErrorMsg = "cannot be empty";
				return View();
			}
			else
			{
				if (file != null && file.Length > 0) //cheack have image file
				{
					try
					{
						//save image to local project
						//determiner the path to save the upload file within the project
						var uploadsfolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image");
						//cheack if the directory exixts, if not ,create it
						if (!Directory.Exists(uploadsfolder))
						{
							Directory.CreateDirectory(uploadsfolder);
						}
						var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						var filepath = Path.Combine(uploadsfolder, filename);
						//save the file to the server
						using (var stream = new FileStream(filepath, FileMode.Create))
						{
							file.CopyTo(stream);
						}
						//save image to local project
						newcategories.image = filename;
					}
					catch
					{
						ViewBag.ErrorMsg = "Upload Image Failde";
					}
				}
			}
		
			db.categories.Add(newcategories);
			db.SaveChanges();
			return RedirectToAction(nameof(categorieslist));
		}
        public IActionResult updatcategories(int? id)
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));
            var categories = db.categories.FirstOrDefault(x => x.Id == id);
            if (categories == null)
                return RedirectToAction(nameof(categorieslist));

            return View(categories);
        }
        [HttpPost]
		public IActionResult updatcategories(IFormFile? file, categories updatecategpries ,int? Id)
		{
			var loginuser = LoginUser().Result;
			if (loginuser == null)
				return RedirectToAction(nameof(login));
          
            var categorieid = db.categories.FirstOrDefault(x=>x.Id ==Id);
			if (categorieid == null || categorieid.Id == 0)
				return RedirectToAction(nameof(categorieslist));
			if (string.IsNullOrEmpty(updatecategpries.Categoryname) || string.IsNullOrEmpty(updatecategpries.description))

			{
				ViewBag.ErrorMsg = "cannot be empty";
				return View();
			}

			else
			{
				if (file != null && file.Length > 0) //cheack have image file
				{
					try
					{
						//save image to local project
						//determiner the path to save the upload file within the project
						var uploadsfolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image");
						//cheack if the directory exixts, if not ,create it
						if (!Directory.Exists(uploadsfolder))
						{
							Directory.CreateDirectory(uploadsfolder);
						}
						var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						var filepath = Path.Combine(uploadsfolder, filename);
						//save the file to the server
						using (var stream = new FileStream(filepath, FileMode.Create))
						{
							file.CopyTo(stream);
						}
						//save image to local project
						updatecategpries.image = filename;
					}
					catch
					{
						ViewBag.ErrorMsg = "Upload Image Failde";
					}
				}
			}
             categorieid.Categoryname= updatecategpries.Categoryname;
            categorieid.description = updatecategpries.description;
            categorieid.image = updatecategpries.image;
			db.SaveChanges();
			return RedirectToAction(nameof(categorieslist));
		}
		public IActionResult categoriesdetail(int? id)
		{
			var loginuser = LoginUser().Result;
			if (loginuser == null)
				return RedirectToAction(nameof(login));

			var categories = db.categories.FirstOrDefault(x => x.Id == id);
			if (categories == null)
				return RedirectToAction(nameof(productlist));

			return View(categories);
		}
		public IActionResult categorieslist()
		{
			var loginuser = LoginUser().Result;
			if (loginuser == null)
				return RedirectToAction(nameof(login));
            var result = db.categories.ToList();
            return View(result);


        }

        //public IActionResult Chart(string dateRange = "2024-07-19")
        //{
        //    var loginuser = LoginUser().Result;
        //    if (loginuser == null)
        //        return RedirectToAction(nameof(login));
        //    try
        //    {
        //        var totalSales = db.detail.Sum(d => d.QTY); // Summing the QTY field for total sales
        //        var totalComments = db.review.Count(); // Assuming 'review' is your DbSet for comments
        //        var totalCustomers = db.customers.Count(); // Assuming 'customers' is your DbSet for customers
        //        var totalSellers = db.sellers.Count(); // Assuming 'sellers' is your DbSet for sellers

        //        var startDate = DateTime.Parse(dateRange);
        //        var endDate = startDate.AddDays(1);

        //        var earningsToday = db.detail
        //                              .Where(d => d.tanshhistory.time >= startDate && d.tanshhistory.time < endDate)
        //                              .Sum(d => d.price * d.QTY);

        //        var earningsYesterday = db.detail
        //                                   .Where(d => d.tanshhistory.time >= startDate.AddDays(-1) && d.tanshhistory.time < startDate)
        //                                   .Sum(d => d.price * d.QTY);

                

        //        var dashboardData = new DashboardViewModel
        //        {
        //            TotalSales = totalSales,
        //            TotalComments = totalComments,
        //            TotalCustomers = totalCustomers,
        //            TotalSellers = totalSellers,
        //            EarningsToday = earningsToday,
        //            EarningsYesterday = earningsYesterday,
                   
        //            DateRange = dateRange
        //        };

        //        return View(dashboardData);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //        return Json(new { success = false, message = "An error occurred while processing your request." });
        //    }
        //}

        public IActionResult Chart(string dateRange = "2024-07-18")
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));

            try
            {
                var totalSales = db.detail.Sum(d => d.QTY); // Summing the QTY field for total sales
                var totalComments = db.review.Count(); // Assuming 'review' is your DbSet for comments
                var totalCustomers = db.customers.Count(); // Assuming 'customers' is your DbSet for customers
                var totalSellers = db.sellers.Count(); // Assuming 'sellers' is your DbSet for sellers

                if (!DateTime.TryParse(dateRange, out DateTime startDate))
                {
                    // If parsing fails, set startDate to today
                    startDate = DateTime.Today;
                }

                var endDate = startDate.AddDays(1);

                var earningsToday = db.detail
                                      .Where(d => d.tanshhistory.time >= startDate && d.tanshhistory.time < endDate)
                                      .Sum(d => d.price * d.QTY);

                var earningsYesterday = db.detail
                                          .Where(d => d.tanshhistory.time >= startDate.AddDays(-1) && d.tanshhistory.time < startDate)
                                          .Sum(d => d.price * d.QTY);

                var dashboardData = new DashboardViewModel
                {
                    TotalSales = totalSales,
                    TotalComments = totalComments,
                    TotalCustomers = totalCustomers,
                    TotalSellers = totalSellers,
                    EarningsToday = earningsToday,
                    EarningsYesterday = earningsYesterday,
                    DateRange = dateRange
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }

        public async Task<admins?> LoginUser()
        {
            var adminid = HttpContext.Session.GetString("adminsId");
            if (adminid == null)
                return null;

            var admin = await db.admins.FirstOrDefaultAsync(x => x.Id.ToString() == adminid);
            return admin;


        }
    }




}
