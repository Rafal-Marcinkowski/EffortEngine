CREATE TABLE [dbo].[PomodoroSessionTasks]
(
    [PomodoroSessionId] INT NOT NULL, 
    [TaskId] INT NOT NULL, 
    PRIMARY KEY ([PomodoroSessionId], [TaskId]), 
    FOREIGN KEY ([PomodoroSessionId]) REFERENCES [dbo].[PomodoroSessions]([Id]),
    FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks]([Id])
);