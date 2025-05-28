using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;

namespace RenderMentor.Areas.Student.Controllers
{
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_User_Individual)]
    [Area("Student")]
    public class StudentController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<StudentVM> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly EmailOptions emailOptions;

        [BindProperty]
        public OrderDetailsVM OrderVM { get; set; }

        public StudentController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<StudentVM> logger,
            IUnitOfWork unitOfWork,
            IEmailSender emailSender,
            IOptions<EmailOptions> options)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            emailOptions = options.Value;
        }

        public IActionResult Index()
        {
            var studentIndexVM = new StudentIndexVM();
            var studentCoursesVM = new List<StudentCourseVM>();
            var trialStatus = TrialStatus.InActive;
            var packageStatus = PackageStatus.InActive;
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var studentCourses = _unitOfWork.Course.GetAll(includeProperties: "CourseCategory");

            if (User.IsInRole(SD.Role_Admin))
            {
                // Admin kullanıcıları için özel işlemler
                packageStatus = PackageStatus.Active;
                studentIndexVM.PackageActive = true;
                studentIndexVM.PackageName = "Yönetici Hesabı";
                studentIndexVM.PackageDaysToExpire = 999; // Admin hesapları için süresiz erişim
            }
            else if (User.IsInRole(SD.Role_User_Individual))
            {
                var userStudent = _unitOfWork.Student.GetFirstOrDefault(i => i.UserId == userId);
                if (userStudent == null)
                {
                    return NotFound("Öğrenci kaydı bulunamadı. Lütfen yönetici ile iletişime geçin.");
                }

                var studentId = userStudent.Id;
                var packageActive = userStudent.SubscribeActive;
                packageStatus = PackageStatus.Ready;
                var homeContent = _unitOfWork.HomeContent.GetFirstOrDefault();
                var trialOnline = homeContent?.TrialOnline ?? false;

                if (packageActive)
                {
                    if (userStudent.SubscribeStarted)
                    {
                        packageStatus = PackageStatus.Started;
                        userStudent.SubscribeStarted = false;
                        _unitOfWork.Save();
                    }
                    else
                    {
                        packageStatus = PackageStatus.Active;
                    }
                    studentIndexVM.PackageActive = true;
                    DateTime packageExpireDate = userStudent.SubscribeExpireDate;
                    studentIndexVM.PackageDaysToExpire = Math.Round((packageExpireDate - DateTime.Now).TotalDays);
                    
                    var subscription = _unitOfWork.Subscription.GetFirstOrDefault(i => i.Id == 1);
                    studentIndexVM.PackageName = subscription?.Name ?? "Standart Paket";

                    if (packageExpireDate <= DateTime.Now)
                    {
                        packageStatus = PackageStatus.Completed;
                        userStudent.SubscribeActive = false;
                        _unitOfWork.Save();
                        studentIndexVM.PackageActive = false;
                        studentCourses = _unitOfWork.StudentCourse.GetAll()
                            .Where(i => i.StudentId == studentId)
                            .Select(c => c.Course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == c.CourseId, includeProperties: "CourseCategory"))
                            .OrderBy(i => i.ListOrder);
                    }
                    else
                    {
                        studentCourses = _unitOfWork.Course.GetAll(includeProperties: "CourseCategory").Where(i => i.OnSale == true).OrderBy(i => i.ListOrder);
                    }
                }
                else
                {
                    packageStatus = PackageStatus.Ready;
                    studentCourses = _unitOfWork.StudentCourse.GetAll().Where(i => i.StudentId == studentId)
                        .Select(c => c.Course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == c.CourseId, includeProperties: "CourseCategory")).OrderBy(i => i.ListOrder);
                    if (trialOnline)
                    {
                        trialStatus = TrialStatus.Ready;
                    }
                }
                
                var trialMember = _unitOfWork.TrialMember.GetFirstOrDefault(s => s.StudentId == studentId);
                if (trialMember != null)
                {
                    if (trialMember.IsActive)
                    {
                        if (trialMember.Started)
                        {
                            trialStatus = TrialStatus.Started;
                            trialMember.Started = false;
                            _unitOfWork.Save();
                        }
                        else
                        {
                            trialStatus = TrialStatus.Active;
                        }
                        DateTime expireDate = trialMember.StartDate.AddMinutes(120);
                        if (expireDate <= DateTime.Now)
                        {
                            trialStatus = TrialStatus.Completed;
                            trialMember.IsActive = false;
                            _unitOfWork.Save();
                            studentCourses = _unitOfWork.StudentCourse.GetAll().Where(i => i.StudentId == studentId)
                        .Select(c => c.Course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == c.CourseId, includeProperties: "CourseCategory")).OrderBy(i => i.ListOrder);

                        }
                        else
                        {
                            studentCourses = _unitOfWork.Course.GetAll(includeProperties: "CourseCategory").Where(i => i.IsTrial == true).OrderBy(i => i.ListOrder);
                        }
                    }
                    else
                    {
                        trialStatus = TrialStatus.Expired;
                        if (!packageActive)
                        {
                            studentCourses = _unitOfWork.StudentCourse.GetAll().Where(i => i.StudentId == studentId)
                    .Select(c => c.Course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == c.CourseId, includeProperties: "CourseCategory")).OrderBy(i => i.ListOrder);
                        }
                    }
                }
                else
                {
                    if (!packageActive)
                    {
                        studentCourses = _unitOfWork.StudentCourse.GetAll().Where(i => i.StudentId == studentId)
                    .Select(c => c.Course = _unitOfWork.Course.GetFirstOrDefault(x => x.Id == c.CourseId, includeProperties: "CourseCategory")).OrderBy(i => i.ListOrder);
                    }
                }
            }
            else
            {
                studentCourses = studentCourses.OrderBy(i => i.ListOrder);
            }

            foreach (var course in studentCourses)
            {                                
                course.TotalDuration = new TimeSpan(_unitOfWork.CourseSection.GetAll().Where(n => n.CourseId == course.Id)
                    .Select(n => new CourseSection()
                    {
                        TotalDuration = new TimeSpan(_unitOfWork.Lecture.GetAll().Where(x => x.CourseSectionId == n.Id).Sum(x => x.Duration.Ticks))
                    }).Sum(m => m.TotalDuration.Ticks));
                course.TotalLectures = _unitOfWork.CourseSection.GetAll().Where(n => n.CourseId == course.Id)
                    .Select(n => new CourseSection()
                    {
                        TotalLectures = _unitOfWork.Lecture.GetAll().Where(x => x.CourseSectionId == n.Id).Count()
                    }).Sum(x => x.TotalLectures);
                course.Instructor = _unitOfWork.Instructor.GetAll().Where(m => m.Id == course.InstructorId).FirstOrDefault();
                if (course.Instructor == null)
                {
                    course.Instructor = new Instructor()
                    {
                        ApplicationUser = new ApplicationUser()
                        {
                            Name = "Evren Çavuşoğlu"
                        }
                    };
                }
                else
                {
                    course.Instructor = new Instructor()
                    {
                        ApplicationUser = _unitOfWork.ApplicationUser.GetAll().Where(n => n.Id == course.Instructor.UserId)
                            .Select(n => new ApplicationUser()
                            {
                                Name = n.Name
                            }).FirstOrDefault()
                    };
                }
                bool hasReview = false;
                var studentReview = _unitOfWork.CourseReview.GetFirstOrDefault(i => i.CourseId == course.Id && i.UserId == userId);
                if (studentReview != null)
                {
                    hasReview = true;
                }
                studentCoursesVM.Add(new StudentCourseVM() { 
                    Course = course,
                    StudentHasReview = hasReview
                });
            }
            studentIndexVM.StudentCourseVM = studentCoursesVM;
            studentIndexVM.PackageStatus = packageStatus;
            studentIndexVM.TrialStatus = trialStatus;
            return View(studentIndexVM);
        }

        public IActionResult Presentation(int? id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var course = new CourseSectionsLecturesVM()
            {
                Course = _unitOfWork.Course.GetFirstOrDefault(m => m.Id == id),
                CourseSections = _unitOfWork.CourseSection.GetAll().Where(m => m.CourseId == id).OrderBy(n => n.ListOrder)
                    .Select(m => new CourseSection()
                    {
                        Id = m.Id,
                        Title = m.Title,
                        TotalDuration = new TimeSpan(_unitOfWork.Lecture.GetAll().Where(n => n.CourseSectionId == m.Id).Sum(n => n.Duration.Ticks)),
                        Lectures = _unitOfWork.Lecture.GetAll().Where(n => n.CourseSectionId == m.Id).OrderBy(n => n.ListOrder).ToList(),
                        TotalLectures = _unitOfWork.Lecture.GetAll().Where(n => n.CourseSectionId == m.Id).Count(),
                        ListOrder = m.ListOrder
                    })
            };

            if (User.IsInRole(SD.Role_Admin))
            {
                // Admin kullanıcıları için özel işlemler
                course.StudentPackageActive = true;
                course.StudentCourseIDs = _unitOfWork.Course.GetAll().Select(c => c.Id).ToArray();
            }
            else if (User.IsInRole(SD.Role_User_Individual))
            {
                var userStudent = _unitOfWork.Student.GetFirstOrDefault(i => i.UserId == userId);
                if (userStudent == null)
                {
                    return NotFound("Öğrenci kaydı bulunamadı. Lütfen yönetici ile iletişime geçin.");
                }

                // Prevent students jumping to the courses they don't have by changing courseId from URL
                int[] StudentCourseIDs = _unitOfWork.StudentCourse.GetAll().Where(s => s.StudentId == userStudent.Id).Select(c => c.CourseId).ToArray();
                var packageActive = userStudent.SubscribeActive;
                if (packageActive)
                {
                    DateTime packageExpireDate = userStudent.SubscribeExpireDate;
                    if (packageExpireDate <= DateTime.Now)
                    {
                        userStudent.SubscribeActive = false;
                        _unitOfWork.Save();
                    }
                    else
                    {
                        StudentCourseIDs = _unitOfWork.Course.GetAll(includeProperties: "CourseCategory").Where(i => i.OnSale == true).Select(c => c.Id).ToArray();
                    }
                }
                var trialMember = _unitOfWork.TrialMember.GetFirstOrDefault(s => s.StudentId == userStudent.Id);
                if (trialMember != null && trialMember.IsActive)
                {
                    DateTime expireDate = trialMember.StartDate.AddMinutes(120);
                    if (expireDate <= DateTime.Now)
                    {
                        trialMember.IsActive = false;
                        _unitOfWork.Save();
                    }
                    else
                    {
                        StudentCourseIDs = _unitOfWork.Course.GetAll(includeProperties: "CourseCategory").Where(i => i.IsTrial == true).Select(c => c.Id).ToArray();
                    }
                }
                if (!StudentCourseIDs.Contains(course.Course.Id))
                {
                    return RedirectToAction(nameof(Index));
                }

                var studentCourse = _unitOfWork.StudentCourse.GetFirstOrDefault(m => m.CourseId == id && m.StudentId == userStudent.Id);
                if (studentCourse != null)
                {
                    course.WatchingLectureId = studentCourse.WatchingLectureId;
                }
                course.StudentId = userStudent.Id;
            }

            course.Instructor = _unitOfWork.Instructor.GetFirstOrDefault(m => m.Id == course.Course.InstructorId);
            if (course.Instructor == null)
            {
                course.Instructor = new Instructor()
                {
                    ApplicationUser = new ApplicationUser()
                    {
                        Name = "Evren Çavuşoğlu"
                    },
                    AvatarImage = "/images/placeholder/instructor-avatar-image.jpg"
                };
            }
            else
            {
                course.Instructor = _unitOfWork.Instructor.GetAll().Where(i => i.Id == course.Course.InstructorId)
                    .Select(i => new Instructor()
                    {
                        ApplicationUser = _unitOfWork.ApplicationUser.GetAll().Where(n => n.Id == i.UserId)
                            .Select(n => new ApplicationUser()
                            {
                                Name = n.Name
                            }).FirstOrDefault(),
                        AvatarImage = i.AvatarImage,
                        Desc = i.Desc
                    }).FirstOrDefault();
                if (course.Instructor.AvatarImage == null)
                {
                    course.Instructor.AvatarImage = "/images/placeholder/instructor-avatar-image.jpg";
                }
            }

            // InstructorCategories'i ayarla
            course.InstructorCategories = _unitOfWork.InstructorCategory.GetAll().Where(i => i.InstructorId == course.Course.InstructorId);
            if (course.InstructorCategories != null)
            {
                foreach (var item in course.InstructorCategories)
                {
                    item.CourseCategory = _unitOfWork.CourseCategory.GetAll().Where(i => i.Id == item.CourseCategoryId)
                        .Select(i => new CourseCategory()
                        {
                            Name = i.Name
                        }).FirstOrDefault();
                }
            }

            if (id == null)
            {
                // Create
                return View(course);
            }
            // Edit
            course.Course = _unitOfWork.Course.Get(id.GetValueOrDefault());
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        public IActionResult Profile()
        {
            var userData = new StudentVM();
            var userName = _userManager.GetUserName(User);
            var student = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.UserName == userName);
            if (student == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            } else
            {
                userData.UserName = userName;
                userData.Name = student.Name;
                userData.PhoneNumber = student.PhoneNumber;
                userData.Gender = student.Gender;
                return View(userData);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(StudentVM userData)
        {
            var userName = _userManager.GetUserName(User);
            var student = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.UserName == userName);
            if (student == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            if (ModelState.IsValid)
            {
                userData.UserName = userName;
                student.Name = userData.Name;
                student.PhoneNumber = userData.PhoneNumber;
                student.Gender = userData.Gender;
                _unitOfWork.ApplicationUser.Update(student);
                _unitOfWork.Save();
                userData.StatusMessage = "Profil bilgileriniz başarıyla güncellendi.";
            }
            return View(userData);

        }

        public IActionResult Email()
        {
            var userData = new StudentEmailVM();
            var userName = _userManager.GetUserName(User);
            var student = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.UserName == userName);
            if (student == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            } else
            {
                userData.Email = userName;
                userData.NewEmail = userName;
                userData.IsEmailConfirmed = student.EmailConfirmed;
                userData.Name = student.Name;
            }

            return View(userData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Email(StudentEmailVM userData)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var email = await _userManager.GetEmailAsync(user);
                    if (userData.NewEmail != email)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateChangeEmailTokenAsync(user, userData.NewEmail);
                        byte[] codeBytes = Encoding.UTF8.GetBytes(code);
                        var codeEncoded = WebEncoders.Base64UrlEncode(codeBytes);
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmailChange",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, email = userData.NewEmail, code = codeEncoded },
                            protocol: Request.Scheme);
                        await _emailSender.SendEmailAsync(userData.NewEmail, "Hesabınızı onaylayın.",
                        $"Sayın {userData.Name}, lütfen RenderMentor hesabınızı onaylamak için <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>linke</a> tıklayınız.");

                        userData.StatusMessage = "Onay maili gönderildi. Lütfen hesabınızı kontrol edin.";
                        return View(userData);

                    }
                    userData.StatusMessage = "Mailiniz değiştirilmedi.";
                }
                return View(userData);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(StudentEmailVM userData)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            } else
            {
                if (ModelState.IsValid)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    var email = await _userManager.GetEmailAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    byte[] codeBytes = Encoding.UTF8.GetBytes(code);
                    var codeEncoded = WebEncoders.Base64UrlEncode(codeBytes);
                    var callbackUrl = Url.Page(
                        "Identity/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = codeEncoded },
                        protocol: Request.Scheme);
                    await _emailSender.SendEmailAsync(
                        email,
                        "Hesabınızı onaylayın.",
                        $"Sayın {userData.Name}, lütfen RenderMentor hesabınızı onaylamak için <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>linke</a> tıklayınız.");

                    userData.StatusMessage = "Onay maili gönderildi. Lütfen hesabınızı kontrol edin.";
                }
                return View(userData);
            }
        }

        public IActionResult Password()
        {
            var userData = new StudentPasswordVM();
            var userName = _userManager.GetUserName(User);
            var student = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.UserName == userName);
            if (student == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }          

            return View(userData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password(StudentPasswordVM userData)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, userData.OldPassword, userData.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(userData);
                    }
                    await _signInManager.RefreshSignInAsync(user);
                    _logger.LogInformation("User changed their password successfully.");
                    userData.StatusMessage = "Parolanız başarıyla değiştirildi.";
                }
                return View(userData);
            }
        }

        [Route("rememberWatching/{studentId}/{courseId}")]
        public void RememberWatching(int watchingId, int studentId, int courseId)
        {
            if (User.IsInRole(SD.Role_User_Individual))
            {
                var watching = _unitOfWork.StudentCourse.GetFirstOrDefault(m => m.CourseId == courseId && m.StudentId == studentId);
                if (watching != null)
                {
                    watching.WatchingLectureId = watchingId;
                }
                _unitOfWork.Save();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartTrial()
        {
            var userId = _userManager.GetUserId(User);
            var userStudent = new Models.Student();            
            if (User.IsInRole(SD.Role_User_Individual))
            {
                userStudent = _unitOfWork.Student.GetFirstOrDefault(i => i.UserId == userId);
            }
            if (userId == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var trialOnline = _unitOfWork.HomeContent.GetFirstOrDefault().TrialOnline;
                    if (!trialOnline)
                    {
                        return Json(new { success = false, message = "Deneme üyeliği şu anda aktif değildir. Lütfen bu özelliği kullanmak için gelecekteki kampanyaları takip ediniz." });
                    }

                    //string deviceId = new DeviceIdBuilder().AddMachineName().AddMacAddress().OnWindows(windows => windows.AddProcessorId().AddMotherboardSerialNumber()).ToString();
                    string deviceId = Dns.GetHostEntry(HttpContext.Connection.RemoteIpAddress).HostName;
                    string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                    int[] studentIDs = _unitOfWork.TrialMember.GetAll().Select(i => i.StudentId).ToArray();

                    // servis sağlayıcılar mac adres veya herhangi bir cihaz bilgisini engelliyor DESKTOP-JHDSAF şeklinde gitmesi gereken bilgi 82.222.239.143.dynamic.ttnet.com.tr olarak gidiyor. Daha sonra bunu Cookie olarak çözmeyi deneyelim.
                    // IP ile sorgulama da aynı okul ve firmadan kayıt olacak öğrenciler durumu için geçici olarak devre dışı bırakıldı. Daha sonra client cihaz tespit etme durumuyla birlikte değerlendirilerek tekrar eklenip eklenmemesine karar verilecek.
                    //string[] trialDeviceIDs = _unitOfWork.TrialMember.GetAll().Select(i => i.DeviceId).ToArray();
                    //string[] trialIPAddresses = _unitOfWork.TrialMember.GetAll().Select(i => i.IPAddress).ToArray();
                    //if (studentIDs.Contains(userStudent.Id) || trialDeviceIDs.Contains(deviceId) || trialIPAddresses.Contains(ipAddress))
                    if (userStudent.SubscribeActive)
                    {
                        return Json(new { success = false, message = "Zaten bir üyelik pakedine sahipsiniz. Deneme üyeliği üyelik pakedi olmayan kullanıcılar içindir." });
                    }
                    
                    if (studentIDs.Contains(userStudent.Id))
                    {
                        return Json(new { success = false, message = "Deneme üyeliği hakkınızı daha önceden kullandığınız için aktifleştirme işleminiz gerçekleşmemiştir. Deneme üyeliğini sadece bir kez kullanabilirsiniz." });
                    }

                    var trialMember = new TrialMember();
                    trialMember.DeviceId = deviceId;
                    trialMember.IPAddress = ipAddress;
                    trialMember.StartDate = DateTime.Now;
                    trialMember.StudentId = userStudent.Id;
                    trialMember.IsActive = true;
                    trialMember.Started = true;
                    _unitOfWork.TrialMember.Add(trialMember);
                    _unitOfWork.Save();

                }
                var redirectUrl = Url.Action("Index", "Student");
                return Json(new { success = true, Url = redirectUrl });
            }
        }

        public IActionResult Orders()
        {
            return View();
        }

        public IActionResult OrderDetails(int id)
        {
            OrderVM = new OrderDetailsVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == id, includeProperties: "Course,Subscription")
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (OrderVM.OrderHeader.ApplicationUserId != claim.Value)
            {
                return RedirectToAction(nameof(Orders));
            }

            return View(OrderVM);
        }

        public IActionResult Questions()
        {
            return View();
        }

        public IActionResult Answer(int id)
        {
            CultureInfo culture = new CultureInfo("tr-TR");
            var objFromDb = _unitOfWork.LectureQuestion.Get(id);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (objFromDb.UserId != claim.Value)
            {
                return RedirectToAction(nameof(Questions));
            }

            var question = new LectureQuestionAdminVM()
            {
                Id = objFromDb.Id,
                Date = objFromDb.Date.ToString("d/MMMM/yyy HH:mm", culture).Replace(".", " "),
                Question = objFromDb.Question,
                Answered = objFromDb.Answered,               
            };
            var sectionId = _unitOfWork.Lecture.Get(objFromDb.LectureId).CourseSectionId;
            question.LectureName = _unitOfWork.Lecture.Get(objFromDb.LectureId).Title;
            question.CourseSectionName = _unitOfWork.CourseSection.Get(sectionId).Title;
            question.CourseId = _unitOfWork.CourseSection.Get(sectionId).CourseId;
            question.CourseName = _unitOfWork.Course.Get(question.CourseId).Title;
            question.LectureAnswers = _unitOfWork.LectureAnswer.GetAll().Where(i => i.LectureQuestionId == id)
                .Select(n => new LectureAnswerVM()
                {
                    Id = n.Id,
                    Date = n.Date.ToString("d/MMMM/yyy HH:mm", culture).Replace(".", " "),
                    Answer = n.Answer,
                    Name = _unitOfWork.ApplicationUser.GetFirstOrDefault(o => o.Id == n.UserId).Name,
                    IsInstructor = _userManager.IsInRoleAsync(_userManager.FindByIdAsync(n.UserId).Result, SD.Role_Instructor).Result ? true : false,
                }).ToList();
            if (question == null)
            {
                return NotFound();
            }            

            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult AskQuestion(int courseId, int lectureId, string question)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var isStudent = claimsIdentity.FindFirst(ClaimTypes.Role).Value;
                if (isStudent == SD.Role_User_Individual)
                {
                    var quest = new LectureQuestion()
                    {
                        Date = DateTime.Now,
                        Question = question,
                        UserId = claim.Value,
                        LectureId = lectureId
                    };
                    if (question == null || String.IsNullOrEmpty(question.Trim()))
                    {
                        return Json(new { success = false, message = "Lütfen soru alanını doldurunuz." });
                    }
                    else
                    {
                        _unitOfWork.LectureQuestion.Add(quest);
                        _unitOfWork.Save();

                        var studentName = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == quest.UserId).Name;
                        var studentEmail = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == quest.UserId).Email;
                        var sectionId = _unitOfWork.Lecture.Get(quest.LectureId).CourseSectionId;
                        var lectureName = _unitOfWork.Lecture.Get(quest.LectureId).Title;
                        var courseSectionName = _unitOfWork.CourseSection.Get(sectionId).Title;
                        var courseName = _unitOfWork.Course.Get(courseId).Title;

                        _emailSender.SendEmailAsync(emailOptions.supportAddress, $"{courseName} kursundaki öğrenciniz size bir soru sordu.",
                        $"<strong>{studentEmail}</strong> e-posta adresli öğrenciniz <strong>{studentName}</strong>, <strong>{courseName}</strong> adlı kursun <strong>{courseSectionName}</strong> bölümündeki <strong>{lectureName}</strong> dersi için size bir soru sordu.");
                    }
                }
            }

            var redirectUrl = $"{this.Request.Scheme}://{this.Request.Host}/Student/Student/Presentation/" + courseId;
            return Json(new { success = true, Url = redirectUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult AnswerQuestion(int courseId, int questionId, string answer)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var findRole = claimsIdentity.FindFirst(ClaimTypes.Role).Value;
                if (findRole == SD.Role_User_Individual || findRole == SD.Role_Instructor)
                {
                    var ans = new LectureAnswer()
                    {
                        Date = DateTime.Now,
                        Answer = answer,
                        UserId = claim.Value,
                        LectureQuestionId = questionId
                    };
                    if (answer == null || String.IsNullOrEmpty(answer.Trim()))
                    {
                        return Json(new { success = false, message = "Lütfen cevap alanını doldurunuz." });
                    }
                    if (findRole == SD.Role_Instructor)
                    {
                        var question = _unitOfWork.LectureQuestion.Get(questionId);
                        question.Answered = true;

                        var studentName = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == question.UserId).Name;
                        var studentEmail = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == question.UserId).Email;
                        var sectionId = _unitOfWork.Lecture.Get(question.LectureId).CourseSectionId;
                        var lectureName = _unitOfWork.Lecture.Get(question.LectureId).Title;
                        var courseSectionName = _unitOfWork.CourseSection.Get(sectionId).Title;
                        var courseName = _unitOfWork.Course.Get(courseId).Title;

                        _emailSender.SendEmailAsync(studentEmail, $"{courseName} kursundaki sorunuz eğitmen tarafından cevaplandı.", $"Sayın {studentName}, <strong>{courseName}</strong> adlı kursun <strong>{courseSectionName}</strong> bölümündeki <strong>{lectureName}</strong> dersi için sormuş olduğunuz soru eğitmeniniz tarafından cevaplandı.");
                    }
                    _unitOfWork.LectureAnswer.Add(ans);
                    _unitOfWork.Save();
                }
            }

            var redirectUrl = $"{this.Request.Scheme}://{this.Request.Host}/Student/Student/Presentation/" + courseId;
            return Json(new { success = true, Url = redirectUrl });
        }

        #region API CALLS

        [HttpGet]
        [Route("findLectureView/{courseId}/{lectureId}")]
        public IActionResult FindLecture(int courseId, int lectureId)
        {
            CultureInfo culture = new CultureInfo("tr-TR");
            var lectureJson = new LectureViewVM()
            {
                Lecture = _unitOfWork.Lecture.Get(lectureId),
                LectureQuestions = _unitOfWork.LectureQuestion.GetAll().Where(n => n.LectureId == lectureId).OrderBy(n => n.Date)
                    .Select(m => new LectureQuestionVM()
                    {
                        Id = m.Id,
                        Date = m.Date.ToString("d/MMMM/yyy HH:mm", culture).Replace(".", " "),
                        Question = m.Question,
                        Name = _unitOfWork.ApplicationUser.GetFirstOrDefault(n => n.Id == m.UserId).Name,
                        LectureAnswers = _unitOfWork.LectureAnswer.GetAll().Where(n => n.LectureQuestionId == m.Id).OrderBy(n => n.Date)
                            .Select(n => new LectureAnswerVM()
                            {
                                Date = n.Date.ToString("d/MMMM/yyy HH:mm", culture).Replace(".", " "),
                                Answer = n.Answer,
                                Name = _unitOfWork.ApplicationUser.GetFirstOrDefault(o => o.Id == n.UserId).Name,
                                IsInstructor = _userManager.IsInRoleAsync(_userManager.FindByIdAsync(n.UserId).Result, SD.Role_Instructor).Result ? true : false,
                            }).ToList()
                    }).ToList()
            };
            // var sections = _unitOfWork.Course.Get(courseId).CourseSections;
            return new JsonResult(lectureJson);
        }

        public IActionResult GetOrderList(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;

            orderHeaderList = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");

            switch (status)
            {
                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusInProcess || o.OrderStatus == SD.StatusApproved);
                    break;
                case "pending":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusPending);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "rejected":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusCancelled || o.OrderStatus == SD.StatusRefunded);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaderList });

        }

        public IActionResult GetQuestions()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            CultureInfo culture = new CultureInfo("tr-TR");

            var allObj = _unitOfWork.LectureQuestion.GetAll().Where(i => i.UserId == claim.Value)
                .Select(m => new LectureQuestionAdminVM()
                {
                    Id = m.Id,
                    //Date = m.Date.ToString("d/MMMM/yyy HH:mm", culture).Replace(".", " "),
                    DateTime = m.Date,
                    CourseSectionId = _unitOfWork.Lecture.GetFirstOrDefault(n => n.Id == m.LectureId).CourseSectionId,
                    LectureName = _unitOfWork.Lecture.GetFirstOrDefault(n => n.Id == m.LectureId).Title,
                    Answered = m.Answered
                }).ToList();
            foreach (var item in allObj)
            {
                item.CourseSectionName = _unitOfWork.CourseSection.GetFirstOrDefault(i => i.Id == item.CourseSectionId).Title;
                item.CourseId = _unitOfWork.CourseSection.GetFirstOrDefault(i => i.Id == item.CourseSectionId).CourseId;
                item.CourseName = _unitOfWork.Course.GetFirstOrDefault(i => i.Id == item.CourseId).Title;
            }

            return Json(new { data = allObj });

        }
        #endregion

    }
}