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

    [Flags]
    public enum Days
    {
        MON = 1, TUE = 2, WED = 4, THU = 8, FRI = 16, SAT = 32, SUN = 64
    }

    [Flags]
    public enum DaysInMonth : Int64
    {
        D1 = 1, D2 = 2, D3 = 4, D4 = 8, D5 = 16, D6 = 32, D7 = 64, D8 = 128, D9 = 256, D10 = 512,
        D11 = 1024, D12 = 2048, D13 = 4096, D14 = 8192, D15 = 16384, D16 = 32768, D17 = 65536, D18 = 131072, D19 = 262144, D20 = 524288,
        D21 = 1048576, D22 = 2097152, D23 = 4194304, D24 = 8388608, D25 = 16777216, D26 = 33554432, D27 = 67108864, D28 = 134217728, D29 = 268435456, D30 = 536870912,
        D31 = 1073741824, LAST = 2147483648
    }

    [Flags]
    public enum Months
    {
        JAN = 1, FEB = 2, MAR = 4, APR = 8, MAY = 16, JUNE = 32, JULY = 64, AUG = 128, SEPT = 256, OCT = 512, NOV = 1024, DEC = 2048
    }

    [Flags]
    public enum InModifiers
    {
        FIRST = 1, SECOND = 2, THIRD = 4, FOURTH = 8, LAST = 16
    }
   
    [Table("Cortex_Task")]
    public partial class Task
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
        
        public Days? Weekly_Days { get; set; }

        [Display(Name = "Měsíce")]
        public Months? Monthly_Months { get; set; }

        [Display(Name = "Dny")]
        public DaysInMonth? Monthly_Days { get; set; }

        public InModifiers? Monthly_In_Modifiers { get; set; }
        public Days? Monthly_In_Days { get; set; }

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
