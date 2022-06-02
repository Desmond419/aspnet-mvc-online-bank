using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;
using Scrypt;
using ShareResources;
using ShareResources.ViewModels;
using System.Web.Security;

namespace Customer.Controllers
{
    public class UsersController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult TestAction()
        {
            return View();
        }

        public ActionResult RenderImage(int id)
        {
            User user = db.Users.Find(id);

            byte[] byteData = user.Data;

            return File(byteData, "image/png");
        }

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                ScryptEncoder encoder = new ScryptEncoder();
                bool areEquals = false;
                var user = db.Users.SingleOrDefault(u => u.Username == login.Username);
                if (user != null)
                {
                    areEquals = encoder.Compare(login.Password, user.Password);
                    if (areEquals)
                    {
                        Session["auth"] = user.Id;
                        Session.Timeout = 5;   //When timeout then Session.Abandon
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.fail = "Please Try Again. Wrong Username or Password!";
                    }
                }
            }
            return View(login);
        }

        public ActionResult LogOff()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email, string Name)
        {
            var user = db.Users.SingleOrDefault(u => u.Email == Email && u.Name == Name);
            if (user != null)
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                WebMail.EnableSsl = true;
                WebMail.UserName = "7547461@gmail.com";
                WebMail.Password = "7547461lzk";
                WebMail.From = "7547461@gmail.com";
                string password = Membership.GeneratePassword(6,1);
                user.Password = new ScryptEncoder().Encode(password);
                db.SaveChanges();
                string body = "New Password is " + password;
                WebMail.Send(to: user.Email, subject: "Maybank Password Reset", body: body, isBodyHtml: true);
                ViewBag.Status = "Email Send. Pls Check Your Mail Box.";
            }
            // If we got this far, something failed, redisplay form
            return View();
        }




        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,AccountId,Balance,Card,Ic,Username,Password,DateOfBirth,Email")] User users)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,AccountId,Balance,Card,Ic,Username,Password,DateOfBirth,Email")] User users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User users = db.Users.Find(id);
            db.Users.Remove(users);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ChangePin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePin(ChangePasswordViewModel model)
        {
            ScryptEncoder encoder = new ScryptEncoder();
            bool areEquals = false;
            if (!ModelState.IsValid)
            {
                return View();
            }

            int result = Convert.ToInt32(Session["auth"]);
            var user = db.Users.Find(result);
            if (Session["auth"] != null)
            {
                areEquals = encoder.Compare(model.OldPassword, user.Password);
                if (areEquals)
                {
                    user.Password = encoder.Encode(model.NewPassword);
                    db.SaveChanges();
                }
                ViewBag.Success = "Change Pin Successfully";
                return RedirectToAction("Index", "Home");
            }
            return View();
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
