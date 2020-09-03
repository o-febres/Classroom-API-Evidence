using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gradebook.Helpers;
using Microsoft.AspNetCore.Mvc;
using Ruffer.Security.Web;
using Ruffer.Security.Web.Filters;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;

//GradebookController
namespace Gradebook.Controllers
{
    [TokenAuthorize]
    [Route("api/[controller]")]
    public class StudentController : Controller //, IRestRepository
    {
        private readonly IBus _bus;
        private readonly IClaimsHelper _claimsHelper;
        private readonly IQueryCountWrapper _queryCountWrapper;

        public StudentController(IBus bus, IClaimsHelper claimsHelper, IQueryCountWrapper queryCountWrapper)
        {
            _claimsHelper = claimsHelper;
            _queryCountWrapper = queryCountWrapper;
            _bus = bus;
        }

        //_GetAll 
        [HttpGet("filter={query}&sort={sortby}&pagesize={pagesize}&page={page}")]
        [Route("")]
        public IOrderedEnumerable<IStudent> GetAll(string query, string sortby, int pagesize, int page)
        {
            var request = new GetAllStudentsMessage(Guid.NewGuid(), query, sortby, pagesize, page);
            var reply = _bus.SendRequest<GetAllStudentsMessage, GetAllStudentsReplyMessage>("ClassroomApi_Consumer",
                request, null, (int) TimeSpan.FromMinutes(2).TotalMilliseconds);
            return reply.Students.OrderBy(_ => _.Id);
        }


        // GetStudentGrades -- With or Without query
        [HttpGet("{id:int}/grades/StartingWith={startingWith}&OrderBy={orderBy}&Descending={descendingbool}")]
        [Route("{id:int}/grades/")]
        public IEnumerable<Grade> GetGrades(int id, string startingWith, bool descendingbool, string orderBy)
        {
            var request = new GetStudentIdMessage(Guid.NewGuid(), id);

            var reply = _bus.SendRequest<GetStudentIdMessage, GetStudentIdReplyMessage>("ClassroomApi_Consumer",
                request);

            if (_queryCountWrapper.QueryCount <= 1) return reply.Student.Grades;

            if (orderBy == "score")
            {
                var itemsorted = reply.Student.Grades.Where(x => x.Subject.StartsWith(startingWith))
                    .OrderBy(x => x.Score)
                    .ThenBy(_ => _.Subject);
                return descendingbool == false ? itemsorted : itemsorted.Reverse();
            }

            if (orderBy == "name")
            {
                var itemsorted = reply.Student.Grades.Where(x => x.Subject.StartsWith(startingWith))
                    .OrderBy(x => x.Subject)
                    .ThenBy(_ => _.Score);
                return descendingbool == false ? itemsorted : itemsorted.Reverse();
            }

            throw new ArgumentException("Incorrect Query \n OrderBy = score or name");
        }

        // GetStudentSubject
        [HttpGet("{id:int}/grades/{subject}")]
        public IEnumerable<Grade> GetStudentGrade(int id, string subject)
        {
            var request = new GetStudentIdMessage(Guid.NewGuid(), id);
            var reply = _bus.SendRequest<GetStudentIdMessage, GetStudentIdReplyMessage>("ClassroomApi_Consumer",
                request);
            return reply.Student.Grades.Where(_ => _.Subject.ToLowerInvariant() == subject.ToLowerInvariant());
        }

        //GetByID
        [HttpGet("{id:int}")]
        public Student GetById(int id)
        {
            var request = new GetStudentIdMessage(Guid.NewGuid(), id);
            var reply = _bus.SendRequest<GetStudentIdMessage, GetStudentIdReplyMessage>("ClassroomApi_Consumer",
                request, null, (int) TimeSpan.FromMinutes(2).TotalMilliseconds);
            return reply.Student;
        }

        //GetBylastName
        [HttpGet("{lastName}")]
        public Student GetByName(string lastName)
        {

            var request = new GetStudentLastnameMessage(Guid.NewGuid(), lastName);
            var reply =
             _bus.SendRequest<GetStudentLastnameMessage, GetStudentLastNameReplyMessage>("ClassroomApi_Consumer",
                    request);
            return reply.Student;
        }

        //CreateStudent
        [HttpPost]
        public IActionResult CreateStudent([FromBody] Student newStudent)
        {
            if (newStudent == null) return NotFound("Incomplete student data");
            var request = new CreateStudentMessage(Guid.NewGuid(), newStudent);
            _bus.SendRequest<CreateStudentMessage, CreateStudentReplyMessage>("ClassroomApi_Consumer", request, null,
                (int) TimeSpan.FromMinutes(2).TotalMilliseconds);
            return new AcceptedResult{Value = "Student Created"};
        }

        //CreateStudentGrade
        [Route("{id:int}/grades/")]
        [HttpPost]
        public IActionResult CreateGrade([FromBody] Grade newGrade, int id)
        {
            if (newGrade == null) return NotFound();
            var request = new CreateStudentGradeMessage(Guid.NewGuid(), id, newGrade);
            _bus.SendRequest<CreateStudentGradeMessage, CreateStudentGradeReplyMessage>("ClassroomApi_Consumer",
                request);
            return new AcceptedResult();
        }

        //UpdateStudent
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Student updateStudent)
        {
            var request = new UpdateStudentMessage(Guid.NewGuid(), id, updateStudent);
            var reply = _bus.SendRequest<UpdateStudentMessage, UpdateStudentReplyMessage>("ClassroomApi_Consumer",
                request);

            return new AcceptedResult();
        }

        //UpdateStudentGrade
        [HttpPut("{id:int}/grades/{subject}")]
        public IActionResult UpdateGrade(long id, [FromBody] Grade updateGrade, string subject)
        {
            var request = new UpdateStudentGradeMessage(Guid.NewGuid(), id, updateGrade, subject);
            var reply =
                _bus.SendRequest<UpdateStudentGradeMessage, UpdateStudentGradeReplyMessage>("ClassroomApi_Consumer",
                    request);
            return new AcceptedResult();
        }

        // DeleteStudent
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var request = new DeleteStudentMessage(Guid.NewGuid(), id);
            var reply = _bus.SendRequest<DeleteStudentMessage, DeleteStudentReplyMessage>("ClassroomApi_Consumer",
                request);
            return new ObjectResult(reply.CorrelationId);
        }

        // DeleteStudentGrade
        [HttpDelete("{id:int}/grades/{subject}")]
        public IActionResult DeleteStudentGrade(int id, string subject)
        {
            var request = new DeleteStudentGradeMessage(Guid.NewGuid(), id, subject);
            var reply =
                _bus.SendRequest<DeleteStudentGradeMessage, DeleteStudentGradeReplyMessage>("ClassroomApi_Consumer",
                    request);
            return new ObjectResult(reply.CorrelationId);
        }
    }
}


/*var updatedFields = new Dictionary<string, object>();
          var propertiesOnStudent = typeof(Student).GetProperties();
          foreach (var propertyInfo in propertiesOnStudent)
          {
              var newValue = propertyInfo.GetValue(updateStudent);
              if (newValue != null)
                  updatedFields.Add(propertyInfo.Name, newValue);
          }*/