// using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;

public class AnswerController : Controller
{
    
    private readonly AppDbContext _context;
    public AnswerController(AppDbContext context)
    {
        _context=context;
    }

    [HttpGet]
    public IActionResult Create(int QuestionId)
    {
        var kq=_context.questions.Any(t=>t.Id==QuestionId);
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create([Bind("AnswerText,IsCorrect")] Answer answer)
    {

        if(!ModelState.IsValid)
        {
            ModelState.AddModelError("","Tạo không thành công");
            return View();
        }
        await _context.answers.AddAsync(answer);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        var kq=_context.tests.Where(c=>c.Id==id).FirstOrDefault();
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("AnswerText,IsCorrect")] Answer answer)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.answers.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return NotFound();
        }

        kq.AnswerText=answer.AnswerText;
        kq.IsCorrect=answer.IsCorrect;
        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    public IActionResult Delete(int? id)
    {
        var kq=_context.answers.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        _context.answers.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    




}
