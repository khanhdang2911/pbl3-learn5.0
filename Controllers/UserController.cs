using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PBL3_Course.Models;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.ComponentModel.DataAnnotations;
namespace PBL3_Course.Controllers;

public class UserController : Controller
{

    public static string EmailEnter{set;get;}
    public static int _randomCode{set;get;}
    private readonly ILogger<UserController> _logger;
    private readonly HashPasswordByBC _hashPasswordByBC;
    private readonly AppDbContext _context;
    public UserController(ILogger<UserController> logger,AppDbContext context,HashPasswordByBC hashPasswordByBC)
    {
        _context=context;
        _logger = logger;
        _hashPasswordByBC=hashPasswordByBC;
    }

    public IActionResult Index()
    {
        return RedirectToAction("NotFound","Home");
    }
    public IActionResult Forbidden()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register([Bind("Email,Password,Name,Phone")] Users users, string ConfirmPassword)
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
        if(string.IsNullOrEmpty(ConfirmPassword)==true)
        {
            ModelState.AddModelError("","Bạn chưa nhập lại mật khẩu");
            
        }
        if(users.Password!=ConfirmPassword)
        {
            ModelState.AddModelError("","Nhập lại mật khẩu không chính xác");
        }
        if(ModelState.ErrorCount>=1)
        {
            return View();
        }
        users.Password=_hashPasswordByBC.HashPassword(users.Password);
        _context.users.Add(users);
        _context.SaveChanges();
        return RedirectToAction("Login","User");
    }
    [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string Email,string Password)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            var kq=_context.users.Where(u=>u.Email==Email).FirstOrDefault();
            if(kq==null)
            {
                ModelState.AddModelError("","Sai thông tin đăng nhập");
                return View();
            }
            if(kq!=null)
            {
                if(_hashPasswordByBC.VerifyPassword(Password,kq.Password)==false)
                {
                    ModelState.AddModelError("","Sai mật khẩu");
                    return View();
                }
            }

            List<Claim> claims=new List<Claim>();
            
            var CheckRoleNameForUser=(from u in _context.users join r in _context.usersRoles on kq.Id equals r.UsersId select r).ToList();
            if(CheckRoleNameForUser.Count==0)
            {
                // claims.Add(new Claim(ClaimTypes.Role,"Guest"));
                // Role role=new Role();
                // var checkRoleExists=_context.roles.Where(r=>r.RoleName=="Guest").FirstOrDefault();
                // if(checkRoleExists==null)
                // {
                //     role.RoleName="Guest";
                //     _context.roles.Add(role);
                //     _context.SaveChanges();
                // }
                UsersRole roleUsers=new UsersRole();
                roleUsers.UsersId=kq.Id;
                roleUsers.RoleId=_context.roles.Where(r=>r.RoleName=="Student").Select(r=>r.Id).FirstOrDefault();

                _context.usersRoles.Add(roleUsers);
                _context.SaveChanges();

            }
            else
            {
                foreach(var item in CheckRoleNameForUser)
                {
                    var rolename=_context.roles.Where(r=>r.Id==item.RoleId).Select(r=>r.RoleName).FirstOrDefault();
                    claims.Add(new Claim(ClaimTypes.Role,rolename));
                }
            }
            claims.Add(new Claim("Id",kq.Id+""));
            var claimIdentity=new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal=new ClaimsPrincipal(claimIdentity);
            await HttpContext.SignInAsync(claimPrincipal);
            return RedirectToAction("Index","Home");
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index","Home");
        }
        [HttpGet]
         public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPasswordConfirmCode(string code)
        {
            if(string.IsNullOrEmpty(code))
            {
                ModelState.AddModelError("","Phải nhập code để xác nhận");
                return View();
            }
            else if(code!=_randomCode.ToString())
            {
                Console.WriteLine(code+"  "+_randomCode);
                ModelState.AddModelError("","Mã xác nhận bạn nhập không đúng, vui lòng kiểm tra lại");
                return View();
            }
            var user=_context.users.Where(u=>u.Email==EmailEnter).FirstOrDefault();
            return RedirectToAction("EnterNewPassword",new {UsersId=user.Id});
        }
        [HttpGet]
        public IActionResult EnterNewPassword(int UsersId)
        {
            Users kq=_context.users.Where(u=>u.Id==UsersId).First();
            if(kq==null)
            {
                return RedirectToAction("NotFound","Home");
            }
            return View(kq);
        }
        [HttpPost]
        public async Task<IActionResult> EnterNewPassword([FromForm]string newpassword,[FromForm]string newpasswordConfirm,[FromQuery]int UsersId)
        {
            Users kq=(from u in _context.users where u.Id==UsersId select u).FirstOrDefault();
            Console.WriteLine(newpassword+"  "+newpasswordConfirm);
            if(string.IsNullOrEmpty(newpassword))
            {
                ModelState.AddModelError("","Bạn chưa nhập mật khẩu mới");
                return View(kq);
            }

            if(string.IsNullOrEmpty(newpasswordConfirm))
            {
                ModelState.AddModelError("","Bạn chưa xác nhận lại mật khẩu mới");
                return View(kq);
            }
            if(newpassword!=newpasswordConfirm)
            {
                ModelState.AddModelError("","Nhập lại mật khẩu chưa chính xác");
                return View(kq);
            }
            
            _context.Entry(kq).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
            kq.Password=_hashPasswordByBC.HashPassword(newpassword);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }
        [HttpPost]
        public IActionResult ForgotPassword(string? Email)
        {
            if(string.IsNullOrEmpty(Email))
            {
                ModelState.AddModelError("","Phải nhập Email của bạn");
                return View();
            }
            var user=_context.users.Where(u=>u.Email==Email).FirstOrDefault();
            if(user==null)
            {
                ModelState.AddModelError("","Email không tồn tại");
                return View();
            }
            EmailEnter=Email;
            Random rnd = new Random();
            int randomCode=rnd.Next(100000,999999);
            _randomCode=randomCode;
             var email = new MimeMessage();

            email.From.Add(new MailboxAddress("khanhdang", "khanhdang3152@gmail.com"));
            email.To.Add(new MailboxAddress("", $"{Email}"));

            email.Subject = "XÁC NHẬN NGƯỜI DÙNG";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { 
                Text = $"<b>Mã code xác nhận của bạn là: {_randomCode}</b>"};
            Console.WriteLine("Ma code:"+_randomCode);

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, false);

                // Note: only needed if the SMTP server requires authentication
                smtp.Authenticate("khanhdang3152@gmail.com", "tlol kfvg mubs yyyl");

                smtp.Send(email);
                smtp.Disconnect(true);
            }
            Console.WriteLine("da toi buoc nay roi nay");
            
           

            return View("ForgotPasswordConfirmCode",(object)randomCode);
        }


    [HttpGet]
    [Authorize]
    public IActionResult EditUser(int UsersId)
    {
        if(User.IsInRole("Admin"))
        {

        }
        else if(UsersId.ToString()!=User.Claims.FirstOrDefault(c=>c.Type=="Id")?.Value)
        {
            return RedirectToAction("NotFound","Home");
        }


        var kq=_context.users.Where(u=>u.Id==UsersId).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");

        }
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> EditUser(int UsersId,[Bind("Email,Password,Name,Phone")] Users users)
    {
        
        var kq=_context.users.Where(u=>u.Id==UsersId).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        
        if(!ModelState.IsValid)
        {
            return View(kq);
        }


        var checkUserExists1=_context.users.Where(u=>u.Email==users.Email&&u.Id!=UsersId).FirstOrDefault();
        if(checkUserExists1!=null)
        {
            ModelState.AddModelError("","Email đã tồn tại");
            return View(kq);
        }
        var checkUserExists2=_context.users.Where(u=>u.Phone==users.Phone&&u.Id!=UsersId).FirstOrDefault();
        if(checkUserExists2!=null)
        {
            ModelState.AddModelError("","Phone đã tồn tại");
            return View(kq);
        }

        _context.Entry(kq).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
        kq.Email=users.Email;
        kq.Password=users.Password;
        kq.Name=users.Name;
        kq.Phone=users.Phone;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index","Home");
    }
    [Authorize]
    public IActionResult ChangePassword(int id)
    {
        var kq=_context.users.Where(u=>u.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        return View(kq);
    }
    [HttpPost]
    public IActionResult ChangePassword(int id,string oldPassword,string newpassword,string newpasswordConfirm)
    {
        var kq=_context.users.Where(u=>u.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        if(string.IsNullOrWhiteSpace(oldPassword)||string.IsNullOrWhiteSpace(newpassword)||string.IsNullOrWhiteSpace(newpasswordConfirm))
        {
            ModelState.AddModelError("","Bạn chưa nhập đủ thông tin, xin hãy nhập lại");
            return View(kq);
        }
        if(_hashPasswordByBC.VerifyPassword(oldPassword,kq.Password)==false)
        {
            ModelState.AddModelError("","Mật khẩu cũ không chính xác");
            
        }
        
        if(newpassword!=newpasswordConfirm)
        {
            ModelState.AddModelError("","Nhập lại mật khẩu không chính xác");
            
        }
        if(_hashPasswordByBC.VerifyPassword(newpassword,kq.Password))
        {
            ModelState.AddModelError("","Mật khẩu mới không được trùng mật khẩu cũ");
            
        }
        if(ModelState.ErrorCount>0)
        {
            return View(kq);
        }
        _context.Entry(kq).State=Microsoft.EntityFrameworkCore.EntityState.Modified;
        kq.Password=_hashPasswordByBC.HashPassword(newpassword);
        _context.SaveChanges();
        return RedirectToAction("EditUser",new {UsersId=id});
    }
    //Dang ki giang day
    public IActionResult TeacherRegister()
    {
        return RedirectToAction("Create","Course");
    }
    //
    public IActionResult CourseManageForTeacher()
    {
        return View();
    }

    //Thong ke cho teacher
    
    

    
}
