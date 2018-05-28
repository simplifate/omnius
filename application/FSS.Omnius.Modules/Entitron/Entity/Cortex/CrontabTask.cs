using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Entity.Cortex
{
    [Table("Cortex_CrontabTasks")]
    public partial class CrontabTask : IEntity
    {
        public CrontabTask()
        {
            Id = 1;
            IsDeleted = false;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// after x(sec|min|hour|day|week|month|year) y(sec|min|hour|day|week|month|year)
        /// at yyyy-MM-dd hh-mm-ss
        /// (sec[0 - 59]) min[0 - 59] hour[0 - 23] dayOfMonth[1 - 31] month[1 - 31] dayOfWeek[0-6, 0=sunday]
        /// */2 - every second day
        /// 1,2
        /// 1-4
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Schedule { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public ScheduleStartAt ScheduleStart { get; set; }

        public string BlockName { get; set; }
        public string Executor { get; set; }
        public int? ModelId { get; set; }

        public DateTime? LastStartTask { get; set; }
        public DateTime? LastEndTask { get; set; }

        public int? ApplicationId { get; set; }
        public virtual Application Application { get; set; }

        public enum ScheduleStartAt
        {
            taskEnd,
            taskStart
        }
    }
}
