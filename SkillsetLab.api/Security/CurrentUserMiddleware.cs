using System.Security.Claims;
using Application.Security;
using Domain.Entities.Comon;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SkillsetLab.Security
{
    public class CurrentUserMiddleware : IMiddleware
    {
        private readonly ICurrentUser _currentUser;
        private readonly UserManager<User> _userManager;

        public CurrentUserMiddleware(
            ICurrentUser currentUser, UserManager<User> userManager)
        {
            _currentUser = currentUser;
            _userManager = userManager;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                await next(context);
                return;
            }

            var userId = long.Parse(context.User.FindFirst(ClaimTypes.Sid)?.Value);
            
            var user = await _userManager
                .Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user is not { Status: Status.Enabled })
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            _currentUser.Id = user.Id;
            _currentUser.Email = user.Email;
            _currentUser.Role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            await next(context);
        }
    }
}
