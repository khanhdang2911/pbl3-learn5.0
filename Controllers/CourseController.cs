// using System.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PBL3_Course.Models;
using PBL3_Course.Services;

namespace PBL3_Course.Controllers;
[Authorize]
public class CourseController : Controller
{
    private readonly IWebHostEnvironment _environment;

    private readonly AppDbContext _context;
    private readonly IVnPayServices _vnPayServices;
    private readonly ILogger<CourseController> _logger;

    public CourseController(ILogger<CourseController> logger,AppDbContext context,IWebHostEnvironment environment,IVnPayServices vnPayServices)
    {
        _logger = logger;
        _context=context;
        _environment=environment;
        _vnPayServices=vnPayServices;
        
    }
    //Quản lí khóa học(admin)
    [Authorize(Roles ="Admin")]
    public IActionResult Index()
    {
        var allCourse=_context.courses.Where(c=>c.IsActive==1).Include(c=>c.Category).ToList();
        return View(allCourse);
    }
    [Authorize(Roles ="Admin")]
    public IActionResult IndexForCourseNotActive()
    {
        var allCourse=_context.courses.Where(c=>c.IsActive==0).Include(c=>c.Category).ToList();
        return View("Index",allCourse);
    }
    [HttpGet]
    public IActionResult Create()
    {
        SelectList categoryList=new SelectList(_context.categories,"Id","CategoryName");
        ViewData["categoryList"]=categoryList;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create([Bind("CourseName,Description,ImageFile,status,Price,CategoryId")] Course course)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        if(course.ImageFile!=null)
        {
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",course.ImageFile.FileName);
            if(!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream=new FileStream(filepath,FileMode.Create);
                course.ImageFile.CopyTo(fileStream);
            }
            course.CourseImageLink=$"uploads/{course.ImageFile.FileName}";
        }
        if(User.IsInRole("Admin"))
        {
            course.IsActive=1;
        }
        var UserId=User.Claims.FirstOrDefault(c=>c.Type=="Id")?.Value;
        course.TeacherId=int.Parse(UserId);
        course.DateCreated=DateTime.Now;
        await _context.courses.AddAsync(course);
        await _context.SaveChangesAsync();
        if(User.IsInRole("Admin")==false)
        {
            return RedirectToAction("Index","Home");
        }
        return RedirectToAction("Index");
    }
    [AllowAnonymous]
    public IActionResult Detail(int? id)
    {
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        if(kq.IsActive==0)
        {
            return RedirectToAction("NotFound","Home");
        }
        _context.Entry(kq).Collection(c=>c.chapters).Load();
        if(kq.chapters!=null)
        {
            foreach(var item in kq.chapters)
            {
                _context.Entry(item).Collection(i=>i.lessons).Load();
            }
        }
        
        
        return View(kq);
    }
    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }

        SelectList categoryList=new SelectList(_context.categories,"Id","CategoryName");
        ViewData["categoryList"]=categoryList;
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        int userId=Int32.Parse(User.Claims.First(c => c.Type == "Id").Value);
        if((User.IsInRole("Teacher")&&kq.TeacherId==userId)==false)
        {
            return RedirectToAction("NotFound","Home");
        }
        return View(kq);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int? id,[Bind("CourseName,Description,ImageFile,status,Price,CategoryId")] Course course)
    {
        if(!ModelState.IsValid)
        {
            return View();
        }
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        int userId=Int32.Parse(User.Claims.First(c => c.Type == "Id").Value);
        if((User.IsInRole("Teacher")&&kq.TeacherId==userId)==false)
        {
            return RedirectToAction("NotFound","Home");
        }
        // Xoa file cu di
            // if(string.IsNullOrEmpty(kq.CourseImageLink)==false)
            // {
            //     Console.WriteLine("Hello go to here");
            //     var FolderName="wwwroot/uploads";
            //     string fileNameDelete=kq.CourseImageLink.Substring(8);
            //     Console.WriteLine(FolderName+ fileNameDelete);
            //     DirectoryInfo dir = new DirectoryInfo(FolderName);

            //     foreach(FileInfo fi in dir.GetFiles())
            //     {
            //         if(fi.Name==fileNameDelete)
            //         {
            //             fi.Delete();
            //             break;
            //         }
            //     }
            // }
        // end
        if(course.ImageFile!=null)
        {
            var filepath=Path.Combine(_environment.WebRootPath,"uploads",course.ImageFile.FileName);
            if(!System.IO.File.Exists(filepath))
            {
                using FileStream fileStream=new FileStream(filepath,FileMode.Create);
                course.ImageFile.CopyTo(fileStream);
            }

            course.CourseImageLink=$"uploads/{course.ImageFile.FileName}";
        }
        kq.CategoryId=course.CategoryId;
        kq.CourseName=course.CourseName;
        kq.Price=course.Price;
        kq.DateEdited=DateTime.Now;
        kq.Description=course.Description;
        kq.status=course.status;
        kq.CourseImageLink=course.CourseImageLink;
        _context.Entry(kq).State=EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Search(string courseName)
    {
        var query=_context.courses;
        if(string.IsNullOrWhiteSpace(courseName))
        {
            var AllCourse=await query.ToListAsync();
            return View("AllCourse",AllCourse);
        }
        var kq=_context.courses.Where(c=>c.CourseName.Contains(courseName)).ToList();
        return View("AllCourse",kq);
    }
    [Authorize(Roles ="Admin")]
    public IActionResult SearchForAdmin(string courseName)
    {
         var kq=_context.courses.Where(c=>c.CourseName.Contains(courseName)).ToList();
        return View("Index",kq);
    }
    public IActionResult Delete(int? id)
    {
        if(id==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return RedirectToAction("NotFound","Home");
        }

        int userId=Int32.Parse(User.Claims.First(c => c.Type == "Id").Value);
        if((User.IsInRole("Teacher")&&kq.TeacherId==userId)==false)
        {
            return RedirectToAction("NotFound","Home");
        }
        _context.courses.Remove(kq);
        _context.SaveChanges();
        //Kiem tra xem user còn đang dạy khóa nào hay không
        bool checkExistCourseWithTeacherId=_context.courses.Any(c=>c.TeacherId==kq.TeacherId);
        //Lấy ID của teacher trong database
        int TeacherId=_context.roles.Where(r=>r.RoleName=="Teacher").Select(r=>r.Id).FirstOrDefault();
        if(checkExistCourseWithTeacherId==false)
        {
            var deleteRoleTeacher=_context.usersRoles.Where(c=>c.UsersId==kq.TeacherId&&c.RoleId==TeacherId).FirstOrDefault();
            //Neu ton tai thi xoa role teacher cho user do
            if(deleteRoleTeacher!=null)
            {
                _context.usersRoles.Remove(deleteRoleTeacher);
            }
        }
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    [AllowAnonymous]
    public IActionResult AllCourse(int?id,string sortBy)
    {
        List<Course> allCourse=new List<Course>();
        if(id==null)
        {
            allCourse=_context.courses.ToList();
            //sort
            if(sortBy=="price1")
            {
                allCourse=_context.courses.OrderByDescending(c=>c.Price).ToList();
            }
            else if(sortBy=="price2")
            {
                allCourse=_context.courses.OrderBy(c=>c.Price).ToList();
            }
            allCourse=allCourse.Where(c=>c.IsActive==1).ToList();
            return View(allCourse);
        }
        //Neu id khac null
        allCourse=_context.courses.Where(c=>c.CategoryId==id).ToList();
        if(_context.categories.Any(c=>c.Id==id)==false)
        {
            return RedirectToAction("NotFound","Home");
        }
        //sort
        if(sortBy=="name")
        {
            allCourse=_context.courses.OrderBy(c=>c.CourseName).ToList();
        }
        allCourse=allCourse.Where(c=>c.IsActive==1).ToList();
        return View(allCourse);
    }
    public IActionResult CourseActivate(int id)
    {
        var kq=_context.courses.Where(c=>c.Id==id).FirstOrDefault();
        if(kq==null)
        {
            return  RedirectToAction("NotFound","Home");
        }
        _context.Entry(kq).State=EntityState.Modified;
        kq.IsActive=1;
        //Lay ra id cua user hien tai
        int UserId=kq.TeacherId;
        //Tim Teacher Id Trong role
        UsersRole usersRole=new UsersRole();
        var RoleTeacherID=_context.roles.Where(r=>r.RoleName=="Teacher").Select(r=>r.Id).FirstOrDefault();
        
        //Kiem tra nguoi dung hien tai da co role teacher hay chua
        bool checkAddUserRole=_context.usersRoles.Any(u=>u.RoleId==RoleTeacherID&& u.UsersId==UserId);
        if(checkAddUserRole==false)
        {
            usersRole.UsersId=UserId;
            usersRole.RoleId=RoleTeacherID;
            _context.usersRoles.Add(usersRole);
        }
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    public IActionResult Checkout(int? courseId)
    {
        if(courseId==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        bool checkCourse=_context.courses.Any(c => c.Id==courseId);
        if(checkCourse==false)
        {
            return  RedirectToAction("NotFound","Home");
        }
        ViewData["courseId"]=courseId;
        return View();
    }
    public IActionResult CourseEnroll(int courseId)
    {
        if(User.Identity.IsAuthenticated==false)
        {
            return RedirectToAction("Login","User");
        }
        bool checkCourse=_context.courses.Any(c => c.Id==courseId);
        if(checkCourse==false)
        {
            return RedirectToAction("NotFound","Home");
        }
        int userId=Int32.Parse(User.Claims.First(c => c.Type == "Id").Value);
        if(_context.usersCourses.Any(u=>u.UsersId==userId&&u.CourseId==courseId))
        {
            TempData["Message"]="Bạn đã đăng kí khóa học này rồi";
            return RedirectToAction("NotFound","Home");
        }
        UsersCourse usersCourse=new UsersCourse();
        usersCourse.CourseId=courseId;
        usersCourse.UsersId=userId;
        _context.usersCourses.Add(usersCourse);
        _context.SaveChanges();
        return RedirectToAction("Index","Home");
    }
    public IActionResult CheckoutVNPay(int courseId)
    {
        
        if(User.Identity.IsAuthenticated==false)
        {
            return RedirectToAction("Login","User");
        }
        if(User.IsInRole("Admin"))
        {
            TempData["Message"]="Bạn là Admin mà, bạn có quyền xem mà không cần phải mua khóa học này";
            return RedirectToAction("NotFound","Home");
        }
        var checkCourse=_context.courses.Where(c => c.Id==courseId).FirstOrDefault();
        if(checkCourse==null)
        {
            return RedirectToAction("NotFound","Home");
        }
        int userId=Int32.Parse(User.Claims.First(c => c.Type == "Id").Value);
        //Kiem tra neu khoa học hiện tại của user hiện tại đang đăng nhập thì không cần phải mua
        if(checkCourse.TeacherId==userId)
        {
            TempData["Message"]="Bạn là chủ sở hữu của khóa học này mà, bạn có quyền xem mà không cần phải mua khóa học này!";
            return RedirectToAction("NotFound","Home");
        }
        if(checkCourse.Price==0)
        {
            TempData["Message"]="Because this course is free, you do not need to pay.";
            UsersCourse usersCourse=new UsersCourse();
            usersCourse.UsersId=userId;
            usersCourse.CourseId=courseId;
            _context.usersCourses.Add(usersCourse);
            _context.SaveChanges();
            return RedirectToAction("Success","Home");
        }
        if(_context.usersCourses.Any(u=>u.UsersId==userId&&u.CourseId==courseId))
        {
            TempData["Message"]="Bạn đã mua khóa học này rồi đó, không cần mua lại nhé!";
            return RedirectToAction("NotFound","Home");
        }

        var vnPayModel = new VnPaymentRequestModel
					{
						Amount = (double)checkCourse.Price*23000,
						CreatedDate = DateTime.Now,
						Description = $"",
						FullName = "",
						OrderId = new Random().Next(1000, 100000),
                        courseId=courseId,
					};
        // Serialize vnPayModel thành chuỗi JSON
        var vnPayModelJson = JsonConvert.SerializeObject(vnPayModel);

    // Lưu chuỗi JSON trong TempData
        TempData["vnPayModel"] = vnPayModelJson;

        return Redirect(_vnPayServices.CreatePaymentUrl(HttpContext,vnPayModel));
    }
    public IActionResult PaymentCallBack()
		{
			var response = _vnPayServices.PaymentExecute(Request.Query);

			if (response == null || response.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
				return RedirectToAction("NotFound","Home");
			}

            var vnPayModelJson = TempData["vnPayModel"] as string;
            var vnPayModel = JsonConvert.DeserializeObject<VnPaymentRequestModel>(vnPayModelJson);


            //User cua id hien tai dang dang nhap
            int userId=Int32.Parse(User.Claims.First(c => c.Type == "Id").Value);

            if (vnPayModel != null)
            {
                // Lưu đơn hàng vô database
                Order order=new Order();
                order.UserId=userId;
                order.courseId=vnPayModel.courseId;
                order.TotalMoney=vnPayModel.Amount;
                order.DateCreated=vnPayModel.CreatedDate;
                _context.orders.Add(order);


                //Kich hoat khoa hoc cho user o day
                UsersCourse usersCourse=new UsersCourse();
                usersCourse.CourseId=vnPayModel.courseId;
                usersCourse.UsersId=userId;
                _context.usersCourses.Add(usersCourse);
            }
            
            _context.SaveChanges();
			

			TempData["Message"] = $"Thanh toán VNPay thành công";
			return RedirectToAction("Success","Home");
		}


        //Comment
        [HttpPost]
        public async Task<IActionResult> Comment([FromQuery]int courseId, string commentText, int rate)
        {
            int userId=Int32.Parse(User.Claims.First(c => c.Type == "Id").Value);
            bool checkUserComment=((_context.usersCourses.Any(u=>u.UsersId==userId&&u.CourseId==courseId))&&(_context.comments.Any(c=>c.UserId==userId&&c.CourseId==courseId))==false);
            if(checkUserComment==false)
            {
                return RedirectToAction("NotFound","Home");
            }
            Comment comment=new Comment();
            comment.UserId=userId;
            comment.CourseId=courseId;
            comment.CommentText=commentText;
            comment.DateComment=DateTime.Now;

            //Cap nhat rate
            var usersCourse=_context.usersCourses.Where(u=>u.CourseId==courseId&&u.UsersId==userId).First();
            _context.Entry(usersCourse).State=EntityState.Modified;
            usersCourse.rate=rate;

            await _context.comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail","Course",new {id=courseId});
        }
        // Search Course Admin
        [HttpPost]
        public IActionResult SearchCourse(string courseName)
        {
            var kq=_context.courses.Where(c=>c.CourseName.Contains(courseName)&&c.IsActive==1).Include(c=>c.Category).ToList();
            return View("Index",kq);
        }
}
