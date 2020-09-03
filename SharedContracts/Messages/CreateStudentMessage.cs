using System;
using ServiceConnect.Interfaces;
using SharedContracts.Models;

namespace SharedContracts.Messages
{
    public class CreateStudentMessage : Message
    {
        public CreateStudentMessage(Guid correlationId, Student newStudent) : base(correlationId)
        {
            Student = newStudent;
        }

        public Student Student { get; set; }
    }
}