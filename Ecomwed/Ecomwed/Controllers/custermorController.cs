using Ecomwed.Models;
using Ecomwed.Models.custermor;
using Ecomwed.Models.product;
using Ecomwed.Models.seller;
using Ecomwed.viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Stripe;
using Stripe.Climate;
using System.Text.RegularExpressions;
using Scrypt;
using NuGet.Protocol.Plugins;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using RestSharp;
using Polly;
using Microsoft.AspNetCore.Identity;
using System.Threading.Channels;

namespace Ecomwed.Controllers
{
    public class custermorController : Controller
    {
        ScryptEncoder encoder = new ScryptEncoder();
        private static string generatedOtp;
        private readonly Cappdbcontext db;
        public static string ErrorMsg { get; set; } = "";
        public static string profilecheack { get; set; } = "cheackd";
        public static string accountcheack { get; set; } = "";
        public static Regex cheackic { get; set; } = new Regex("\\d{6}\\-\\d{2}\\-\\d{4}");
        public static Regex cheakckpoit { get; set; } = new Regex(@"^(01)[0-46-9]*[0-9]{7,8}$");
        public static Regex gmail1 { get; set; } = new Regex(@"^[a-z0-9](\.?[a-z0-9]){5,}@g(oogle)?mail\.com$");
        public static Regex poscode { get; set; } = new Regex("\\d{5}");
        public static Regex Address { get; set; } = new Regex(@"^\d{1,5}\s[\w\s]{1,50},\s[\w\s]{1,50},\s\d{5},\s[\w\s]{1,50}$");
        public custermorController(Cappdbcontext context)
        {
            db = context;
        }
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult login(string? username, string? password)
        {

            bool check = true;
            string msg = "";

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                msg = "Cannot be empty";
                check = false;
                return Json(new { msg, check });
            }

            var result = db.customers.FirstOrDefault(x => x.username == username && x.status == customerstatus.Active);

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
            db.customers.Update(result);
            db.SaveChanges();

            HttpContext.Session.SetString("customerid", result.ID.ToString());
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
            var cheackusername = db.customers.FirstOrDefault(x => x.username == username);
            var cheackusername1 = db.customers.FirstOrDefault(x => x.email == email);
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

            customerprofile newprofile = new customerprofile()
            {
                fullname = username,
                NRIC = nric,
                phonenumber = phonenumber,
                birthday = birthday,
                age = age,
                gender = gender,
                address = "",
                city = "",
                state = "",
                zipcode = ""

            };
            db.customerprofile.Add(newprofile);
            db.SaveChanges();
            string hashpassword = encoder.Encode(password);
            var profiledid = db.customerprofile.FirstOrDefault(x => x.fullname == username);
            customers newuser = new customers
            {
                username = username,
                password = hashpassword,
                email = email,
                customerprofileID = profiledid.ID
            };
            db.customers.Add(newuser);
            db.SaveChanges();
            TempData["Success"] = "Register Successful";
            check = true;
            return Json(new { msg, check });

        }
        public IActionResult LogOut()
        {
            var logiunser = LoginUser().Result;
            if (logiunser == null)
            {
                return RedirectToAction(nameof(login));
            }
            logiunser.laslogintime = DateTime.Now;
            db.customers.Update(logiunser);
            db.SaveChanges();

            HttpContext.Session.Clear();
            return RedirectToAction(nameof(login));
        }
        public IActionResult homepage()
        {
            var topProducts = db.products.Where(x => x.status == productstatus.Active).Take(8).ToList();
            categoriesview categoriesview = new categoriesview();

           
                categoriesview = new categoriesview()
                {
                    categories = db.categories.ToList(),
                    products = topProducts
                };
           
            return View(categoriesview);

        }
        public IActionResult cartpage()
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var cart = db.cart.Include(x => x.products)
                .Where(x => x.customersID == loginuser.ID)
                .Select(x => new cartproduct
                {
                    cart = x,
                    products = x.products
                }).ToList();

            var subtotal = cart.Where(x => x.products.status == productstatus.Active)
                               .Sum(x => x.cart.quantity * x.products.afterdiscountprice);

            ViewBag.Subtotal = subtotal;
            ViewBag.Tax = subtotal * 0.08m;
            ViewBag.GrandTotal = subtotal * 1.08m + 5;

            return View(cart);
        }

        [HttpPost]
        public IActionResult updatecartpage([FromBody] CartUpdateModel model)
        {
            bool check = true;
            string msg = "";

            if (model == null || !ModelState.IsValid)
            {
                msg = "Invalid request.";
                check = false;
                return Json(new { msg, check });
            }

            var loginUser = LoginUser().Result;

            var cart = db.cart.Include(x => x.products)
                .FirstOrDefault(x => x.productsId == model.ProductId && x.customersID == loginUser.ID);

            if (cart == null)
            {
                msg = "Product not found in cart.";
                check = false;
                return Json(new { msg, check });
            }

            if (model.Quantity <= 0)
            {
                msg = "Quantity must be greater than zero.";
                check = false;
                return Json(new { msg, check });
            }

            if (model.Quantity > cart.products.stock)
            {

                msg = "Insufficient stock available.";
                check = false;
                return Json(new { msg, check });
            }
            cart.quantity = model.Quantity;
            db.SaveChanges();

            var total = db.cart.Include(X => X.products).Where(x => x.customersID == loginUser.ID && x.products.status == productstatus.Active && x.productsId == model.ProductId).Sum(x => x.quantity * x.products.afterdiscountprice);

            msg = "Cart updated successfully.";
            check = true;
            return Json(new { msg, check, total });
        }

        public IActionResult calcart()
        {
            var loginUser = LoginUser().Result;

            var subtotal = db.cart
                .Include(x => x.products)
                .Where(x => x.customersID == loginUser.ID && x.products.status == productstatus.Active)
                .Sum(x => x.quantity * x.products.afterdiscountprice);

            var tax = subtotal * 0.08m;
            var grandtotal = subtotal * 1.08m + 5;
            return Json(new { subtotal, tax, grandtotal });
        }
        public IActionResult chekouclear()
        {
            var loginUser = LoginUser().Result;
            var cartcount = db.cart.Include(x => x.products).Where(x => x.customersID == loginUser.ID && x.products.status == productstatus.Active).Count();
            return Json(cartcount);
        }

        [HttpPost]
        public JsonResult AddToCart(int? productsId)
        {
            bool check = true;
            string msg = "";
            var logiunser = LoginUser().Result;
            if (logiunser == null)
            {
                check = false;
                msg = "User not logged in";
                return Json(new { check, msg });
            }
            if (productsId == null || productsId == 0)
            {
                check = false;
                msg = "Invalid product ID";
                return Json(new { check, msg });
            }

            var product = db.products.FirstOrDefault(x => x.Id == productsId);
            if (product != null)
            {
                if (product.stock <= 0)
                {
                    check = false;
                    msg = "Out of stock";
                    return Json(new { check, msg });
                }

                var existingCartItem = db.cart.FirstOrDefault(x => x.productsId == productsId && x.customersID == logiunser.ID);
                if (existingCartItem != null)
                {
                    if (existingCartItem.quantity >= product.stock)
                    {
                        check = false;
                        msg = "Out of stock";
                        return Json(new { check, msg });
                    }
                    existingCartItem.quantity++;
                    db.SaveChanges();
                    msg = "Quantity updated in cart";
                    return Json(new { check, msg });
                }
                else
                {
                    cart newCart = new cart()
                    {
                        productsId = productsId,
                        customersID = logiunser.ID,
                        quantity = 1
                    };
                    db.cart.Add(newCart);
                    db.SaveChanges();
                    msg = "Added to cart";
                    return Json(new { check, msg });
                }
            }
            return Json(new { check = false, msg = "Product not found" });
        }

        [HttpPost]
        public IActionResult deletetonecart(int? id)
        {
            bool check = true;
            string msg = "";
            var logiunser = LoginUser().Result;
            if (logiunser == null)
            {
                check = false;
                msg = "User not logged in";
                return Json(new { check, msg });
            }
            if (id == null || id == 0)
            {
                check = false;
                msg = "Invalid cart ID";
                return Json(new { check, msg });
            }
            var cart = db.cart.FirstOrDefault(x => x.Id == id && x.customersID == logiunser.ID);
            if (cart == null)
            {
                check = false;
                msg = "This product not found";
                return Json(new { check, msg });
            }
            db.cart.Remove(cart);
            db.SaveChanges();
            check = true;
            msg = "This product has been deleted";
            return Json(new { check, msg });
        }
        public IActionResult deleteallinvactive()
        {
            var logiunser = LoginUser().Result;
            if (logiunser == null)
            {
                return RedirectToAction(nameof(login));
            }
            var inactivecart = db.cart.Include(x => x.products).Where(x => x.products.status != productstatus.Active && x.customersID == logiunser.ID);
            if (inactivecart.Count() > 0)
            {
                db.cart.RemoveRange(inactivecart);
                db.SaveChanges();
            }

            return RedirectToAction(nameof(cartpage));
        }
        [HttpPost]
        public IActionResult deleteactive()
        {
            bool check = true;
            string msg = "";
            var logiunser = LoginUser().Result;
            if (logiunser == null)
            {
                return RedirectToAction(nameof(login));
            }
            var inactivecart = db.cart.Include(x => x.products).Where(x => x.products.status == productstatus.Active && x.customersID == logiunser.ID);
            if (inactivecart.Count() > 0)
            {
                db.cart.RemoveRange(inactivecart);
                db.SaveChanges();
                check = true;
                msg = "This product has been deleted";
                return Json(new { check, msg });
            }
            check = false;
            msg = "No active products found in the cart";
            return Json(new { check, msg });
        }
        public IActionResult profile(IFormFile? file)
        {
            var logiunser = LoginUser().Result;
            if (logiunser == null)
            {
                return RedirectToAction(nameof(login));
            }

            var profile = db.customers.Include(x => x.profile).Select(x => new customer_profile
            {
                customers = x,
                profile = x.profile
            })
                .FirstOrDefault(x => x.customers.ID == logiunser.ID);
            if (profile == null)
                return RedirectToAction(nameof(login));

            ViewBag.accountcheack = accountcheack;
            ViewBag.profilecheack = profilecheack;

            if (accountcheack == "checked")
            {
                ViewBag.ErrorMsg = ErrorMsg;
                ViewBag.profileErrorMsg = "";
            }
            else
            {
                ViewBag.ErrorMsg = "";
                ViewBag.profileErrorMsg = ErrorMsg;
            }
            return View(profile);
        }

       

        [HttpPost]
        public IActionResult Personal(IFormFile? file, string phonenumber)
        {
            bool check = true;
            string msg = "";

            var loginUser = LoginUser().Result;
            if (loginUser == null)
            {
                return RedirectToAction(nameof(Login));
            }
            if (string.IsNullOrWhiteSpace(phonenumber))
            {
                check = false;
                msg = "Fields cannot be empty";
                return Json(new { msg, check });
            }
            if (!cheakckpoit.IsMatch(phonenumber))
            {
                check = false;
                msg = "Error format phone number";
                return Json(new { msg, check });
            }
            var existingPhoneNumber = db.customerprofile.FirstOrDefault(x => x.ID == loginUser.ID && x.phonenumber == phonenumber);
            if (existingPhoneNumber != null)
            {
                check = false;
                msg = "Cannot be same phone number";
                return Json(new { msg, check });
            }

            var userProfile = db.customerprofile.FirstOrDefault(x => x.ID == loginUser.customerprofileID);
            if (userProfile != null)
            {
                userProfile.phonenumber = phonenumber;

                if (file != null && file.Length > 0)
                {
                    try
                    {
                        // Save image to local project
                        // Determine the path to save the uploaded file within the project
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image");
                        // Check if the directory exists, if not, create it
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filepath = Path.Combine(uploadsFolder, filename);
                        // Save the file to the server
                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        // Save image to user profile
                        userProfile.image = filename;
                    }
                    catch
                    {
                        check = false;
                        msg = "Upload Image Failed";
                        return Json(new { msg, check });
                    }
                }

                db.SaveChanges();
                check = true;
                msg = "Profile updated successfully";
                return Json(new { msg, check });
            }
            else
            {
                check = false;
                msg = "User profile not found";
                return Json(new { msg, check });
            }
        }
        [HttpPost]
        public IActionResult updateprofile(IFormFile? file, string? fullname, string? NRIC, string? phonenumber)
        {
            var logiunser = LoginUser().Result;
            if (logiunser == null)
            {
                return RedirectToAction(nameof(login));
            }
            var findprofile = db.customerprofile.FirstOrDefault(x => x.ID == logiunser.customerprofileID);
            if (findprofile == null)
                return RedirectToAction(nameof(login));

            if (string.IsNullOrWhiteSpace(fullname) || string.IsNullOrWhiteSpace(NRIC) || string.IsNullOrWhiteSpace(phonenumber))
            {
                ErrorMsg = "Please DO NOT Empty";
            }

            else if (!cheackic.IsMatch(NRIC))
            {
                ErrorMsg = "Error Format 000000-00-0000";
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
                        findprofile.image = filename;
                    }
                    catch
                    {
                        ErrorMsg = "Upload Image Failde";
                    }
                }
                findprofile.fullname = fullname;

                findprofile.NRIC = NRIC;
                db.SaveChanges();
                ErrorMsg = "";
            }
            accountcheack = "";
            profilecheack = "checked";
            return RedirectToAction(nameof(profile));
        }
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            bool check = true;
            string msg = "";
            var logiunser = LoginUser().Result;
            if (logiunser == null)
            {
                return RedirectToAction(nameof(login));
            }


            var result = db.customers.FirstOrDefault(x => x.ID == logiunser.ID);


            if (result ==null || !encoder.Compare(oldPassword, result.password))
            {
                check = false;
                msg = "Error: Old password is incorrect";
                return Json(new { msg, check });
            }

           
            if (newPassword.Length < 6 || confirmPassword.Length < 6)
            {
                check = false;
                msg = "Please enter more than 6 characters";
                return Json(new { msg, check });

            }

            if (newPassword != confirmPassword)
            {
                check = false;
                msg = "Passwords do not match";
                return Json(new { msg, check });

            }

            string hashedPassword = encoder.Encode(newPassword);

            result.password = hashedPassword;
            db.SaveChanges();
            check = true;
            msg = "Password changed successfully";
            return Json(new { msg, check });


        }
        public IActionResult updateadress(string address ,string city, string state, string zipcode ,string fullname)
        {
            bool check = true;
            string msg = "";

            var logiunser = LoginUser().Result;
            if (logiunser == null)
            {
                return RedirectToAction(nameof(login));
            }

            var findprofile = db.customerprofile.FirstOrDefault(x => x.ID == logiunser.customerprofileID);
            if (findprofile == null)
            {
                return RedirectToAction(nameof(login));
            }

            if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(fullname))
            {
                check = false;
                msg = "Fields cannot be empty";
                return Json(new { msg, check });
            }

            // Assume Address.IsMatch() is a method that validates the address format
            if (!Address.IsMatch(address))
            {
                check = false;
                msg = "Invalid address format";
                return Json(new { msg, check });
            }

            try
            {
                findprofile.fullname = fullname;
                findprofile.address = address;
                findprofile.city = city;
                findprofile.state = state;
                findprofile.zipcode = zipcode;

                db.Update(findprofile);
                db.SaveChanges();

                check = true;
                msg = "Update successful";
            }
            catch (Exception ex)
            {
                check = false;
                msg = "Error updating address: " + ex.Message;
            }

            return Json(new { msg, check });

        }

        public IActionResult transhtiondetail(int? id)
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return Json(new { success = false, message = "User not logged in" });


            var product = db.detail.Include(x => x.products).Include(x => x.tanshhistory)
                                     .Where(x => x.tanshhistory.id == id)
                                     .Select(x => new DETAIL { detail = x, products = x.products });


            if (product == null)
                return Json(new { success = false, message = "Product not found" });

            return Json(new { success = true, data = product });
        }

        //public IActionResult CATEGORIES()
        //{
        //    var itemp = db.products.Where(x=>x.status == productstatus.Active).ToList();
            
        //    var categoriesView = new categoriesview()
        //    {
        //        categories = db.categories.ToList(),
        //        products = itemp
        //    };
        //    return View(categoriesView);
        //}
        public IActionResult CATEGORIES(int page = 1, int pageSize = 12)
        {
            var activeProducts = db.products.Where(x => x.status == productstatus.Active);
            var totalProducts = activeProducts.Count();

            var products = activeProducts
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            var categoriesView = new categoriesview()
            {
                categories = db.categories.ToList(),
                products = products,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize)
            };

            return View(categoriesView);
        }

        [HttpGet]
        public IActionResult SortProducts(string sortOption)
        {
            var itemp = db.products.Where(x => x.status == productstatus.Active);

            switch (sortOption)
            {
                case "name_asc":
                    itemp = itemp.OrderBy(p => p.productname);
                    break;
                case "price_low":
                    itemp = itemp.OrderBy(p => p.afterdiscountprice);
                    break;
                case "price_high":
                    itemp = itemp.OrderByDescending(p => p.afterdiscountprice);
                    break;
                default:
                    itemp = itemp.OrderBy(p => p.productname); // Default sort by name
                    break;
            }

            var sortedProducts = itemp.Select(p => new
            {
                p.Id,
                p.productname,
                p.image,
                p.normalprice,
                p.afterdiscountprice,
                p.stock,
                p.categoriesId
            }).ToList();

            return Json(sortedProducts);
        }

        public IActionResult Details(int id)
        {
            var productDetail = db.products
                .Include(p => p.categories)
                .FirstOrDefault(p => p.Id == id && p.categoriesId == p.categories.Id);

            if (productDetail == null)
            {
                return NotFound();
            }

            var reviews = db.review.Where(r => r.productsId == id).ToList();
            float? averageRating = reviews.Any() ? reviews.Average(r => (float?)r.StarRating) : null;

            var productDetailViewModel = new productdetailcs()
            {
                categories = productDetail.categories,
                products = productDetail,
                customers = db.customers.ToList(),
                review = reviews,
                profile = db.customerprofile.ToList(),
                AverageRating = averageRating ?? 0 // Default to 0 if averageRating is null
            };

            return View(productDetailViewModel);
        }

       
        //public IActionResult frogetpassword()
        //{
        //    return View();
        //}

        //public string GenerateOTP(int length)
        //{
        //    Random random = new Random();
        //    char[] word = new char[length];
        //    for (int i = 0; i < word.Length; i++)
        //    {
        //        word[i] = (char)random.Next(48, 58);
        //    }
        //    generatedOtp = new string(word);
        //    return generatedOtp;
        //}

        //[HttpPost]
        //public async Task<IActionResult> SendOtp()
        //{
         
        //    string otp = GenerateOTP(6);
        //    var client = new RestClient(new RestClientOptions("https://6gv4kd.api.infobip.com"));
        //    var request = new RestRequest("/sms/2/text/advanced", Method.Post);
        //    request.AddHeader("Authorization", "App 4be49be495348412131593888e4b6273-31c17e1c-470a-465a-9965-a13e98b76916");
        //    request.AddHeader("Content-Type", "application/json");
        //    request.AddHeader("Accept", "application/json");
        //    var body = $@"{{""messages"":[{{""destinations"":[{{""to"":""60178503091""}}],""from"":""ServiceSMS"",""text"":""OTP = {otp}""}}]}}";
        //    request.AddStringBody(body, DataFormat.Json);
        //    RestResponse response = await client.ExecuteAsync(request);

        //    return Json(new { Success = response.IsSuccessful, Message = response.IsSuccessful ? "OTP sent successfully!" : "Failed to send OTP." });
        //}

        //[HttpPost]
        //public IActionResult ValidateOtp(string otp)
        //{
        //    var response = new
        //    {
        //        Success = otp == generatedOtp,
        //        Message = otp == generatedOtp ? "OTP validated successfully!" : "Invalid OTP!"
        //    };

        //    return Json(response);
        //}

        //[HttpPost]
        //public IActionResult ChangePassword1(string username, string password, string newpassword)
        //{
        //    bool check = true;
        //    string msg = "";

        //    if (password.Length < 6 || newpassword.Length < 6)
        //    {
        //        check = false;
        //        msg = "Please enter more than 6 characters";
        //        return Json(new { msg, check });
              
        //    }

        //    if (password != newpassword)
        //    {
        //        check = false;
        //        msg = "Passwords do not match";
        //        return Json(new { msg, check });
               
        //    }

        //    var customer = db.customers.FirstOrDefault(c => c.username == username);
        //    if (customer != null)
        //    {
        //        string hashedPassword = encoder.Encode(password);
              
        //        customer.password = hashedPassword;
        //        db.SaveChanges();
        //        check = true;
        //        msg = "Password changed successfully";
        //        return Json(new { msg, check });

        //    }
        //    else
        //    {
        //        check = false;
        //        msg = "User not found";
        //        return Json(new { msg, check });
        //    }
        //}
        public IActionResult frogetpassword()
        {
            return View();
        }

        public string GenerateOTP(int length)
        {
            Random random = new Random();
            char[] word = new char[length];
            for (int i = 0; i < word.Length; i++)
            {
                word[i] = (char)random.Next(48, 58);
            }
            generatedOtp = new string(word);
            return generatedOtp;
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp(string username)
        {
            var customer = db.customers.FirstOrDefault(c => c.username == username);
            if (customer == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            string otp = GenerateOTP(6);
            var client = new RestClient(new RestClientOptions("https://6gv4kd.api.infobip.com"));
            var request = new RestRequest("/sms/2/text/advanced", Method.Post);
            request.AddHeader("Authorization", "App 4be49be495348412131593888e4b6273-31c17e1c-470a-465a-9965-a13e98b76916");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            var body = $@"{{""messages"":[{{""destinations"":[{{""to"":""60178503091""}}],""from"":""ServiceSMS"",""text"":""OTP = {otp}""}}]}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                // Store the OTP in the session or any temporary storage for validation
                HttpContext.Session.SetString("otp", otp);
            }

            return Json(new { success = response.IsSuccessful, message = response.IsSuccessful ? "OTP sent successfully!" : "Failed to send OTP." });
        }

        [HttpPost]
        public IActionResult ValidateOtp(string otp)
        {
            var storedOtp = HttpContext.Session.GetString("otp");
            var success = otp == storedOtp;
            var message = success ? "OTP validated successfully!" : "Invalid OTP!";
            return Json(new { success, message });
        }

        [HttpPost]
        public IActionResult ChangePassword1(string username, string newpassword, string confirmpassword)
        {
            bool check = true;
            string msg = "";

            if (newpassword.Length < 6 || confirmpassword.Length < 6)
            {
                check = false;
                msg = "Please enter more than 6 characters.";
                return Json(new { msg, check });
            }

            if (newpassword != confirmpassword)
            {
                check = false;
                msg = "Passwords do not match.";
                return Json(new { msg, check });
            }

            var customer = db.customers.FirstOrDefault(c => c.username == username);
            if (customer != null)
            {
                string hashedPassword = encoder.Encode(newpassword);
                customer.password = hashedPassword;
                db.SaveChanges();
                check = true;
                msg = "Password changed successfully.";
                return Json(new { msg, check });
            }
            else
            {
                check = false;
                msg = "User not found.";
                return Json(new { msg, check });
            }
        }

        [HttpPost]
        public JsonResult AddReview(string review, int starrating, int productId)
        {
            bool check = true;
            string msg = "";
            var logiunser = LoginUser().Result;
            if (logiunser == null)
            {
                check = false;
                msg = "User not logged in";
                return Json(new { check, msg });

            }

            if (string.IsNullOrEmpty(review) || starrating < 1 || starrating > 5)
            {

                check = false;
                msg = "Invalid review or star rating";
                return Json(new { check, msg });
            }

            var newReview = new review
            {
                productsId = productId,
                customersID = logiunser.ID,
                Review = review,
                StarRating = starrating,
                reviewdate = DateTime.Now
            };

            db.review.Add(newReview);
            db.SaveChanges();
            check = true;
            msg = "Review submitted successfully";
            return Json(new { check, msg });
        }



        public async Task<customers?> LoginUser()
        {
            var custermerid = HttpContext.Session.GetString("customerid");
            if (custermerid == null)
                return null;

            var cusotmer = await db.customers.FirstOrDefaultAsync(x => x.ID.ToString() == custermerid);

            return cusotmer;

        }
    }
}
