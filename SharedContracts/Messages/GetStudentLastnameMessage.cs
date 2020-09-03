using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages
{
    public class GetStudentLastnameMessage : Message
    {
        public GetStudentLastnameMessage(Guid correlationId, string lastName) : base(correlationId)
        {
            LastName = lastName;
        }

        public string LastName { get; set; }
    }
}