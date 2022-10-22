using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MOneDbContext.Models;
using MOneDbContext.Models.Context;

using MOneWebApi.Models;

using System.Security.Cryptography;

namespace MOneWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly Context _context;
        public RegisterController(Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Attempt to register user via <see cref="UserDataTransfer"/> data transfer object.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>User summary <see cref="UserInfo"/> object.</returns>
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(301)]
        [ProducesResponseType(500)]
        [HttpPost("Register")]
        public async Task<ActionResult<UserInfo>> RegisterAsync(UserDataTransfer request)
        {
            if (_context.Users.Any(x => x.Username.Equals(request.Username)))
            {
                return StatusCode(301, $"User with nickname '{request.Username}' already exists.");
            }
            else
            {
                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

                try
                {
                    UserInfo info = new()
                    {
                        Username = request.Username,
                        DateRegister = DateTime.UtcNow,
                        Gender = request.Gender,
                    };

                    User user = new()
                    {
                        Username = request.Username,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Info = info
                    };

                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    return StatusCode(200, info);
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
        }

        /// <summary>
        /// Password hash creation.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
