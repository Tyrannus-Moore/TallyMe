using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Budget
    {
        private readonly DAL.Budget dal = new DAL.Budget();

        /// <summary>
        /// 获取当月预算
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="month">当前月号</param>
        /// <returns>月预算列表</returns>
        public decimal[] GetBudget(Model.TUser user, int month)
        {
            return dal.GetBudget(user.Id, month);
        }

        /// <summary>
        /// 保存当月预算
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="budgetBox">前端当月预算表0-25,对应ClassId，0号位闲置</param>
        /// <param name="month">月号</param>
        /// <returns>结果</returns>
        public bool SetBudget(Model.TUser user, Array budgetBox, int month)
        {
            Model.Budget[] budgets = new Model.Budget[24];
            int i = 1; // 遍历用的ClassId

            foreach(var b in budgetBox)
            {
                budgets[i-1] = new Model.Budget()
                {
                    ClassId = i,
                    Amount = Convert.ToDecimal(b),
                    CreateMonth = month,
                    TUserId = user.Id
                };
                i++;
            }

            return dal.SetBudget(user.Id, budgets, month);
        }
    }
}
