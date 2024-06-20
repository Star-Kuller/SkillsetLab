using Application.Features.Features.Account;
using Application.Features.Features.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillsetLab.Models;
using SkillsetLab.Security;

namespace SkillsetLab.Controllers;

[Route("/tasks")]
public class TasksController : BaseController
{
    public TasksController(IMediator mediator) : base(mediator)
    {
    }
    
    
    [HttpPost("{id}/test")]
    public async Task<IActionResult> Test(long id, IFormFile zipFile)
    {
        var isSuccess = await Mediator.Send(new TaskTest.Command() { ZipFile = zipFile, Id = id});
        if (isSuccess) 
            return Ok("Все тесты успешно пройдены.");
        return BadRequest("Тест провален: Ошибка создания пользователя.");
    }
    
    [HttpPost("/create")]
    public async Task<IActionResult> Create(IFormFile zipFile)
    {
        var taskId = await Mediator.Send(new TaskCreate.Command() { ZipFile = zipFile});
        return Created("", taskId);
    }
}