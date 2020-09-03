using Microsoft.AspNetCore.Http;

namespace Gradebook.Helpers
{
    public interface IQueryCountWrapper
    {
        int QueryCount { get; }
    }


    public class QueryCountWrapper : IQueryCountWrapper
    {

        public QueryCountWrapper(IHttpContextAccessor httpContextAccessor)
        {
            QueryCount = httpContextAccessor.HttpContext.Request.Query.Count;
        }

        public int QueryCount { get; }
    }
}