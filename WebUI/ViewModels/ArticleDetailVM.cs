using WebUI.Models;

namespace WebUI.ViewModels
{
    public class ArticleDetailVM
    {
        public Article Article { get; set; }
        public Article NextArticle { get; set; }
        public Article PrevArticle { get; set; }
    }
}
