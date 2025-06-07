using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Extensions;
using Todo.Application.Dto;


namespace Todo.Api.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController(IMediator mediator) : ControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] UserRequestDto userRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await mediator.Send(userRequest.ToUserRequest(User.Id()));

        return Ok(response);
    }
}