using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Task.Models;

namespace Task.Controllers
{
    public class AdminController : Controller
    {
        private readonly TaskManagementDBContext db = new TaskManagementDBContext();

        
        // [Authorize(Roles = "Admin")]
        public ActionResult UserList()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/CreateUser
        public ActionResult CreateUser()
        {
            return View();
        }

        // POST: Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                user.SignupDate = DateTime.Now;
                user.PasswordHash = HashPassword(user.PasswordHash); // Add password hashing logic
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("UserList");
            }
            return View(user);
        }

        
        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.Find(user.UserID);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }
                
                existingUser.Username = user.Username;
                existingUser.Role = user.Role;
                existingUser.IsBlocked = user.IsBlocked;
                existingUser.IsDeleted = user.IsDeleted;

                db.Entry(existingUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserList");
            }
            return View(user);
        }

        // GET: Admin/DeleteUser/5
        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            if (user != null)
            {
                user.IsDeleted = true;
                db.SaveChanges();
            }
            return RedirectToAction("UserList");
        }

        // GET: Admin/ViewAllTasks
        public ActionResult ViewAllTasks()
        {
            var tasks = (from t in db.TaskViews
                         join u in db.Users on t.AssignedTo equals u.UserID.ToString() into taskUsers
                         from user in taskUsers.DefaultIfEmpty() 
                         select new
                         {
                             t.TaskID,
                             t.Title,
                             t.Description,
                             t.Status,
                             AssignedTo = user != null ? user.Username : "Unassigned", 
                             t.DueDate
                         }).ToList();

            
            var taskViewModels = tasks.Select(t => new TaskView
            {
                TaskID = t.TaskID,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                AssignedTo = t.AssignedTo,
                DueDate = t.DueDate
            }).ToList();

            return View(taskViewModels);
        }

        public ActionResult CreateTask()
        {
            
            var users = db.Users.ToList();
            ViewBag.Users = new SelectList(users, "Username", "Username"); 
            return View();
        }
        // POST: Admin/CreateTask
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTask(TaskView task)
        {
            if (ModelState.IsValid)
            {
                
                task.Status = "Pending"; 
                db.TaskViews.Add(task);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            
            var users = db.Users.ToList();
            ViewBag.Users = new SelectList(users, "Username", "Username"); 
            return View(task);
        }


        
        private string HashPassword(string password)
        {
            // Implement password hashing logic here
            return password; // Replace with hashed password
        }
    }
}
