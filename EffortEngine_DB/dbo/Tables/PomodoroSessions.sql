CREATE TABLE [dbo].[PomodoroSessions]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [StartTime] DATETIME NOT NULL, 
    [EndTime] DATETIME NOT NULL,
    [RoundWorkTime] decimal(10,2) NOT NULL,
    [TotalWorkTime] decimal(10,2) NOT NULL,
    [BreakTime] INT NOT NULL,
    [Rounds] INT NOT NULL,
    [Status] INT NOT NULL,
    [ProgramId] INT NULL, 
    FOREIGN KEY ([ProgramId]) REFERENCES [dbo].[Programs]([Id]),
);