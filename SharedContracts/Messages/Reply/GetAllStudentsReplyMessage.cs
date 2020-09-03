using System;
using System.Collections.Generic;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Models;

namespace SharedContracts.Messages.Reply
{
    public class GetAllStudentsReplyMessage : Message
    {
        public GetAllStudentsReplyMessage(Guid correlationId) : base(correlationId)
        {
            Students = new List<Student>();
        }

        public IEnumerable<Student> Students { get; set; }
    }
}