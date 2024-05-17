using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SkillsetLab.Controllers;

public class BaseController : ControllerBase
{
    protected IMediator Mediator;

    public BaseController(IMediator mediator)
    {
        Mediator = mediator;
    }
}