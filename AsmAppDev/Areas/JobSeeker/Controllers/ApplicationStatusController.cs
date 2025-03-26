using AsmAppDev.Models;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
    public class ApplicationStatusController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        public ApplicationStatusController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            // Lấy thông tin người dùng hiện tại từ UserManager
            var currentUser = await _userManager.GetUserAsync(User);

            var userEmail = currentUser.Email;
            List<JobApplication> myList = _unitOfWork.JobApplicationRepository.GetAll("Job").Where(c => c.Email == userEmail).ToList();

            return View(myList);
        }
    }
}
