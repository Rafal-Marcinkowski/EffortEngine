using SharedProject.Models;

namespace SharedProject.Events;

public class CurrentTaskChangedEvent : PubSubEvent<TaskBase> { }
