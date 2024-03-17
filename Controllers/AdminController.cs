using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;

public class AdminController : Controller
{

    private readonly AppDbContext _context;
    public AdminController(AppDbContext context)
    {
        _context=context;
    }

    public IActionResult Index()
    {
        var kq=(from u in _context.users select u).ToList();
        return View(kq);
    }

    public IActionResult AddUser()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AddUser([Bind("Email,Password,Name,Phone")] Users users) 
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        _context.users.Add(users);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult EditUser(int UsersId)
    {
        if(UsersId==null)
        {
            return Content("Khong tim thay user");
        }
        var kq=_context.users.Where(u=>u.Id==UsersId).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay user");

        }
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> EditUser(int UsersId,[Bind("Email,Password,Name,Phone")] Users users)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.users.Where(u=>u.Id==UsersId).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay user nay");
        }
        _context.Entry(kq).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
        kq.Email=users.Email;
        kq.Password=users.Password;
        kq.Name=users.Name;
        kq.Phone=users.Phone;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    
    public IActionResult DeleteUser([FromRoute]int? id)
    {
        if(id==null)
        {
            return Content("Khong tim thay User");
        }
        var kq=_context.users.Where(u=>u.Id==id).FirstOrDefault();
        Console.WriteLine(id+"  sdasdsa here");
        if(kq==null)
        {
            return Content("Khong tim thay user nay dau");
        }
        _context.users.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
