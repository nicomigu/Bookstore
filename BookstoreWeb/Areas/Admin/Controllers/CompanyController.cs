using Bookstore.DataAccess.Repositories.Interfaces;
using Bookstore.Models;
using Bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreWeb.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {

            return View();
        }


        public IActionResult Upsert(int? id)
        {
            Company company = new();
            if (id == null || id == 0)
            {
                return View(company);
            }
            else
            {
                company = _unitOfWork.CompanyRepository.GetFirstOrDefault(i => i.Id == id);
                return View(company);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _unitOfWork.CompanyRepository.Add(obj);
                    TempData["success"] = "Company Created Successfully";
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(obj);
                    TempData["success"] = "Company Updated Successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(obj);
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.CompanyRepository.GetAll();
            return Json(new { data = companyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var company = _unitOfWork.CompanyRepository.GetFirstOrDefault(i => i.Id == id);
            if (company == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }


            _unitOfWork.CompanyRepository.Remove(company);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Book is Deleted" });
        }
        #endregion
    }

}
