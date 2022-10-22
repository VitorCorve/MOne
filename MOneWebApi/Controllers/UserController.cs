using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MOneDbContext.Models;
using MOneDbContext.Models.Context;

namespace MOneWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Context _context;
        public UserController(Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Calling information about current user.
        /// <para></para>
        /// Requires Authorization.
        /// </summary>
        /// <returns>User summary <see cref="UserInfo"/> object.</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Authorize]
        [HttpGet("UserInfo")]
        public async Task<ActionResult<UserInfo>> GetUserInfoAsync()
        {
            string username = User?.Identity?.Name ?? string.Empty;

            UserInfo info = _context.Users.Include(x => x.Info).FirstOrDefault(x => x.Username.Equals(username))?.Info;

            if (info is null)
                return StatusCode(404, "User not found");
            else
                return StatusCode(200, info);

        }
    }
}
