using API.Mapping;
using Application.Models;
using Application.Repositories;
using Contracts.Requests;
using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieRepository.GetAllAsync();

            return Ok(movies.MapToResponse());
        }

        [HttpGet(ApiEndpoints.Movies.GetById)]
        public async Task<IActionResult> GetMovieById(string idOrSlug)
        {
            var movie = Guid.TryParse(idOrSlug, out var id) 
                            ? await _movieRepository.GetByIdAsync(id) 
                            : await _movieRepository.GetBySlugAsync(idOrSlug);

            if (movie is null)
                return NotFound();

            return Ok(movie.MapToResponse());
        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
        {
            var newMovie = request.MapToMovie();

            await _movieRepository.CreateAsync(newMovie);

            return CreatedAtAction(nameof(GetMovieById), new { idOrSlug = newMovie.Id }, newMovie);
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
        {
            Movie movie = request.MapToMovie(id);

            bool updated = await _movieRepository.UpdateAsync(movie);

            if (!updated)
                return NotFound();

            MovieResponse response = movie.MapToResponse();

            return Ok(response);
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            bool deleted = await _movieRepository.DeleteByIdAsync(id);

            if (!deleted)
                return NotFound();

            return Ok();
        }
    }
}