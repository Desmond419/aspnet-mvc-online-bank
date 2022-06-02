using Customer.Utilities;
using ShareResources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Customer.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            var userId = Session["auth"];
            if (userId != null)
            {
                var user = db.Users.Find(userId);
                return View(user);
            }
            else
            {
                Log.Error("No user found");
                return RedirectToAction("Login", "Users");
            }
        }


        [HttpGet]
        public ActionResult NewChart()
        {
            var output = db.Transactions.ToList().Where(t => t.UserId == 1); //This is the userID that chart bar show.
            List<object> iData = new List<object>();

            //Create Sample Data Here!
            DataTable dt = new DataTable();
            dt.Columns.Add("Employee", Type.GetType("System.String"));
            dt.Columns.Add("Credit", Type.GetType("System.Int32"));

            DataRow dr = dt.NewRow();
            dr["Employee"] = "Income";
            //dr["Credit"] = output.Sum(t => t.Debit);
            dr["Credit"] = 1000;
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Employee"] = "Expenses";
            //dr["Credit"] = output.Sum(t => t.Credit);
            dr["Credit"] = 2000;
            dt.Rows.Add(dr);

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }
            return Json(iData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}