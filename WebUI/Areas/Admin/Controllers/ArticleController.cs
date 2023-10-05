using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebUI.Data;
using WebUI.Helper;
using WebUI.Models;


// SOLID - 
// N-Layer/N-Tier
// Onion
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
            var articles = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Include(x => x.User)
                .Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
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
        public async Task<IActionResult> Create(Article article, IFormFile Photo, List<int> tagIds)
        {
            try
            {
                var categories = _context.Categories.ToList();
                var tags = _context.Tags.ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
                ViewData["tags"] = tags;

                if (tagIds.Count == 0)
                {
                    ViewBag.TagError = "Please select tag!";
                    return View();
                }


                article.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);

                article.CreatedDate = DateTime.Now;
                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                article.UserId = userId;
                var user = await _userManager.FindByIdAsync(userId);
                article.CreatedBy = user.Firstname;
                article.ViewCount = 0;
                article.SeoUrl = SeoHelper.ReplaceInvalidChars(article.Title);

                article.Status = false;
                article.IsDeleted = false;

                await _context.Articles.AddAsync(article);
                await _context.SaveChangesAsync();

                List<ArticleTag> tagList = new();

                for (int i = 0; i < tagIds.Count; i++)
                {
                    ArticleTag articleTag = new()
                    {
                        ArticleId = article.Id,
                        TagId = tagIds[i]
                    };
                    tagList.Add(articleTag);
                }
                await _context.ArticleTags.AddRangeAsync(tagList);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == null) return NotFound();
            var article = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .FirstOrDefault(x => x.Id == id);
            if (article == null) return NotFound();

            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewData["tags"] = tags;

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Article article, IFormFile Photo, List<int> tagIds)
        {
            try
            {
                var categories = _context.Categories.ToList();
                var tags = _context.Tags.ToList();
                ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
                ViewData["tags"] = tags;

                if (tagIds.Count == 0)
                {
                    ViewBag.TagError = "Please select tag!";
                    return View();
                }

                if (Photo != null)
                {
                    article.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);
                }

                article.SeoUrl = SeoHelper.ReplaceInvalidChars(article.Title);

                article.UpdatedDate = DateTime.Now;
                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                article.UpdatedBy = user.Firstname;
                var articleTagDelete = _context.ArticleTags.Where(x => x.ArticleId == article.Id).ToList();
                _context.ArticleTags.RemoveRange(articleTagDelete);

                List<ArticleTag> tagList = new();
                for (int i = 0; i < tagIds.Count; i++)
                {
                    ArticleTag articleTag = new()
                    {
                        ArticleId = article.Id,
                        TagId = tagIds[i]
                    };
                    tagList.Add(articleTag);
                }
                await _context.ArticleTags.AddRangeAsync(tagList);
                await _context.SaveChangesAsync();

                _context.Articles.Update(article);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) return NotFound();
            var article = _context.Articles.FirstOrDefault(x => x.Id == id);
            if (article == null) return NotFound();

            var filePath = (_env.WebRootPath + article.PhotoUrl).ToLower();

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
