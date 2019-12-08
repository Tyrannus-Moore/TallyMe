using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TallyMe.Controllers
{
    public class AnalyseController : Controller
    {
        BLL.TUser btu = new BLL.TUser();
        BLL.Bill bbi = new BLL.Bill();


        [Authorize]
        public ActionResult Index()
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            ViewData["analysis"] = bbi.GetBillsAnalysis(tuser, DateTime.Now); 

            return View();
        }

        /// <summary>
        /// 下载用户使用记账通的EXCEL表
        /// </summary>
        /// <returns>EXCEL表</returns>
        [Authorize]
        public ActionResult DownLoad()
        {
            Model.TUser tuser = btu.GetModel(User.Identity.Name);

            string fileLoc = Server.MapPath("~") + "TUserRecords\\"+tuser.Name+"\\Excel\\";

            DirectoryInfo dir = new DirectoryInfo(fileLoc);
            if(!dir.Exists)dir.Create();

            string fileName = tuser.Id + "-" + tuser.Name + "-Bills.xlsx";
            bbi.SaveBillsAsExcel(tuser, fileLoc);
            fileLoc += "\\" + fileName;
            return File(fileLoc, "text/plain", fileName);
        }

    }
}