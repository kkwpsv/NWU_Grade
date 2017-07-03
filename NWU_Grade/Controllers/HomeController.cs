using Lsj.Util.Text;
using NWU_Grade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NWU_Grade.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(UserModel model)
        {
            using (var wcfclient = new WCFService.MURPNewsServiceSoapClient())
            {
                if (model.ID.IsNullOrEmpty())
                {
                    ModelState.AddModelError("", "请填写学号");
                }
                else if (model.ID.Length != 10 || model.ID.ConvertToInt(0) == 0)
                {
                    ModelState.AddModelError("", "请填写正确的学号");
                }

                else if (model.Name.IsNullOrEmpty())
                {
                    ModelState.AddModelError("", "请填写姓名");
                }
                else
                {
                    var user = wcfclient.SearchUser(model.ID, null, null).userinfo;
                    if (user == null || user.Count() == 0)
                    {
                        ModelState.AddModelError("", "找不到该学号对应的学生");
                    }
                    else if (user[0].customname != model.Name)
                    {
                        ModelState.AddModelError("", "姓名和学号不匹配");
                    }
                    else
                    {
                        var grades = wcfclient.Mark(user[0].mcid.ConvertToInt(0)).Select(x => new Tuple<string, string>(x.Extend1, x.Extend2)).ToList();
                        return View("Grade", new GradeModel { Data = grades });
                    }
                }
                return View(model);
            }

        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}