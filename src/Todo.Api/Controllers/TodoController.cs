using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Extensions;
using Todo.Core.Users;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController(IMediator mediator) : ControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] UserRequest userRequest)
    {
        userRequest.UserId = User.Id();
            
        var response = await mediator.Send(userRequest);

        return Ok(response);
    }
}