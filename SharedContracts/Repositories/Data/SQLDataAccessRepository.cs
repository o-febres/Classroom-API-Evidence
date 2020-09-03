using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using SharedContracts.Interfaces;
using SharedContracts.Models;

namespace SharedContracts.Repositories.Data
{
    public abstract class SqlDataAccessRepository : ISqlRepository<IStudent>
        //<TImplementation, TInterface> : IRepository<TImplementation, TInterface>
        //  where TInterface : IEntity where TImplementation : TInterface, new()
    {
        private SqlConnection _sqlConnection;

        protected SqlDataAccessRepository() { 

            //sqlConnection = new SqlConnection("Server=LONSQLDEV02; DataBase=Classroom; Trusted_Connection=True; MultipleActiveResultSets=True");
        }

        public void CreateStudent(IStudent st)
        {
            var sql = @"INSERT INTO [Classroom].[dbo].[Students] ([Id], [Name], [Age], [FirstName], [LastName]) VALUES('" + st.Id + "' , '" + st.Name + "' , '" + st.Age + "' , '" + st.FirstName + "' , '" + st.LastName  +  "')";
            using (_sqlConnection = CreateSqlConnection())
            {
                _sqlConnection.Open();
                _sqlConnection.Query(sql);
               
            }

        }

        public void CreateGrade( Grade gr)
        {
        var InsertSql = @"INSERT INTO [Classroom].[dbo].[Grades] ([Id], [Subject], [Score], [StudentId]) VALUES( '" +  gr.Id + "' , '" + gr.Subject + "' , '" + gr.Score + "' , '" + gr.StudentId + "')";
            using (_sqlConnection = CreateSqlConnection())
            {
                _sqlConnection.Open();
                _sqlConnection.Query(InsertSql);
             }

        }



        public IStudent Read(long id)
        {
            var sql =
                "select * from dbo.Students Left JOIN dbo.Grades on dbo.Students.Id = dbo.Grades.StudentId WHERE Students.Id = " + id;
            using (_sqlConnection = CreateSqlConnection())
            {
                _sqlConnection.Open();
                var lookup = new Dictionary<int, Student>();
                var studentQuery = _sqlConnection.Query<Student, Grade, Student>(sql, (s, a) =>
                    {
                        //Student student;
                        if (!lookup.TryGetValue((int) s.Id, out Student student)) lookup.Add((int) s.Id, student = s);
                        if (student.Grades == null)
                            student.Grades = new List<Grade>();
                        student.Grades.Add(a);
                        return student;
                    }
                ).AsQueryable().FirstOrDefault();
                return studentQuery;
            }
        }

        public IEnumerable<IStudent> List(string filter = null, string sort = null, int pageSize = 0, int page = 0)
        {
            var sql =
                "select * from dbo.Students Left JOIN dbo.Grades on dbo.Students.Id = dbo.Grades.StudentId";
            Student student;
            using (_sqlConnection = CreateSqlConnection())
            {
                _sqlConnection.Open();

               var lookup = new Dictionary<int, Student>();
                var studentQuery = _sqlConnection.Query<Student, Grade, Student>(sql, (s, a) =>
                    {
                        if (!lookup.TryGetValue((int) s.Id, out student)) lookup.Add((int) s.Id, student = s);

                        if (student.Grades == null)
                            student.Grades = new List<Grade>();
                        student.Grades.Add(a);
                        return student;
                    }
                ).AsQueryable().FirstOrDefault();
                return lookup.Values;
            }
 }

        public IEnumerable<Grade> ListGrades()
        {
            var grades = new List<Grade>();
            var gradeQuerySql = "select * from dbo.Grades";
            using (_sqlConnection = CreateSqlConnection())
            {
                _sqlConnection.Open();
                var studentQuery = _sqlConnection.Query<Grade>(gradeQuerySql);
                return studentQuery;
            }
        }

        public void UpdateStudent(IStudent replacement)
        {
            var sql = @"UPDATE dbo.Students
                        SET [Name] = '" + replacement.Name + @"' , 
	                        [Age] = '" + replacement.Age + @"',
	                        [FirstName] = '" + replacement.FirstName+ @"',
	                        [LastName] = '" + replacement.LastName + @"'
                            WHERE Id = " + replacement.Id;

            using (_sqlConnection = CreateSqlConnection())
            {
                _sqlConnection.Open();
                _sqlConnection.Query(sql);
            }
        }

        public void UpdateGrade(long id, Grade grade)
        {
            var sql = @"UPDATE dbo.Grades
                        SET [Subject] = '" + grade.Subject + @"' , 
	                        [Score] = '" + grade.Score + @"',
	                        [StudentID] = '" + grade.StudentId + @"'
                            WHERE Id = " + grade.Id;

            using (_sqlConnection = CreateSqlConnection())
            {
                _sqlConnection.Open();
                _sqlConnection.Query(sql);
            }
        }

        public void DeleteStudent(long id)
        {
            var sql = 
            @"DELETE 
            FROM dbo.Grades
            WHERE StudentId = " + id + @"
            DELETE
            FROM   dbo.Students
            WHERE  Id = " + id;

            using (_sqlConnection = CreateSqlConnection())
            {
                _sqlConnection.Open();
                _sqlConnection.Query(sql);
            }
        }

        public void DeleteGrade(string subject)
        {
            var sql = @"DELETE 
            FROM dbo.Grades
            WHERE Subject = '" + subject + "'";
            using (_sqlConnection = CreateSqlConnection())
            {
                _sqlConnection.Open();
                _sqlConnection.Query(sql);
            }
        }

        private SqlConnection CreateSqlConnection()
        {
            return new SqlConnection("Server=LONSQLDEV02; DataBase=Classroom; Trusted_Connection=True; MultipleActiveResultSets=True");
        }
    }
}