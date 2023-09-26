using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using WebUI.Data;
using WebUI.Helper;
using WebUI.Models;

namespace WebUI.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        public ArticleController(AppDbContext context, IHttpContextAccessor contextAccessor, UserManager<User> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _env = env;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles.ToList();
            return View(articles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewData["tags"] = tags;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Article article, IFormFile Photo, List<Tag> Tags)
        {
            try
            {
                article.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);

                article.CreatedDate = DateTime.Now;
                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                article.UserId = userId;
                var user = await _userManager.FindByIdAsync(userId);
                article.CreatedBy = user.Email;
                article.ViewCount = 0;

                await _context.Articles.AddAsync(article);
                await _context.SaveChangesAsync();
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
