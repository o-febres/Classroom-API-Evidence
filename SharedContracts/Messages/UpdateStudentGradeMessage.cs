using System;
using System.Collections.Generic;
using ServiceConnect.Interfaces;
using SharedContracts.Models;

namespace SharedContracts.Messages
{
    public class UpdateStudentGradeMessage : Message
    {
        public UpdateStudentGradeMessage(Guid correlationId, long studentId, Grade updatedGrade, string subject) : base(
            correlationId)
        {
            StudentId = studentId;
            Grade = updatedGrade;
            SubjectName = subject;
        }

        public long StudentId { get; set; }
        public Grade Grade { get; set; }
        public string SubjectName { get; set; }
        public IDictionary<string, object> UpdatedFields { get; set; }
    }
}