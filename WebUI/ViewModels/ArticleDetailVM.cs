using WebUI.Models;

namespace WebUI.ViewModels
{
    public class ArticleDetailVM
    {
        public Article Article { get; set; }
        public Article NextArticle { get; set; }
        public Article PrevArticle { get; set; }
        public List<Article> SimilarArticles { get; set; }
        public List<Article> PopularPosts { get; set; }
        public List<ArticleComment> ArticleComments { get; set; }
    }
}
