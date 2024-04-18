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
            return Content("Khong hop le id");
        }
        var kq=_context.blogs.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong hop le id");
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
            return Content("Khong tim thay bai hoc nay");
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
            return Content("Khong tim thay danh muc");
        }
        _context.blogs.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
