using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class Bill
    {
        private readonly DAL.Bill dal = new DAL.Bill();

        #region Tally
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        /// <param name="id">对象id</param>
        /// <returns>对象实体</returns>
        public Model.Bill GetModel(int id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 根据时间获取账单列表
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>当天账单列表</returns>
        public IQueryable<Model.Bill> GetList(int id , DateTime date)
        {
            return dal.GetList(id,date);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(Model.Bill bill)
        {
            return dal.Add(bill);
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// 按照用户删除其下记录
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>结果</returns>
        public bool DeleteBatch(string name)
        {
            return dal.DeleteBatch(name);
        }

        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="bill">账单</param>
        /// <returns>结果</returns>
        public bool Update(Model.Bill bill)
        {
            return dal.Update(bill);
        }

        /// <summary>
        /// 获取当前月消费最多5个种类类名
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="date">当前时间</param>
        /// <returns>种类名</returns>
        public ArrayList GetTop5MonthlyName(Model.TUser user, DateTime date)
        {
            return dal.GetTop5MonthlyName(user.Id, date.Month, date.Year);
        }

        /// <summary>
        /// 获取当前月消费最多5个种类金额
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="date">当前时间</param>
        /// <returns>种类金额</returns>
        public ArrayList GetTop5MonthlyAmount(Model.TUser user, DateTime date)
        {
            return dal.GetTop5MonthlyAmount(user.Id, date.Month, date.Year);
        }

        /// <summary>
        /// 获取本季度消费数据
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="date">时间</param>
        /// <returns>本季度数据</returns>
        public decimal[] GetQuarterlyGraphicValue(Model.TUser user, DateTime date)
        {
            return dal.GetQuarterlyGraphicValue(user.Id, date.Month, date.Year);
        }

        /// <summary>
        /// 使用存储过程 --警告！存在问题
        /// </summary>
        /// <returns></returns>
        public int UseProc()
        {
            return dal.UseProc();
        } 
        #endregion

        #region Daily
        /// <summary>
        /// 获取收支统计 --根据月份
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="date">所选日期</param>
        /// <returns>账单列表</returns>
        public IQueryable<Model.Bill> GetMonthlyBillsViaMonth(Model.TUser user, DateTime date)
        {
            return dal.GetMonthlyBillsViaMonth(user.Id, date.Year, date.Month);
        }

        /// <summary>
        /// 获取收支统计图 --根据月份
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="date">所选日期</param>
        /// <returns>图表数据</returns>
        public Dictionary<String, decimal> GetMonthlyStatisticsViaMonth(Model.TUser user, DateTime date)
        {
            return dal.GetMonthlyStatisticsViaMonth(user.Id, date.Year, date.Month);
        }

        /// <summary>
        /// 获取收支统计 --根据年份
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="date">所选日期</param>
        /// <returns>账单列表</returns>
        public IQueryable<Model.Bill> GetMonthlyBillsViaYear(Model.TUser user,DateTime date)
        {
            return dal.GetMonthlyBillsViaYear(user.Id, date.Year);
        }

        /// <summary>
        /// 获取收支统计图 --根据年份
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="date">所选日期</param>
        /// <returns>图表数据</returns>
        public Dictionary<String, decimal> GetMonthlyStatisticsViaYear(Model.TUser user, DateTime date)
        {
            return dal.GetMonthlyStatisticsViaYear(user.Id, date.Year);
        }

        /// <summary>
        /// 获取收支统计 --根据时间范围
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="fDate">开始日期</param>
        /// <param name="tDate">结束日期</param>
        /// <returns>账单列表</returns>
        public IQueryable<Model.Bill> GetPeriodicalBills(Model.TUser user, DateTime fDate, DateTime tDate)
        {
            return dal.GetPeriodicalBills(user.Id, fDate, tDate);
        }

        /// <summary>
        /// 获取收支统计图 --根据时间范围
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="fDate">起始日期</param>
        /// <param name="tDate">结束日期</param>
        /// <returns>图表数据</returns>
        public Dictionary<String, decimal> GetPeriodicalStatistics(Model.TUser user, DateTime fDate, DateTime tDate)
        {
            return dal.GetPeriodicalStatistics(user.Id, fDate, tDate);
        }

        /// <summary>
        /// 获取年度报表
        /// </summary>
        /// <param name="year">当前年</param>
        /// <returns>报表数组</returns>
        public decimal[,] GetAnnualBills(Model.TUser user ,DateTime date,out decimal annualIncome, out decimal annualExpense)
        {
            return dal.GetAnnualBills(user.Id , date.Year, out annualIncome, out annualExpense);
        }

        /// <summary>
        /// 获取种类字典
        /// </summary>
        /// <returns>种类字典</returns>
        public Dictionary<int,String> GetCategoryDic()
        {
            return dal.GetCategoryDic();
        }
        #endregion

        #region Analyse

        /// <summary>
        /// 获取用户记账EXCEL记录
        /// </summary>
        /// <param name="user">用户</param>
        public void SaveBillsAsExcel(Model.TUser user , string fileLoc)
        {
            dal.SaveBillsAsExcel(user , fileLoc);
        }

        /// <summary>
        /// 获取记账分析描述
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="date">日期</param>
        /// <returns>记账分析描述</returns>
        public string GetBillsAnalysis(Model.TUser user, DateTime date)
        {
            return dal.GetBillsAnalysis(user, date.Month, date.Year);
        }
        #endregion
    }
}
