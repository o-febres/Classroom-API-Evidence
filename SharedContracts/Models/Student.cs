using System.Collections.Generic;
using SharedContracts.Interfaces;

namespace SharedContracts.Models
{
    public class Student : IStudent
    {
        public Student()
        {
        }

        public Student(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            Name = firstName + " " + lastName;
        }
        public string Name { get; set; }
        public int Age { get; set; }
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Grade> Grades { get; set; }
    }
}