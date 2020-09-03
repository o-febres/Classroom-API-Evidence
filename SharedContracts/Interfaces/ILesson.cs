using System.Collections.Generic;
using SharedContracts.Models;

namespace SharedContracts.Interfaces
{
    public interface ILesson : IEntity
    {
        string Subject { get; set; }

        List<Student> StudentsInLesson { get; set; }
    }
}