using AsmAppDev.Models.ViewModels;
using AsmAppDev.Models;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
    public class ViewJobController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public ViewJobController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            List<Job> myList = _unitOfWork.JobRepository.GetAll("Category").ToList();
            return View(myList);
        }
        public IActionResult Apply(int? Id)
        {

            if (Id == null)
            {
                return NotFound();
            }
            Job? job = _unitOfWork.JobRepository.Get(c => c.Id == Id);
            if (job == null)
            {
                return NotFound();
            }
            JobVM JobVm = new JobVM();
            JobVm.apply = new JobApplication();
            JobVm.apply.JobId = job.Id;
            JobVm.Job = job;

            return View(JobVm);
        }
        [HttpPost]
        public async Task<IActionResult> Apply(JobVM job)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin người dùng hiện tại từ UserManager
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    // Xử lý trường hợp người dùng không tồn tại
                    return RedirectToAction("Login", "Account");
                }

                // Gán email của người dùng hiện tại vào đối tượng JobApplication
                job.apply.Email = currentUser.Email;

                // Lấy ngày giờ hiện tại
                DateTime currentDate = DateTime.Now;

                // Gán ngày giờ hiện tại vào trường DayApply của đối tượng JobApplication
                job.apply.DayApply = currentDate;

                // Lưu đối tượng JobApplication vào cơ sở dữ liệu
                _unitOfWork.JobApplicationRepository.Add(job.apply);
                _unitOfWork.Save();

                TempData["success"] = "Job applied successfully";
                return RedirectToAction("Index");
            }

            // Nếu ModelState không hợp lệ, trả về lại view với dữ liệu và thông báo lỗi
            return View(job);
        }
    }
}
