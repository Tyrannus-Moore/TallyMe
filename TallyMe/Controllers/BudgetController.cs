using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TallyMe.Controllers
{
    public class BudgetController : Controller
    {
        BLL.TUser btu = new BLL.TUser();
        BLL.Budget bbt = new BLL.Budget();
        BLL.Bill bbi = new BLL.Bill();

        [Authorize]
        public ActionResult Index()
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            decimal[] preBills = new decimal[25];
            decimal[] bills = new decimal[25];
            // 获取上月实际消费数据
            var list = bbi.GetMonthlyBillsViaMonth(tuser, DateTime.Now.AddMonths(-1));
            foreach(var bill in list)
            {
                preBills[(int)bill.ClassId] += bill.Amount;
            }
            ViewData["preExpense"] = preBills;

            // 获取本月实际消费数据
            list = bbi.GetMonthlyBillsViaMonth(tuser, DateTime.Now);
            foreach (var bill in list)
            {
                bills[(int)bill.ClassId] += bill.Amount;
            }
            ViewData["expense"] = bills;

            // 获取上月预算
            ViewData["preBudgets"] = bbt.GetBudget(tuser, DateTime.Now.AddMonths(-1).Month);

            // 获取本月预算
            ViewData["budgets"] = bbt.GetBudget(tuser, DateTime.Now.Month);

            ViewData["categoryDic"] = bbi.GetCategoryDic();

            return View();
        }


        [Authorize][HttpPost]
        public EmptyResult SaveBudget(Array budgets)
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            bbt.SetBudget(tuser, budgets, DateTime.Now.Month);

            return null;
        }
    }
}