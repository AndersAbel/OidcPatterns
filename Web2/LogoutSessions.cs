using System.Collections.Concurrent;

namespace Web2;

public class LogoutSessions
{
    private ConcurrentBag<(string, string)> loggedOutSessions = new();

    public void Add(string sub, string sid) =>
        loggedOutSessions.Add((sub, sid));

    public bool IsLoggedOut(string sub, string sid) =>
        loggedOutSessions.Contains((sub, sid));
}
