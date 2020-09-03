using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consumer_dotFramework.Handlers.Sql;
using Moq;
using Ruffer.Security.Claims;
using Ruffer.Security.Filters;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;
using SharedContracts.Repositories;
using SharedContracts.Repositories.Data;
using Xunit;

namespace Consumer_dotFrameworkUnitTests
{
    public class ConsumerDotFrameworkTests
    {
        [Fact]
        public void GivenCreateStudentGradeMessage_WhenCallingCreateGradeHandler_ThenCorrespondingReplyIsSend()
        {
            // Arrange
            var repositry = new Mock<ISqlRepository<IStudent>>(MockBehavior.Strict);
            repositry.Setup(s => s.ListGrades()).Returns(new List<Grade>
            {
                new Grade {Subject = "Maths" , Id = 1},
                new Grade {Subject = "English" , Id = 2}
            });
            var createdGrade = new Grade { Subject = "TestGrade" };
            repositry.Setup(h => h.CreateGrade(createdGrade));
            
            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            claimsHelper.Setup(h => h.GetClaims()).Returns(new Claims() {UserClaims = new UserClaim() {Username = "ofebres-cordero"}});

            var context = new Mock<IConsumeContext>(MockBehavior.Strict);
            context.Setup(c => c.Reply(It.IsAny<CreateStudentGradeReplyMessage>()));

            var handler = new SqlCreateStudentGradeHandler(repositry.Object, claimsHelper.Object)
            {
                Context = context.Object
            };

            CreateStudentGradeMessage request = new CreateStudentGradeMessage(Guid.NewGuid(), 1, createdGrade);
            CreateStudentGradeReplyMessage reply = null;
            
            // Act
            handler.Execute(request);

            //var okObjectResult = result as ObjectResult;

            // Assert
            context.Verify(c => c.Reply(It.IsAny<CreateStudentGradeReplyMessage>()), Times.Once);
            // Assert.Equal(reply.CorrelationId, okObjectResult.Value);
        }

        [Fact]
        public void GivenCreateStudentMessage_WhenCallingCreateStudentHandler_ThenCorrespondingReplyIsSend()
        {
            // Arrange
            var repositry = new Mock<ISqlRepository<IStudent>>(MockBehavior.Strict);
            repositry.Setup(s => s.List(null, null, 0, 0)).Returns( new List<Student>
            {
                new Student("test1", "1") {Id = 1},
                new Student("test2" , "2"){Id = 2}
            });
            var createdStudent = new Student("oli" , "");
            repositry.Setup(h => h.CreateStudent(createdStudent));

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            claimsHelper.Setup(h => h.GetClaims()).Returns(new Claims() { UserClaims = new UserClaim() { Username = "ofebres-cordero" } });

            var context = new Mock<IConsumeContext>(MockBehavior.Strict);
            context.Setup(c => c.Reply(It.IsAny<CreateStudentReplyMessage>()));

            var handler = new SqlCreateStudentHandler(repositry.Object, claimsHelper.Object)
            {
                Context = context.Object
            };

            CreateStudentMessage request = new CreateStudentMessage(Guid.NewGuid(), createdStudent);
            CreateStudentReplyMessage reply = null;

            // Act
            handler.Execute(request);

           

            // Assert
            context.Verify(c => c.Reply(It.IsAny<CreateStudentReplyMessage>()), Times.Once);
         
        }

        [Fact]
        public void GivenDeleteStudentGradeMessage_WhenCallingDeleteGradeHandler_ThenCorrespondingReplyIsSend()
        {
            // Arrange
            var repositry = new Mock<ISqlRepository<IStudent>>(MockBehavior.Strict);
            repositry.Setup(s => s.ListGrades()).Returns(new List<Grade>
            {
                new Grade {Subject = "Maths" , Id = 1},
                new Grade {Subject = "English" , Id = 2}
            });
            var createdGrade = new Grade { Subject = "TestGrade" };
            repositry.Setup(h => h.DeleteGrade(createdGrade.Subject));

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            claimsHelper.Setup(h => h.GetClaims()).Returns(new Claims() { UserClaims = new UserClaim() { Username = "ofebres-cordero" } });

            var context = new Mock<IConsumeContext>(MockBehavior.Strict);
            context.Setup(c => c.Reply(It.IsAny<DeleteStudentGradeReplyMessage>()));

            var handler = new SqlDeleteStudentGradeHandler(repositry.Object, claimsHelper.Object)
            {
                Context = context.Object
            };

            DeleteStudentGradeMessage request = new DeleteStudentGradeMessage(Guid.NewGuid(), 1, createdGrade.Subject);
            DeleteStudentGradeReplyMessage reply = null;

            // Act
            handler.Execute(request);

            //var okObjectResult = result as ObjectResult;

            // Assert
            context.Verify(c => c.Reply(It.IsAny<DeleteStudentGradeReplyMessage>()), Times.Once);
            // Assert.Equal(reply.CorrelationId, okObjectResult.Value);
        }

        [Fact]
        public void GivenDeleteStudentMessage_WhenCallingDeleteStudentHandler_ThenCorrespondingReplyIsSend()
        {
            // Arrange
            var repositry = new Mock<ISqlRepository<IStudent>>(MockBehavior.Strict);
            repositry.Setup(s => s.List(null, null, 0, 0)).Returns(new List<Student>
            {
                new Student("test1", "1") {Id = 1},
                new Student("test2" , "2"){Id = 2}
            });
            var createdStudent = new Student("oli", "");
            repositry.Setup(h => h.DeleteStudent(createdStudent.Id));

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            claimsHelper.Setup(h => h.GetClaims()).Returns(new Claims() { UserClaims = new UserClaim() { Username = "ofebres-cordero" } });

            var context = new Mock<IConsumeContext>(MockBehavior.Strict);
            context.Setup(c => c.Reply(It.IsAny<DeleteStudentReplyMessage>()));

            var handler = new SqlDeleteStudentHandler(repositry.Object, claimsHelper.Object)
            {
                Context = context.Object
            };

            DeleteStudentMessage request = new DeleteStudentMessage(Guid.NewGuid(), (int)createdStudent.Id);
            DeleteStudentReplyMessage reply = null;
            // Act
            handler.Execute(request);

            // Assert
            context.Verify(c => c.Reply(It.IsAny<DeleteStudentReplyMessage>()), Times.Once);
        }

        [Fact]
        public void GivenGetAllMessage_WhenCallingGetAllStudentsHandler_ThenCorrespondingReplyIsSend()
        {
            // Arrange
            var repositry = new Mock<ISqlRepository<IStudent>>(MockBehavior.Strict);
            repositry.Setup(s => s.List(null, null, 0, 0)).Returns(new List<Student>
            {
                new Student("test1", "1") {Id = 1},
                new Student("test2" , "2"){Id = 2}
            });
           
            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            claimsHelper.Setup(h => h.GetClaims()).Returns(new Claims() { UserClaims = new UserClaim() { Username = "ofebres-cordero" } });

            var context = new Mock<IConsumeContext>(MockBehavior.Strict);
            context.Setup(c => c.Reply(It.IsAny<GetAllStudentsReplyMessage>()));

            var handler = new SqlGetAllStudentsHandler(repositry.Object, claimsHelper.Object)
            {
                Context = context.Object
            };

            GetAllStudentsMessage request = new GetAllStudentsMessage(Guid.NewGuid(), null , null, 0, 0);
            GetAllStudentsReplyMessage reply = null;

            // Act
            handler.Execute(request);
            
            // Assert
            context.Verify(c => c.Reply(It.IsAny<GetAllStudentsReplyMessage>()), Times.Once);
            
        }

        [Fact] 
        public void GivenGetIdMessage_WhenCallingGetStudentByIdHandler_ThenCorrespondingReplyIsSend()
        {
            // Arrange
            var repositry = new Mock<ISqlRepository<IStudent>>(MockBehavior.Strict);
            repositry.Setup(s => s.Read(1)).Returns(new Student("test1" , "1"){Id = 1});
           
            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            claimsHelper.Setup(h => h.GetClaims()).Returns(new Claims() { UserClaims = new UserClaim() { Username = "ofebres-cordero" } });

            var context = new Mock<IConsumeContext>(MockBehavior.Strict);
            context.Setup(c => c.Reply(It.IsAny<GetStudentIdReplyMessage>()));

            var handler = new SqlGetStudentIdHandler(repositry.Object, claimsHelper.Object)
            {
                Context = context.Object
            };

            GetStudentIdMessage request = new GetStudentIdMessage(Guid.NewGuid() , 1);
            GetStudentIdReplyMessage reply = null;

            // Act
            handler.Execute(request);

            // Assert
            context.Verify(c => c.Reply(It.IsAny<GetStudentIdReplyMessage>()), Times.Once);

        }

        [Fact] 
        public void GivenGetLastNameMessage_WhenCallingGetStudentByLastNameHandler_ThenCorrespondingReplyIsSend()
        {
            // Arrange
            var repositry = new Mock<ISqlRepository<IStudent>>(MockBehavior.Strict);
            repositry.Setup(s => s.List(null, null, 0, 0)).Returns(new List<Student>
            {
                new Student("test1", "1") {Id = 1},
                new Student("test2" , "2"){Id = 2}
            });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            claimsHelper.Setup(h => h.GetClaims()).Returns(new Claims() { UserClaims = new UserClaim() { Username = "ofebres-cordero" } });

            var context = new Mock<IConsumeContext>(MockBehavior.Strict);
            context.Setup(c => c.Reply(It.IsAny<GetStudentLastNameReplyMessage>()));

            var handler = new SqlGetStudentLastNameHandler(repositry.Object, claimsHelper.Object)
            {
                Context = context.Object
            };

            GetStudentLastnameMessage request = new GetStudentLastnameMessage(Guid.NewGuid() , "2");
            GetStudentLastNameReplyMessage reply = null;

            // Act
            handler.Execute(request);

            // Assert
            context.Verify(c => c.Reply(It.IsAny<GetStudentLastNameReplyMessage>()), Times.Once);

        }

        [Fact] 
        public void GivenUpdateGradeMessage_WhenCallingUpgradeStudentGradeHandler_ThenCorrespondingReplyIsSend()
        {
            // Arrange
            var repositry = new Mock<ISqlRepository<IStudent>>(MockBehavior.Strict);
            var testGrade = new Grade {Subject = "TestGradeUpdated"};

            repositry.Setup(s => s.UpdateGrade(1, testGrade));
            repositry.Setup(s => s.Read(1)).Returns(new Student("test1", "1") { Id = 1 , Grades = new List<Grade>
            {
                testGrade,
                new Grade { Subject = "English" }
            }
            });
            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            claimsHelper.Setup(h => h.GetClaims()).Returns(new Claims() { UserClaims = new UserClaim() { Username = "ofebres-cordero" } });
           
            var context = new Mock<IConsumeContext>(MockBehavior.Strict);
            context.Setup(c => c.Reply(It.IsAny<UpdateStudentGradeReplyMessage>()));
        
            var handler = new SqlUpdateStudentGradeHandler(repositry.Object, claimsHelper.Object)
            {
                Context = context.Object
            };

            UpdateStudentGradeMessage request = new UpdateStudentGradeMessage(Guid.NewGuid(), 1, testGrade, "TestGradeUpdated");
            UpdateStudentGradeReplyMessage reply = null;

            // Act
            handler.Execute(request);

            // Assert
            context.Verify(c => c.Reply(It.IsAny<UpdateStudentGradeReplyMessage>()), Times.Once);

            Assert.Equal("TestGradeUpdated", testGrade.Subject);

        }

        [Fact] // TODO
        public void GivenUpdateStudentMessage_WhenCallingUpgradeStudentHandler_ThenCorrespondingReplyIsSend()
        {
            // Arrange
            var repositry = new Mock<ISqlRepository<IStudent>>(MockBehavior.Strict);
            var updatedStudent = new Student("test1Updated", "1Updated");
            repositry.Setup(s => s.UpdateStudent(updatedStudent));
            repositry.Setup(s => s.Read(1)).Returns(updatedStudent);
            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            claimsHelper.Setup(h => h.GetClaims()).Returns(new Claims() { UserClaims = new UserClaim() { Username = "ofebres-cordero" } });

            var context = new Mock<IConsumeContext>(MockBehavior.Strict);
            context.Setup(c => c.Reply(It.IsAny<UpdateStudentReplyMessage>()));

            var handler = new SqlUpdateStudentHandler(repositry.Object, claimsHelper.Object)
            {
                Context = context.Object
            };

            UpdateStudentMessage request = new UpdateStudentMessage(Guid.NewGuid(), 1 , updatedStudent);
            UpdateStudentReplyMessage reply = null;

            // Act
            handler.Execute(request);

            // Assert
            context.Verify(c => c.Reply(It.IsAny<UpdateStudentReplyMessage>()), Times.Once);

        }
    }
}
