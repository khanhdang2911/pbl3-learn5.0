// using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;

public class QuestionController : Controller
{
    
    private readonly AppDbContext _context;
    public QuestionController(AppDbContext context)
    {
        _context=context;
    }

    [HttpGet]
    public IActionResult Create(int? TestId)
    {
        if(TestId==null)
        {
            return Content("Test ID khong hop le");
        }
        ViewData["TestId"]=TestId;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(int? TestId,[FromForm] List<Question> questions,[FromForm]List<string> answerText,[FromForm]List<int>answerCorrect)
    {
        if(!ModelState.IsValid)
        {
            ModelState.AddModelError("","Tạo không thành công");
            ViewData["TestId"]=TestId;
            return View();
        }
        List<Answer> answers=new List<Answer>();
        int i=0;
        foreach(var item in questions)
        {
            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
            for(int j=i;j<i+4;j++)
            {
                Answer ans=new Answer();
                ans.AnswerText=answerText[j];
                ans.IsCorrect=0;
                int QuestionId=_context.questions.Where(q=>q.QuestionName==item.QuestionName).Select(q=>q.Id).FirstOrDefault();
                ans.QuestionId=QuestionId;
                
                if(j%4==answerCorrect[j/4])
                {
                    ans.IsCorrect=1;
                }
                await _context.answers.AddAsync(ans);
            }
            i+=4;
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index","Home");
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        var kq=_context.questions.Where(c=>c.Id==id).FirstOrDefault();
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("QuestionName,TestId")] Question question)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.questions.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return NotFound();
        }

        kq.QuestionName=question.QuestionName;
        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    public IActionResult Delete(int? id)
    {
        var kq=_context.tests.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay danh muc");
        }
        _context.tests.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
