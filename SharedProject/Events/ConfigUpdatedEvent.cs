using SharedProject.Models;

namespace SharedProject.Events;

public class ConfigUpdatedEvent : PubSubEvent<PomodoroConfig>;
