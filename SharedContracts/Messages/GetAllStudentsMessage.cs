using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages
{
    public class GetAllStudentsMessage : Message
    {
        public GetAllStudentsMessage(Guid correlationId, string query, string sortBy, int pageSize, int page) :
            base(correlationId)
        {
            Query = query;
            SortBy = sortBy;
            PageSize = pageSize;
            Page = page;
        }

        public string Query { get; set; }
        public string SortBy { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
    }
}