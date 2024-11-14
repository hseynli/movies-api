namespace API;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Movies
    {
        private const string Base = $"{ApiBase}/movies";
        public const string Create = Base;
        public const string Get = Base;
        public const string GetById = $"{Base}/{{idOrSlug}}";
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
    }
}