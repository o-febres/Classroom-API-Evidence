using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages
{
    public class GetStudentIdMessage : Message
    {
        public GetStudentIdMessage(Guid correlationId, int id) : base(correlationId)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}