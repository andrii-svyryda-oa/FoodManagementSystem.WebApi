using Microsoft.AspNetCore.Mvc;
using Optional;
using System.Security.Claims;

public static class GetUserIdFromClaimsExtension
{
    public static Option<Guid> GetUserIdFromClaims(this ControllerBase controllerBase)
    {
        var userIdClaim = controllerBase.User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Option.Some(Guid.Parse(userIdClaim.Value)) : Option.None<Guid>();
    }
}
