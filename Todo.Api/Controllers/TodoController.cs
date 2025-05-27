using Microsoft.AspNetCore.Mvc;

namespace Todo.Api.Controllers
{
    [ApiController]
    [Route("api/todo")]
    public class TodoController : ControllerBase
    {
        [HttpPost]
        public Task<IActionResult> Send()
        {
            throw new NotImplementedException();
        }
    }
}
