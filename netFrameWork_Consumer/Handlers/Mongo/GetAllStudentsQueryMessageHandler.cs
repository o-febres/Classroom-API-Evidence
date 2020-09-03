using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MongoDB.Driver;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;

namespace Consumer_dotFramework.Handlers
{
    public class GetAllStudentsQueryMessageHandler : IMessageHandler<GetAllStudentsQueryMessage>
    {
        private readonly IRepository<Student, IStudent> _studentRepository;

        public GetAllStudentsQueryMessageHandler(IRepository<Student, IStudent> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public void Execute(GetAllStudentsQueryMessage message)
        {
            var test = Thread.CurrentPrincipal.Identity.Name;
            if (test == "ofebres-cordero")
            {
                var filter = GetFilter(message.Query);
                var sort = GetSorting(message.SortBy);

                var allStudents = _studentRepository.List(filter, sort, message.PageSize, message.Page);

                var replyMessage = new GetAllStudentsQueryReplyMessage(message.CorrelationId)
                {
                    Students = allStudents.Cast<Student>().ToList()
                };
                Context.Reply(replyMessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
                Context.Reply(new GetAllStudentsReplyMessage(message.CorrelationId));
            }
        
         }

        public IConsumeContext Context { get; set; }

        public SortDefinition<Student> GetSorting(string sortingDefinitions)
        {
            var sortDefinitions = new List<SortDefinition<Student>>();
            if (!string.IsNullOrWhiteSpace(sortingDefinitions))
            {
                var descendingDirections = new List<string> {"DESC", "-1"};

                var sortDefinitionTexts = sortingDefinitions.Split(',');
                foreach (var sortDefinitionText in sortDefinitionTexts)
                {
                    var direction = SortDirection.Ascending;

                    if (descendingDirections.Any(d =>
                        sortDefinitionText.IndexOf(d, StringComparison.OrdinalIgnoreCase) != -1))
                        direction = SortDirection.Descending;

                    var fieldName = sortDefinitionText.Split(' ')[0];


                    if (direction == SortDirection.Ascending)
                        sortDefinitions.Add(Builders<Student>.Sort.Ascending(fieldName));
                    else
                        sortDefinitions.Add(Builders<Student>.Sort.Descending(fieldName));
                }
            }

            return Builders<Student>.Sort.Combine(sortDefinitions);
        }

        public FilterDefinition<Student> GetFilter(string queryFilter)
        {
            var fieldFilters = new List<FilterDefinition<Student>>();
            if (!string.IsNullOrEmpty(queryFilter))
            {
                var regex = new Regex("^([^><=]+)([><=]*)([^><=]+)$");
                var fields = queryFilter.Split('+', ',');
                foreach (var field in fields)
                {
                    var regResult = regex.Match(field);

                    var fieldName = regResult.Groups[1].Value;
                    var fieldOperator = regResult.Groups[2].Value;
                    var fieldValue = regResult.Groups[3].Value;

                    FilterDefinition<Student> fieldFilter = null;

                    switch (fieldOperator)
                    {
                        case "=":
                            fieldFilter = Builders<Student>.Filter.Eq(fieldName, fieldValue);
                            break;
                        case ">":
                            fieldFilter = Builders<Student>.Filter.Gt(fieldName, fieldValue);
                            break;
                        case "<":
                            fieldFilter = Builders<Student>.Filter.Lt(fieldName, fieldValue);
                            break;
                        case ">=":
                            fieldFilter = Builders<Student>.Filter.Gte(fieldName, fieldValue);
                            break;
                        case "<=":
                            fieldFilter = Builders<Student>.Filter.Lte(fieldName, fieldValue);
                            break;
                    }

                    fieldFilters.Add(fieldFilter);
                }
            }

            if (fieldFilters.Count == 0)
                return FilterDefinition<Student>.Empty;
            return fieldFilters.Count == 1 ? fieldFilters[0] : Builders<Student>.Filter.And(fieldFilters);
        }
    }
}