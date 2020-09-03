using System.Collections.Generic;
using SharedContracts.Interfaces;

namespace SharedContracts.Models
{
    public class Lesson : ILesson
    {
        public string Subject { get; set; }
        public List<Student> StudentsInLesson { get; set; }
        public long Id { get; set; }
    }
}