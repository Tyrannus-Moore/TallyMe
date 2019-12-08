using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TallyMe.Controllers
{
    public class ProfileController : Controller
    {
        BLL.TUser btu = new BLL.TUser();
        BLL.Bill bbi = new BLL.Bill();

        [Authorize]
        public ActionResult Index()
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            ViewData["user"] = tuser;

            ViewData["Name"] = tuser.Name;

            ViewData["Pwd"] = tuser.Pwd;

            ViewData["Sex"] = tuser.Sex == true ? 0 : 1;

            ViewData["PhoneNum"] = tuser.PhoneNum;

            ViewData["Email"] = tuser.Email;

            ViewData["Abode"] = SetList(tuser.Abode);

            ViewData["Birthday"] = tuser.Birthday.ToString();

            if (tuser.IsPic == true)
            {
                ViewData["PicUrl"] = tuser.PicUrl.ToString();
            }
            else
            {
                ViewData["PicUrl"] = "\\Content\\Images\\default-profile.jpg";
            }

            return View();
        }

        [HttpPost]
        public RedirectResult SetProfile(Model.TUser tuser)
        {
            Model.TUser oldUser = btu.GetModel(tuser.Name);

            tuser.Id = oldUser.Id;

            if (Request["Sex"].ToString() == "0")
            {
                tuser.Sex = true;
            }
            else tuser.Sex = false;

            tuser.Birthday = GetDateTime(Convert.ToInt32(Request["date"].ToString()));

            if (oldUser.Pwd != tuser.Pwd && tuser.Pwd != "")
            {
                tuser.Pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(tuser.Pwd, "MD5");
            }

            if (Request["ImagePath"] != null)
            {
                tuser.IsPic = true;
                tuser.PicUrl = Request["ImagePath"].ToString();
            }

            bool result = btu.Update(tuser);

            return Redirect("Index");
        }

        [HttpPost]
        public Boolean ResetRecord(string name)
        {
            bool result = bbi.DeleteBatch(name);

            return result;
        }

        private List<SelectListItem> SetList(string province)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "北京市", Value = "北京市" });
            items.Add(new SelectListItem { Text = "天津市", Value = "天津市" });
            items.Add(new SelectListItem { Text = "上海市", Value = "上海市" });
            items.Add(new SelectListItem { Text = "重庆市", Value = "重庆市" });
            items.Add(new SelectListItem { Text = "河北省", Value = "河北省" });
            items.Add(new SelectListItem { Text = "河南省", Value = "河南省" });
            items.Add(new SelectListItem { Text = "云南省", Value = "云南省" });
            items.Add(new SelectListItem { Text = "辽宁省", Value = "辽宁省" });
            items.Add(new SelectListItem { Text = "黑龙江省", Value = "黑龙江省" });
            items.Add(new SelectListItem { Text = "湖南省", Value = "湖南省" });
            items.Add(new SelectListItem { Text = "安徽省", Value = "安徽省" });
            items.Add(new SelectListItem { Text = "山东省", Value = "山东省" });
            items.Add(new SelectListItem { Text = "新疆维吾尔", Value = "新疆维吾尔" });
            items.Add(new SelectListItem { Text = "江苏省", Value = "江苏省" });
            items.Add(new SelectListItem { Text = "浙江省", Value = "浙江省" });
            items.Add(new SelectListItem { Text = "江西省", Value = "江西省" });
            items.Add(new SelectListItem { Text = "湖北省", Value = "湖北省" });
            items.Add(new SelectListItem { Text = "广西壮族", Value = "广西壮族" });
            items.Add(new SelectListItem { Text = "甘肃省", Value = "甘肃省" });
            items.Add(new SelectListItem { Text = "山西省", Value = "山西省" });
            items.Add(new SelectListItem { Text = "内蒙古", Value = "内蒙古" });
            items.Add(new SelectListItem { Text = "陕西省", Value = "陕西省" });
            items.Add(new SelectListItem { Text = "吉林省", Value = "吉林省" });
            items.Add(new SelectListItem { Text = "福建省", Value = "福建省" });
            items.Add(new SelectListItem { Text = "贵州省", Value = "贵州省" });
            items.Add(new SelectListItem { Text = "广东省", Value = "广东省" });
            items.Add(new SelectListItem { Text = "青海省", Value = "青海省" });
            items.Add(new SelectListItem { Text = "西藏", Value = "西藏" });
            items.Add(new SelectListItem { Text = "四川省", Value = "四川省" });
            items.Add(new SelectListItem { Text = "宁夏回族", Value = "宁夏回族" });
            items.Add(new SelectListItem { Text = "海南省", Value = "海南省" });
            items.Add(new SelectListItem { Text = "台湾省", Value = "台湾省" });
            items.Add(new SelectListItem { Text = "香港特别行政区", Value = "香港特别行政区" });
            items.Add(new SelectListItem { Text = "澳门特别行政区 ", Value = "澳门特别行政区 " });
            items.Add(new SelectListItem { Text = "海外", Value = "海外" });

            foreach(var item in items)
            {
                if (item.Value == province) item.Selected = true;
            }

            return items;
        }

        /// <summary>  
        /// 时间戳Timestamp转换成日期  
        /// </summary>  
        /// <param name="timeStamp"></param>  
        /// <returns></returns>  
        private DateTime GetDateTime(int timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = ((long)timeStamp * 10000000);
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }

        public ActionResult FileUpload()
        {
            HttpPostedFileBase file = Request.Files["MenuIcon"];
            if (file == null)
            {
                return Content("no:上传文件不能为空!");
            }
            else
            {
                string fileName = Path.GetFileName(file.FileName); // 获取文件名称
                string fileExt = Path.GetExtension(fileName); // 获取文件扩展名


                string dir = "/TUserRecords/" + User.Identity.Name + "/Profile/";
                Directory.CreateDirectory(Path.GetDirectoryName(Request.MapPath(dir))); // 创建文件夹
                string newfileName = Guid.NewGuid().ToString();  // 新的文件名
                string fullDir = dir + newfileName + fileExt; // 完整路径
                file.SaveAs(Request.MapPath(fullDir));
                return Content("ok:" + fullDir);
            }
        }


    }
}