using Quartz;
using Quartz.Impl;

namespace Table.Jobs
{
    public class EmailSheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailSender>().Build();

            // Setting trigger.
            ITrigger trigger = TriggerBuilder.Create()
                // Identify trigger with group and name.
                .WithIdentity("trigger1", "group1")     
                // Starts with launch of application.
                .StartNow()                          
                // Setting shedule for task.
                // Repeats every 24 hours   
                .WithSimpleSchedule(x => x              
                    .WithIntervalInHours(24)            
                    .RepeatForever())                                                                
                .Build();                              
            // Start this job.
            scheduler.ScheduleJob(job, trigger);
        }
    }
}