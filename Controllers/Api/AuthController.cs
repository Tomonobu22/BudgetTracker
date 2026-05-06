using BudgetTracker.DTOs.Auth;
using BudgetTracker.Models;
using BudgetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BudgetTracker.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthenticationAppService _authenticationAppService;
        private readonly IConfiguration _configuration;

        // Refresh tokens
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthController(UserManager<IdentityUser> userManager, 
            IAuthenticationAppService authenticationAppService, 
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _configuration = configuration;
            _authenticationAppService = authenticationAppService;
            _tokenValidationParameters = tokenValidationParameters;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Auth API is working");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Missing fields");
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, payload.Password))
            {
                return Unauthorized();
            }

            var tokenValue = await GenerateJwtTokenAsync(user, "");
            return Ok(tokenValue);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto payload)
        {
            try
            {
                var result = await VerifyAndGenerateTokenAsync(payload);
                if (result == null) return BadRequest("Invalid token");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<AuthResultDto> VerifyAndGenerateTokenAsync(TokenRequestDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try {
                // 1. Check token format
                var jwtToken = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                // 2. Check Encryption algorithm
                if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                // 3. Check token expiry
                var utcExpiryDate = long.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDate = UnixTimeStampToDateTimeInUTC(utcExpiryDate);
                if (expiryDate > DateTime.UtcNow)
                {
                    throw new Exception("This token has not yet expired");
                }

                // 4. Check if refresh token exists in the db
                var dbRefreshToken = await _authenticationAppService.GetRefreshTokenAsync(tokenRequest.RefreshToken);
                if (dbRefreshToken == null)
                {
                    throw new Exception("This refresh token does not exist");
                }

                // 5. Check Id
                var jti = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (dbRefreshToken.JwtId != jti) throw new Exception("This refresh token does not match this JWT");

                // 6. Check if refresh token is expired
                if (dbRefreshToken.DateExpire <= DateTime.UtcNow) throw new Exception("This refresh token has expired, plese re-authenticate");

                // 7. Check if refresh token is revoked
                if (dbRefreshToken.IsRevoked)
                {
                    throw new Exception("This refresh token has been revoked");
                }

                // Generate new token with existing refresh token
                var userData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                return await GenerateJwtTokenAsync(userData!, tokenRequest.RefreshToken);
            }
            catch (SecurityTokenException)
            {
                var dbRefreshToken = await _authenticationAppService.GetRefreshTokenAsync(tokenRequest.RefreshToken);
                // Generate new token with existing refresh token
                var userData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                return await GenerateJwtTokenAsync(userData!, tokenRequest.RefreshToken);
            }
        }

        private async Task<AuthResultDto> GenerateJwtTokenAsync(IdentityUser user, string currentRefreshToken)
        {
            // Generate JWT token here
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add User Roles as Claims
            //var userRoles = await _userManager.GetRolesAsync(user);
            //foreach (var userRole in userRoles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, userRole));
            //}

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = new RefreshToken();

            if (string.IsNullOrEmpty(currentRefreshToken))
            {
                refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.Now,
                    DateExpire = DateTime.Now.AddDays(7),
                    Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
                };
                await _authenticationAppService.SaveRefreshTokenAsync(refreshToken);
            }

            var response = new AuthResultDto()
            {
                Token = jwtToken,
                RefreshToken = string.IsNullOrEmpty(currentRefreshToken) ?  refreshToken.Token : currentRefreshToken,
                ExpiresAt = token.ValidTo,
            };

            return response;
        }

        private DateTime UnixTimeStampToDateTimeInUTC(long unixTimeStamp) 
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTimeVal;
        }
    }
}