using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace API.Controllers;

[ApiController]
[ApiVersion("2.0")]
public class MoviesV2Controller : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesV2Controller(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet(ApiEndpoints.Movies.GetById)]
    //[MapToApiVersion(2.0)]
    public IActionResult GetV2(string idOrSlug, [FromServices] LinkGenerator linkGenerator, CancellationToken token)
    {       
        return Ok(new { Message = "API version is deprecated" });
    }
}