using SharedContracts.Interfaces;
using SharedContracts.Models;
using SharedContracts.Repositories.Data;

namespace SharedContracts.Repositories
{
    public class LessonRepository : DataAccessRepository<Lesson, ILesson>
    {
    }
}