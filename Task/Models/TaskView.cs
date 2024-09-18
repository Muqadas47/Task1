using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task.Models
{


    
        public enum TaskPriority
        {
            Low,
            Medium,
            High,
            Urgent
        }
        public class TaskView
        {

            public int TaskID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
            public string AssignedTo { get; set; }
            public DateTime DueDate { get; set; }
            public string Priority { get; set; }
            public Nullable<System.DateTime> CreatedAt { get; set; }



        
    }

}
