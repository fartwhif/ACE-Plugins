using ACE.Plugin.Web.Model;
using ACE.Plugin.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ACE.Plugin.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [Route("login-token")]
    [HttpPost]
    public async Task<IActionResult> GetLoginToken(Login login)
    {
        var result = await _accountService.GetAuthTokens(login);
        if (result == null)
        {
            return ValidationProblem("invalid credentials");
        }
        return Ok(result);
    }
}