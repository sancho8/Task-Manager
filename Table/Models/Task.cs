using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Table.Models
{
    public class Task
    {
        public int Id;
        public int UserId;
        public string Description;
        public string Data;
        public int Priority;
        public bool IsComplete;
        public Task(int id, int userid, string desc, string data, int priority, bool iscomplete)
        {
            this.Id = id;
            this.UserId = userid;
            this.Description = desc;
            this.Data = data;
            this.Priority = priority;
            this.IsComplete = iscomplete;
        }
    }
}