using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TallyMe.Controllers
{
    public class TallyController : Controller
    {
        BLL.TUser btu = new BLL.TUser();
        BLL.Bill bbi = new BLL.Bill();

        // GET: Tally
        [Authorize]
        public ActionResult Index()
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            string dt1 = DateTime.Now.ToShortDateString();
            DateTime dt = Convert.ToDateTime(dt1);
            DateTime currentDate = Request["date"] != null ? Convert.ToDateTime(Request["date"]) : dt; // 选了日期则用选了的，没选则用当前日期

            /*处理8-1号这种情况*/
            if (currentDate.Day < 5)
            {
                if(currentDate.Month == 1)
                {
                    int days = DateTime.DaysInMonth((currentDate.Year - 1), 12);
                    ViewData["pCurrentYearMonth"] = (currentDate.Year - 1) + "-" + 12 + "-";
                    ViewData["pDays"] = days;
                }
                else
                {
                    int days = DateTime.DaysInMonth(currentDate.Year, currentDate.Month - 1);
                    ViewData["pCurrentYearMonth"] = currentDate.Year + "-" + (currentDate.Month - 1) + "-";
                    ViewData["pDays"] = days;
                }
            }

            IQueryable<Model.Bill> briefList = bbi.GetList(tuser.Id, currentDate);  // 记账首页下方当日记账略表

            ViewData["monthlyName"] = bbi.GetTop5MonthlyName(tuser, dt); // 记账首页本月消费最高5类别名
            ViewData["monthlyAmount"] = bbi.GetTop5MonthlyAmount(tuser, dt); // 记账首页本月消费最高5类别金额

            ViewData["quartlyData"] = bbi.GetQuarterlyGraphicValue(tuser, dt); // 记账首页最近四个月的消费金额 在前台形如data: [15.0000,0.0,173.0000,25.0000]

            ViewData["currentYearMonth"] = currentDate.Year + "-" + currentDate.Month + "-";
            ViewData["currentDay"] = currentDate.Day;
            ViewData["briefList"] = briefList;

            return View();
        }

        /// <summary>
        /// 添加一条账单
        /// </summary>
        /// <param name="bill">账单数据</param>
        /// <returns></returns>
        public ActionResult CreateBill(Model.Bill bill)
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            bill.TUserId = tuser.Id;

            if (bill.ClassId != 24) bill.PaymentType = true; // 确定为收入或支出
            else bill.PaymentType = false;

            if (bill.ClassId == null) bill.ClassId = 23; // 如果用户不填ClassId则默认为其他

            bool result = bbi.Add(bill);

            return Content(result == true ? "ok" : "no");
        }

        /// <summary>
        /// 更新一条账单
        /// </summary>
        /// <param name="bill">账单</param>
        /// <returns></returns>
        public ActionResult UpdateBill(Model.Bill bill)
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            bill.TUserId = tuser.Id;
            bool result = bbi.Update(bill);

            return Content(result == true ? "ok" : "no");
        }

        /// <summary>
        /// 删除一条账单
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteBill(int id)
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            bool result = bbi.Delete(id);

            return Content(result == true ? "ok" : "no");
        }

        /// <summary>
        /// 根据Id获取一条账单详细信息
        /// </summary>
        /// <param name="id">账单号</param>
        /// <returns></returns>
        public ActionResult GetABill(int id)
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            Model.Bill bill = bbi.GetModel(id);

            return Json(bill, JsonRequestBehavior.AllowGet);
        }
    }
}