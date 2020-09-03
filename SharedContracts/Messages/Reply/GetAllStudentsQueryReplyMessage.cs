using System;
using System.Collections.Generic;
using ServiceConnect.Interfaces;
using SharedContracts.Models;

namespace SharedContracts.Messages.Reply
{
    public class GetAllStudentsQueryReplyMessage : Message
    {
        public GetAllStudentsQueryReplyMessage(Guid correlationId) : base(correlationId)
        {
        }

        public IList<Student> Students { get; set; }
    }
}