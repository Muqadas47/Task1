using System;
using System.Collections.Generic;
using BCrypt.Net;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Task.Models;
using Microsoft.Ajax.Utilities;

namespace Task.Controllers
{
    public class AdminController : Controller
    {
        private readonly TaskManagementDBContext db = new TaskManagementDBContext();


        //[Authorize(UserRole = "Admin")]
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
                // Hash the password before saving
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                user.SignupDate = DateTime.Now;
                user.IsBlocked = false;
                user.IsDeleted = false;

                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("UserList"); // Redirect to a relevant action
            }

            return View(user);
        }

        // GET: Admin/EditUser/5
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

        // POST: Admin/DeleteUser/5
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
        public ActionResult ViewAllTasks(string status, string assignedTo, string title, string searchTerm)
        {
            var tasksQuery = from t in db.TaskViews
                             join u in db.Users on t.AssignedTo equals u.UserID.ToString() into taskUsers
                             from user in taskUsers.DefaultIfEmpty()

                             select new
                             {
                                 t.TaskID,
                                 t.Title,
                                 t.Description,
                                 t.Status,
                                 AssignedTo = user != null ? user.Username : "Unassigned",
                                 t.DueDate,
                                 t.Priority,         // New column
                                 t.CreatedAt,        // New column

                             };

            // Apply filters
            if (!string.IsNullOrEmpty(status))
            {
                tasksQuery = tasksQuery.Where(t => t.Status == status);
            }

            if (!string.IsNullOrEmpty(assignedTo))
            {
                tasksQuery = tasksQuery.Where(t => t.AssignedTo == assignedTo);
            }

            if (!string.IsNullOrEmpty(title))
            {
                tasksQuery = tasksQuery.Where(t => t.Title == title);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                tasksQuery = tasksQuery.Where(t => t.Title.Contains(searchTerm) ||
                                                   t.Description.Contains(searchTerm) ||
                                                   t.AssignedTo.Contains(searchTerm));
            }

            var taskViewModels = tasksQuery.Select(t => new TaskViews
            {
                TaskID = t.TaskID,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                AssignedTo = t.AssignedTo,
                DueDate = t.DueDate,
                Priority = t.Priority,     // New column
                CreatedAt = t.CreatedAt,   
                                           
            }).ToList();

            // Populate ViewBag with list of usernames
            ViewBag.Users = db.Users.Select(u => u.Username).ToList();

            return View(taskViewModels);
        }








        public ActionResult CreateTask()
        {
            // Fetch users for the dropdown list
            var users = db.Users.ToList();
            ViewBag.Users = new SelectList(users, "Username", "Username");

            return View();
        }

        [HttpPost]
        public ActionResult CreateTask(TaskView model)
        {
            if (ModelState.IsValid)
            {
                // Check if Status is set; if not, assign a default value
                if (string.IsNullOrEmpty(model.Status))
                {
                    model.Status = "Pending"; // Set a default status
                }

                // Add the new task to the database
                db.TaskViews.Add(new TaskView
                {
                    Title = model.Title,
                    Description = model.Description,
                    Status = model.Status,
                    AssignedTo = model.AssignedTo,
                    DueDate = model.DueDate,
                    Priority = model.Priority,
                    CreatedAt = DateTime.Now // Set CreatedAt to the current date/time
                });

                db.SaveChanges();

                // Redirect to the task list or another relevant view
                return RedirectToAction("ViewAllTasks");
            }

            // Fetch users for the dropdown list if validation fails
            var users = db.Users.ToList();
            ViewBag.Users = new SelectList(users, "Username", "Username");

            return View(model);
        }


        public ActionResult TaskReport()
        {
            // Get total task count
            var totalTasks = db.TaskViews.Count();

            // Get completed task count
            var completedTasks = db.TaskViews.Count(t => t.Status == "Completed");

            // Get tasks in progress
            var inProgressTasks = db.TaskViews.Count(t => t.Status == "In Progress");

            // Get pending tasks
            var pendingTasks = db.TaskViews.Count(t => t.Status == "Pending");

            // Calculate completion rate (completed tasks / total tasks)
            double completionRate = totalTasks > 0 ? ((double)completedTasks / totalTasks) * 100 : 0;

            // Prepare the report data in a model or ViewBag
            ViewBag.TotalTasks = totalTasks;
            ViewBag.CompletedTasks = completedTasks;
            ViewBag.InProgressTasks = inProgressTasks;
            ViewBag.PendingTasks = pendingTasks;
            ViewBag.CompletionRate = completionRate;

            return View();
        }

        public ActionResult GenerateReport()
        {
            var reportData = GetReportData();
            var reportFile = ReportGenerator.GeneratePdf(reportData);
            return File(reportFile, "application/pdf", "Report.pdf");
        }

        private IEnumerable<ReportItem> GetReportData()
        {
            // Replace with actual data retrieval logic
            return db.TaskViews.Select(t => new ReportItem
            {
                TaskName = t.Title,
                DueDate = t.DueDate
            }).ToList();
        }

        private string HashPassword(string password)
        {
            // Implement password hashing logic here
            return password; // Replace with hashed password
        }
    }
}
