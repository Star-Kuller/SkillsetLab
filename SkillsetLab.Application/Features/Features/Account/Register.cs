using Application.Infrastructure;
using AutoMapper;
using Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Features.Account
{
    public class Register
    {
        public class Command : IRequest<(string, string)>
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        
            public void CreateMappings(Profile configuration)
            {
                configuration.CreateMap<Command, User>()
                    .ForMember(entity => entity.UserName, opt =>
                        opt.MapFrom(cmd => cmd.Name))
                    .ForMember(entity => entity.NormalizedUserName, opt =>
                        opt.MapFrom(cmd => cmd.Name.ToUpperInvariant()))
                    .ForMember(entity => entity.NormalizedEmail, opt =>
                        opt.MapFrom(cmd => cmd.Email.ToUpperInvariant()))
                    .ForAllMembers(opt => 
                        opt.Ignore());
            }
        }
    
        public class Handler : IRequestHandler<Command, (string, string)>
        {
            private readonly UserManager<User> _userManager;
            private readonly ILogger<Register> _logger;
            private readonly IMediator _mediator;
            private readonly IMapper _mapper;


            public Handler(UserManager<User> userManager, ILogger<Register> logger, IMediator mediator, IMapper mapper)
            {
                _userManager = userManager;
                _logger = logger;
                _mediator = mediator;
                _mapper = mapper;
            }

            public async Task<(string, string)> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    await Transaction.Do(async () =>
                    {
                        var user = _mapper.Map<User>(request);

                        user.EmailConfirmed = true;
                        
                        var result = await _userManager.CreateAsync(user, request.Password);
                        if (!result.Succeeded)
                            throw new Exception("Не удалось создать пользователя");

                        result = await _userManager.AddToRoleAsync(user, Role.Standard);
                        if (!result.Succeeded)
                            throw new Exception("Не удалось добавить роль");
                        
                        result = await _userManager.UpdateAsync(user);
                        if (!result.Succeeded)
                            throw new Exception("Не удалось сохранить пользователя");
                    });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    throw;
                }

                return await _mediator.Send(
                    new Login.Command() { Email = request.Email, Password = request.Password }
                    , cancellationToken);
            }
        }
    }
}