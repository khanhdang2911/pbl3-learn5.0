// using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;

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
        Console.WriteLine("tao chuong hoc");
        if(courseId==null)
        {
            return Content("Khong ton tai sdsd khoa hoc nay");
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

    public IActionResult Detail(int? id)
    {
        if(id==null)
        {
            return Content("Không tìm thấy chuong học này");
        }
        var kq=_context.chapters.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Không tìm thấy chuong học này");
        }
        _context.Entry(kq).Collection(c=>c.lessons).Load();
        return View(kq);
    }
    [HttpGet]
    public IActionResult Edit(int? id)
    {
        var kq=_context.chapters.Where(c=>c.Id==id).FirstOrDefault();
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
            return Content("Khong tim thay");
        }
        kq.ChapterName=chapter.ChapterName;

        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    public IActionResult Delete(int? id)
    {
        if(id==null)
        {
            return Content("Khong tim thay khoa hoc");
        }
        var kq=_context.chapters.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay khoa hoc");
        }
        _context.chapters.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }




}
