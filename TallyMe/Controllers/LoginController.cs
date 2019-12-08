using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TallyMe.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 检查用户登录
        /// </summary>
        /// <returns>登录判断</returns>
        public ActionResult CheckLogin()
        {
            string validateCode = Session["validateCode"] == null ? string.Empty : Session["validateCode"].ToString(); // null是对象，如果Session为空，就会报错

            if (string.IsNullOrEmpty(validateCode))
            {
                return Content("no:验证码为空!");
            }
            Session["validateCode"] = null; // 清空Session

            string requestCode = Request["vCode"];
            if (!requestCode.Equals(validateCode, StringComparison.InvariantCultureIgnoreCase))
            {
                return Content("no0:验证码错误!");
            }

            string userName = Request["LoginCode"];
            string userPwd = Request["LoginPwd"];
            userPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(userPwd, "MD5");

            BLL.TUser btu = new BLL.TUser();
            Model.TUser user = btu.GetModel(userName);

            if (user == null)
            {
                return Content("no1:用户不存在!");
            }

            if (user.Pwd.ToString() != userPwd)
            {
                return Content("no2:密码错误!");
            }

            FormsAuthentication.SetAuthCookie(userName, true);
            Session["userName"] = userName.ToString();
            return Content("ok:登录成功!");

        }

        /// <summary>
        /// 用户注销登录
        /// </summary>
        /// <returns>注销登录状态</returns>
        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return View("Index");
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns>验证码图片</returns>
        public ActionResult ValidateCode()
        {
            Common.ValidateCode validateCode = new Common.ValidateCode();
            string code = validateCode.CreateValidateCode(4);
            Session["validateCode"] = code;
            byte[] buffer = validateCode.CreateValidateGraphic(code);
            return File(buffer, "image/jpeg");
        }

    }
}