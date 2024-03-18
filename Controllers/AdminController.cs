using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PBL3_Course.Models;

namespace PBL3_Course.Controllers;
[Authorize(Roles ="Admin")]
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
        foreach(var item in kq)
        {
            _context.Entry(item).Collection(r=>r.usersRoles).Load();
        }
        return View(kq);
    }
    public IActionResult RoleManage()
    {
        var kq=(from r in _context.roles select r).ToList();
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
        var checkUserExists1=_context.users.Where(u=>u.Email==users.Email).FirstOrDefault();
        if(checkUserExists1!=null)
        {
            ModelState.AddModelError("","Email đã tồn tại");
        }
        var checkUserExists2=_context.users.Where(u=>u.Phone==users.Phone).FirstOrDefault();
        if(checkUserExists2!=null)
        {
            ModelState.AddModelError("","Phone đã tồn tại");
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
    public IActionResult AddRole()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AddRole([Bind("RoleName")] Role role)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        await _context.roles.AddAsync(role);
        await _context.SaveChangesAsync();
        return RedirectToAction("RoleManage");
    }
    public IActionResult DeleteRole(int id)
    {
        if(id==null)
        {
            return Content("Khong tim thay role");
        }
        var kq=_context.roles.Where(r=>r.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Khong tim thay Role");
        }
        _context.roles.Remove(kq);
        _context.SaveChanges();
        return RedirectToAction("RoleManage");
    }
    public IActionResult EditRole(int id)
    {
        var kq=_context.roles.Where(r=>r.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Không tìm thấy User để chỉnh sửa");
        }
        return View(kq);
    }
    [HttpPost]
    public IActionResult EditRole(int id,[Bind("RoleName")] Role role)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.roles.Where(r=>r.Id==id).FirstOrDefault();
        _context.Entry(kq).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
        kq.RoleName=role.RoleName;
        _context.SaveChanges();
        return RedirectToAction("RoleManage");
    }
    [HttpGet]
    public IActionResult AddRoleForUser(int id)
    {
        List<RoleCheckbox> RoleList=new List<RoleCheckbox>();
        foreach(var item in _context.roles)
        {
            RoleList.Add(new RoleCheckbox(){Id=item.Id,RoleName=item.RoleName,IsChecked=false});
        }
        Users user=_context.users.Where(u=>u.Id==id).FirstOrDefault();
        if(user==null)
        {
            return Content("Không tìm thấy User để thêm role");
        }
        ViewData["user"]=user;
        return View(RoleList);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRoleForUser(int id,[Bind]List<RoleCheckbox> RoleList)
    {
        var kq=_context.users.Where(u=>u.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return Content("Không tìm thấy user");
        }
        RoleList=RoleList.Where(r=>r.IsChecked==true).ToList();
        
        var userRoleList=_context.usersRoles.Where(r=>r.UsersId==id).ToList();
        foreach(var deleteItem in userRoleList)
        {
            if(RoleList.Any(r=>r.Id==deleteItem.RoleId)==false)
            {
                _context.usersRoles.Remove(deleteItem);
            }
        }
        await _context.SaveChangesAsync();
        foreach(var addItem in RoleList)
        {
            if(userRoleList.Any(ur=>ur.RoleId==addItem.Id)==false)
            {
                UsersRole usersRole=new UsersRole(){UsersId=id,RoleId=addItem.Id};
                await _context.usersRoles.AddAsync(usersRole);
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
