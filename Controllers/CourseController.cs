// using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;

public class CourseController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public CourseController(ILogger<HomeController> logger,AppDbContext context)
    {
        _logger = logger;
        _context=context;
    }

    public IActionResult Index()
    {
        var allCourse=(from c in _context.courses select c).ToList();
        return View(allCourse);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create([Bind("CourseName,Price,Description")] Course course)
    {
        if(!ModelState.IsValid)
        {
            return View();
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
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("CourseName,Price,Description")] Course course)
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
        kq.CourseName=course.CourseName;
        kq.Price=course.Price;
        kq.DateEdited=DateTime.Now;
        kq.Description=course.Description;
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




}
