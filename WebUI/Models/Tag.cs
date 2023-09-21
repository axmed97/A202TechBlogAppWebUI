﻿using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
    }
}
