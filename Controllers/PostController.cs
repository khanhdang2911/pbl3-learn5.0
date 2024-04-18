// using System.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;
[Authorize(Roles="Admin")]
public class PostController : Controller
{
    
    private readonly IWebHostEnvironment _environment;
    private readonly AppDbContext _context;

    public PostController(AppDbContext context,IWebHostEnvironment environment)
    {
        _environment = environment;
        _context=context;
    }
    public IActionResult Index()
    {
        var allPost=(from p in _context.posts select p).Include(p=>p.Blog).ToList();
        return View(allPost);
    }
    [AllowAnonymous]
    public IActionResult AllPost(int? BlogId)
    {
        List<Post> posts=new List<Post>();
        Console.WriteLine("Blog ID="+BlogId);
        if(BlogId==null)
        {
            posts=(from p in _context.posts select p).Include(p=>p.Blog).ToList();
            return View(posts);
        }
        posts=_context.posts.Where(p=>p.BlogId==BlogId).Include(p=>p.Blog).ToList();

        return View(posts);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create([Bind("PostName,PostContent,DateCreatedOrEdited,BlogId,FormFile")] Post post)
    {

        if(!ModelState.IsValid)
        {
            ModelState.AddModelError("","Tạo không thành công");
            return View();
        }
        if(post.FormFile!=null)
        {
            
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",post.FormFile.FileName);
            if(!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream=new FileStream(filepath,FileMode.Create);
                post.FormFile.CopyTo(fileStream);
            }
            post.ImageLink=$"uploads/{post.FormFile.FileName}";
        }
        post.DateCreatedOrEdited=DateTime.Now;

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
    public async Task<IActionResult> Edit(int? id,[Bind("PostName,PostContent,DateCreatedOrEdited,BlogId,FormFile")] Post post)
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
        //Xoa file cu di
        // var fileDelete=Path.Combine(_environment.WebRootPath,"uploads",kq.ImageLink.Remove(0,8));
        // System.IO.File.Delete(fileDelete);
        if(post.FormFile!=null)
        {
            
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",post.FormFile.FileName);
            if(!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream=new FileStream(filepath,FileMode.Create);
                post.FormFile.CopyTo(fileStream);
            }
            post.ImageLink=$"uploads/{post.FormFile.FileName}";
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
    [AllowAnonymous]
    public IActionResult Detail(int id)
    {
        var kq=_context.posts.Where(p=>p.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        return View(kq);
    }
    [AllowAnonymous]
    public IActionResult Search(string postName)
    {
        var kq=_context.posts.Where(p=>p.PostName.Contains(postName)==true).ToList();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        return View("AllPost",kq);
    }
}
