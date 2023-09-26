namespace WebUI.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PhotoUrl { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public int CategoryId { get; set; }
        public Category category { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<ArticleComment> ArticleComments { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
    }
}
