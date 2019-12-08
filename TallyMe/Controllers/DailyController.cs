using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace TallyMe.Controllers
{
    public class DailyController : Controller
    {
        BLL.TUser btu = new BLL.TUser();
        BLL.Bill bbi = new BLL.Bill();

        [Authorize]
        public ActionResult Index()
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            decimal annualIncome = 0, annualExpense = 0;
            DateTime annualTime = Request["currentYear"] == null ? DateTime.Now: Convert.ToDateTime(Request["currentYear"] + "-5-1");

            if (Request["req"] == "month")
            {
                DateTime date = Convert.ToDateTime(Request["currentYear"] + "-" + Request["currentMonth"] + "-1");

                ViewData["bills"] = bbi.GetMonthlyBillsViaMonth(tuser, date);

                ViewData["graphicData"] = bbi.GetMonthlyStatisticsViaMonth(tuser, date);

                ViewData["currentMonth"] = date.Month;

                ViewData["currentYear"] = date.Year;

                StringBuilder sb = new StringBuilder();

                sb.Append(date.Year);
                sb.Append("年");
                sb.Append(date.Month);
                sb.Append("月1日-");
                sb.Append(date.Year);
                sb.Append("年");
                sb.Append(date.Month);
                sb.Append("月");
                sb.Append(DateTime.DaysInMonth(date.Year, date.Month));
                sb.Append("日消费情况");

                ViewData["caption"] = sb.ToString();
            }
            else if (Request["req"] == "year")
            {
                DateTime date = Convert.ToDateTime(Request["currentYear"]  + "-5-1");

                ViewData["bills"] = bbi.GetMonthlyBillsViaYear(tuser, date);

                ViewData["graphicData"] = bbi.GetMonthlyStatisticsViaYear(tuser, date);

                ViewData["currentMonth"] = date.Month;

                ViewData["currentYear"] = date.Year;

                StringBuilder sb = new StringBuilder();

                sb.Append(date.Year);
                sb.Append("年");
                sb.Append("1月1日-");
                sb.Append(date.Year);
                sb.Append("年12月");
                sb.Append(DateTime.DaysInMonth(date.Year, 12));
                sb.Append("日消费情况");

                ViewData["caption"] = sb.ToString();
            }
            else if(Request["req"] == "period")
            {
                DateTime date1 = Convert.ToDateTime(Request["time1"]);
                DateTime date2 = Convert.ToDateTime(Request["time2"]);

                ViewData["bills"] = bbi.GetPeriodicalBills(tuser, date1, date2);

                ViewData["graphicData"] = bbi.GetPeriodicalStatistics(tuser, date1, date2);

                ViewData["currentMonth"] = date2.Month;

                ViewData["currentYear"] = date2.Year;

                StringBuilder sb = new StringBuilder();

                sb.Append(date1.GetDateTimeFormats()[10]);
                sb.Append("-");
                sb.Append(date2.GetDateTimeFormats()[10]);
                sb.Append("消费情况");

                ViewData["caption"] = sb.ToString();
            }
            else
            {
                ViewData["bills"] = bbi.GetMonthlyBillsViaMonth(tuser, DateTime.Now);

                ViewData["graphicData"] = bbi.GetMonthlyStatisticsViaMonth(tuser, DateTime.Now);

                ViewData["currentMonth"] = DateTime.Now.Month;

                ViewData["currentYear"] = DateTime.Now.Year;

                StringBuilder sb = new StringBuilder();

                sb.Append(DateTime.Now.Year);
                sb.Append("年");
                sb.Append(DateTime.Now.Month);
                sb.Append("月1日-");
                sb.Append(DateTime.Now.Year);
                sb.Append("年");
                sb.Append(DateTime.Now.Month);
                sb.Append("月");
                sb.Append(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                sb.Append("日消费情况");

                ViewData["caption"] = sb.ToString();
            }

            DateTime dt = DateTime.Now;
            DateTime startQuarter = dt.AddMonths(0 - (dt.Month - 1) % 3).AddDays(1 - dt.Day);  //本季度初
            DateTime endQuarter = startQuarter.AddMonths(3).AddDays(-1);  //本季度末

            ViewData["categoryDic"] = bbi.GetCategoryDic();
            ViewData["annualBills"] = bbi.GetAnnualBills(tuser, annualTime, out annualIncome,out annualExpense);
            ViewData["annualIncome"] = annualIncome;
            ViewData["annualExpense"] = annualExpense;

            return View();
        }

    }
}