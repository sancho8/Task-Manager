using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Table.Models
{
    [Serializable]
    public class Task
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public string Data { get; set; }
        public int Priority { get; set; }
        public bool IsComplete { get; set; }
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