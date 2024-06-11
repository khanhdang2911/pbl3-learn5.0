// using System.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;


public class LessonController : Controller
{
    private readonly IAuthorizationService _iAuthorize;
    private readonly IWebHostEnvironment _environment;
    
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public LessonController(ILogger<HomeController> logger,AppDbContext context,IWebHostEnvironment environment,IAuthorizationService iAuthorize)
    {
        _environment=environment;
        _logger = logger;
        _context=context;
        _iAuthorize=iAuthorize;
    }
    [Authorize]
    public IActionResult Index()
    {
        var allLesson=(from c in _context.lessons select c).ToList();
        return View(allLesson);
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Create(int? chapterId)
    {

        var chapter=_context.chapters.Where(c=>c.Id==chapterId).FirstOrDefault();

        if(chapter==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var course=_context.courses.Find(chapter.CourseId);
        int teacherId = course.TeacherId;
        int UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        if(teacherId!=UserId)
        {
            return RedirectToAction("NotFound","Home");
        }
        
        // var chapter=_context.chapters.Where(c=>c.Id==chapterId).FirstOrDefault();

        // if(chapter==null)
        // {
        //     return RedirectToAction("NotFound","Home");
        // }
        // var course=_context.courses.Find(chapter.CourseId);
        // var checkAuthor= await _iAuthorize.AuthorizeAsync(User,
        //                                                   course,
        //                                                   "TeacherInCourse");
        // if(!checkAuthor.Succeeded)
        // {
        //         return RedirectToAction("NotFound","Home");
        // }
        ViewData["chapterId"]=chapterId;
        return View();
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(int chapterId,[Bind("LessonName,Description,FormFile,DocumentFile,IsFree")] Lesson lesson)
    {
        
        var chapter=_context.chapters.Where(c=>c.Id==chapterId).FirstOrDefault();

        if(chapter==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var course=_context.courses.Find(chapter.CourseId);
        int teacherId = course.TeacherId;
        int UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        if(teacherId!=UserId)
        {
            return RedirectToAction("NotFound","Home");
        }

        if(!ModelState.IsValid)
        {
            // ModelState.AddModelError("","Tạo không thành công");
            return View();
        }
        if(lesson.FormFile!=null)
        {
            
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",lesson.FormFile.FileName);
            if(!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream=new FileStream(filepath,FileMode.Create);
                lesson.FormFile.CopyTo(fileStream);
            }
            
            lesson.FileLinkContent=$"uploads/{lesson.FormFile.FileName}";
        }
        
        if(lesson.DocumentFile!=null)
        {
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",lesson.DocumentFile.FileName);
            if(!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream=new FileStream(filepath,FileMode.Create);
                lesson.DocumentFile.CopyTo(fileStream);
            }
            lesson.DocumentLink=$"uploads/{lesson.DocumentFile.FileName}";
        }
        int courseId=_context.chapters.Where(c=>c.Id==chapterId).Select(c=>c.CourseId).FirstOrDefault();
        lesson.ChapterId=chapterId;
        await _context.lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
        return RedirectToAction("Detail","Course",new {id=courseId});
    }
    
    public async Task<IActionResult> Detail(int? id)
    {
        if(User.Identity.IsAuthenticated==false)
        {
            TempData["Message"]="Please login to watch this lecture";
            return RedirectToAction("NotFound","Home");
        }
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var kq=await _context.lessons.Where(c=>c.Id==id).FirstOrDefaultAsync();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        if(kq.IsFree==0)
        {

        }
        else{
            var chapter=_context.chapters.Where(c=>c.Id==kq.ChapterId).FirstOrDefault();

            var course=_context.courses.Find(chapter.CourseId);
            //Neu giao vien cua khoa hoc do thi chac chan dc xem
            int teacherId = course.TeacherId;
            int UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
            if(teacherId==UserId)
            {
            }
            else if(User.IsInRole("Admin")){}
            else if(_context.usersCourses.Any(c=>c.CourseId==course.Id&&c.UsersId==UserId)==false)
            {
                return RedirectToAction("NotFound","Home");
            }
        }

        kq.View++;
        await _context.SaveChangesAsync();
        return View(kq);
    }
    [HttpGet]
    [Authorize]
    public IActionResult Edit(int? id)
    {
        var kq=_context.lessons.Where(c=>c.Id==id).FirstOrDefault();
        //Check author
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var chapter=_context.chapters.Where(c=>c.Id==kq.ChapterId).FirstOrDefault();

        var course=_context.courses.Find(chapter.CourseId);
        //Neu giao vien cua khoa hoc do thi chac chan dc xem
        int teacherId = course.TeacherId;
        int UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        if(teacherId==UserId)
        {
        }
        // else if(User.IsInRole("Admin")){}
        else 
        {
            return RedirectToAction("NotFound","Home");
        }
        //Check author end
        return View(kq);
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(int? id,[Bind("LessonName,Description,FormFile,DocumentFile,IsFree")] Lesson lesson)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.lessons.Where(c=>c.Id==id).FirstOrDefault();
        
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        // var fileDelete=Path.Combine(_environment.WebRootPath,"uploads",kq.FileLinkContent.Remove(0,8));
        // System.IO.File.Delete(fileDelete);
        if(lesson.FormFile!=null)
        {
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",lesson.FormFile.FileName);
            if(!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream=new FileStream(filepath,FileMode.Create);
                lesson.FormFile.CopyTo(fileStream);
            }
            kq.FileLinkContent=$"uploads/{lesson.FormFile.FileName}";
        }
        if(lesson.DocumentFile!=null)
        {
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",lesson.DocumentFile.FileName);
            if(!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream=new FileStream(filepath,FileMode.Create);
                lesson.DocumentFile.CopyTo(fileStream);
            }
            
            kq.DocumentLink=$"uploads/{lesson.DocumentFile.FileName}";
        }
        int courseId=(from c in _context.chapters join l in _context.lessons on c.Id equals l.ChapterId where c.Id==kq.ChapterId select c.CourseId).FirstOrDefault();
        kq.LessonName=lesson.LessonName;
        kq.Description=lesson.Description;
        kq.IsFree=lesson.IsFree;
        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Detail","Course", new{id=courseId});
    }
    [Authorize]
    public IActionResult Delete(int? id)
    {
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var kq=_context.lessons.Where(c=>c.Id==id).FirstOrDefault();
        //Check author
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var chapter=_context.chapters.Where(c=>c.Id==kq.ChapterId).FirstOrDefault();

        var course=_context.courses.Find(chapter.CourseId);
        //Neu giao vien cua khoa hoc do thi chac chan dc xem
        int teacherId = course.TeacherId;
        int UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        if(teacherId==UserId)
        {
        }
        // else if(User.IsInRole("Admin")){}
        else 
        {
            return RedirectToAction("NotFound","Home");
        }
        //Check author end
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        int courseId=(from c in _context.chapters join l in _context.lessons on c.Id equals l.ChapterId where c.Id==kq.ChapterId select c.CourseId).FirstOrDefault();
        _context.lessons.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Detail","Course",new{id=courseId});
    }

}
