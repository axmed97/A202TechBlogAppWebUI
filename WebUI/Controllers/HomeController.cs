﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebUI.Data;
using WebUI.Helper;
using WebUI.Models;
using WebUI.ViewModels;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;


        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int pg = 1)
        {

            const int pageSize = 5;

            if (pg < 1)
            {
                pg = 1;
            }

            int articleCount = _context.Articles.Count();

            var pager = new PageHelper(articleCount, pg, pageSize);

            int artSkip = (pg - 1) * pageSize;
            ViewBag.Pager = pager;

            var featuredArticles = _context.Articles
                .Include(x => x.Category)
                .Where(x => x.IsFeatured == true
            && x.Status == true && x.IsDeleted == false).OrderByDescending(x => x.UpdatedDate).Take(3).ToList();

            var articles = _context.Articles
                .Include(x => x.Category)
                .Where(x => x.Status == true && x.IsDeleted == false)
                .Skip(artSkip)
                .Take(pager.PageSize)
                .OrderByDescending(x => x.CreatedDate).ToList();
            HomeVM homeVM = new()
            {
                FeaturedArticles = featuredArticles,
                Articles = articles
            };
            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}