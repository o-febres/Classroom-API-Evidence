using System;
using System.Collections.Generic;
using System.Linq;
using Gradebook.Controllers;
using Gradebook.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Ruffer.Security.Web;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;
using Xunit;

namespace GradebookUnitTests
{
    public class StudentControllerTest
    {
        [Fact]
        public void GivenEmptyQueryStrings_WhenCallingGetAll_ThenTwoDummyResultsShouldBeReturned()
        {
            // Arrange
            var students = new List<IStudent>
            {
                new Student("Joe", "Smith"),
                new Student("John", "Bloggs")
            };

            var reply = new GetAllStudentsReplyMessage(Guid.NewGuid())
            {
                Students =  students.Cast<Student>()
            };

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(3);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest<GetAllStudentsMessage, GetAllStudentsReplyMessage>(It.IsAny<string>(),
                    It.IsAny<GetAllStudentsMessage>(), null, It.IsAny<int>()))
                .Returns(reply);

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.GetAll("", "", 0, 0);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GivenASearchValue_WhenCallingGetAll_ThenThatSearchValueIsPassedToTheMessage()
        {
            // Arrange
            var query = "Name=Joe";
            var sortby = "Name DESC";
            var pageSize = 10;
            var page = 0;

            var students = new List<IStudent>
            {
                new Student("Joe", "Smith1") {Id = 3},
                new Student("Joe", "Smith2") {Id = 1},
                new Student("Joe", "Smith3") {Id = 2}
            };

            GetAllStudentsMessage request = null;
            GetAllStudentsReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(3);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest<GetAllStudentsMessage, GetAllStudentsReplyMessage>(It.IsAny<string>(),
                    It.IsAny<GetAllStudentsMessage>(), null, It.IsAny<int>()))
                .Returns<string, GetAllStudentsMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new GetAllStudentsReplyMessage(message.CorrelationId)
                        {
                            Students = students.Cast<Student>()
                        };
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.GetAll(query, sortby, pageSize, page);

            // Assert
            Assert.Equal(query, request.Query);
            Assert.Equal(sortby, request.SortBy);
            Assert.Equal(pageSize, request.PageSize);
            Assert.Equal(page, request.Page);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Equal(1, result.First().Id);
            Assert.Equal(3, result.Last().Id);
        }

        [Fact]
        public void GivenSearchQueryByScore_WhenCallingGetStudentGrades_ThenThatReplyShouldBeSortedCorrectly()
        {
            // Arrange
            var id = 2;
            var startingWith = "ma";
            var descendingbool = true;
            var orderBy = "score";

            var students = new List<Student>
            {
                new Student("Joe", "Smith1")
                {
                    Id = 2,
                    Grades = new List<Grade>
                    {
                        new Grade {Id = 1, Score = 50, Subject = "Maths", StudentId = 2},
                        new Grade {Id = 1, Score = 50, Subject = "maths1", StudentId = 2},
                        new Grade {Id = 1, Score = 50, Subject = "twoMaths", StudentId = 2}
                    }
                }
            };

            GetStudentIdMessage request = null;
            GetStudentIdReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(3);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest<GetStudentIdMessage, GetStudentIdReplyMessage>(It.IsAny<string>(),
                    It.IsAny<GetStudentIdMessage>(), null, It.IsAny<int>()))
                .Returns<string, GetStudentIdMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new GetStudentIdReplyMessage(message.CorrelationId)
                        {
                            Student = students.First()
                        };
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.GetGrades((int) students.First().Id, startingWith, descendingbool, orderBy);


            // Assert
            Assert.Equal(id, request.Id);

            var expectedResult = reply.Student.Grades.Where(x => x.Subject.StartsWith(startingWith))
                .OrderBy(x => x.Score)
                .ThenBy(_ => _.Subject);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GivenSearchQueryByName_WhenCallingGetStudentGrades_ThenThatReplyShouldBeSortedCorrectly()
        {
            // Arrange
            var id = 2;
            var startingWith = "ma";
            var descendingbool = true;
            var orderBy = "name";

            var students = new List<Student>
            {
                new Student("Joe", "Smith1")
                {
                    Id = 2,
                    Grades = new List<Grade>
                    {
                        new Grade {Id = 1, Score = 50, Subject = "Maths", StudentId = 2},
                        new Grade {Id = 1, Score = 50, Subject = "maths1", StudentId = 2},
                        new Grade {Id = 1, Score = 50, Subject = "twoMaths", StudentId = 2}
                    }
                }
            };

            GetStudentIdMessage request = null;
            GetStudentIdReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(3);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest<GetStudentIdMessage, GetStudentIdReplyMessage>(It.IsAny<string>(),
                    It.IsAny<GetStudentIdMessage>(), null, It.IsAny<int>()))
                .Returns<string, GetStudentIdMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new GetStudentIdReplyMessage(message.CorrelationId)
                        {
                            Student = students.First()
                        };
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.GetGrades((int) students.First().Id, startingWith, descendingbool, orderBy);


            // Assert
            Assert.Equal(id, request.Id);

            var expectedResult = reply.Student.Grades.Where(x => x.Subject.StartsWith(startingWith))
                .OrderBy(x => x.Subject)
                .ThenBy(_ => _.Score);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GivenNoSearchQuery_WhenCallingGetStudentGrades_ThenThatReplyShouldShowAllGrades()
        {
            // Arrange
            var id = 2;
            string startingWith = null;
            var descendingbool = false;
            string orderBy = null;

            var students = new List<Student>
            {
                new Student("Joe", "Smith1")
                {
                    Id = 2,
                    Grades = new List<Grade>
                    {
                        new Grade {Id = 1, Score = 50, Subject = "Maths", StudentId = 2},
                        new Grade {Id = 1, Score = 50, Subject = "maths1", StudentId = 2},
                        new Grade {Id = 1, Score = 50, Subject = "twoMaths", StudentId = 2}
                    }
                }
            };

            GetStudentIdMessage request = null;
            GetStudentIdReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(1);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest<GetStudentIdMessage, GetStudentIdReplyMessage>(It.IsAny<string>(),
                    It.IsAny<GetStudentIdMessage>(), null, It.IsAny<int>()))
                .Returns<string, GetStudentIdMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new GetStudentIdReplyMessage(message.CorrelationId)
                        {
                            Student = students.First()
                        };
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.GetGrades((int) students.First().Id, startingWith, descendingbool, orderBy);


            // Assert
            Assert.Equal(id, request.Id);

            var expectedResult = reply.Student.Grades;

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void SearchGradeGivenStudent_WhenCallingGetStudentGradesSubject_ThenThatReplyShouldShowOneGrade()
        {
            // Arrange
            var id = 2;

            var students = new List<Student>
            {
                new Student("Joe", "Smith1")
                {
                    Id = 2,
                    Grades = new List<Grade>
                    {
                        new Grade {Id = 1, Score = 50, Subject = "Maths", StudentId = 2},
                        new Grade {Id = 1, Score = 50, Subject = "maths1", StudentId = 2},
                        new Grade {Id = 1, Score = 50, Subject = "twoMaths", StudentId = 2}
                    }
                }
            };

            GetStudentIdMessage request = null;
            GetStudentIdReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(0);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest<GetStudentIdMessage, GetStudentIdReplyMessage>(It.IsAny<string>(),
                    It.IsAny<GetStudentIdMessage>(), null, It.IsAny<int>()))
                .Returns<string, GetStudentIdMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new GetStudentIdReplyMessage(message.CorrelationId)
                        {
                            Student = students.First()
                        };
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.GetStudentGrade((int) students.First().Id, "Maths");


            // Assert
            Assert.Equal(id, request.Id);

            var expectedResult = reply.Student.Grades.Where(_ => _.Subject == "Maths");

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void SearchStudentGivenId_WhenCallingGetById_ThenThatReplyShouldShowOneStudent()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student("Joe", "Smith1")
                {
                    Id = 1,
                    Grades = new List<Grade>
                    {
                        new Grade {Id = 1, Score = 50, Subject = "Maths", StudentId = 1},
                        new Grade {Id = 1, Score = 50, Subject = "maths1", StudentId = 1},
                        new Grade {Id = 1, Score = 50, Subject = "twoMaths", StudentId = 1}
                    }
                },
                new Student("Oli2", "Test2") {Id = 2},
                new Student("Oli3", "Test3") {Id = 3},
                new Student("Oli4", "Test4") {Id = 4}
            };

            GetStudentIdMessage request = null;
            GetStudentIdReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(0);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest<GetStudentIdMessage, GetStudentIdReplyMessage>(It.IsAny<string>(),
                    It.IsAny<GetStudentIdMessage>(), null, It.IsAny<int>()))
                .Returns<string, GetStudentIdMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new GetStudentIdReplyMessage(message.CorrelationId)
                        {
                            Student = students.First(_ => _.Id == message.Id)
                        };
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.GetById(3);

            // Assert

            var expectedResult = reply.Student;

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void SearchStudentGivenLastName_WhenCallingGetByName_ThenThatReplyShouldShowOneStudent()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student("Joe", "Smith1")
                {
                    Id = 1,
                    Grades = new List<Grade>
                    {
                        new Grade {Id = 1, Score = 50, Subject = "Maths", StudentId = 1},
                        new Grade {Id = 1, Score = 50, Subject = "maths1", StudentId = 1},
                        new Grade {Id = 1, Score = 50, Subject = "twoMaths", StudentId = 1}
                    }
                },
                new Student("Oli2", "Test2") {Id = 2},
                new Student("Oli3", "Test3") {Id = 3},
                new Student("Oli4", "Test4") {Id = 4}
            };
            var testName = "Test4";
            GetStudentLastnameMessage request = null;
            GetStudentLastNameReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(0);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest
                    <GetStudentLastnameMessage, GetStudentLastNameReplyMessage>
                    (It.IsAny<string>(), It.IsAny<GetStudentLastnameMessage>(), null, It.IsAny<int>()))
                .Returns<string, GetStudentLastnameMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new GetStudentLastNameReplyMessage(message.CorrelationId)
                        {
                            Student = students.First(_ => _.LastName == testName)
                        };
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.GetByName(testName);

            // Assert

            var expectedResult = reply.Student;

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GivenCreatingStudent_WhenPosting_ThenCorrespondingStudentIsCreated()
        {
            // Arrange

            var testStudent = new Student("OliCreationTest", "test");
            CreateStudentMessage request = null;
            CreateStudentReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(0);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest
                    <CreateStudentMessage, CreateStudentReplyMessage>
                    (It.IsAny<string>(), It.IsAny<CreateStudentMessage>(), null, It.IsAny<int>()))
                .Returns<string, CreateStudentMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new CreateStudentReplyMessage(message.CorrelationId);
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.CreateStudent(testStudent);
            var okObjectResult = result as AcceptedResult;

            // Assert

            Assert.NotNull(okObjectResult);
        }

        [Fact]
        public void GivenCreatingGrade_WhenPosting_ThenCorrespondingGradeIsCreated()
        {
            // Arrange

            var testStudent = new Student("OliCreationTest", "test")
            {
                Id = 1,
                Grades = new List<Grade>
                {
                    new Grade {Subject = "Maths"}
                }
            };
            var testGrade = new Grade {Subject = "ICT", Score = 100};
            CreateStudentGradeMessage request = null;
            CreateStudentGradeReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(0);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest
                    <CreateStudentGradeMessage, CreateStudentGradeReplyMessage>
                    (It.IsAny<string>(), It.IsAny<CreateStudentGradeMessage>(), null, It.IsAny<int>()))
                .Returns<string, CreateStudentGradeMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new CreateStudentGradeReplyMessage(message.CorrelationId);
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.CreateGrade(testGrade, 1);
            var okObjectResult = result as AcceptedResult;

            // Assert

            Assert.NotNull(okObjectResult);
        }

        [Fact]
        public void GivenUpdatingStudent_WhenPutting_ThenCorrespondingStudentIsUpdated()
        {
            // Arrange

            var testStudent = new Student("OliCreationTest", "test") {Id = 1};
            var updatedStudent = new Student("override", "override");
            UpdateStudentMessage request = null;
            UpdateStudentReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(0);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest
                    <UpdateStudentMessage, UpdateStudentReplyMessage>
                    (It.IsAny<string>(), It.IsAny<UpdateStudentMessage>(), null, It.IsAny<int>()))
                .Returns<string, UpdateStudentMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new UpdateStudentReplyMessage(message.CorrelationId);
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.Update(1, updatedStudent);
            var okObjectResult = result as AcceptedResult;

            // Assert

            Assert.NotNull(okObjectResult);
        }

        [Fact]
        public void GivenUpdatingGrade_WhenPutting_ThenCorrespondingGradeIsUpdated()
        {
            // Arrange

            var testStudent = new Student("OliCreationTest", "test")
            {
                Id = 1,
                Grades = new List<Grade>
                {
                    new Grade {Subject = "Maths", Score = 10}
                }
            };
            var testGrade = new Grade {Score = 100};
            UpdateStudentGradeMessage request = null;
            UpdateStudentGradeReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(0);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest
                    <UpdateStudentGradeMessage, UpdateStudentGradeReplyMessage>
                    (It.IsAny<string>(), It.IsAny<UpdateStudentGradeMessage>(), null, It.IsAny<int>()))
                .Returns<string, UpdateStudentGradeMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new UpdateStudentGradeReplyMessage(message.CorrelationId);
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.UpdateGrade(testStudent.Id, testGrade, "Maths");
            var okObjectResult = result as AcceptedResult;

            // Assert

            Assert.NotNull(okObjectResult);
        }

        [Fact]
        public void GivenDeletingStudent_WhenDeleting_ThenCorrespondingStudentIsDeleted()
        {
            // Arrange

            var testStudent = new Student("OliCreationTest", "test") {Id = 1};
            DeleteStudentMessage request = null;
            DeleteStudentReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(0);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest
                    <DeleteStudentMessage, DeleteStudentReplyMessage>
                    (It.IsAny<string>(), It.IsAny<DeleteStudentMessage>(), null, It.IsAny<int>()))
                .Returns<string, DeleteStudentMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new DeleteStudentReplyMessage(message.CorrelationId);
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.Delete((int) testStudent.Id);
            var okObjectResult = result as ObjectResult;

            // Assert

            Assert.Equal(reply.CorrelationId, okObjectResult.Value);
        }

        [Fact]
        public void GivenDeletingStudentGrade_WhenDeleting_ThenCorrespondingStudentGradeIsDeleted()
        {
            // Arrange

            var testStudent = new Student("OliCreationTest", "test")
            {
                Id = 1,
                Grades = new List<Grade>
                {
                    new Grade {Subject = "Maths"}
                }
            };
            DeleteStudentGradeMessage request = null;
            DeleteStudentGradeReplyMessage reply = null;

            var queryCountWrapper = new Mock<IQueryCountWrapper>();
            queryCountWrapper.SetupGet(qcw => qcw.QueryCount).Returns(0);

            var bus = new Mock<IBus>(MockBehavior.Strict);
            bus
                .Setup(b => b.SendRequest
                    <DeleteStudentGradeMessage, DeleteStudentGradeReplyMessage>
                    (It.IsAny<string>(), It.IsAny<DeleteStudentGradeMessage>(), null, It.IsAny<int>()))
                .Returns<string, DeleteStudentGradeMessage, Dictionary<string, string>, int>(
                    (endpoint, message, headers, timeout) =>
                    {
                        request = message;
                        reply = new DeleteStudentGradeReplyMessage(message.CorrelationId);
                        return reply;
                    });

            var claimsHelper = new Mock<IClaimsHelper>(MockBehavior.Strict);
            var controller = new StudentController(bus.Object, claimsHelper.Object, queryCountWrapper.Object);

            // Act
            var result = controller.DeleteStudentGrade((int) testStudent.Id, "Maths");
            var okObjectResult = result as ObjectResult;

            // Assert

            Assert.Equal(reply.CorrelationId, okObjectResult.Value);
        }
    }
}