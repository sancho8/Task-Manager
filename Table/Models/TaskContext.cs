using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Table.Models
{
    public class TaskContext:DbContext
    {
        public DbSet<User> Users;
        public DbSet<Task> Tasks;
    }
}