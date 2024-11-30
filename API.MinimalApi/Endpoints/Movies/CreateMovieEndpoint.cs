using Microsoft.AspNetCore.OutputCaching;
using API.Auth;
using API.Mapping;
using Application.Services;
using Contracts.Requests;
using Contracts.Responses;
using API;

namespace API.MinimalApi.Endpoints.Movies;

public static class CreateMovieEndpoint
{
    public const string Name = "CreateMovie";

    public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Movies.Create, async (CreateMovieRequest request, IMovieService movieService, IOutputCacheStore outputCacheStore, 
                        CancellationToken token) =>
            {
                var movie = request.MapToMovie();
                await movieService.CreateAsync(movie, token);
                await outputCacheStore.EvictByTagAsync("movies", token);
                var response = movie.MapToResponse();
                return TypedResults.CreatedAtRoute(response, GetMovieEndpoint.Name, new { idOrSlug = movie.Id });
            })
            .WithName(Name)
            .Produces<MovieResponse>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
}
