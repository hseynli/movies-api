using Application.Models;

namespace Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = new();

    public Task<bool> CreateAsync(Movie movie)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        var removedCount = _movies.RemoveAll(p => p.Id == id);
        var movieRemoved = removedCount > 0;
        return Task.FromResult(movieRemoved);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_movies.SingleOrDefault(p => p.Id == id));
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        return Task.FromResult(_movies.SingleOrDefault(p => p.Slug == slug));
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var movieIndex = _movies.FindIndex(p => p.Id == movie.Id);

        if (movieIndex == -1)
        {
            return Task.FromResult(false);
        }

        _movies[movieIndex] = movie;
        return Task.FromResult(true);
    }
}
