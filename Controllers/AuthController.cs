using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        model.VerifyPasswordsMatch();
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(result.Errors);
    }
    [HttpPost("AddRole")]
    public async Task<IActionResult> AddRole([FromBody] AddRoleModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var result = await _userManager.AddToRoleAsync(user, model.Role.ToString());
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest("Failed to add role.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var roles = (await _userManager.GetRolesAsync(user)).ToArray();
            var token = _jwtService.GenerateToken(user, roles);
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }


    [HttpGet("getUsers")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        var userDtos = await Task.WhenAll(users.Select(async u => new
        {
            u.UserName,
            u.Email,
            u.Id,
            Roles = await _userManager.GetRolesAsync(u)
        }));

        return Ok(userDtos);
    }
}