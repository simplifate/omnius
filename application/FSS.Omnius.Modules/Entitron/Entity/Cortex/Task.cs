using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Entity.Cortex
{
    public enum ScheduleType
    {
        MINUTE, HOURLY, DAILY, WEEKLY, MONTHLY, ONCE, ONSTART, ONIDLE
    }

    [Flags]
    public enum Days
    {
        MON = 1, TUE, WED, THU, FRI, SAT, SUN
    }

    [Flags]
    public enum Months
    {
        JAN = 1, FEB, MAR, APR, MAY, JUNE, JULY, AUG, SEPT, OCT, NOV, DEC
    }

    [Flags]
    public enum InModifiers
    {
        FIRST = 1, SECOND, THIRD, FOURTH, LAST
    }

    [Table("Cortex_Task")]
    public partial class Task
    {
        public int Id { get; set; }

        [Index(IsClustered = false, IsUnique = false)]
        [ForeignKey("Application")]
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
        [Display(Name = "Opakování")]
        public int? Minute_Repeat { get; set; }

        [Range(1, 23)]
        [Display(Name = "Opakování")]
        public int? Hourly_Repeat { get; set; }

        [Range(1,365)]
        [Display(Name = "Opakování")]
        public int? Daily_Repeat { get; set; }

        [Range(1,52)]
        [Display(Name = "Opakování")]
        public int? Weekly_Repeat { get; set; }
        
        public Days? Weekly_Days { get; set; }

        [Display(Name = "Měsíce")]
        public Months? Monthly_Months { get; set; }

        [Display(Name = "Dny")]
        public Days? Monthly_Days { get; set; }

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
        public DateTime Start_Date { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Datum ukončení")]
        public DateTime? End_Date { get; set; }

        public Application Application { get; set; } 
    }
}
