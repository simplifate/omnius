﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Entity.Cortex
{
    public enum ScheduleType
    {
        DAILY = 1, WEEKLY = 2, MONTHLY = 4, ONCE = 8, ONIDLE = 16
    }

    public enum MonthlyType
    {
        IN = 1, DAYS
    }

    public enum RepetitionMinutes
    {
        m1 = 1, m2 = 2, m3 = 3, m4 = 4, m5 = 5, m6 = 6, m10 = 10, m12 = 12, m15 = 15, m20 = 20, m30 = 30, m60 = 60
    }

    [Flags]
    public enum Days
    {
        Monday = 1, Tuesday = 2, Wednesday = 4, Thursday = 8, Friday = 16, Saturday = 32, Sunday = 64
    }

    [Flags]
    public enum DaysInMonth : Int64
    {
        _1 = 1, _2 = 2, _3 = 4, _4 = 8, _5 = 16, _6 = 32, _7 = 64, _8 = 128, _9 = 256, _10 = 512,
        _11 = 1024, _12 = 2048, _13 = 4096, _14 = 8192, _15 = 16384, _16 = 32768, _17 = 65536, _18 = 131072, _19 = 262144, _20 = 524288,
        _21 = 1048576, _22 = 2097152, _23 = 4194304, _24 = 8388608, _25 = 16777216, _26 = 33554432, _27 = 67108864, _28 = 134217728, _29 = 268435456, _30 = 536870912,
        _31 = 1073741824, Last = 2147483648
    }

    [Flags]
    public enum Months
    {
        January = 1, February = 2, March = 4, April = 8, May = 16, June = 32, July = 64, August = 128, September = 256, October = 512, November = 1024, December = 2048
    }

    [Flags]
    public enum InModifiers
    {
        FIRST = 1, SECOND = 2, THIRD = 4, FOURTH = 8, LAST = 16
    }
   
    [Table("Cortex_Task")]
    public partial class Task : IEntity
    {
        public int? Id { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [ForeignKey("Application")]
        [Display(Name = "Application")]
        public int? AppId { get; set; }

        [Display(Name = "Is active")]
        public bool Active { get; set; }
        
        [Required]
        [StringLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Schedule type")]
        public ScheduleType Type { get; set; }

        [Required]
        [StringLength(400)]
        [Display(Name = "Task URL")]
        public string Url { get; set; }

        [Display(Name = "Repeat")]
        public bool Repeat { get; set; }

        [Range(1, 1439)]
        [Display(Name = "Repeat every (minutes)")]
        public RepetitionMinutes? Repeat_Minute { get; set; }

        [Range(1, 23)]
        [Display(Name = "For")]
        public int? Repeat_Duration { get; set; }

        [Range(1,365)]
        [Display(Name = "Repeat every (days)")]
        public int? Daily_Repeat { get; set; }

        [Range(1,52)]
        [Display(Name = "Repeat every (week)")]
        public int? Weekly_Repeat { get; set; }
        
        // Flags: Days
        public int? Weekly_Days { get; set; }

        public MonthlyType? Monthly_Type { get; set; }

        // Flags: Months
        [Display(Name = "Months")]
        public int? Monthly_Months { get; set; }

        // Flags: DaysInMonth
        [Display(Name = "Days")]
        public Int64? Monthly_Days { get; set; }

        // Flags: InModifiers
        public int? Monthly_In_Modifiers { get; set; }

        // Flags: Days
        public int? Monthly_In_Days { get; set; }

        [Range(1,999)]
        [Display(Name = "Idle time")]
        public int? Idle_Time { get; set; }

        [Required]
        [Column(TypeName = "time")]
        [Display(Name = "Task start")]
        public TimeSpan Start_Time { get; set; }

        [Column(TypeName = "time")]
        [Display(Name = "Task end")]
        public TimeSpan? End_Time { get; set; }

        [Column(TypeName = "time")]
        [Display(Name = "Duration")]
        public TimeSpan? Duration { get; set; }

        [Required]
        [Column(TypeName = "date")]
        [Display(Name = "Start date")]
        public DateTime? Start_Date { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "End date")]
        public DateTime? End_Date { get; set; }

        public virtual Application Application { get; set; } 
    }
}
