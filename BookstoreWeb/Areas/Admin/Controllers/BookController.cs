using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models.ViewModel;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookstoreWeb.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {

            return View();
        }


        public IActionResult Upsert(int? id)
        {
            BookVM bookVM = new()
            {
                Book = new(),
                CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverList = _unitOfWork.CoverRepository.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),


            };
            if (id == null || id == 0)
            {
                return View(bookVM);
            }
            else
            {
                bookVM.Book = _unitOfWork.BookRepository.GetFirstOrDefault(i => i.Id == id);
                return View(bookVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(BookVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootpath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootpath, @"images\bookCovers");
                    var extension = Path.GetExtension(file.FileName);

                    if (obj.Book.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootpath, obj.Book.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Book.ImageUrl = @"\images\bookCovers\" + fileName + extension;

                }
                if (obj.Book.Id == 0)
                {
                    _unitOfWork.BookRepository.Add(obj.Book);
                }
                else
                {
                    _unitOfWork.BookRepository.Update(obj.Book);
                }
                _unitOfWork.Save();
                TempData["success"] = "Book Created Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var bookList = _unitOfWork.BookRepository.GetAll(includeProperties: "Category,Cover");
            return Json(new { data = bookList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.BookRepository.GetFirstOrDefault(i => i.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.BookRepository.Remove(obj);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Book is Deleted" });
        }
        #endregion
    }

}
