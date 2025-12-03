using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API_Laundry.Models;
using System.IdentityModel.Tokens.Jwt; // Untuk JWT
using Microsoft.IdentityModel.Tokens; // Untuk SecurityKey
using System.Text;
using Microsoft.Extensions.Configuration; // Untuk mengakses appsettings



[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    // ------------------------------------------------------------------
    //                          ACTION REGISTER
    // ------------------------------------------------------------------
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new ApplicationUser 
        { 
            // Properti bawaan Identity
            UserName = model.Email,
            Email = model.Email, 
            
            NamaLengkap = model.NamaLengkap, 
            NomorTelepon = model.TeleponPengguna,
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Tambahkan role default
            await _userManager.AddToRoleAsync(user, "Pengguna");

            return Ok(new { Message = "Registrasi berhasil! Silakan Login." });
        }

        return BadRequest(new 
        { 
            Message = "Registrasi gagal.", 
            Errors = result.Errors.Select(e => e.Description) 
        });
    }

    // ------------------------------------------------------------------
    //                          ACTION LOGIN
    // ------------------------------------------------------------------
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized(new { Message = "Email atau password salah." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // 1. Buat Token JWT
            var token = GenerateJwtToken(user);
            
            // 2. Berikan token ke client
            return Ok(new 
            { 
                Token = token,
                Expires = DateTime.Now.AddDays(7) // Contoh masa berlaku token
            });
        }
        
        return Unauthorized(new { Message = "Email atau password salah." });
    }


    // ------------------------------------------------------------------
    //                         JWT HELPER METHOD
    // ------------------------------------------------------------------
    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Tambahkan ROLE
        var userRoles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ------------------------------------------------------------------
    //                         ACTION INIT ROLES
    // ------------------------------------------------------------------
    [HttpPost("init-roles")]
    public async Task<IActionResult> InitRoles([FromServices] RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Pengguna" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        return Ok("Roles berhasil dibuat.");
    }
    // ------------------------------------------------------------------
    //                         ACTION CREATE ADMIN
    // ------------------------------------------------------------------
    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin([FromBody] RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            NamaLengkap = model.NamaLengkap,
            NomorTelepon = model.TeleponPengguna,
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Admin");

            return Ok("Admin berhasil dibuat.");
        }

        return BadRequest(result.Errors);
    }
    // ------------------------------------------------------------------
    //                         ACTION GET USER
    // ------------------------------------------------------------------
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound("User not found");

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.NamaLengkap,
            user.NomorTelepon
        });
    }

}