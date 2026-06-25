using Opus.Mobile.Shared.Lookup;

namespace Opus.Mobile.Shared.Authentication;

public class UserDetailsItem
{
    public int? UserId { get; set; }

    public string? Name { get; set; }

    public string? ShortName { get; set; }

    public RoleLookupItem? DefaultRole { get; set; }
}
