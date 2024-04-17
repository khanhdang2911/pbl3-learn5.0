// using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;

public class PostController : Controller
{
    
    
    private readonly AppDbContext _context;

    public PostController(AppDbContext context)
    {
        _context=context;
    }

    public IActionResult Index()
    {
        var allPost=(from c in _context.posts select c).ToList();
        return View(allPost);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create([Bind("PostName,PostContent,DateCreatedOrEdited,BlogId")] Post post)
    {

        if(!ModelState.IsValid)
        {
            ModelState.AddModelError("","Tạo không thành công");
            return View();
        }
        
        await _context.posts.AddAsync(post);
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
        var kq=_context.posts.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong hop le id");
        }
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("PostName,PostContent,DateCreatedOrEdited,BlogId")] Post post)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.posts.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay bai hoc nay");
        }

        kq.PostName=post.PostName;
        kq.DateCreatedOrEdited=DateTime.Now;
        kq.PostContent=post.PostContent;
        kq.BlogId=post.BlogId;
        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");

    }
    public IActionResult Delete(int? id)
    {
        var kq=_context.posts.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay danh muc");
        }
        _context.posts.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
