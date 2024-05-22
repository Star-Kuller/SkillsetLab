using Application.Features.Features.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillsetLab.Models;

namespace SkillsetLab.Controllers;

[Route("/accounts")]
public class AccountController : BaseController
{
    public AccountController(IMediator mediator) : base(mediator)
    {
    }
    
    /// <summary>
    /// Войти в аккаунт
    /// </summary>
    /// <param name="command">Логин + Пароль</param>
    /// <returns>JWT-токен</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] Login.Command command)
    {
        var token = await Mediator.Send(command);
        return Ok(new TokenResponse { AccessToken = token });
    }
    
    /// <summary>
    /// Зарегистрировать аккаунт и отправить письмо с кодом
    /// </summary>
    /// <param name="command">Данные об аккаунте</param>
    /// <returns>ID аккаунта</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] Register.Command command)
    {
        var token = await Mediator.Send(command);
        return Ok(new TokenResponse() { AccessToken = token });
    }
}