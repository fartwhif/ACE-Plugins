using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ACE.Plugin.Web.Controllers;

[Authorize]
public class SayHiController : ControllerBase
{
    [Route("sayhi/{name}")]
    public IActionResult Get(string name)
    {
        return Ok($"Hello {name}");
    }
    [Route("getguid")]
    public IActionResult GetGuid(string name)
    {
        return Ok($"{Guid.NewGuid().ToString()}");
    }
}