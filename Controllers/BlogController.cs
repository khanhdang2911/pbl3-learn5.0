using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;
using Microsoft.AspNetCore.Authorization;

namespace PBL3_Course.Controllers;
[Authorize(Roles ="Admin")]
public class BlogController : Controller
{
    
    private readonly AppDbContext _context;

    public BlogController(AppDbContext context)
    {
        _context=context;
    }

    public IActionResult Index()
    {
        var allBlog=(from c in _context.blogs select c).ToList();
        return View(allBlog);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create([Bind("BlogName")] Blog blog)
    {

        if(!ModelState.IsValid)
        {
            ModelState.AddModelError("","Tạo không thành công");
            return View();
        }
        
        await _context.blogs.AddAsync(blog);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var kq=_context.blogs.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("BlogName")] Blog blog)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.blogs.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }

        kq.BlogName=blog.BlogName;
        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");

    }
    public IActionResult Delete(int? id)
    {
        var kq=_context.blogs.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        _context.blogs.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    public IActionResult SearchBlog(string blogName)
    {
        var allBlog=(from c in _context.blogs select c).Where(c=>c.BlogName.Contains(blogName)).ToList();
        return View(allBlog);
    }
}
