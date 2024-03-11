// using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;

public class LessonController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public LessonController(ILogger<HomeController> logger,AppDbContext context)
    {
        _logger = logger;
        _context=context;
    }

    public IActionResult Index()
    {
        var allLesson=(from c in _context.lessons select c).ToList();
        return View(allLesson);
    }
    [HttpGet]
    public IActionResult Create(int? courseId)
    {
        Console.WriteLine("Asdasdsad");
        if(courseId==null)
        {
            return Content("Khong ton tai khoa hoc nay");
        }
        ViewData["courseId"]=courseId;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(int courseId,[Bind("LessonName,Description,FileLinkContent")] Lesson lesson)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        lesson.CourseId=courseId;
        await _context.lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index","Course");
    }

    public IActionResult Detail(int? id)
    {
        if(id==null)
        {
            return Content("Không tìm thấy bài học này");
        }
        var kq=_context.lessons.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Không tìm thấy bài học này");
        }
        return View(kq);
    }
    [HttpGet]
    public IActionResult Edit(int? id)
    {
        var kq=_context.lessons.Where(c=>c.Id==id).FirstOrDefault();
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("LessonName,Description,FileLinkContent,CourseId")] Lesson lesson)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.lessons.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay bai hoc nay");
        }
        kq.LessonName=lesson.LessonName;
        kq.Description=lesson.Description;
        kq.FileLinkContent=lesson.FileLinkContent;
        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    public IActionResult Delete(int? id)
    {
        if(id==null)
        {
            return Content("Khong tim thay bai hoc");
        }
        var kq=_context.lessons.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay khoa hoc");
        }
        _context.lessons.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }




}
