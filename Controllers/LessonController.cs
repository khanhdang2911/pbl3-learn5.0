// using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;

public class LessonController : Controller
{
    // [DataType(DataType.Upload)]
    // [Display(Name ="Upload video bài giảng")]
    // [BindProperty]
        // [NotMapped]
    // public IFormFile FormFile{set;get;}
    private readonly IWebHostEnvironment _environment;
    
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public LessonController(ILogger<HomeController> logger,AppDbContext context,IWebHostEnvironment environment)
    {
        _environment=environment;
        _logger = logger;
        _context=context;
    }

    public IActionResult Index()
    {
        var allLesson=(from c in _context.lessons select c).ToList();
        return View(allLesson);
    }
    [HttpGet]
    public IActionResult Create(int? chapterId)
    {
        if(chapterId==null)
        {
            return Content("Khong ton tai chuong hoc nay");
        }
        ViewData["chapterId"]=chapterId;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(int chapterId,[Bind("LessonName,Description,FileLinkContent,FormFile")] Lesson lesson)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        if(lesson.FormFile!=null)
        {
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",lesson.FormFile.FileName);
            using FileStream fileStream=new FileStream(filepath,FileMode.Create);
            lesson.FormFile.CopyTo(fileStream);
            lesson.FileLinkContent=$"~/uploads/{lesson.FormFile.FileName}";
        }
        int courseId=_context.chapters.Where(c=>c.Id==chapterId).Select(c=>c.CourseId).FirstOrDefault();
        lesson.ChapterId=chapterId;
        await _context.lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
        return RedirectToAction("Detail","Course",new{id=courseId});
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
        int courseId=(from c in _context.chapters join l in _context.lessons on c.Id equals kq.ChapterId select c.CourseId).FirstOrDefault();
        kq.LessonName=lesson.LessonName;
        kq.Description=lesson.Description;
        kq.FileLinkContent=lesson.FileLinkContent;
        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Detail","Course", new{id=courseId});
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
