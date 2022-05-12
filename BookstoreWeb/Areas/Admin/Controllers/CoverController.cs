using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CoverController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Cover> covers = _unitOfWork.CoverRepository.GetAll();
            return View(covers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cover obj)
        {
            //if(_unitOfWork.CoverRepository.)
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverRepository.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Cover Created Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var cover = _unitOfWork.CoverRepository.GetFirstOrDefault(i => i.Id == id);

            if (cover == null)
            {
                return NotFound();
            }
            return View(cover);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cover obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.CoverRepository.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Cover Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var cover = _unitOfWork.CoverRepository.GetFirstOrDefault(i => i.Id == id);

            if (cover == null)
            {
                return NotFound();
            }
            return View(cover);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOfWork.CoverRepository.GetFirstOrDefault(i => i.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.CoverRepository.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Cover Deleted Successfully";
            return RedirectToAction("Index");
        }

    }
}
