// using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using PBL3_Course.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

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
    public IActionResult Create([Bind("TestName,Time,CourseId,NumberOfQuestion")] Test test)
    {

        if(!ModelState.IsValid)
        {
            ModelState.AddModelError("","Tạo không thành công");
            return View();
        }
        
        _context.tests.Add(test);
        _context.SaveChanges();
        int Id=_context.tests.Where(t=>t.TestName==test.TestName).Select(t=>t.Id).FirstOrDefault();
        return RedirectToAction("Create","Question",new{TestId=Id});
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        
        var kq=_context.tests.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("Home","NotFound");
        }
        var courseOfTest=_context.courses.Where(c=>c.Id==kq.CourseId).First();
        var UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        if(User.IsInRole("Admin"))
        {
            
        }
        else if(User.IsInRole("Admin")&&courseOfTest.TeacherId==UserId)
        {

        }
        else{
            return RedirectToAction("Home","NotFound");
        }
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
        var courseOfTest=_context.courses.Where(c=>c.Id==kq.CourseId).First();
        var UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        if(User.IsInRole("Admin"))
        {
            
        }
        else if(User.IsInRole("Admin")&&courseOfTest.TeacherId==UserId)
        {

        }
        else{
            return RedirectToAction("Home","NotFound");
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
        var courseOfTest=_context.courses.Where(c=>c.Id==kq.CourseId).First();
        var UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        if(User.IsInRole("Admin"))
        {
            
        }
        else if(User.IsInRole("Admin")&&courseOfTest.TeacherId==UserId)
        {

        }
        else{
            return RedirectToAction("Home","NotFound");
        }

        _context.tests.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Detail","Course",new{id=kq.CourseId});
    }

    public IActionResult DetailNote(int ?id)//Trước khi người dùng click vào để làm bài
    {   
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        int UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        var kq=_context.tests.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {

            return RedirectToAction("NotFound","Home");
        }
        //check xem phai giao vien cua khoa hoc do hay khong
        var Course=_context.courses.Find(kq.CourseId);
        if(User.IsInRole("Admin"))
        {

        }
        else if(UserId==Course.TeacherId)
        {
            
        }
        else if(_context.usersCourses.Any(c=>c.UsersId==UserId&&c.CourseId==Course.Id)==false)
        {
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
        


        var kq=_context.tests.Where(t=>t.Id==id).Include(q=>q.Questions).ThenInclude(a=>a.Answers).FirstOrDefault();
        int UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        var Course=_context.courses.Find(kq.CourseId);
        if(User.IsInRole("Admin"))
        {

        }
        else if(UserId==Course.TeacherId)
        {
            
        }
        else if(_context.usersCourses.Any(c=>c.UsersId==UserId&&c.CourseId==Course.Id)==false)
        {
            return RedirectToAction("NotFound","Home");
        }

        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> SubmitTest(int id,[FromForm] int []answers)
    {

        var kq=await _context.tests.Where(c=>c.Id==id).FirstOrDefaultAsync();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        ViewData["id"]=id;
        //Check role
        int UserId=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        var Course=_context.courses.Find(kq.CourseId);
        if(User.IsInRole("Admin"))
        {

        }
        else if(UserId==Course.TeacherId)
        {
            
        }
        else if(_context.usersCourses.Any(c=>c.UsersId==UserId&&c.CourseId==Course.Id)==false)
        {
            return RedirectToAction("NotFound","Home");
        }
        //Check role end

        // Luu lich su bai thi
        int userID=int.Parse(User.Claims.First(c=>c.Type=="Id").Value);
        UsersTest usersTest=new UsersTest();
        usersTest.UsersId=userID;
        usersTest.TestId=id;
        usersTest.DateSubmited=DateTime.Now;
        int correctAns=0;
        foreach(var item in answers.ToList())
        {
            if(_context.answers.Any(c=>c.Id==item&&c.IsCorrect==1))
            {
                correctAns++;
            }
        }                                                                                                    
        usersTest.correctAnswer=correctAns;
        TempData["id"]=JsonConvert.SerializeObject(id);
        TempData["answers"]=JsonConvert.SerializeObject(answers);
        TempData["usersTest"]=JsonConvert.SerializeObject(usersTest);
        // await _context.usersTests.AddAsync(usersTest);
        // await _context.SaveChangesAsync();
        // ViewData["usersTest"]=usersTest;
        // return View(answers.ToList());
        return RedirectToAction("SubmitTestResult");
    }
    
    public async Task<IActionResult> SubmitTestResult()
    {
        if (TempData["id"] == null || TempData["answers"] == null || TempData["usersTest"] == null)
        {
            return RedirectToAction("NotFound", "Home");
        }
        var answers=JsonConvert.DeserializeObject<List<int>>(TempData["answers"] as string);
        var usersTest=JsonConvert.DeserializeObject<UsersTest>(TempData["usersTest"] as string);
        var id=JsonConvert.DeserializeObject<int>(TempData["id"] as string);
        await _context.usersTests.AddAsync(usersTest);
        await _context.SaveChangesAsync();
        ViewData["usersTest"]=usersTest;
        ViewData["id"]=id;
        return View(answers.ToList());
    }
}

    
