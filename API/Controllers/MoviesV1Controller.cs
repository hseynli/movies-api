using API.Auth;
using API.Mapping;
using Application.Models;
using Application.Services;
using Contracts.Requests;
using Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API;
using Asp.Versioning;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
public class MoviesV1Controller : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesV1Controller(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        GetAllMoviesOptions options = request.MapToOptions()
                                             .WithUser(userId);

        IEnumerable<Movie> movies = await _movieService.GetAllAsync(options, token);
        int movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
        MoviesResponse moviesResponse = movies.MapToResponse(request.Page, request.PageSize, movieCount);
        return Ok(moviesResponse);
    }

    [HttpGet(ApiEndpoints.Movies.GetById)]    
    public async Task<IActionResult> GetV1(string idOrSlug, [FromServices] LinkGenerator linkGenerator, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id)
                        ? await _movieService.GetByIdAsync(id, userId, token)
                        : await _movieService.GetBySlugAsync(idOrSlug, userId, token);

        if (movie is null)
            return NotFound();

        MovieResponse response = movie.MapToResponse();

        var movieObj = new { id = movie.Id };

        #region Links
        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(GetV1), values: new { idOrSlug = movie.Id }),
            Rel = "self",
            Type = "GET"
        });

        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: movieObj),
            Rel = "self",
            Type = "PUT"
        });

        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: movieObj),
            Rel = "self",
            Type = "DELETE"
        });
        #endregion

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetById)]
    //[ApiVersion("0.1", Deprecated = true)]
    [MapToApiVersion(2.0)]
    [ApiVersion("2.0")]
    public IActionResult GetV2(string idOrSlug, [FromServices] LinkGenerator linkGenerator, CancellationToken token)
    {       
        return Ok(new { Message = "API version is deprecated" });
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var newMovie = request.MapToMovie();

        await _movieService.CreateAsync(newMovie, token);

        var movieResponse = newMovie.MapToResponse();

        return CreatedAtAction(nameof(GetV1), new { idOrSlug = newMovie.Id }, movieResponse);
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
    {
        Movie movie = request.MapToMovie(id);

        var userId = HttpContext.GetUserId();

        var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);

        if (updatedMovie is null)
            return NotFound();

        MovieResponse response = updatedMovie.MapToResponse();

        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        bool deleted = await _movieService.DeleteByIdAsync(id, token);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}