using Ecomwed.Models;
using Ecomwed.Models.custermor;
using Ecomwed.Models.product;
using Ecomwed.Models.seller;
using Ecomwed.viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Text.RegularExpressions;
using Polly;
using Newtonsoft.Json;
using System.Linq;
using Scrypt;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RestSharp;
using Microsoft.Extensions.Options;
using static System.Net.WebRequestMethods;

namespace Ecomwed.Controllers
{
    public class sellerController : Controller
    {
        private static string generatedOtp;
        private readonly Cappdbcontext db;
        public sellerController(Cappdbcontext context)
        {
            db = context;
        }
        public static Regex cheackic { get; set; } = new Regex("\\d{6}\\-\\d{2}\\-\\d{4}");
        public static Regex cheakckpoit { get; set; } = new Regex(@"^(01)[0-46-9]*[0-9]{7,8}$");
        public static Regex gmail1 { get; set; } = new Regex(@"^[a-z0-9](\.?[a-z0-9]){5,}@g(oogle)?mail\.com$");
        ScryptEncoder encoder = new ScryptEncoder();
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult login(string? username, string password)
        {
            bool check = true;
            string msg = "";

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                msg = "Cannot be empty";
                check = false;
                return Json(new { msg, check });
            }

            var result = db.sellers.FirstOrDefault(x => x.username == username && x.status == sellersstatus.Active);

            if (result == null)
            {
                msg = "Username not found";
                check = false;

                return Json(new { msg, check });
            }
            bool hashpassword = encoder.Compare(password, result.password);
            if (!hashpassword)
            {
                msg = "Incorrect password";
                check = false;
                return Json(new { msg, check });
            }

            result.logintimenow = DateTime.Now;
            db.sellers.Update(result);
            db.SaveChanges();

            HttpContext.Session.SetString("sellerid", result.ID.ToString());
            TempData["Success"] = "Login successful";
            return Json(new { msg = "Login Successful", check });


        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Register(string? username, string password, string? confirmpassword, string? email, string nric, string phonenumber, string birthday, string age, string gender)
        {
            bool check = true;
            string msg = "";
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmpassword) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(nric) || string.IsNullOrWhiteSpace(phonenumber) || string.IsNullOrWhiteSpace(birthday) || string.IsNullOrWhiteSpace(age) || string.IsNullOrWhiteSpace(gender))
            {
                msg = "Cannot be Empty";
                check = false;
                return Json(new { msg, check });
            }
            username = username.Trim();
            password = password.Trim();
            confirmpassword = confirmpassword.Trim();
            email = email.Trim();
            nric = nric.Trim();
            phonenumber = phonenumber.Trim();

            if (username.Length < 6 || password.Length < 6)
            {
                msg = "Please enter more than 6 characters";
                check = false;
                return Json(new { msg, check });
            }
            else if (password != confirmpassword)
            {
                msg = "Please enter same password ";
                check = false;
                return Json(new { msg, check });
            }
            else if (!gmail1.IsMatch(email))
            {
                msg = "error email ";
                check = false;
                return Json(new { msg, check });
            }
            else if (!cheackic.IsMatch(nric))
            {
                msg = "error NRIC ";
                check = false;
                return Json(new { msg, check });
            }
            else if (!cheakckpoit.IsMatch(phonenumber))
            {
                msg = "error Phone Number ";
                check = false;
                return Json(new { msg, check });
            }
            var cheackusername = db.sellers.FirstOrDefault(x => x.username == username);
            var cheackusername1 = db.sellerprofile.FirstOrDefault(x => x.email == email);
            if (cheackusername != null)
            {
                msg = "UserName had been Registered .Please enter another username";
                check = false;
                return Json(new { msg, check });
            }
            if (cheackusername1 != null)
            {
                msg = "email had been Registered .Please enter another email";
                check = false;
                return Json(new { msg, check });
            }

            sellerprofile newprofile = new sellerprofile()
            {
                fullname = username,
                NRIC = nric,
                phonenumber = phonenumber,
                birthday = birthday,
                age = age,
                email = email,
                gender = gender,
                address = ""

            };
            db.sellerprofile.Add(newprofile);
            db.SaveChanges();
            string hashpassword = encoder.Encode(password);
            var profiledid = db.sellerprofile.FirstOrDefault(x => x.fullname == username);
            sellers newuser = new sellers
            {
                username = username,
                password = hashpassword,
                sellerprofileID = profiledid.ID
            };
            db.sellers.Add(newuser);
            db.SaveChanges();
            TempData["Success"] = "Register Successful";
            check = true;
            return Json(new { msg, check });

        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(login));
        }
        public IActionResult Dashboard()
        {
            var loginuser = loginseller().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));

            return View(nameof(Dashboard));
        }
        public IActionResult GetProductPurchaseData()
        {
            var loginuser = loginseller().Result;
            var purchaseData = db.detail
                .Where(d => d.products.sellersID == loginuser.ID)
                .GroupBy(d => d.productsId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Count = group.Sum(d => d.QTY) // Summing the quantities bought
                })
                .ToList();

            return Json(purchaseData);
        }

        public IActionResult productlist()
        {
            var loginuser = loginseller().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));
            var result = db.products.Where(x => x.sellersID == loginuser.ID && x.status != productstatus.AdminDelete && x.status != productstatus.SellerDelete).ToList();
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
        public IActionResult GetProductStatus(int id)
        {
            var finpproduct = db.products.FirstOrDefault(x => x.Id == id);
            if (finpproduct != null)
            {
                return Json(new { status = finpproduct.status.ToString() });
            }
            return Json(new { status = "Unknown" });
        }
        public IActionResult createproduct()
        {
            var loginuser = loginseller().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));
            cate_pro cate_Pro = new cate_pro();
            cate_Pro.categories = db.categories.ToList();
            return View(cate_Pro);
        }
        [HttpPost]
        public IActionResult createproduct(IFormFile? file, cate_pro? newproduct)
        {
            var loginuser = loginseller().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));
            products newpro = new products();
            //if(!ModelState.IsValid)
            //{
            //	ViewBag.ErrorMsg = "Please DO NOT Empty";
            //	return View();
            //}   all item need to use

            //1.product name cannot same
            var result = db.products.FirstOrDefault(x => x.productname == newproduct.products.productname);
            if (result != null)
            {
                ViewBag.ErrorMsg = "cannot same name";
                return View(newproduct);
            }
            if (newproduct.products.discount < 0 || newproduct.products.normalprice < 0 || newproduct.products.stock < 0)
            {
                ViewBag.ErrorMsg = "cannot less than zero";
                return View(newproduct);
            }
            if (newproduct.products.discount > newproduct.products.normalprice)
            {
                ViewBag.ErrorMsg = "cannot more than normal price";
                return View(newproduct);
            }
            if (string.IsNullOrEmpty(newproduct.products.productname) || string.IsNullOrEmpty(newproduct.products.description))
            {
                ViewBag.ErrorMsg = "cannot be empty";
                return View(newproduct);
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
                        newpro.image = filename;
                    }
                    catch
                    {
                        ViewBag.ErrorMsg = "Upload Image Failde";
                    }
                }
            }
            //var categories = db.categories.FirstOrDefault(x => x.Id == newproduct.categories);
            //if(categories == null)
            //{
            //    ViewBag.ErrorMsg = "ivalid category";
            //    return View(newproduct);
            //}

            newpro.sellersID = loginuser.ID;
            newpro.productname = newproduct.products.productname;
            newpro.description = newproduct.products.description;
            newpro.normalprice = newproduct.products.normalprice;
            newpro.stock = newproduct.products.stock;
            newpro.discount = newproduct.products.discount;
            newpro.categoriesId = newproduct.categoriesid;
            newpro.afterdiscountprice = newproduct.products.normalprice * (100 - newproduct.products.discount) / 100;
            db.products.Add(newpro);
            db.SaveChanges();
            return RedirectToAction(nameof(productlist));
        }
        public IActionResult updataproduct(int? id)
        {
            var loginuser = loginseller().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));
            var product = db.products.FirstOrDefault(x => x.Id == id && x.sellersID == loginuser.ID && x.status != productstatus.AdminDelete && x.status != productstatus.SellerDelete);
            if (product == null)
                return RedirectToAction(nameof(productlist));

            HttpContext.Session.SetInt32("productid", product.Id);
            return View(product);
        }
        [HttpPost]
        public IActionResult updataproduct(IFormFile? file, products updateproduct)
        {
            var loginuser = loginseller().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));

            var productid = HttpContext.Session.GetInt32("productid");
            if (productid == null || productid == 0)
                return RedirectToAction(nameof(productlist));

            var product = db.products.FirstOrDefault(x => x.Id == productid);
            if (product == null)
                return RedirectToAction(nameof(productlist));

            //2.normal price,discount stock cannot less than 0
            if (updateproduct.stock < 0 || updateproduct.discount < 0 || updateproduct.normalprice < 0)
            {
                ViewBag.ErrorMsg = "cannot less than zero";
                return View(product);
            }
            if (updateproduct.discount > updateproduct.normalprice)
            {
                ViewBag.ErrorMsg = "cannot more than normal price";
                return View(updateproduct);
            }
            else if (string.IsNullOrEmpty(updateproduct.productname) || string.IsNullOrEmpty(updateproduct.description))

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
                        updateproduct.image = filename;
                    }
                    catch
                    {
                        ViewBag.ErrorMsg = "Upload Image Failde";
                    }
                }
            }

            //cheacking
            product.productname = updateproduct.productname;
            product.image = updateproduct.image;
            product.description = updateproduct.description;
            product.normalprice = updateproduct.normalprice;
            product.discount = updateproduct.discount;
            product.afterdiscountprice = updateproduct.normalprice * (100 - updateproduct.discount) / 100;
            product.status = updateproduct.status;
            product.stock = updateproduct.stock;
            db.SaveChanges();


            return RedirectToAction(nameof(productlist));

        }
        public IActionResult productdetail(int? id)
        {
            var loginuser = loginseller().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));

            var product = db.products.FirstOrDefault(x => x.Id == id);
            if (product == null)
                return RedirectToAction(nameof(productlist));

            return View(product);
        }
        public IActionResult deleleproduct(int? id)
        {
            var loginuser = loginseller().Result;
            if (loginuser == null)
                return Json(new { success = false, message = "Not logged in" });


            var product = db.products.FirstOrDefault(x => x.Id == id && x.sellersID == loginuser.ID);
            if (product != null)
            {
                product.status = productstatus.SellerDelete;
                db.SaveChanges();
                return Json(new { success = true, message = "Delete done" });
            }
            return Json(new { success = false, message = "Product not found" });

        }
        public IActionResult sellproductchat()
        {

            var loginUser = loginseller().Result;
            if (loginUser == null)
                return Json(new { success = false, message = "User not logged in" });

            try
            {
                var salesData = db.detail
                                  .Include(d => d.products)
                                  .Where(d => d.products.sellersID == loginUser.ID)
                                  .GroupBy(d => d.productsId)
                                  .Select(g => new SalesDetailViewModel
                                  {
                                      ProductId = g.Key,
                                      ProductName = g.First().products.productname,
                                      TotalQuantitySold = g.Sum(d => d.QTY)
                                  })
                                  .ToList();

                return View(salesData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }


        }
        public IActionResult payment()
        {
            var loginuser = loginseller().Result;
            if (loginuser == null)
                return RedirectToAction(nameof(login));

            var details = db.detail
        .Include(d => d.products)
        .Where(d => d.products.sellersID == loginuser.ID)
        .ToList();

            // Fetch transaction histories for the details
            var transHistoryIds = details.Select(d => d.tanshhistoryid).Distinct().ToList();
            var transHistory = db.tanshhistory
                .Where(t => transHistoryIds.Contains(t.id))
                .ToList();

            return View(transHistory);
        }
        public IActionResult invview(int? id)
        {

            var loginUser = loginseller().Result;
            if (loginUser == null)
                return Json(new { success = false, message = "User not logged in" });


            var tanshHistory = db.tanshhistory.Include(x => x.customers)
                                               .ThenInclude(c => c.profile)
                                               .FirstOrDefault(x => x.id == id);

            if (tanshHistory == null)
                return Json(new { success = false, message = "Transaction history not found" });

            var detail = db.detail.Include(d => d.products)
                                  .Where(d => d.tanshhistoryid == id).ToList();

            var viewModel = new inv
            {
                customers = tanshHistory.customers,
                profile = tanshHistory.customers?.profile,
                detail = detail,
                tanshhistory = tanshHistory
            };

            return View(viewModel);

        }

        public IActionResult frogetpassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult frogetpassword(string username)
        {

            var seller = db.sellers.FirstOrDefault(c => c.username == username);

            if (seller == null)
            {
                ViewBag.Error = "User not found";
                return View();
            }

            // Store the username in the session or send it to the next view/page
            HttpContext.Session.SetString("Username", username);

            return RedirectToAction("SendOtp");

        }

        public string GenerateOTP(int length)
        {
            Random random = new Random();
            char[] word = new char[length];
            for (int i = 0; i < word.Length; i++)
            {
                word[i] = (char)random.Next(48, 58);
            }
            return new string(word);

        }

        public async Task<IActionResult> SendOtp()
        {
            var options = new RestClientOptions("https://6gv4kd.api.infobip.com")
            {
                //MaxTimeout = -1,
            };
            string otp = GenerateOTP(6);
            HttpContext.Session.SetString("OTP", otp);
            var client = new RestClient(options);
            var request = new RestRequest("/sms/2/text/advanced", RestSharp.Method.Post);
            request.AddHeader("Authorization", "App 4be49be495348412131593888e4b6273-31c17e1c-470a-465a-9965-a13e98b76916");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            var body = $@"{{""messages"":[{{""destinations"":[{{""to"":""60178503091""}}],""from"":""ServiceSMS"",""text"":""OTP = {otp}""}}]}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);


            if (response.IsSuccessful)
            {
                return RedirectToAction("ValidateOtp");
            }
            else
            {
                ViewBag.Error = "Failed to send OTP.";
                return View("frogetpassword");
            }


        }
        public IActionResult ValidateOtp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ValidateOtp(string otp)
        {
            var storedOtp = HttpContext.Session.GetString("OTP");

            if (storedOtp == otp)
            {
                return RedirectToAction("ChangePassword1");
            }
            else
            {
                ViewBag.Error = "Invalid OTP. Please try again.";
                return View();
            }
        }

        public IActionResult ChangePassword1()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword1(string password, string confirmPassword)
        {
            var username = HttpContext.Session.GetString("Username");
            var seller = db.sellers.FirstOrDefault(c => c.username == username);

            if (password.Length < 6 || password.Length < 6)
            {
                ViewBag.Success = "No less than 6  !";
            }
            if (password != confirmPassword)
            {
                ViewBag.Success = "No Same Password  !";

            }
            if (seller != null)
            {
                string hashpassword = encoder.Encode(password);
                seller.password = password; // Ideally, you should hash the password before storing it
                db.SaveChanges();
                ViewBag.Success = "Password changed successfully!";
                return View(nameof(login));
            }
              
                return View();
            
        }
        public async Task<sellers?> loginseller()
        {
            var sellerid = HttpContext.Session.GetString("sellerid");
            if (sellerid == null)
                return null;
            var result = await db.sellers.FirstOrDefaultAsync(x => x.ID.ToString() == sellerid);
            return result;

        }
    }
}
