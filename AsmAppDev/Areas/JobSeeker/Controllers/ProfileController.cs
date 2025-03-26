using AsmAppDev.Models;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProfileController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var profile = await _userManager.GetUserAsync(User);
            return View(profile);
        }
        public IActionResult Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser? jobSeeker = _unitOfWork.AppUserRepository.Get(x => x.Id == id);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            return View(jobSeeker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser jobSeeker, IFormFile? avatarFile, IFormFile? cvFile)
        {
            if (id != jobSeeker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = (ApplicationUser)await _userManager.FindByIdAsync(id);
                    user.Name = jobSeeker.Name;
                    user.Address = jobSeeker.Address;
                    user.Introduction = jobSeeker.Introduction;

                    string wwwrootPath = _webHostEnvironment.WebRootPath;
                    if (avatarFile != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                        string avatarPath = Path.Combine(wwwrootPath, @"img\avatars");
                        // Lưu avatar mới nếu có
                        if (avatarFile != null)
                        {
                            // Delete Old Images
                            if (!string.IsNullOrEmpty(jobSeeker.Avatar))
                            {
                                var oldImagePath = Path.Combine(wwwrootPath, jobSeeker.Avatar.TrimStart('\\'));
                                if (System.IO.File.Exists(oldImagePath))
                                {
                                    System.IO.File.Delete(oldImagePath);
                                }
                            }
                            // Copy File to \img\avatars
                            using (var fileStream = new FileStream(Path.Combine(avatarPath, fileName), FileMode.Create))
                            {
                                avatarFile.CopyTo(fileStream);
                            }
                            // Update ImageUrl in DB
                            user.Avatar = @"\img\avatars\" + fileName;
                        }
                        else
                        {
                            // Copy File to \img\avatars
                            using (var fileStream = new FileStream(Path.Combine(avatarPath, fileName), FileMode.Create))
                            {
                                avatarFile.CopyTo(fileStream);
                            }
                            // Update ImageUrl in DB
                            user.Avatar = @"\img\avatars\" + fileName;
                        }
                    }
                    if (cvFile != null)
                    {
                        string cvFileName = Guid.NewGuid().ToString() + Path.GetExtension(cvFile.FileName);
                        string cvPath = Path.Combine(wwwrootPath, @"img\cv");
                        // Lưu CV mới nếu có
                        if (cvFile != null)
                        {
                            // Delete Old CV
                            if (!string.IsNullOrEmpty(jobSeeker.CV))
                            {
                                var oldCVPath = Path.Combine(wwwrootPath, jobSeeker.CV.TrimStart('\\'));
                                if (System.IO.File.Exists(oldCVPath))
                                {
                                    System.IO.File.Delete(oldCVPath);
                                }
                            }
                            // Copy File to \img\cv
                            using (var fileStream = new FileStream(Path.Combine(cvPath, cvFileName), FileMode.Create))
                            {
                                cvFile.CopyTo(fileStream);
                            }
                            // Update ImageUrl in DB
                            user.CV = @"\img\cv\" + cvFileName;
                        }
                        else
                        {
                            // Copy File to \img\cv
                            using (var fileStream = new FileStream(Path.Combine(cvPath, cvFileName), FileMode.Create))
                            {
                                cvFile.CopyTo(fileStream);
                            }
                            // Update ImageUrl in DB
                            user.CV = @"\img\cv\" + cvFileName;
                        }
                    }

                    // Cập nhật thông tin người dùng trong cơ sở dữ liệu
                    _unitOfWork.AppUserRepository.Update(user);
                    _unitOfWork.Save();
                    TempData["success"] = "Profile edited successfully";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    // Handle exception
                    throw;
                }
            }
            return View(jobSeeker);
        }
    }
}
