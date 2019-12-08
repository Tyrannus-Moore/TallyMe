using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TallyMe.Controllers
{
    public class RegisterController : Controller
    {
        BLL.TUser btu = new BLL.TUser();

        // GET: Register
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public bool IsValid(string userName)
        {
            Model.TUser tuser = btu.GetModel(userName);

            return tuser == null ? false : true;
        }

        public ActionResult Register()
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
            string userPwd1 = Request["LoginPwd1"];
            string userPwd2 = Request["LoginPwd2"];

            if (userName == "")
            {
                return Content("no1:用户名不能为空!");
            }
            if (btu.GetModel(userName) != null)
            {
                return Content("no1:用户名重复");
            }

            if (userPwd1 == "" || userPwd2 == "")
            {
                return Content("no2:密码不能为空!");
            }
            if (userPwd1 != userPwd2)
            {
                return Content("no2:两次密码不一致!");
            }

            Model.TUser user = new Model.TUser
            {
                Name = userName,
                Pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(userPwd1, "MD5")
            };

            btu.Add(user);

            FormsAuthentication.SetAuthCookie(userName, true);
            Session["userName"] = userName.ToString();
            return Content("ok:注册成功!");
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