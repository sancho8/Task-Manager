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
        public string Priority { get; set; }
        public int Number { get; set; }
        public bool IsComplete { get; set; }
        public Task(int id, int userid, string desc, string data, string priority, int number, bool iscomplete)
        {
            this.Id = id;
            this.UserId = userid;
            this.Description = desc;
            this.Data = data;
            this.Priority = priority;
            this.Number = number;
            this.IsComplete = iscomplete;
        }
    }
}