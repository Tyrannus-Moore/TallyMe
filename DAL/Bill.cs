using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Bill
    {
        DbContext dbContext = MyContextFactory.Create();

        #region Tally
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        /// <param name="id">对象id</param>
        /// <returns>对象实体</returns>
        public Model.Bill GetModel(int id)
        {
            Model.Bill bill = new Model.Bill();
            bill = dbContext.Set<Model.Bill>()
                .SingleOrDefault(u => u.Id.Equals(id));

            return bill;
        }

        /// <summary>
        /// 根据时间获取账单列表
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>当天账单列表</returns>
        public IQueryable<Model.Bill> GetList(int id, DateTime date)
        {
            IQueryable<Model.Bill> list = dbContext.Set<Model.Bill>()
                .Where(b => (b.CreateDate == date) && (b.TUserId == id) );
            return list;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(Model.Bill bill)
        {
            if (bill != null)
            {
                dbContext.Set<Model.Bill>().Add(bill);
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        public bool Delete(int id)
        {
            Model.Bill bill = dbContext.Set<Model.Bill>()
                .Where(u => u.Id == id)
                .FirstOrDefault();

            if (bill != null)
            {
                dbContext.Set<Model.Bill>().Remove(bill);
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 按照用户删除其下记录
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>结果</returns>
        public bool DeleteBatch(string name)
        {
            var bills = dbContext.Set<Model.Bill>()
                .Where(u => u.TUser.Name == name);

            if (bills != null)
            {
                foreach(var bill in bills)
                {
                    dbContext.Set<Model.Bill>().Remove(bill);
                }

                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 修改一条记录
        /// </summary>
        /// <param name="bill">账单</param>
        /// <returns>结果</returns>
        public bool Update(Model.Bill bill)
        {
            dbContext.Set<Model.Bill>().AddOrUpdate(bill);

            int result = dbContext.SaveChanges();

            if(result == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取当前月消费最多5个种类类名
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="month">当前月</param>
        /// <param name="year">当前年</param>
        /// <returns>种类名</returns>
        public ArrayList GetTop5MonthlyName(int id, int month, int year)
        {
            var list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.ClassId).Where(b => (b.TUserId == id) && (b.CreateDate.Month == month) && (b.CreateDate.Year == year) && (b.ClassId != 24))
                .GroupBy(b => b.ClassId).Select(g => new
                {
                    Amount = g.Sum(t => t.Amount),
                    Name = g.FirstOrDefault().Class.Name
                }).OrderByDescending(t => t.Amount).Take(5);

            ArrayList nameBox = new ArrayList();

            foreach (var bg in list)
            {
                nameBox.Add(bg.Name);
            }

            return nameBox;
        }

        /// <summary>
        /// 获取当前月消费最多5个种类金额
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="month">当前月</param>
        /// <param name="year">当前年</param>
        /// <returns>种类金额</returns>
        public ArrayList GetTop5MonthlyAmount(int id, int month, int year)
        {
            var list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.ClassId).Where(b => (b.TUserId == id) && (b.CreateDate.Month == month) && (b.CreateDate.Year == year) && (b.ClassId != 24))
                .GroupBy(b => b.ClassId).Select(g => new
                {
                    Amount = g.Sum(t => t.Amount),
                    Name = g.FirstOrDefault().Class.Name
                }).OrderByDescending(t => t.Amount).Take(5);

            ArrayList moneyBox = new ArrayList();

            foreach (var bg in list)
            {
                moneyBox.Add(bg.Amount);
            }

            return moneyBox;
        }

        /// <summary>
        /// 获取本季度消费数据
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="month">当前月</param>
        /// <param name="year">当前年</param>
        /// <returns>本季度数据</returns>
        public decimal[] GetQuarterlyGraphicValue(int id, int month, int year)
        {
            IQueryable<Model.Bill> list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.CreateDate).Where(b => (b.TUserId == id) && ((b.CreateDate.Month == month)|| (b.CreateDate.Month == (month-1))|| (b.CreateDate.Month == (month-2))|| b.CreateDate.Month == (month-3)) && (b.CreateDate.Year == year) &&(b.ClassId!=24));

            decimal[] moneyBox = new decimal[4];

            foreach(var bill in list)
            {
                if (bill.CreateDate.Month == (month - 3)) moneyBox[3] += bill.Amount;
                else if(bill.CreateDate.Month == (month-2)) moneyBox[2] += bill.Amount;
                else if (bill.CreateDate.Month == (month - 1)) moneyBox[1] += bill.Amount;
                else if (bill.CreateDate.Month == month) moneyBox[0] += bill.Amount;
            }

            return moneyBox;
        }

        /// <summary>
        /// 使用存储过程
        /// </summary>
        /// <returns></returns>
        public int UseProc()
        {
            //SqlParameter[] selparms = new SqlParameter[1];
            //selparms[0] = new SqlParameter("@Id", 3);
            var idParam = new SqlParameter
            {
                ParameterName = "@Id",
                Value = 1
            };
            var datetimeParam = new SqlParameter
            {
                ParameterName = "@CreateDate",
                Value = "2018-5-1"
            };
            SqlParameter amountParam = new SqlParameter("@Amount", SqlDbType.Decimal);
            amountParam.Direction = ParameterDirection.Output;
            SqlParameter dateParam = new SqlParameter("@Date", SqlDbType.DateTime);
            dateParam.Direction = ParameterDirection.Output;

            var result = dbContext.Database.SqlQuery<Bill>("exec GetTuserMonthlyBill @Id,@CreateDate,@Amount out,@Date out", idParam, datetimeParam, amountParam, dateParam).ToList();

            var amount = amountParam.Value;
            var date = dateParam.Value;
            return Convert.ToInt32(amount);
            //var result = dbContext.Database.SqlQuery<Bill>("select * from Bill").ToList() ; 执行SQL
        }
        #endregion

        #region Daily

        /// <summary>
        /// 获取收支统计 --根据月份
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="year">年号</param>
        /// <param name="month">月号</param>
        /// <returns>账单列表</returns>
        public IQueryable<Model.Bill> GetMonthlyBillsViaMonth(int id, int year, int month)
        {
            IQueryable<Model.Bill> list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.CreateDate).Where(b => (b.TUserId == id) && (b.CreateDate.Month == month) && (b.CreateDate.Year == year));

            return list;
        }

        /// <summary>
        /// 获取收支统计图 --根据月份
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="year">年号</param>
        /// <param name="month">月号</param>
        /// <returns>图表数据</returns>
        public Dictionary<String, decimal> GetMonthlyStatisticsViaMonth(int id, int year, int month)
        {
            var list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.CreateDate).Where(b => (b.TUserId == id) && (b.CreateDate.Year == year) && (b.CreateDate.Month == month) && (b.ClassId != 24))
                .GroupBy(b => b.ClassId).ToList();

            Dictionary<String, decimal> moneyBox = new Dictionary<String, decimal>();
            string tempName = "";
            decimal tempAmount = 0;
            foreach(var bg in list)
            {
                foreach(var bill in bg)
                {
                    tempName = bill.Class.Name;
                    tempAmount += bill.Amount;
                }

                moneyBox.Add(tempName, tempAmount);

                tempName = "";
                tempAmount = 0;
            }

            return moneyBox;
        }

        /// <summary>
        /// 获取收支统计 --根据年份
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="year">年号</param>
        /// <returns>账单列表</returns>
        public IQueryable<Model.Bill> GetMonthlyBillsViaYear(int id, int year)
        {
            IQueryable<Model.Bill> list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.CreateDate).Where(b => (b.TUserId == id) && (b.CreateDate.Year == year));

            return list;
        }

        /// <summary>
        /// 获取收支统计图 --根据年份
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="year">年号</param>
        /// <returns>图表数据</returns>
        public Dictionary<String, decimal> GetMonthlyStatisticsViaYear(int id, int year)
        {
            var list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.CreateDate).Where(b => (b.TUserId == id) && (b.CreateDate.Year == year) && (b.ClassId != 24))
                .GroupBy(b => b.ClassId).ToList();

            Dictionary<String, decimal> moneyBox = new Dictionary<String, decimal>();
            string tempName = "";
            decimal tempAmount = 0;
            foreach (var bg in list)
            {
                foreach (var bill in bg)
                {
                    tempName = bill.Class.Name;
                    tempAmount += bill.Amount;
                }

                moneyBox.Add(tempName, tempAmount);

                tempName = "";
                tempAmount = 0;
            }

            return moneyBox;
        }

        /// <summary>
        /// 获取收支统计 --根据时间范围
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="fDate">开始时间(一年中的)</param>
        /// <param name="tDate">结束时间(一年中的)</param>
        /// <returns>账单列表</returns>
        public IQueryable<Model.Bill> GetPeriodicalBills(int id, DateTime fDate, DateTime tDate)
        {
            IQueryable<Model.Bill> list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.CreateDate).Where(b => (b.TUserId == id) && ( (b.CreateDate >= fDate) && (b.CreateDate <= tDate) ));

            return list;
        }

        /// <summary>
        /// 获取收支统计图 --根据时间范围
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="fDate">开始时间</param>
        /// <param name="tDate">结束时间</param>
        public Dictionary<String, decimal> GetPeriodicalStatistics(int id, DateTime fDate, DateTime tDate)
        {
            var list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.CreateDate).Where(b => (b.TUserId == id) && (b.CreateDate >= fDate && b.CreateDate <= tDate) && (b.ClassId != 24))
                .GroupBy(b => b.ClassId).ToList();

            Dictionary<String, decimal> moneyBox = new Dictionary<String, decimal>();
            string tempName = "";
            decimal tempAmount = 0;
            foreach (var bg in list)
            {
                foreach (var bill in bg)
                {
                    tempName = bill.Class.Name;
                    tempAmount += bill.Amount;
                }

                moneyBox.Add(tempName, tempAmount);

                tempName = "";
                tempAmount = 0;
            }

            return moneyBox;
        }


        /// <summary>
        /// 获取年度报表
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="year">当前年</param>
        /// <param name="annualIncome">年度总收入</param>
        /// <param name="annualExpense">年度总支出</param>
        /// <returns>年度报表</returns>
        public decimal[,] GetAnnualBills(int id, int year, out decimal annualIncome, out decimal annualExpense)
        {
            var list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.CreateDate).Where(b => (b.TUserId == id) && (b.CreateDate.Year == year))
                .GroupBy(b => b.CreateDate.Month).ToList();

            decimal[,] moneyBox = new decimal[25, 13]; // 数组实为0-24,0-12，一维对应账单种类，二维对应月份
            //若[12]全为0，则[i][0] = -1;
            //[0][month]当月收入合计 [month][0]当月消费合计 

            foreach (var bg in list)
            {
                foreach (var bill in bg)
                {
                    moneyBox[(int)bill.ClassId, bill.CreateDate.Month] += bill.Amount;
                    moneyBox[0, bill.CreateDate.Month] += bill.Amount;
                }
            }

            // 若[12]全为0，则将[i,0]置为-1，方便前端识别。
            for (int i = 0; i < 25; i++)
            {
                decimal temp = 0;
                for(int j = 0; j < 13; j++)
                {
                    temp += moneyBox[i, j];
                }
                if (temp == 0) moneyBox[i, 0] = -1;
            }

            // 将收入项从合计中去除 + 对年度支出和收入赋值
            annualIncome = 0;
            annualExpense = 0;
            for(int j = 0; j < 13; j++)
            {
                moneyBox[0, j] -= moneyBox[24, j];
                annualIncome += moneyBox[24, j];
                if(moneyBox[0, j]!=-1) annualExpense += moneyBox[0, j];
            }

            if (annualIncome == -1) annualIncome = 0;
            return moneyBox;

        }
        
        /// <summary>
        /// 获取种类字典
        /// </summary>
        /// <returns>种类字典</returns>
        public Dictionary<int, String> GetCategoryDic()
        {
            Dictionary<int, String> dic = new Dictionary<int, string>();

            var list = dbContext.Set<Model.Class>().ToList();

            foreach(var item in list)
            {
                dic.Add(item.Id, item.Name);
            }

            return dic;
        }
        #endregion

        #region Analyse
        /// <summary>
        /// 获取用户当月恩格尔系数
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="month">月号</param>
        /// <param name="year">年号</param>
        /// <returns>恩格尔系数</returns>
        public decimal GetEngelsCoefficient(int id, int month, int year)
        {
            IQueryable<Model.Bill> list = dbContext.Set<Model.Bill>()
                .Where(b => (b.TUserId == id) && (b.CreateDate.Month == month) && (b.CreateDate.Year == year) && (b.ClassId==1 || b.ClassId == 4 || b.ClassId == 2 || b.ClassId == 24));

            decimal numerator = 0;
            decimal denominator = 0;

            foreach (Model.Bill bill in list)
            {
                if (bill.ClassId == 24) denominator += bill.Amount;
                else numerator += bill.Amount;
            }

            if (numerator == 0 || denominator == 0) return 0;

            return numerator / denominator;
        }

        /// <summary>
        /// 获取当月盈余
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="month">月号</param>
        /// <param name="year">年号</param>
        /// <returns>盈余</returns>
        public decimal GetBalance(int id, int month, int year)
        {
            IQueryable<Model.Bill> list = dbContext.Set<Model.Bill>()
                .Where(b => (b.TUserId == id) && (b.CreateDate.Month == month) && (b.CreateDate.Year == year));

            decimal expenditure = 0;
            decimal income = 0;

            foreach (Model.Bill bill in list)
            {
                if (bill.ClassId == 24) income += bill.Amount;
                else expenditure += bill.Amount;
            }

            return income - expenditure;
        }

        /// <summary>
        /// 获取用户消费最多的种类
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="year">所选年</param>
        /// <returns>最喜爱Class</returns>
        public Model.Class GetFavoriteClass(int id, int year)
        {
            var list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.CreateDate).Where(b => (b.TUserId == id) && (b.CreateDate.Year == year))
                .GroupBy(b => b.ClassId).ToList();

            Model.Class currentClass = null;
            decimal currentMoneyBox=0;
            Model.Class maxClass = null;
            decimal maxMoneyBox=0;

            foreach (var bg in list)
            {
                foreach (var bill in bg)
                {
                    currentClass = bill.Class;
                    currentMoneyBox += bill.Amount;
                }

                if(currentMoneyBox >= maxMoneyBox && currentClass.Id != 24) // 不能是收入
                {
                    maxClass = currentClass;
                    maxMoneyBox = currentMoneyBox;
                }

                currentClass = null;
                currentMoneyBox = 0;
            }

            return maxClass;
        }

        /// <summary>
        /// 获得用户当前年度总记账
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="year">当年</param>
        /// <returns>记账总数</returns>
        public decimal GetTotalBills(int id, int year)
        {
            IQueryable<Model.Bill> list = dbContext.Set<Model.Bill>()
                .Where(b => (b.TUserId == id) && (b.CreateDate.Year == year));

            decimal moneyBox = 0;

            foreach (Model.Bill bill in list)
            {
                moneyBox += bill.Amount;
            }

            return moneyBox;
        }

        /// <summary>
        /// 获取记账分析描述
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="month">月份</param>
        /// <param name="year">年号</param>
        /// <returns>分析文字描述</returns>
        public string GetBillsAnalysis(Model.TUser user, int month, int year)
        {
            StringBuilder sb = new StringBuilder();
            if (user.Bill.Count == 0)
            {
                sb.Append("<h4 class='alert-heading' style='letter-spacing:3px; word-wrap:break-word; word-break:normal;'>咦，这里空空如也，不如先记一笔?</h4>");
                return sb.ToString();
            }

            sb.Append("<h4 class='alert-heading' style='letter-spacing:3px; word-wrap:break-word; word-break:normal;'>亲爱的");
            sb.Append(user.Name);
            sb.Append(", 欢迎查看你的消费报告。</h4>");
            sb.Append("你在当月的恩格尔系数为<span class='text-danger font-weight-bold'> ");
            sb.Append(GetEngelsCoefficient(user.Id, month, year)+ "</span>,"); // 获取恩格尔系数
            sb.Append(" 当月盈余为: ");
            Model.Class favrtClass = GetFavoriteClass(user.Id, year);

            decimal balance = Math.Round(GetBalance(user.Id, month, year), 2); // 获取盈余
            if(balance > 0)
            {
                sb.Append("<span class='text-success font-weight-bold'>" + balance+ "</span>, 请放心买买买!");
            }
            else
            {
                sb.Append("<span class='text-danger font-weight-bold'>" + balance + "</span>, 再买就要吃土辣!");
            }

            sb.Append(" 你今年一共在记账通记账<span class='text-danger font-weight-bold'>" + Math.Round(GetTotalBills(user.Id, year), 2) + " </span>大洋, "); // 获取当年总共记账金额

            if(favrtClass == null)
            {
                sb.Append("要多多记账呀！");
            }
            else
            {
                sb.Append("并且特别爱在<span class='font-weight-bold'> " + GetFavoriteClass(user.Id, year).Name + " </span>上消费。"); // 获取当年用户消费最多的种类
            }

            sb.Append("</br>记账通, 有你更精彩!</p>");

            return sb.ToString();
        }

        /// <summary>
        /// 返回用户记账记录
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <returns>记账记录</returns>
        public IQueryable<Model.Bill> GetBillsForExcel(int id)
        {
            IQueryable<Model.Bill> list = dbContext.Set<Model.Bill>()
                .OrderBy(b => b.CreateDate).Where(b => (b.TUserId == id));

            return list;
        }

        /// <summary>
        /// 获取用户EXCEL记账记录 用的EPPLUS
        /// </summary>
        /// <param name="user">用户</param>
        public void SaveBillsAsExcel(Model.TUser user , string fileLoc)
        {
            IQueryable<Model.Bill> bills = GetBillsForExcel(user.Id);

            string spreadsheetPath = user.Id + "-" + user.Name + "-Bills.xlsx";

            if (File.Exists(fileLoc +"\\" + spreadsheetPath))
            {
                File.Delete(fileLoc + "\\" + spreadsheetPath);
            }

            FileInfo spreadsheetInfo = new FileInfo(spreadsheetPath);

            ExcelPackage pck = new ExcelPackage(spreadsheetInfo);

            var billsWorksheet = pck.Workbook.Worksheets.Add("Bills");
            billsWorksheet.Cells["A1"].Value = "收支";
            billsWorksheet.Cells["B1"].Value = "金额";
            billsWorksheet.Cells["C1"].Value = "时间";
            billsWorksheet.Cells["D1"].Value = "类别";
            billsWorksheet.Cells["E1"].Value = "备注";
            billsWorksheet.Cells["A1:E1"].Style.Font.Bold = true;

            // populate spreadsheet with data
            int currentRow = 2;
            foreach(var bill in bills)
            {
                billsWorksheet.Cells["A" + currentRow.ToString()].Value = bill.PaymentType == true ? "支出" : "收入";
                billsWorksheet.Cells["B" + currentRow.ToString()].Value = bill.Amount;
                billsWorksheet.Cells["C" + currentRow.ToString()].Value = bill.CreateDate.GetDateTimeFormats()[2];
                billsWorksheet.Cells["D" + currentRow.ToString()].Value = bill.Class.Name;
                billsWorksheet.Cells["E" + currentRow.ToString()].Value = bill.Note;

                currentRow++;
            }

            billsWorksheet.View.FreezePanes(2, 1);

            //pck.Save();
            pck.SaveAs(new FileInfo(fileLoc + "\\" + spreadsheetPath));
        }
        #endregion
    }
}
