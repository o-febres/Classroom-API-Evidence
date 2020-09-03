using System;
using ServiceConnect.Interfaces;
using SharedContracts.Models;

namespace SharedContracts.Messages
{
    public class CreateStudentGradeMessage : Message
    {
        public CreateStudentGradeMessage(Guid correlationId, int id, Grade grade) : base(correlationId)
        {
            Grade = grade;
            Id = id;
        }

        public Grade Grade { get; set; }
        public int Id { get; set; }
    }
}