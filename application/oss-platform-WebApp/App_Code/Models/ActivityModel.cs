using System;
using System.Collections.Generic;

namespace FSSWorkflowDesigner.Models
{
    public enum ActivityTypes
    {
        StartWorkflow = 0,
        Decision,
        Process,
        EndWorkflow,
        Delay,
        WaitForInputs,
        StartTimer,
        CheckTimeout,
        PerformTwoActions,
        PerformThreeActions
    }
    public class Output
    {
        public int Id { get; set; }
        public int Target { get; set; }
        public int SourceSlot { get; set; }
        public int TargetSlot { get; set; }

        public virtual Activity Activity { get; set; }
    }
    public class Input
    {
        public int Id { get; set; }
        public int Source { get; set; }
        public int Slot { get; set; }

        public virtual Activity Activity { get; set; }
    }
    public class Activity
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        
        public virtual ICollection<Input> Inputs { get; set; }
        public virtual ICollection<Output> Outputs { get; set; }

        public virtual Commit Commit { get; set; }

        public Activity()
        {
            Inputs = new List<Input>();
            Outputs = new List<Output>();
        }
    }
    public class Commit
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }

        public Commit()
        {
            Activities = new List<Activity>();
        }
    }
}