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
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieService.GetAllAsync();

            return Ok(movies.MapToResponse());
        }

        [HttpGet(ApiEndpoints.Movies.GetById)]
        public async Task<IActionResult> GetMovieById(string idOrSlug)
        {
            var movie = Guid.TryParse(idOrSlug, out var id) 
                            ? await _movieService.GetByIdAsync(id) 
                            : await _movieService.GetBySlugAsync(idOrSlug);

            if (movie is null)
                return NotFound();

            return Ok(movie.MapToResponse());
        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
        {
            var newMovie = request.MapToMovie();

            await _movieService.CreateAsync(newMovie);

            return CreatedAtAction(nameof(GetMovieById), new { idOrSlug = newMovie.Id }, newMovie);
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
        {
            Movie movie = request.MapToMovie(id);

            var updatedMovie = await _movieService.UpdateAsync(movie);

            if (updatedMovie is null)
                return NotFound();

            MovieResponse response = updatedMovie.MapToResponse();

            return Ok(response);
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            bool deleted = await _movieService.DeleteByIdAsync(id);

            if (!deleted)
                return NotFound();

            return Ok();
        }
    }
}