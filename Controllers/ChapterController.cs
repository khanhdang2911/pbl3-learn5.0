// using System.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;
[Authorize]

public class ChapterController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public ChapterController(ILogger<HomeController> logger,AppDbContext context)
    {
        _logger = logger;
        _context=context;
    }

    public IActionResult Index()
    {
        var allChapter=(from c in _context.chapters select c).ToList();
        return View(allChapter);
    }
    [HttpGet]
    public IActionResult Create(int? courseId)
    {
        var course=_context.courses.Where(c=>c.Id==courseId).FirstOrDefault();
        if(course==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        int UserId=int.Parse(User.Claims.FirstOrDefault(c=>c.Type=="Id").Value);
        if(User.IsInRole("Admin"))
        {

        }
        else if(UserId!=course.TeacherId)
        {
            return RedirectToAction("NotFound","Home");
        }

        ViewData["courseId"]=courseId;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(int courseId,[Bind("ChapterName")] Chapter chapter)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        chapter.CourseId=courseId;
        await _context.chapters.AddAsync(chapter);
        await _context.SaveChangesAsync();
        return RedirectToAction("Detail","Course",new {id=courseId});
    }

    // public IActionResult Detail(int? id)
    // {
    //     if(id==null)
    //     {
    //         return RedirectToAction("NotFound","Home");
    //     }
    //     var kq=_context.chapters.Where(c=>c.Id==id).FirstOrDefault();
    //     if(kq==null)
    //     {
    //         return RedirectToAction("NotFound","Home");
    //     }
    //     _context.Entry(kq).Collection(c=>c.lessons).Load();
    //     return View(kq);
    // }
    [HttpGet]
    public IActionResult Edit(int? id)
    {
        
        var kq=_context.chapters.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var course=_context.courses.Find(kq.CourseId);
         int UserId=int.Parse(User.Claims.FirstOrDefault(c=>c.Type=="Id").Value);
        // if(User.IsInRole("Admin"))
        // {

        // }
        // else
         if(UserId!=course.TeacherId)
        {
            return RedirectToAction("NotFound","Home");
        }
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("ChapterName")] Chapter chapter)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.chapters.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        kq.ChapterName=chapter.ChapterName;

        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Detail","Course",new{id=kq.CourseId});
    }
    public IActionResult Delete(int? id)
    {
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var kq=_context.chapters.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        //Check role cua giao vien
        var course=_context.courses.Find(kq.CourseId);
         int UserId=int.Parse(User.Claims.FirstOrDefault(c=>c.Type=="Id").Value);
        // if(User.IsInRole("Admin"))
        // {

        // }
        // else 
        if(UserId!=course.TeacherId)
        {
            return RedirectToAction("NotFound","Home");
        }
        //
        _context.chapters.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Detail","Course",new{id=kq.CourseId});
    }
}
