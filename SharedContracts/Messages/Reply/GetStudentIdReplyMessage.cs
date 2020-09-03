using System;
using ServiceConnect.Interfaces;
using SharedContracts.Models;

namespace SharedContracts.Messages.Reply
{
    public class GetStudentIdReplyMessage : Message
    {
        public GetStudentIdReplyMessage(Guid correlationId) : base(correlationId)
        {
            Student = new Student();
        }

        public Student Student { get; set; }
    }
}