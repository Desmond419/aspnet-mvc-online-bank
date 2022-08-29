using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ShareResources;

namespace Customer.Controllers
{
    public class TransactionsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        [NonAction]
        public bool CheckSession()
        {
            return Session["auth"] == null;
        }

        public void SendEmail(string to, string name,int amount)
        {
            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.SmtpUseDefaultCredentials = true;
            WebMail.EnableSsl = true;
            WebMail.UserName = "";
            WebMail.Password = "";
            WebMail.From = "7547461@gmail.com";
            string body = "We would like to inform you that " + name + " has transfered RM" + amount + " to your account on " + DateTime.Now;
            WebMail.Send(to: to, subject: "Maybank Interbank Transfer", body: body, isBodyHtml: true);
            ViewBag.Status = "Email Sent Successfully.";
        }

        // GET: Transaction
        public ActionResult Index()
        {
            return View(db.Transactions.ToList());
        }

        // GET: Transaction/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction Transaction = db.Transactions.Find(id);
            if (Transaction == null)
            {
                return HttpNotFound();
            }
            return View(Transaction);
        }

        public ActionResult ThirdPartyTransfer()
        {
            if (CheckSession())
            {
                return RedirectToAction("Login", "Users");
            }
            return View();
        }


        [HttpPost]
        public ActionResult ThirdPartyTransfer(int TransactionAmount, string AccountNumber, string to)
        {
            if (TransactionAmount == 0 )
            {
                var checkUser = db.Users.SingleOrDefault(u => u.AccountNum == AccountNumber);
                if (checkUser != null)
                {
                    return Json(checkUser.Name, JsonRequestBehavior.AllowGet);
                }
            }
            int userId = Convert.ToInt32(Session["auth"]);
            var currentUser = db.Users.SingleOrDefault(u => u.Id == userId);
            var receiver = db.Users.SingleOrDefault(u => u.AccountNum == AccountNumber);

            if (currentUser.Balance <= 20 || TransactionAmount > (currentUser.Balance - 20))
            {
                ViewBag.Fail = "Third party transfer failed. You do not have enough fund to transfer";
                return View();
            }
            else if(TransactionAmount < 1)
            {
                ViewBag.Fail = "Invalid transfer amount. Third party transfer failed";
                return View();
            }

            currentUser.Balance = currentUser.Balance - TransactionAmount;
            receiver.Balance = receiver.Balance + TransactionAmount;            
            Transaction t = new Transaction
            {
                Type = "Transfer",
                UserId = userId,
                TransactionAmount = TransactionAmount,
                BankAccName = receiver.Name,
                SenderName = currentUser.Name,
                TransactionDate = DateTime.Now
            };
            db.Transactions.Add(t);
            db.SaveChanges();
            //SendEmail(to, currentUser.Name,TransactionAmount);
            ViewBag.Success = "Third Party Transfer Successfully.";

            return View();
        }

        public ActionResult TransactionHistory()
        {
            int userId = Convert.ToInt32(Session["auth"]);
            var result = db.Transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.TransactionDate);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TransactionHistory(DateTime fromDate, DateTime toDate)
        {
            int userId = Convert.ToInt32(Session["auth"]);
            DateTime _toDate = toDate.AddDays(1);
            var result = db.Transactions.Where(t => t.TransactionDate >= fromDate && t.TransactionDate < _toDate && t.UserId == userId);
            return View(result);
        }

        public ActionResult MonthlyStatement()
        {
            int userId = Convert.ToInt32(Session["auth"]);
            
            ViewBag.MonthList = ListOfMonths();
            IEnumerable<Transaction> list = db.Transactions.Where(t => t.TransactionDate.Month == DateTime.Now.Month - 1 && t.UserId == userId);
            return View(list);
        }

        [NonAction]
        public List<SelectListItem> ListOfMonths()
        {
            var month = DateTime.Now.Month;
            //var result = db.Transactions.Where(t => t.TransactionDate.Month >= DateTime.Now.Month - 6 && t.TransactionDate.Month <= DateTime.Now.Month - 1 && t.UserId == userId);
            List<SelectListItem> listOfMonths = new List<SelectListItem>();
            int a = 1;
            bool checkMonth = true;
            for (int i = 1; i <= 6; i++)
            {
                if (month - a > 0 && checkMonth)
                {
                    listOfMonths.Add(new SelectListItem() { Text = (month - a).ToString("D2") + "/" + DateTime.Now.Year, Value = (month - a).ToString("D2") + "/" + DateTime.Now.Year });
                    a++;
                }
                else
                {
                    if (a == month)
                    {
                        checkMonth = false;
                        a = 12;
                    }
                    listOfMonths.Add(new SelectListItem() { Text = a.ToString("D2") + "/" + (DateTime.Now.Year - 1), Value = a.ToString("D2") + "/" + (DateTime.Now.Year - 1) });
                    a--;
                }

            }
            return listOfMonths;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyStatement(string MonthSelection)
        {
            int month = Convert.ToInt32(MonthSelection.Substring(0, 2));
            int year = Convert.ToInt32(MonthSelection.Substring(3, 4));
            int userId = Convert.ToInt32(Session["auth"]);
            var result = db.Transactions.Where(t => t.TransactionDate.Month == month && t.TransactionDate.Year == year && t.UserId == userId);
            //DateTime _toDate = toDate.AddDays(1);
            //var result = db.Transactions.Where(t => t.TransactionDate >= fromDate && t.TransactionDate < _toDate && t.UserId == userId);
            ViewBag.MonthList = ListOfMonths();
            return View(result);
        }


        // GET: Transaction/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BankAccName,SenderName,Type,TransactionAmount,TransactionDate")] Transaction Transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(Transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Transaction);
        }

        // GET: Transaction/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction Transaction = db.Transactions.Find(id);
            if (Transaction == null)
            {
                return HttpNotFound();
            }
            return View(Transaction);
        }

        // POST: Transaction/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankAccName,SenderName,Type,TransactionAmount,TransactionDate")] Transaction Transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Transaction);
        }

        // GET: Transaction/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction Transaction = db.Transactions.Find(id);
            if (Transaction == null)
            {
                return HttpNotFound();
            }
            return View(Transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction Transaction = db.Transactions.Find(id);
            db.Transactions.Remove(Transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
