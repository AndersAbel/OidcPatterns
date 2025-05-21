using System.Collections.Concurrent;

namespace Web2;

public class LogoutSessions
{
    private ConcurrentDictionary<string, string> loggedOutSessions = new();

    public void Add(string sub, string sid) =>
        loggedOutSessions.AddOrUpdate(sub, sid, (su, si) => si);

    public bool IsLoggedOut(string sub, string sid) =>
        loggedOutSessions.TryGetValue(sub, out var storedSid)
        && storedSid == sid;
}
