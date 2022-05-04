using Microsoft.AspNetCore.Mvc;
using SlackOverload.Models;
using System.Diagnostics;
using SlackOverload.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace SlackOverload.Controllers {
    [Authorize]
    public class HomeController : Controller {
        private UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _db { get; set; }  
        private RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager) {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string orderby) {
            if(orderby == null) {
                orderby = "date";
            }

            var list = await GetQuestionList(1, orderby);

            

            if (orderby == "date") {
                ViewBag.Order = "mostanswered";
            } else if (orderby == "mostanswered") {
                ViewBag.Order = "date";
            }

            return View(list);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(string orderby, int currentPageIndex = 1) {
            var list = await GetQuestionList(currentPageIndex, orderby);

            if (orderby == "date") {
                ViewBag.Order = "mostanswered";
            } else if (orderby == "mostanswered") {
                ViewBag.Order = "date";
            }
            return View(list);
        }


        public IActionResult CreateRole() { 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string role) {
            if (role == null) {
                return NotFound();
            }
            if (!await _roleManager.RoleExistsAsync(role)) { 
                await _roleManager.CreateAsync(new IdentityRole(role));
            } else {
                TempData["Message"] = "Role already exist";
                return RedirectToAction("Error");
            }

            try {
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            return View();
        }

        [Authorize(Roles="admin")]
        public IActionResult MakeUserAdmin() {
            ViewBag.Users = new SelectList(_db.Users.ToList(), "Id", "UserName");
            ViewBag.Roles = new SelectList(_db.Roles.ToList(), "Id", "Name");
            return View();
        }


        [Authorize(Roles="admin")]
        [HttpPost]
        public async Task<IActionResult> MakeUserAdmin(string userId, string roleId) {
            if (userId == null || roleId == null) {
                return BadRequest();
            }

            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            var getUserTask = _userManager.FindByIdAsync(userId);
            ApplicationUser user = await getUserTask;

            if (user == null) {
                return NotFound();
            }

            if (_db.UserRoles.Select(u => u.RoleId).Contains(roleId) || _db.UserRoles.Select(u => u.UserId).Contains(userId)) {
                TempData["Message"] = "User has already been given that role.";
                return RedirectToAction("Error");
            }

            if(await _roleManager.RoleExistsAsync(role.Name)) {
                if(!await _userManager.IsInRoleAsync(user, role.Name)) {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
            }

            try {
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            return View();
        }

        [Authorize(Roles="admin")]
        public IActionResult AdminPanel() {
            return View(_db.Question.Include(q => q.User));
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<QuestionPaginationViewModel> GetQuestionList(int currentPage, string orderby) {
            int maxRowsPerPage = 10 ;
            QuestionPaginationViewModel model = new QuestionPaginationViewModel();



            List<Question> list = await _db.Question
                .Include(q => q.Answers)
                .Include(q => q.User)
                .ToListAsync();
            if (orderby == "date") {
                list = list.OrderByDescending(list => list.Date).ToList();
            } else if (orderby == "mostanswered") {
                list = list.OrderByDescending(list => list.Answers.Count).ToList();
            }
            list = list
                .Skip((currentPage - 1) * maxRowsPerPage)
                .Take(maxRowsPerPage)
                .ToList();

            model.questionList = list;

            double pageCount = (double)((decimal)_db.Question.Count() / Convert.ToDecimal(maxRowsPerPage));

            model.pageCount = (int)Math.Ceiling(pageCount);
            model.currentPageIndex = currentPage;

            return model;
        }
    }
}
