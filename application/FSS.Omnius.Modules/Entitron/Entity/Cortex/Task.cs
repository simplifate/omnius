using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Entity.Cortex
{
    public enum ScheduleType
    {
        MINUTE = 1, HOURLY = 2, DAILY = 4, WEEKLY = 8, MONTHLY = 16, ONCE = 32, ONSTART = 64, ONIDLE = 128
    }

    public enum MonthlyType
    {
        IN = 1, DAYS
    }

    [Flags]
    public enum Days
    {
        MON = 1, TUE = 2, WED = 4, THU = 8, FRI = 16, SAT = 32, SUN = 64
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
        [Display(Name = "Aplikace")]
        public int? AppId { get; set; }

        [Display(Name = "Aktivní")]
        public bool Active { get; set; }
        
        [Required]
        [StringLength(255)]
        [Display(Name = "Název")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Typ spuštění")]
        public ScheduleType Type { get; set; }

        [Required]
        [StringLength(400)]
        [Display(Name = "URL úlohy")]
        public string Url { get; set; }

        [Range(1, 1439)]
        [Display(Name = "Opakovat každých")]
        public int? Minute_Repeat { get; set; }

        [Range(1, 23)]
        [Display(Name = "Opakovat každých")]
        public int? Hourly_Repeat { get; set; }

        [Range(1,365)]
        [Display(Name = "Opakovat každých")]
        public int? Daily_Repeat { get; set; }

        [Range(1,52)]
        [Display(Name = "Opakovat každých")]
        public int? Weekly_Repeat { get; set; }
        
        // Flags: Days
        public int? Weekly_Days { get; set; }

        public MonthlyType? Monthly_Type { get; set; }

        // Flags: Months
        [Display(Name = "Měsíce")]
        public int? Monthly_Months { get; set; }

        // Flags: DaysInMonth
        [Display(Name = "Dny")]
        public Int64? Monthly_Days { get; set; }

        // Flags: InModifiers
        public int? Monthly_In_Modifiers { get; set; }

        // Flags: Days
        public int? Monthly_In_Days { get; set; }

        [Range(1,999)]
        [Display(Name = "Doba nečinosti")]
        public int? Idle_Time { get; set; }

        [Required]
        [Column(TypeName = "time")]
        [Display(Name = "Začátek úlohy")]
        public TimeSpan Start_Time { get; set; }

        [Column(TypeName = "time")]
        [Display(Name = "Konec úlohy")]
        public TimeSpan? End_Time { get; set; }

        [Column(TypeName = "time")]
        [Display(Name = "Trvání")]
        public TimeSpan? Duration { get; set; }

        [Required]
        [Column(TypeName = "date")]
        [Display(Name = "Datum spuštění")]
        public DateTime? Start_Date { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Datum ukončení")]
        public DateTime? End_Date { get; set; }

        public Application Application { get; set; } 
    }
}
