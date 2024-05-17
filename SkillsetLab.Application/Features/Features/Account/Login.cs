using Application.Security;
using Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Features.Account;

public class Login
{
    public class Command : IRequest<string>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, string>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenProvider _tokenProvider;

        public Handler(UserManager<User> userManager, ITokenProvider tokenProvider)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await GetUser(request.Email, request.Password);
            var token = _tokenProvider.GetToken(new UserClaims
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.UserRoles.Select(ur => ur.Role.Name).First()
            });
            return token;
        }
        
        private async Task<User> GetUser(string email, string password)
        {
            var user = await _userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new Exception("Пользователь не найден");

            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!isValidPassword)
                throw new Exception("Пароль не верный");

            return user;
        }
    }
}