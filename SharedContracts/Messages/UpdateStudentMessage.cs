using System;
using System.Collections.Generic;
using ServiceConnect.Interfaces;
using SharedContracts.Models;

namespace SharedContracts.Messages
{
    public class UpdateStudentMessage : Message
    {
        public UpdateStudentMessage(Guid correlationId, long studentId, Student updatedStudent) :
            base(correlationId)
        {
            StudentId = studentId;
            UpdatedStudent= updatedStudent;
        }

        public long StudentId { get; set; }

        public Student UpdatedStudent { get; set; }

        public IDictionary<string, object> UpdatedFields { get; set; }
    }
}