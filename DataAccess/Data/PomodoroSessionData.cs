using DataAccess.DBAccess;
using SharedProject.Models;

namespace DataAccess.Data;

public class PomodoroSessionData(ISQLDataAccess dbAccess) : IPomodoroSessionData
{
    public async Task<IEnumerable<PomodoroSession>> GetAllSessionsAsync()
    {
        string sql = "SELECT * FROM PomodoroSessions";

        return await dbAccess.LoadDataWithQueryAsync<PomodoroSession>(sql);
    }

    public async Task<PomodoroSession> GetSessionAsync(int id)
    {
        string sql = "SELECT * FROM PomodoroSessions WHERE Id = @Id";
        var sessions = await dbAccess.LoadDataWithQueryAsync<PomodoroSession>(sql, new { Id = id });

        return sessions.FirstOrDefault();
    }

    public async Task InsertSessionAsync(PomodoroSession session)
    {
        string sql = @"
            INSERT INTO PomodoroSessions 
            (StartTime, EndTime, WorkTime, BreakTime, Rounds, Status, ProgramId)
            VALUES 
            (@StartTime, @EndTime, @WorkTime, @BreakTime, @Rounds, @Status, @ProgramId)";
        await dbAccess.SaveDataWithQueryAsync(sql, session);
    }

    public async Task UpdateSessionAsync(PomodoroSession session)
    {
        string sql = @"
            UPDATE PomodoroSessions
            SET StartTime = @StartTime, 
                EndTime = @EndTime, 
                WorkTime = @WorkTime, 
                BreakTime = @BreakTime, 
                Rounds = @Rounds, 
                Status = @Status,
                ProgramId = @ProgramId
            WHERE Id = @Id";
        await dbAccess.SaveDataWithQueryAsync(sql, session);
    }

    public async Task DeleteSessionAsync(int id)
    {
        string sql = "DELETE FROM PomodoroSessions WHERE Id = @Id";
        await dbAccess.SaveDataWithQueryAsync(sql, new { Id = id });
    }
}
