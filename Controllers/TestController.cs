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
            return RedirectToAction("NotFound","Home");
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
            return RedirectToAction("NotFound","Home");
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
            return RedirectToAction("NotFound","Home");
        }
        _context.tests.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Detail","Course",new{id=kq.CourseId});
    }

    public IActionResult DetailNote(int ?id)//Trước khi người dùng click vào để làm bài
    {   
        int UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        var kq=_context.tests.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
                        Console.WriteLine("1 nekkkkkkkkkkkkk");

            return RedirectToAction("NotFound","Home");
        }
        //check xem phai giao vien cua khoa hoc do hay khong
        var Course=_context.courses.Find(kq.CourseId);
        if(UserId==Course.TeacherId)
        {
            
        }
        else if(_context.usersCourses.Any(c=>c.UsersId==UserId&&c.CourseId==Course.Id)==false)
        {
                        Console.WriteLine("2 nekkkkkkkkkkkkk");

            return RedirectToAction("NotFound","Home");
        }
        if(id==null)
        {
                        Console.WriteLine("3 nekkkkkkkkkkkkk");

            return RedirectToAction("NotFound","Home");
        }

        
        return View(kq);
    }
    public IActionResult Detail(int ?id)
    {
        var checkExits=_context.tests.Any(c=>c.Id==id);
        if(checkExits==false)
        {
            return RedirectToAction("NotFound","Home");
        }
        int UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);


        var kq=_context.tests.Where(t=>t.Id==id).Include(q=>q.Questions).ThenInclude(a=>a.Answers).FirstOrDefault();
        var Course=_context.courses.Find(kq.CourseId);
        if(UserId==Course.TeacherId)
        {
            
        }
        else if(_context.usersCourses.Any(c=>c.UsersId==UserId&&c.CourseId==Course.Id)==false)
        {
            return RedirectToAction("NotFound","Home");
        }
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }

        
        

        return View(kq);
    }
    public async Task<IActionResult> SubmitTest(int id,[FromForm] int []answers)
    {
        

        var kq=await _context.tests.Where(c=>c.Id==id).FirstOrDefaultAsync();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        ViewData["id"]=id;


        // Luu lich su bai thi
        int userID=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        UsersTest usersTest=new UsersTest();
        usersTest.UsersId=userID;
        usersTest.TestId=id;
        usersTest.DateSubmited=DateTime.Now;
        int correctAns=0;
        Console.WriteLine(answers.Count()+" vcl that");
        foreach(var item in answers.ToList())
        {
            if(_context.answers.Any(c=>c.Id==item&&c.IsCorrect==1))
            {
                correctAns++;
            }
        }
        usersTest.correctAnswer=correctAns;
        await _context.usersTests.AddAsync(usersTest);
        await _context.SaveChangesAsync();
        ViewData["usersTest"]=usersTest;
        return View(answers.ToList());
    }




}
