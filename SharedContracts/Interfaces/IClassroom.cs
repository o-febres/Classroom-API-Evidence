using System.Collections.Generic;
using SharedContracts.Models;

namespace SharedContracts.Interfaces
{
    public interface IClassroom : IEntity
    {
        string RoomName { get; set; }

        int Capacity { get; set; }

        List<Lesson> LessonsInClassroom { get; set; }
    }
}