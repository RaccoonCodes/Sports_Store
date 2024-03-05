/*
 *  If the query string is present (QueryString.HasValue is true), 
 *  it concatenates the path and the query string using  
 *  ($"{request.Path}{request.QueryString}"). If there is no query string, 
 *  it returns path as string (request.Path.ToString()).
 */
namespace SportsStore.Infrastructure
{
    public static class UrlExtensions{
        public static string PathAndQuery(this HttpRequest request) =>
            request.QueryString.HasValue ? $"{request.Path}{request.QueryString}"
            : request.Path.ToString();
    }
}
