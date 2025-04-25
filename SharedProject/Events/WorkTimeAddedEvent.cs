namespace SharedProject.Events;

public class WorkTimeAddedEvent : PubSubEvent<WorkTimeAddedEventArgs>;

public class WorkTimeAddedEventArgs(int Id, decimal workTimeToAdd, bool isProgram = false)
{
    public int Id { get; set; } = Id;
    public decimal WorkTimeToAdd { get; set; } = workTimeToAdd;
    public bool IsProgram { get; set; } = isProgram;
}