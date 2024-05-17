using MediatR;

namespace SkillsetLab.Controllers;

public class AccountController : BaseController
{
    public AccountController(IMediator mediator) : base(mediator)
    {
    }
}