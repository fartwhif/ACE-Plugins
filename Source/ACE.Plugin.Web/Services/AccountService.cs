using ACE.Database;
using ACE.Database.Models.Auth;
using ACE.Entity.Enum;
using ACE.Plugin.Web.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ACE.Plugin.Web.Services;

public interface IAccountService {
    Task<Token> GetAuthTokens(Login login);
}

public class AccountService : IAccountService
{
    private string CreateJwtToken(Account acct)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("myS34DSas)@%dfF52345upER$ecr3tK3y034978623")
        );
        var credentials = new SigningCredentials(
            symmetricSecurityKey,
            SecurityAlgorithms.HmacSha256
        );

        List<Claim> userCliams = new List<Claim>
    {
        new Claim(ClaimTypes.Name, acct.AccountName),
        new Claim("AccountId", acct.AccountId.ToString()),
        new Claim("AccessLevelId", acct.AccessLevel.ToString()),
        new Claim("AccessLevelName", ((AccessLevel)acct.AccessLevel).ToString()),
        new Claim(((AccessLevel)acct.AccessLevel).ToString(), "1")
    };

        var jwtToken = new JwtSecurityToken(
            issuer: "plugin!",
            expires: DateTime.Now.AddMinutes(20),
            signingCredentials: credentials,
            claims: userCliams,
            audience: "ace web users"
        );

        string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        return token;
    }
    public async Task<Token> GetAuthTokens(Login login)
    {
        Account acct = null;
        Gate.RunGatedAction(() =>
        {
            acct = DatabaseManager.Authentication.GetAccountByName(login.Account);
            if (acct == null)
            {
                //TO-DO: Thread.Sleep(average duration difference between (acct == null) and (acct != null));
                return;
            }
            if (!acct.PasswordMatches(login.Password))
            {
                acct = null;
            }
        });
        if (acct == null)
        {
            //TO-DO: exponential fallback temporary ip address ban
            return null;
        }
        if (acct != null)
        {
            var token = new Token
            {
                AccessToken = CreateJwtToken(acct)
            };
            return token;
        }
        return null;
    }
}