namespace WebApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;


public class BaseApiController : ControllerBase
{
    private IMediator _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
}
