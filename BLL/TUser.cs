using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class TUser
    {
        private readonly DAL.TUser dal = new DAL.TUser();

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        /// <param name="name">对象name</param>
        /// <returns>对象实体</returns>
        public Model.TUser GetModel(string name)
        {
            return dal.GetModel(name);
        }

        /// <summary>
        /// 添加一个用户实体
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>结果</returns>
        public bool Add(Model.TUser user)
        {
            return dal.Add(user);
        }

        /// <summary>
        /// 更新一个用户实体
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>结果</returns>
        public bool Update(Model.TUser user)
        {
            return dal.Update(user);
        }
    }
}
