using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Opus.Mobile.API.Helpers;
using Opus.Mobile.API.Models.Exceptions;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Data.Models;
using Opus.Mobile.Shared.Authentication;
using Opus.Mobile.Shared.Lookup;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Opus.Mobile.API.Services.Authentication;

public class AuthenticationService(OpusDBContext ctx) : IAuthenticationService
{
    public async Task<LoginItem> Login(LoginRequest login)
    {
        var user = await ctx.Employees.FirstOrDefaultAsync(x =>
            x.ShortName != null &&
            x.ShortName.ToUpper() == login.Username.ToUpper() &&
            x.Password == login.Password);

        if (user is null)
            throw new UnauthorizedException("Invalid username / password");

        if (login.TokenUsername != ConfigurationHelpers.GetMandatoryValue("TokenUserName") ||
            login.TokenPassword != ConfigurationHelpers.GetMandatoryValue("TokenPswd"))
            throw new UnauthorizedException("Invalid Token username / password");

        var token = GenerateJsonWebToken(user);

        await ctx.XamaLogin.AddAsync(new XamaLogin
        {
            UserId = user.Id,
            Token = token
        });

        await ctx.SaveChangesAsync();

        return new LoginItem
        {
            Token = token
        };
    }

    private static string GenerateJsonWebToken(Employees user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(ConfigurationHelpers.GetMandatoryValue("Jwt:Secret")));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.ShortName ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: ConfigurationHelpers.GetMandatoryValue("Jwt:Issuer"),
            audience: ConfigurationHelpers.GetMandatoryValue("Jwt:Issuer"),
            claims: claims,
            expires: DateTime.Now.AddHours(13),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UserDetailsItem?> GetUserDetails(int userId)
    {
        var user = await ctx.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(employee => employee.Id == userId);

        if (user is null)
            return null;

        var defaultRole = await (
            from employeeRole in ctx.EmployeesRoles.AsNoTracking()
            join role in ctx.Roles.AsNoTracking()
                on employeeRole.RoleId equals role.Id
            where employeeRole.EmployeeId == userId
            orderby role.ShortName
            select new RoleLookupItem
            {
                RoleId = role.Id,
                ShortName = role.ShortName,
                Name = role.Name
            })
            .FirstOrDefaultAsync();

        return new UserDetailsItem
        {
            UserId = user.Id,
            Name = user.Name,
            ShortName = user.ShortName,
            DefaultRole = defaultRole
        };
    }
}
