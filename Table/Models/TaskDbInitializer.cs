using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Table.Models
{
    public class TaskDbInitializer: DropCreateDatabaseAlways<TaskContext>
    {
            protected override void Seed(TaskContext db)
            {
            db.Tasks.Add(new Task(1, 1,"First Task", DateTime.Now, "A", 2, true));
            db.Tasks.Add(new Task(1, 1, "First Task", DateTime.Now, "A", 2, true));
            db.Tasks.Add(new Task(1, 1, "First Task", DateTime.Now, "A", 2, true));
            base.Seed(db);
            }
    }
}