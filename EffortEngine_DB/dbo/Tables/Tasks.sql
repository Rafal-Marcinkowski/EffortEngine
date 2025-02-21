CREATE TABLE [dbo].[Tasks]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [Priority] INT NOT NULL,
    [CreatedAt] DATETIME NOT NULL,
    [LastUpdated] DATETIME NOT NULL,
    [DueDate] DATETIME NULL,
    [CompletionDate] DATETIME NULL,
    [Status] INT NOT NULL,
    [Type] INT NOT NULL,
    [TotalWorkTime] DECIMAL(10, 2) NOT NULL,
    [Note] NVARCHAR(250) NULL,
    [ProgramId] INT NULL,
    FOREIGN KEY ([ProgramId]) REFERENCES [dbo].[Programs]([Id])
);