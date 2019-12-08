using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Budget
    {
        DbContext dbContext = MyContextFactory.Create();

        /// <summary>
        /// 添加一条记录
        /// </summary>
        /// <param name="budget">一个非类的预算</param>
        /// <returns>结果</returns>
        public bool Add(Model.Budget budget)
        {
            if (budget != null)
            {
                dbContext.Set<Model.Budget>().Add(budget);
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 按月批量删除
        /// </summary>
        /// <param name="month">当前月号</param>
        /// <returns></returns>
        public bool DeleteList(int month)
        {
            IQueryable<Model.Budget> list = dbContext.Set<Model.Budget>()
                .Where(b => b.CreateMonth == month);

            foreach(Model.Budget budget in list)
            {
                dbContext.Set<Model.Budget>().Remove(budget);
            }
            int result = dbContext.SaveChanges();

            return result == 0 ? false : true;
        }

        /// <summary>
        /// 获取当月预算
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="month">月号</param>
        /// <returns>月预算列表</returns>
        public decimal[] GetBudget(int id, int month)
        {
            IQueryable<Model.Budget> list = dbContext.Set<Model.Budget>()
                .OrderBy(b => b.ClassId).Where(b => (b.TUserId == id) && (b.CreateMonth == month));

            decimal[] budgetBox = new decimal[25];

            foreach(var budget in list)
            {
                budgetBox[(int)budget.ClassId] = (decimal)budget.Amount;
            }

            return budgetBox;
        }

        /// <summary>
        /// 保存当月预算
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="budgetBox">当月预算表</param>
        /// <param name="month">月号</param>
        /// <returns>结果</returns>
        public bool SetBudget(int id, Model.Budget[] budgetBox, int month)
        {
            DeleteList(month);

            foreach(Model.Budget b in budgetBox)
            {
                bool result = Add(b);
                if (result == false) return false;
            }
            return true;
        }
    }
}
