using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using MOneDbContext.Models;
using MOneDbContext.Models.Context;
using MOneWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MOneWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Context _context;
        public AuthController(IConfiguration configuration, Context context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Attempt to login via <see cref="UserDataTransfer"/> data transfer object.
        /// </summary>
        /// <param name="request"></param>
        /// <returns><see cref="UserDataTransfer"/> with JWT Bearer Token.</returns>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpPost("Login")]
        public async Task<ActionResult<UserDataTransfer>> LoginAsync(UserDataTransfer request)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Username.Equals(request.Username));

            if (user is null)
            {
                return StatusCode(404, "User not found");
            }
            else
            {
                if (VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    try
                    {
                        request.Token = CreateToken(user);
                        return StatusCode(200, request);
                    }
                    catch (Exception e)
                    {
                        return StatusCode(500, e.Message);
                    }
                }
                else
                {
                    return StatusCode(500, "Wrong password");
                }
            }
        }

        /// <summary>
        /// Verifying user password.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        /// <summary>
        /// Token creation.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Username),
            };

            SymmetricSecurityKey key = new(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:key").Value));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

            JwtSecurityToken token = new(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
