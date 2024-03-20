// using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;

public class CourseController : Controller
{
    private readonly IWebHostEnvironment _environment;

    private readonly AppDbContext _context;
    private readonly ILogger<CourseController> _logger;

    public CourseController(ILogger<CourseController> logger,AppDbContext context,IWebHostEnvironment environment)
    {
        _logger = logger;
        _context=context;
        _environment=environment;
    }
    //Quản lí khóa học(admin)
    public IActionResult Index()
    {
        var allCourse=_context.courses.Include(c=>c.Category).ToList();
        return View(allCourse);
    }
    [HttpGet]
    public IActionResult Create()
    {
        SelectList categoryList=new SelectList(_context.categories,"Id","CategoryName");
        ViewData["categoryList"]=categoryList;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create([Bind("CourseName,Description,ImageFile,status,Price,CategoryId")] Course course)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        if(course.ImageFile!=null)
        {
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",course.ImageFile.FileName);
            if(!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream=new FileStream(filepath,FileMode.Create);
                course.ImageFile.CopyTo(fileStream);
            }
            course.CourseImageLink=$"uploads/{course.ImageFile.FileName}";
        }
        course.DateCreated=DateTime.Now;
        await _context.courses.AddAsync(course);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public IActionResult Detail(int? id)
    {
        if(id==null)
        {
            return Content("Không tìm thấy khóa học này");
        }
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Không tìm thấy khóa học này");
        }
        _context.Entry(kq).Collection(c=>c.chapters).Load();
        if(kq.chapters!=null)
        {
            foreach(var item in kq.chapters)
            {
                _context.Entry(item).Collection(i=>i.lessons).Load();
            }
        }
        
        return View(kq);
    }
    [HttpGet]
    public IActionResult Edit(int? id)
    {
        SelectList categoryList=new SelectList(_context.categories,"Id","CategoryName");
        ViewData["categoryList"]=categoryList;
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Không tìm thấy Course");
        }
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("CourseName,Description,ImageFile,status,Price,CategoryId")] Course course)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay");
        }
        if(course.ImageFile!=null)
        {
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",course.ImageFile.FileName);
            if(!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream=new FileStream(filepath,FileMode.Create);
                course.ImageFile.CopyTo(fileStream);
            }
            course.CourseImageLink=$"uploads/{course.ImageFile.FileName}";
        }
        kq.CategoryId=course.CategoryId;
        kq.CourseName=course.CourseName;
        kq.Price=course.Price;
        kq.DateEdited=DateTime.Now;
        kq.Description=course.Description;
        kq.status=course.status;
        kq.CourseImageLink=course.CourseImageLink;
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
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay khoa hoc");
        }
        _context.courses.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    public IActionResult AllCourse()
    {
        var allCourse=_context.courses.Include(c=>c.Category).ToList();
        return View(allCourse);
    }
}
