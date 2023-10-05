using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebUI.Data;
using WebUI.Models;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public ArticleController(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null) return NotFound();
            var article = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .FirstOrDefault(x => x.Id == id);
            if (article == null) return NotFound();

            var articles = _context.Articles
                .Where(x => x.Status == true && x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedDate).ToList();

            var currentArticleIndex = articles.IndexOf(article);
            var prev = (currentArticleIndex <= 0) ? null : articles[currentArticleIndex - 1];
            var next = (currentArticleIndex + 1 >= articles.Count) ? null : articles[currentArticleIndex + 1];


            var cookie = _contextAccessor.HttpContext.Request.Cookies["Views"];
            string[] findCookie = { "" };

            if(cookie != null) {
                findCookie = cookie.Split("-").ToArray();
            }

            if(!findCookie.Contains(article.Id.ToString()))
            {
                Response.Cookies.Append($"Views", $"{cookie}-{article.Id}",
                    new CookieOptions
                    {
                        Secure = true,
                        Expires = DateTime.Now.AddYears(1),
                        HttpOnly = true
                    });

                article.ViewCount += 1;
                _context.Articles.Update(article);
                await _context.SaveChangesAsync();
            }

            var similarArticles = _context.Articles.Include(x => x.Category)
                .Where(x => x.CategoryId == article.CategoryId && x.Id != article.Id)
                .Take(2).ToList();


            var popularPosts = _context.Articles.OrderByDescending(x => x.ViewCount).Take(3).ToList();

            var articleComments = _context.ArticleComments.Include(x => x.Article).Include(x => x.User).ToList();


            ArticleDetailVM articleDetailVM = new()
            {
                Article = article,
                PrevArticle = prev,
                NextArticle = next,
                SimilarArticles = similarArticles,
                PopularPosts = popularPosts,
                ArticleComments = articleComments
            };

            return View(articleDetailVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(ArticleComment articleComment, int articleId, string seoUrl)
        {
            try
            {
                articleComment.PublishDate = DateTime.Now;
                articleComment.UserId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _context.ArticleComments.AddAsync(articleComment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Detail", new {Id = articleId, SeoUrl = seoUrl });
            }
            catch (Exception)
            {
                return RedirectToAction("Detail", new { Id = articleId,SeoUrl = seoUrl });
            }
            
        }
    }
}
