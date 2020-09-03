using System.Collections.Generic;
using SharedContracts.Models;

namespace SharedContracts.Interfaces
{
    public interface IStudent : IEntity
    {
        string Name { get; set; }
        int Age { get; set; }
        string FirstName { get; set; }

        string LastName { get; set; }

        List<Grade> Grades { get; set; }
    }
}