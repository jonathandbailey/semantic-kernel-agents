using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Extensions;
using Todo.Application.Dto;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/vacationplan")]
public class VacationPlanController(IMediator mediator) : Controller
{
    [HttpGet("catalog")]
    public async Task<IActionResult> GetCatalog()
    {
        var response = await mediator.Send( new GetVacationPlanCatalogQuery(User.Id()));

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await mediator.Send(new GetVacationPlanQuery(User.Id(), id));

        return Ok(response);
    }
}