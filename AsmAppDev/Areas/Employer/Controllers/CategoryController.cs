using AsmAppDev.Models;
using AsmAppDev.Models.ViewModels;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace AsmAppDev.Areas.Users.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = "Employer")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var myList = _unitOfWork.CategoryRepository.GetAll()
                    .Where(c => c.UserId == userId)  // Chỉ lấy các category thuộc về user hiện tại
                    .ToList();
                return View(myList);
            }
            // Nếu không có userId, hoặc không tìm thấy categories, trả về View trống hoặc với danh sách rỗng
            return View(new List<Category>());
        }
        public async Task<IActionResult> ToggleNotification(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            category.NotificationStatus = !category.NotificationStatus;

            /*if (category.NotificationStatus)
            {
                category.NotificationStatus = true;
            }*/

            _unitOfWork.Save();

            TempData["Success"] = "Notification status delete successfully!";
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                if(userId != null)
                {
                    category.UserId = userId;
                    category.DateCreate = DateTime.Now;
                    _unitOfWork.CategoryRepository.Add(category);
                    _unitOfWork.CategoryRepository.Save();
                    TempData["success"] = "Category created successfully";
                }
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {

            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userId != null)
                {
                    
                    category.UserId = userId;
                    category.DateCreate = DateTime.Now;
                    _unitOfWork.CategoryRepository.Update(category);
                    _unitOfWork.CategoryRepository.Save();
                    TempData["success"] = "Category edited successfully";
                }
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Delete(Category category)
        {
            _unitOfWork.CategoryRepository.Delete(category);
            _unitOfWork.CategoryRepository.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
