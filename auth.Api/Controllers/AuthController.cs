using api.Models;
using auth.Core.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PUMP.api.Controllers;

[Route("v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuth _auth;

    public AuthController(IAuth auth)
    {
        _auth = auth;
    }

    [HttpPost]
    public async Task<IActionResult> Auth([FromBody] Users users)
    {
        var result = await this._auth.Login(users.Username, users.Password);

        if (result == null)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Users users)
    {
        var result = await this._auth.Register(users.Username, users.Password);

        if (result == null)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }    
    
    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> Register()
    {
        var result = await this._auth.List();

        if (result == null)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}