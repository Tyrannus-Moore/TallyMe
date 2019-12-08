using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// 创建EF上下文对象,已存在就直接取,不存在就创建,保证线程内是唯一。
    /// </summary>
    /// <returns>上下文对象</returns>
    public class MyContextFactory
    {
        public static DbContext Create()
        {
            DbContext dbContext = CallContext.GetData("DbContext") as DbContext;
            if (dbContext == null)
            {
                dbContext = new MyContext();
                CallContext.SetData("DbContext", dbContext);
            }
            return dbContext;
        }
    }
}
