﻿using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CustomerWeb.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Book> books = _unitOfWork.BookRepository.GetAll(includeProperties: "Category,Cover");
            return View(books);
        }
        public IActionResult Details(int bookId)
        {
            ShoppingCart cart = new()
            {
                Book = _unitOfWork.BookRepository.GetFirstOrDefault(i => i.Id == bookId, includeProperties: "Category,Cover"),
                BookId = bookId,
                Count = 1
            };
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.AppUserId = claim.Value;

            ShoppingCart cart = _unitOfWork.ShoppingCartRepository.GetFirstOrDefault(
                x => x.AppUserId == claim.Value && x.BookId == shoppingCart.BookId);

            if (cart == null)
            {
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                    _unitOfWork.ShoppingCartRepository.GetAll(x => x.AppUserId == claim.Value).ToList().Count);
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.IncreaseBookCount(cart, shoppingCart.Count);
                _unitOfWork.Save();
            }

            
            return RedirectToAction(nameof(Index));
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