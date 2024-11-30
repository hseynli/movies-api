using API.Auth;
using API.Mapping;
using Application.Services;
using Contracts.Requests;
using Contracts.Responses;

namespace API.MinimalApi.Endpoints.Movies;

public static class GetAllMoviesEndpoint
{
    public const string Name = "GetMovies";

    public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.Get, async ([AsParameters] GetAllMoviesRequest request, IMovieService movieService, HttpContext context, CancellationToken token) =>
            {
                var userId = context.GetUserId();
                var options = request.MapToOptions()
                    .WithUser(userId);
                var movies = await movieService.GetAllAsync(options, token);
                var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
                var moviesResponse = movies.MapToResponse(
                    request.Page.GetValueOrDefault(PagedRequest.DefaultPage), 
                    request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize), 
                    movieCount);
                return TypedResults.Ok(moviesResponse);
            })
            .WithName($"{Name}V1")
            .Produces<MoviesResponse>(StatusCodes.Status200OK)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        
        app.MapGet(ApiEndpoints.Movies.Get, async (
                [AsParameters] GetAllMoviesRequest request, IMovieService movieService,
                HttpContext context, CancellationToken token) =>
            {
                var userId = context.GetUserId();
                var options = request.MapToOptions()
                    .WithUser(userId);
                var movies = await movieService.GetAllAsync(options, token);
                var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
                var moviesResponse = movies.MapToResponse(
                    request.Page.GetValueOrDefault(PagedRequest.DefaultPage), 
                    request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize), 
                    movieCount);
                return TypedResults.Ok(moviesResponse);
            })
            .WithName($"{Name}V2")
            .Produces<MoviesResponse>(StatusCodes.Status200OK)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(2.0)
            .CacheOutput("MovieCache");
        return app;
    }
}
