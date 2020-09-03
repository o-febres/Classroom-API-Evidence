using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages
{
    public class DeleteStudentMessage : Message
    {
        public DeleteStudentMessage(Guid correlationId, int id) : base(correlationId)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}