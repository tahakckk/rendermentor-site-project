using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderDetailsVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {            
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderVM = new OrderDetailsVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == id, includeProperties: "Course")
            };
            return View(OrderVM);
        }

        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            orderHeader.OrderStatus = SD.StatusInProcess;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ShipOrder(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            /*orderHeader.TransactionId = OrderVM.OrderHeader.TransactionId;*/
            orderHeader.OrderStatus = SD.StatusShipped;
            var userStudent = _unitOfWork.Student.GetFirstOrDefault(i => i.UserId == orderHeader.ApplicationUserId);
            userStudent.StudentCourses = _unitOfWork.StudentCourse.GetAll(i => i.StudentId == userStudent.Id).ToList();
            IEnumerable<OrderDetails> orderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == id);
            foreach (var courseToAdd in orderDetails)
            {
                if (!userStudent.StudentCourses.Exists(i => i.CourseId == courseToAdd.CourseId))
                {
                    userStudent.StudentCourses.Add(new StudentCourse()
                    {
                        StudentId = userStudent.Id,
                        CourseId = courseToAdd.CourseId,
                        isPaid = true
                    });
                }
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            if (orderHeader.OrderStatus == SD.StatusApproved)
            {
                orderHeader.OrderStatus = SD.StatusRefunded;
            }
            else
            {
                orderHeader.OrderStatus = SD.StatusCancelled;
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            IEnumerable<OrderHeader> orderHeaderList;

            orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

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

        #endregion
    }
}