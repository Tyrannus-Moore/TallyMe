using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class TUser
    {
        DbContext dbContext = MyContextFactory.Create();

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        /// <param name="name">对象name</param>
        /// <returns>对象实体</returns>
        public Model.TUser GetModel(string name)
        {
            Model.TUser user = new Model.TUser();
            user = dbContext.Set<Model.TUser>()
                .SingleOrDefault(u => u.Name.Equals(name));

            return user;
        }

        /// <summary>
        /// 添加一个新的用户实体
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>结果</returns>
        public bool Add(Model.TUser user)
        {
            if (user != null)
            {
                dbContext.Set<Model.TUser>().Add(user);
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新一个用户实体
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>结果</returns>
        public bool Update(Model.TUser user)
        {
            dbContext.Set<Model.TUser>().AddOrUpdate(user);

            int result = dbContext.SaveChanges();

            if (result == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
