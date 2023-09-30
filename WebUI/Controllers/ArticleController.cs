using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUI.Data;
using WebUI.Models;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;

        public ArticleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Detail(int id)
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

            ArticleDetailVM articleDetailVM = new()
            {
                Article = article,
                PrevArticle = prev,
                NextArticle = next
                
            };

            return View(articleDetailVM);
        }
    }
}
