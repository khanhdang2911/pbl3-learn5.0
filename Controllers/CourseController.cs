// using System.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;
[Authorize(Roles ="Admin")]
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
        var allCourse=_context.courses.Where(c=>c.IsActive==1).Include(c=>c.Category).ToList();
        return View(allCourse);
    }
    public IActionResult IndexForCourseNotActive()
    {
        var allCourse=_context.courses.Where(c=>c.IsActive==0).Include(c=>c.Category).ToList();
        return View("Index",allCourse);
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Create()
    {
        SelectList categoryList=new SelectList(_context.categories,"Id","CategoryName");
        ViewData["categoryList"]=categoryList;
        return View();
    }
    [HttpPost]
    [AllowAnonymous]
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
        if(User.IsInRole("Admin"))
        {
            course.IsActive=1;
        }
        var UserId=User.Claims.FirstOrDefault(c=>c.Type=="Id")?.Value;
        course.TeacherId=int.Parse(UserId);
        course.DateCreated=DateTime.Now;
        await _context.courses.AddAsync(course);
        await _context.SaveChangesAsync();
        if(User.IsInRole("Admin")==false)
        {
            return RedirectToAction("Index","Home");
        }
        return RedirectToAction("Index");
    }
    [AllowAnonymous]
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
        if(kq.IsActive==0)
        {
            return NotFound();
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
    [HttpPost]
    public IActionResult Search(string courseName)
    {
        var kq=_context.courses.Where(c=>c.CourseName.Contains(courseName)).ToList();
        return View("AllCourse",kq);
    }
    public IActionResult SearchForAdmin(string courseName)
    {
         var kq=_context.courses.Where(c=>c.CourseName.Contains(courseName)).ToList();
        return View("Index",kq);
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
    [AllowAnonymous]
    public IActionResult AllCourse(int?id,string sortBy)
    {
        List<Course> allCourse=new List<Course>();
        if(id==null)
        {
            allCourse=_context.courses.ToList();
            //sort
            if(sortBy=="price1")
            {
                allCourse=_context.courses.OrderByDescending(c=>c.Price).ToList();
            }
            else if(sortBy=="price2")
            {
                allCourse=_context.courses.OrderBy(c=>c.Price).ToList();
            }
            return View(allCourse);
        }
        //Neu id khac null
        allCourse=_context.courses.Where(c=>c.CategoryId==id).ToList();
        //sort
        if(sortBy=="name")
        {
            allCourse=_context.courses.OrderBy(c=>c.CourseName).ToList();
        }
        return View(allCourse);
    }
    public IActionResult CourseActivate(int id)
    {
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay khoa hoc");
        }
        _context.Entry(kq).State=EntityState.Modified;
        kq.IsActive=1;
        //Lay ra id cua user hien tai
        int UserId=int.Parse(User.Claims.FirstOrDefault(c=>c.Type=="Id")?.Value);
        
        //Tim Teacher Id Trong role
        UsersRole usersRole=new UsersRole();
        var RoleTeacherID=_context.roles.Where(r=>r.RoleName=="Teacher").Select(r=>r.Id).FirstOrDefault();
        
        //Kiem tra nguoi dung hien tai da co role teacher hay chua
        bool checkAddUserRole=_context.usersRoles.Any(u=>u.RoleId==RoleTeacherID&& u.UsersId==UserId);
        if(checkAddUserRole==false)
        {
            Console.WriteLine("IDasdasda");
            usersRole.UsersId=UserId;
            usersRole.RoleId=RoleTeacherID;
            _context.usersRoles.Add(usersRole);
        }
        
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
