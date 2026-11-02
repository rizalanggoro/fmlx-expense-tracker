using System.Security.Claims;
using ExpenseTracker.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    protected ActionResult HandleResult<T>(ServiceResult<T> result)
    {
        return result.StatusCode switch
        {
            200 => Ok(result),
            201 => StatusCode(StatusCodes.Status201Created, result),
            204 => NoContent(),
            400 => BadRequest(result),
            401 => Unauthorized(result),
            403 => StatusCode(StatusCodes.Status403Forbidden, result),
            404 => NotFound(result),
            409 => Conflict(result),
            422 => UnprocessableEntity(result),
            _ => StatusCode(result.StatusCode, result)
        };
    }
}

