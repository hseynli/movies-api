using API.Mapping;
using Application.Models;
using Application.Repositories;
using Application.Services;
using Contracts.Requests;
using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> GetMovies(CancellationToken token)
        {
            var movies = await _movieService.GetAllAsync(token);

            return Ok(movies.MapToResponse());
        }

        [HttpGet(ApiEndpoints.Movies.GetById)]
        public async Task<IActionResult> GetMovieById(string idOrSlug, CancellationToken token)
        {
            var movie = Guid.TryParse(idOrSlug, out var id) 
                            ? await _movieService.GetByIdAsync(id, token) 
                            : await _movieService.GetBySlugAsync(idOrSlug, token);

            if (movie is null)
                return NotFound();

            return Ok(movie.MapToResponse());
        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
        {
            var newMovie = request.MapToMovie();

            await _movieService.CreateAsync(newMovie, token);

            return CreatedAtAction(nameof(GetMovieById), new { idOrSlug = newMovie.Id }, newMovie);
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
        {
            Movie movie = request.MapToMovie(id);

            var updatedMovie = await _movieService.UpdateAsync(movie, token);

            if (updatedMovie is null)
                return NotFound();

            MovieResponse response = updatedMovie.MapToResponse();

            return Ok(response);
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
        {
            bool deleted = await _movieService.DeleteByIdAsync(id, token);

            if (!deleted)
                return NotFound();

            return Ok();
        }
    }
}