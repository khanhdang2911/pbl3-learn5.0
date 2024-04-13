// using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using PBL3_Course.Models;
using Microsoft.AspNetCore.Authorization;

namespace PBL3_Course.Controllers;
[Authorize(Roles ="Admin")]
public class OrderController : Controller
{
    private readonly AppDbContext _context;
    public OrderController(AppDbContext context)
    {
        _context=context;
    }

    public IActionResult Index()
    {
        var orders=_context.orders.ToList();
        return View(orders);
        
    }
    public IActionResult IndexDate(int month,int year)
    {
        var orders=_context.orders.Where(o=>o.DateCreated.Month==month&&o.DateCreated.Year==year).ToList();
        return View("Index",orders);        
    }
    public IActionResult Delete(int? id)
    {
        var kq=_context.orders.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay hoa don");
        }
        _context.orders.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index","Order");
    }

    public IActionResult Detail(int ?id)
    {
        if(id==null)
        {
            return Content("Khong tim thay hoa don");
        }

        var order=_context.orders.Where(c=>c.Id==id).FirstOrDefault();
        if(order==null)
        {
            return Content("Khong tim thay hoa don");
        }
        ViewData["user"]=_context.users.Where(c=>c.Id==order.UserId).FirstOrDefault();
        ViewData["course"]=_context.courses.Where(c=>c.Id==order.courseId).FirstOrDefault();
        return View(order);
    }





}
