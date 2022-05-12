using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models;
using Bookstore.Models.ViewModel;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookstoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;

        }


        [Area("Customer")]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.AppUserId == claim.Value, includeProperties: "Book"),
                OrderHeader = new()
            };
            foreach (var cart in ShoppingCartVM.CartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Book.Price, cart.Book.Price100, cart.Book.Price200);
                ShoppingCartVM.OrderHeader.Total += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.AppUserId == claim.Value, includeProperties: "Book"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.AppUser = _unitOfWork.AppUserRepository.GetFirstOrDefault(x => x.Id == claim.Value);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.AppUser.Name;
            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.AppUser.Address;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.AppUser.City;
            ShoppingCartVM.OrderHeader.Province = ShoppingCartVM.OrderHeader.AppUser.Province;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.AppUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.AppUser.PhoneNumber;

            foreach (var cart in ShoppingCartVM.CartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Book.Price, cart.Book.Price100, cart.Book.Price200);
                ShoppingCartVM.OrderHeader.Total += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.CartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.AppUserId == claim.Value, includeProperties: "Book");


            ShoppingCartVM.OrderHeader.CreatedDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.AppUserId = claim.Value;

            foreach (var cart in ShoppingCartVM.CartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Book.Price, cart.Book.Price100, cart.Book.Price200);
                ShoppingCartVM.OrderHeader.Total += (cart.Price * cart.Count);
            }

            AppUser appUser = _unitOfWork.AppUserRepository.GetFirstOrDefault(x => x.Id == claim.Value);
            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
            }

            _unitOfWork.OrderHeaderRepository.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.CartList)
            {
                Order order = new()
                {
                    BookId = cart.BookId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderRepository.Add(order);
                _unitOfWork.Save();
            }

            //individual user
            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                //Stripe settings
                var domain = "https://localhost:44386/";
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>()
                    ,
                    Mode = "payment",
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/Index",
                };

                foreach (var item in ShoppingCartVM.CartList)
                {

                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "cad",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Book.Title,
                            },

                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeaderRepository.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
            }

        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(x => x.Id == id, includeProperties: "AppUser");
            if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            _emailSender.SendEmailAsync(orderHeader.AppUser.Email, "New Bookstore Order", "<p>New Order Created</p>");
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepository.GetAll(x => x.AppUserId == orderHeader.AppUserId).ToList();
            HttpContext.Session.Clear();
            _unitOfWork.ShoppingCartRepository.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);

        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCartRepository.IncreaseBookCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCartRepository.Remove(cart);
                var count = _unitOfWork.ShoppingCartRepository.GetAll(x => x.AppUserId == cart.AppUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, count);
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.DecreaseBookCount(cart, 1);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int cartId)
        {
            var cart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCartRepository.Remove(cart);
            _unitOfWork.Save();
            var count = _unitOfWork.ShoppingCartRepository.GetAll(x => x.AppUserId == cart.AppUserId).ToList().Count;
            HttpContext.Session.SetInt32(StaticDetails.SessionCart, count);
            return RedirectToAction(nameof(Index));
        }


        private decimal GetPriceBasedOnQuantity(double quantity, decimal price, decimal price100, decimal price200)
        {
            if (quantity <= 100)
            {
                return price;
            }
            else if (quantity <= 200)
            {
                return price100;
            }
            else
            {
                return price200;
            }
        }

    }
}
