using API.MinimalApi.Endpoints.Movies;
using API.MinimalApi.Endpoints.Ratings;

namespace API.MinimalApi.Endpoints;

public static class EndpointsExtension
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {       
        app.MapMovieEndpoints();
        app.MapRatingEndpoints();

        return app;
    }

}
