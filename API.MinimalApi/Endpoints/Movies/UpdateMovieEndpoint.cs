﻿using Microsoft.AspNetCore.OutputCaching;
using API.Auth;
using API.Mapping;
using Application.Services;
using Contracts.Requests;
using Contracts.Responses;

namespace API.MinimalApi.Endpoints.Movies;

public static class UpdateMovieEndpoint
{
    public const string Name = "UpdateMovie";

    public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.Update, async (
                Guid id, UpdateMovieRequest request, IMovieService movieService,
                IOutputCacheStore outputCacheStore, HttpContext context, CancellationToken token) =>
            {
                var movie = request.MapToMovie(id);
                var userId = context.GetUserId();
                var updatedMovie = await movieService.UpdateAsync(movie, userId, token);
                if (updatedMovie is null)
                {
                    return Results.NotFound();
                }

                await outputCacheStore.EvictByTagAsync("movies", token);
                var response = updatedMovie.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
}
