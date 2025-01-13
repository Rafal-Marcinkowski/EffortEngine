using SharedProject.Models;

namespace DataAccess.Data;
public interface IPomodoroSessionData
{
    Task DeleteSessionAsync(int id);
    Task<IEnumerable<PomodoroSession>> GetAllSessionsAsync();
    Task<PomodoroSession> GetSessionAsync(int id);
    Task InsertSessionAsync(PomodoroSession session);
    Task UpdateSessionAsync(PomodoroSession session);
}