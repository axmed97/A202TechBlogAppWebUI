using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.Data;
using WebUI.Models;

namespace WebUI.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin, Admin Editor")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tags = _context.Tags.ToList();
            return View(tags);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tag tag)
        {
            try
            {
                var checkTag = _context.Tags.FirstOrDefault(x => x.Id == tag.Id);
                if (checkTag != null) return View();
                _context.Tags.Add(tag);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var tagCount = _context.Tags.Count();
            if (tagCount <= 1)
                return RedirectToAction("Index");
            if (id == null) return NotFound();
            var tags = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (tags == null) return NotFound();
            return View(tags);
        }

        [HttpPost]
        public IActionResult Delete(Tag tag)
        {
            var tagCount = _context.Tags.Count();
            if(tagCount <= 1)
            {
                return RedirectToAction("Index");
            }
            _context.Tags.Remove(tag);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == null) return NotFound();
            var tags = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (tags == null) return NotFound();
            return View(tags);
        }

        [HttpPost]
        public IActionResult Edit(Tag tag)
        {
            try
            {
                var checkTag = _context.Tags.FirstOrDefault(x => x.TagName == tag.TagName);
                if (checkTag != null)
                {
                    ViewBag.Error = "Tag name is already exists!";
                    return View();
                };
                _context.Tags.Update(tag);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
    }
}
