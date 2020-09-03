using System;
using System.Collections.Generic;
using System.Text;
using SharedContracts.Models;

namespace SharedContracts.Interfaces
{
    public interface ISqlRepository<T>
    {
        T Read(long id);
        IEnumerable<T> List(string filter = null, string sort = null, int pageSize = 0, int page = 0);
        IEnumerable<Grade> ListGrades();
        void CreateStudent(T student);
        void CreateGrade(Grade gr);
        void UpdateStudent(T replacement);
        void UpdateGrade(long id , Grade replacement);
        void DeleteStudent(long id);
        void DeleteGrade(string subject);
   }
}
