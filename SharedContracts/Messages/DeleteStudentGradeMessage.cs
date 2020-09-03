using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages
{
    public class DeleteStudentGradeMessage : Message
    {
        public DeleteStudentGradeMessage(Guid correlationId, int id, string subject) : base(correlationId)
        {
            Id = id;
            Subject = subject;
        }

        public string Subject { get; set; }
        public int Id { get; set; }
    }
}