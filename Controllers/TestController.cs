// using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using PBL3_Course.Models;
using Microsoft.AspNetCore.Authorization;

namespace PBL3_Course.Controllers;
[Authorize]
public class TestController : Controller
{
    private readonly AppDbContext _context;
    public TestController(AppDbContext context)
    {
        _context=context;
    }

    [HttpGet]
    public IActionResult Create(int CourseId)
    {
        var kq=_context.courses.Where(c=>c.Id==CourseId).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay khoa hoc de them bai kiem tra");
        }
        ViewData["CourseId"]=CourseId;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create([Bind("TestName,Time,CourseId,NumberOfQuestion")] Test test)
    {

        if(!ModelState.IsValid)
        {
            ModelState.AddModelError("","Tạo không thành công");
            return View();
        }
        
        await _context.tests.AddAsync(test);
        await _context.SaveChangesAsync();
        int Id=_context.tests.Where(t=>t.TestName==test.TestName).Select(t=>t.CourseId).FirstOrDefault();
        return RedirectToAction("Detail","Course",new{id=Id});
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        var kq=_context.tests.Where(c=>c.Id==id).FirstOrDefault();
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("TestName,Time,CourseId,NumberOfQuestion")] Test test)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.tests.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay bai kiem tra nay");
        }
        kq.TestName=test.TestName;
        kq.Time=test.Time;
        kq.NumberOfQuestion=test.NumberOfQuestion;
        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    public IActionResult Delete(int? id)
    {
        var kq=_context.tests.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay bai kiem tra");
        }
        _context.tests.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Detail","Course",new{id=kq.CourseId});
    }

    public IActionResult DetailNote(int ?id)//Trước khi người dùng click vào để làm bài
    {   
        if(id==null)
        {
            return Content("Khong tim thay bai kiem tra");
        }

        var kq=_context.tests.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay bai kiem tra");
        }
        return View(kq);
    }
    public IActionResult Detail(int ?id)
    {
        if(id==null)
        {
            return Content("Khong tim thay bai kiem tra");
        }

        var checkExits=_context.tests.Any(c=>c.Id==id);
        if(checkExits==false)
        {
            return Content("Khong tim thay bai kiem tra");
        }
        
        var kq=_context.tests.Where(t=>t.Id==id).Include(q=>q.Questions).ThenInclude(a=>a.Answers).FirstOrDefault();

        return View(kq);
    }
    public IActionResult SubmitTest(int?id,[FromForm] int []answers)
    {
        int i=0;
        foreach(var item in answers)
        {
            i++;
            Console.WriteLine("dap an ne:"+i+"   "+item);
        }
        if(id==null)
        {
            return Content("Khong tim thay bai kiem tra");
        }

        var kq=_context.tests.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay bai kiem tra");
        }
        ViewData["id"]=id;
        return View(answers.ToList());
    }




}
