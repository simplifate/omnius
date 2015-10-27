namespace FSS.FSPOC.BussinesObjects.Entities.Workflow
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
}