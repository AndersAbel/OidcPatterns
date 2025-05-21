using System.Security.Claims;

namespace Web1.Models;

public class SecureModel
{
    public required IEnumerable<Claim> Claims { get; init; }

    public required IDictionary<string, string> Properties { get; init; }
}
