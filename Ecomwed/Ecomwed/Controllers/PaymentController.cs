using Ecomwed.Models;
using Ecomwed.Models.custermor;
using Ecomwed.Models.product;
using Ecomwed.viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MimeKit;
using Stripe.Checkout;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Text;

namespace Ecomwed.Controllers
{
    public class PaymentController : Controller
    {
        private readonly Cappdbcontext db;

        public PaymentController(Cappdbcontext context)
        {
            db = context;
        }
        public IActionResult paymentcheckout()
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction("login", "custermor");

            var result = db.cart.Include(x => x.products).Where(x => x.quantity > x.products.stock && x.customersID == loginuser.ID).ToList();

            if (result.Any())
            {
                ViewBag.ErrorMsg = "Not enough stock available for some products.";
                return RedirectToAction("cartpage", "custermor");
            }

            string domain = "http://wolf97-001-site1.ktempurl.com/";  //WriteOnceBlock project domain
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>()
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + "payment/paymentsucersspage",
                CancelUrl = domain + "payment/paymenterrorpage"
            };
            var id = Convert.ToInt32(HttpContext.Session.GetString("customerid"));
            //var logiunser = LoginUser().Result;
            if (id == null)
            {
                return RedirectToAction("login", "custermor");

            }

            var cartlist = db.cart.Include(e => e.products)
                .Where(e => e.customersID == id);

            foreach (var cart in cartlist)
            {
                var sessionitem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cart.products.afterdiscountprice * 100),  //if rm10 =1000
                        Currency = "myr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.products.productname,
                        },
                    },
                    Quantity = cart.quantity
                };
                options.LineItems.Add(sessionitem);
            }

            var sessionitem1 = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(5 * 100),  //if rm10 =1000
                    Currency = "myr",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Shpping",
                    },
                },
                Quantity = 1
            };
            options.LineItems.Add(sessionitem1);
            var sst = db.cart
               .Include(x => x.products)
               .Where(x => x.customersID == loginuser.ID && x.products.status == productstatus.Active)
               .Sum(x => x.quantity * x.products.afterdiscountprice);

            var sessionitem2 = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {

                    UnitAmount = (long)(sst * 0.08m *100),  //if rm10 =1000
                    Currency = "myr",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "SST",
                    },
                },
                Quantity = 1
            };
            options.LineItems.Add(sessionitem2);

            var service = new SessionService();
            Session session = service.Create(options);
            Response.Headers.Add("location", session.Url);
            return new StatusCodeResult(303);

        }
        public IActionResult paymentsucersspage()
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction("login", "custermor");
            var cart = db.cart.Include(x => x.products)
              .Where(x => x.customersID == loginuser.ID)
              .Select(x => new cartproduct
              {
                  cart = x,
                  products = x.products
              });
            var subttottal = (cart.Where(x => x.products.status == productstatus.Active).Sum(x => x.cart.quantity * x.products.afterdiscountprice)+5);

            tanshhistory transhihisoty = new tanshhistory()
            {
                subtotal = subttottal.ToString("F2"),
                sst = (subttottal * 0.08m).ToString("F2"),
                grandtotal = (subttottal * 1.08m).ToString("F2"),
                time = DateTime.Now,
                customersID = loginuser.ID,
                status = historystatus.Paid
            };
            db.tanshhistory.Add(transhihisoty);
            db.SaveChanges();
            var details = new List<detail>();
            foreach (var item in cart)
            {
                var productId = item.products.Id;
                var detail = new detail
                {
                    tanshhistoryid = transhihisoty.id,
                    productsId = productId,
                    QTY = item.cart.quantity,
                    price = item.products.afterdiscountprice
                };
                db.detail.Add(detail);
                db.SaveChanges();
                details.Add(detail);
                item.products.stock -= item.cart.quantity;
                //db.products.Update(item.products);
                db.SaveChanges();

            }
            var cartItemsToRemove = db.cart.Where(x => x.customersID == loginuser.ID);
            db.cart.RemoveRange(cartItemsToRemove);
            db.SaveChanges();

            var emailBody = new StringBuilder();
            emailBody.AppendLine("<style>");
            emailBody.AppendLine("table { width: 100%; border-collapse: collapse; }");
            emailBody.AppendLine("th, td { border: 1px solid #dddddd; text-align: left; padding: 8px; }");
            emailBody.AppendLine("th { background-color: #f2f2f2; }");
            emailBody.AppendLine("img { width: 100px; height: 100px; }");
            emailBody.AppendLine("</style>");

            emailBody.AppendLine("<h1>Transaction Details:</h1>");
            emailBody.AppendLine("<table>");
            emailBody.AppendLine("<tr><th>Subtotal</th><td>RM" + transhihisoty.subtotal + "</td></tr>");
            emailBody.AppendLine("<tr><th>SST</th><td>RM" + transhihisoty.sst + "</td></tr>");
            emailBody.AppendLine("<tr><th>Grand Total</th><td>RM" + transhihisoty.grandtotal + "</td></tr>");
            emailBody.AppendLine("<tr><th>Time</th><td>" + transhihisoty.time + "</td></tr>");
            emailBody.AppendLine("</table>");

            emailBody.AppendLine("<h2>Details:</h2>");
            emailBody.AppendLine("<table>");
            emailBody.AppendLine("<tr>");
            emailBody.AppendLine("<th>Product ID</th>");
            emailBody.AppendLine("<th>Quantity</th>");
            emailBody.AppendLine("<th>Price</th>");
            emailBody.AppendLine("<th>Total Price</th>");
            emailBody.AppendLine("<th>Image</th>");
            emailBody.AppendLine("</tr>");


            foreach (var detail in details)
            {
                var product = detail.products;
                if (product != null)
                {
                    decimal totalPrice = detail.price * detail.QTY;
                    emailBody.AppendLine("<tr>");
                    emailBody.AppendLine("<td>" + detail.productsId + "</td>");
                    emailBody.AppendLine("<td>" + detail.QTY + "</td>");
                    emailBody.AppendLine("<td>RM" + detail.price.ToString("F2") + "</td>");
                    emailBody.AppendLine("<td>RM" + totalPrice.ToString("F2") + "</td>");
                    emailBody.AppendLine("<td><img src='" + product.image + "' alt='Product Image'/></td>");
                    emailBody.AppendLine("</tr>");
                }
            }
            emailBody.AppendLine("</ul>");

            string email = "cristalvip368@gmail.com";
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("candychia0907@gmail.com"));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Ivoice";
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailBody.ToString()
            };
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587 , SecureSocketOptions.StartTls);
            smtp.Authenticate("candychia0907@gmail.com", "hlpenmadoteezyco");
            smtp.Send(message);
            smtp.Disconnect(true);
            
            return View(transhihisoty);
        }
        public IActionResult paymenterrorpage()
        {
            return View();
        }
        public IActionResult trasnstionhistory()
        {
            var loginuser = LoginUser().Result;
            if (loginuser == null)
                return RedirectToAction("login", "custermor");

            //var tanshHistories = db.tanshhistory.ToList();
            var tanshHistories = db.tanshhistory.Where(x => x.customersID == loginuser.ID);
           
            var singleTanshHistory = tanshHistories; // or any logic to select one item
            return View(singleTanshHistory);

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
