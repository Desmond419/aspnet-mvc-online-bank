using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Scrypt;
using ShareResources;
using ShareResources.ViewModels;

namespace Admin.Controllers
{
    public class UsersController : Controller
    {
        private AppDbContext db = new AppDbContext();

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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.profilePicture = user.PhotoFileName;
            return View(user);
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
                        if (user.IsAdmin)
                        {
                            Session["userId"] = user.Id;
                            Session.Timeout = 5;
                            return RedirectToAction("Index", "Home");
                        }
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
        public ActionResult Create([Bind(Include = "Id,Name,AccountNum,Balance,Card,Ic,IsAdmin,Username,Password,DateOfBirth")] User user, HttpPostedFileBase postedFile)
        {
            if (ModelState.IsValid)
            {
                byte[] bytes;

                // Image cannot be null!
                using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                {
                    bytes = br.ReadBytes(postedFile.ContentLength);
                }
                user.PhotoFileName = Path.GetFileName(postedFile.FileName);
                user.Data = bytes;
                ScryptEncoder encoder = new ScryptEncoder();
                user.Password = encoder.Encode(user.Password);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult RenderImage(int id)
        {
            User user = db.Users.Find(id);

            byte[] byteData = user.Data;

            return File(byteData, "image/png");
        }


        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,AccountId,Balance,Card,Ic,IsAdmin,Username,Password,DateOfBirth")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            bool areEquals = false;
            if (!ModelState.IsValid)
            {
                return View();
            }
            ScryptEncoder encoder = new ScryptEncoder();
            int result = Convert.ToInt32(Session["userId"]);
            var user = db.Users.Find(result);
            if (Session["userId"] != null)
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

