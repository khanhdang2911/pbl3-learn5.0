// using System.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Course.Models;
using Microsoft.AspNetCore.Authorization;

namespace PBL3_Course.Controllers;
[Authorize]
public class QuestionController : Controller
{
    private readonly AppDbContext _context;
    public QuestionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create(int? TestId)
    {
        if (TestId == null)
        {
            return RedirectToAction("NotFound", "Home");
        }
        ViewData["TestId"] = TestId;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(int? TestId, [FromForm] List<Question> questions, [FromForm] List<string> answerText, [FromForm] List<int> answerCorrect)
    {
        if (!ModelState.IsValid)
        {
            // ModelState.AddModelError("", "Tạo không thành công");
            ViewData["TestId"] = TestId;
            return View();
        }
        if (TestId == null)
        {
            return RedirectToAction("NotFound", "Home");
        }
        List<Answer> answers = new List<Answer>();
        int i = 0;
        foreach (var item in questions)
        {
            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
            for (int j = i; j < i + 4; j++)
            {
                Answer ans = new Answer();
                ans.AnswerText = answerText[j];
                ans.IsCorrect = 0;
                int QuestionId = _context.questions.Where(q => q.QuestionName == item.QuestionName).Select(q => q.Id).FirstOrDefault();
                ans.QuestionId = QuestionId;

                if (j % 4 == answerCorrect[j / 4])
                {
                    ans.IsCorrect = 1;
                }
                await _context.answers.AddAsync(ans);
            }
            i += 4;
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Edit(int? TestId)
    {
        if (TestId == null)
        {
            return RedirectToAction("NotFound", "Home");
        }
        bool checkExits = _context.tests.Any(t => t.Id == TestId);
        ViewData["TestId"] = TestId;
        if (checkExits == false)
        {
            return RedirectToAction("NotFound", "Home");
        }
        var kq = _context.tests.Where(t => t.Id == TestId).Include(t => t.Questions).ThenInclude(t => t.Answers).FirstOrDefault();

        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? TestId, [FromForm] List<Question> questions, [FromForm] List<string> answerText, [FromForm] List<int> answerCorrect)
    {
        Console.WriteLine("da vao day roi");
        if (!ModelState.IsValid)
        {
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }
            ModelState.AddModelError("", "Creation failed, not enough information was entered");
            ViewData["TestId"] = TestId;//khong can cung duoc vi co model truyen qua
            var kq = _context.tests.Where(t => t.Id == TestId).Include(t => t.Questions).ThenInclude(t => t.Answers).FirstOrDefault();
            return View(kq);
        }
        if (TestId == null)
        {
            return RedirectToAction("NotFound", "Home");
        }
        List<Answer> answers = new List<Answer>();
        int i = 0;
        int k = 0;
        // var questionsOfTest = _context.questions.Where(q => q.TestId == TestId).ToList();
        foreach (var item in questions)
        {
            if (_context.questions.Any(q => q.Id == item.Id) == true)
            {
                _context.Entry(item).State = EntityState.Modified;
            }
            item.QuestionName = questions[k].QuestionName;
            //Neu chua cho question thi them vao
            if (_context.questions.Any(q => q.Id == item.Id) == false)
            {
                _context.questions.Add(item);
            }
            await _context.SaveChangesAsync();
            await _context.Entry(item).Collection(q => q.Answers).LoadAsync();
            if (item.Answers.Count==4)
            {
                foreach (var ans in item.Answers)
                {
                    if (_context.answers.Any(q => q.Id == ans.Id) == true)
                    {
                        _context.Entry(ans).State = EntityState.Modified;
                    }
                    ans.IsCorrect = 0;
                    ans.AnswerText = answerText[i];
                    if (i % 4 == answerCorrect[k])
                    {
                        ans.IsCorrect = 1;
                    }
                    i++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                
                for(int temp=1;temp<=4;temp++)
                {
                    Answer ans = new Answer();
                    ans.IsCorrect = 0;
                    ans.AnswerText = answerText[i];
                    ans.QuestionId=item.Id;
                    if (i % 4 == answerCorrect[k])
                    {
                        ans.IsCorrect = 1;
                    }
                    //Neu chua co thi them vao
                    _context.answers.Add(ans);
                    i++;
                }
                await _context.SaveChangesAsync();
            }
            k++;
        }
        await _context.SaveChangesAsync();
        int courseId = _context.tests.Where(c => c.Id == TestId).Select(c => c.CourseId).First();
        return RedirectToAction("Detail", "Course", new { id = courseId });
    }
    public IActionResult Delete(int? id)
    {
        var kq = _context.tests.Where(c => c.Id == id).FirstOrDefault();
        if (kq == null)
        {
            return RedirectToAction("NotFound", "Home");
        }
        _context.tests.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
