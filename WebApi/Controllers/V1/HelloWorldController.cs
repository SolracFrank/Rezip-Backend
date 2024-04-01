using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloWorldController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public HelloWorldController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        [HttpGet]
        public async Task<ActionResult> Hello()
        {
            return Ok("Hola mundo");
        }
        [HttpGet("name/{name}")]
        [SwaggerResponse(StatusCodes.Status200OK, "refreshing succesful")]
        public async Task<ActionResult> Probando(string name)
        {
            var r = await _unitOfWork.UserRepository.FirstOrDefaultAsync(r => r.Name == name, CancellationToken.None);
            return r == null ? Ok("no exist") : Ok(r);
        }
    }
}
