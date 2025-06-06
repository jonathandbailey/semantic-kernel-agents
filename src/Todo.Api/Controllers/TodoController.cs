using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Agents;
using Todo.Api.Extensions;
using Todo.Application.Dto;
using Todo.Application.Users;


namespace Todo.Api.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController(IMediator mediator) : ControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] UserRequestDto userRequest)
    {
        var response = await mediator.Send(new UserRequest
        {
            UserId = User.Id(), 
            SendTaskRequest = AgentExtensions.CreateSendTaskRequest(userRequest.TaskId, userRequest.SessionId, userRequest.Message)
        });

        return Ok(response);
    }
}