using System.Collections.Generic;
using SharedContracts.Interfaces;

namespace SharedContracts.Models
{
    public class Classroom : IClassroom
    {
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public List<Lesson> LessonsInClassroom { get; set; }
        public long Id { get; set; }
    }
}