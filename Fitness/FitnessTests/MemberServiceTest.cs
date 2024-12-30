using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Enums;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using FitnessBL.Services;
using Moq;

namespace FitnessTests
{
    public class MemberServiceTest
    {
        private readonly Mock<IMemberRepo> memberRepo;
        private readonly MemberService memberService;

        public MemberServiceTest()
        {
            memberRepo = new Mock<IMemberRepo>();
            memberService = new MemberService(memberRepo.Object);
        }

        [Fact]
        public void GetMembers_WhenNoMembers_ReturnsServiceException()
        {
            // Arrange
            memberRepo.Setup(repo => repo.GetMembers()).Returns(new List<Member>());

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetMembers()
            );
            Assert.Equal("Er zitten nog geen members in de database!", exception.Message);
        }

        [Fact]
        public void GetMembers_WhenMembersExist_ReturnsMembers()
        {
            // Arrange
            Member member1 = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Fitness", "Swimming" },
                TypeKlant.Gold
            );

            Member member2 = new Member(
                2,
                "Jane",
                "Doe",
                "Jane.doe@example.com",
                "Some Other Street 123",
                new DateTime(1991, 1, 1),
                new List<string> { "Fitness", "running" },
                TypeKlant.Silver
            );

            List<Member> members = new List<Member>();

            members.Add(member1);
            members.Add(member2);

            memberRepo.Setup(repo => repo.GetMembers()).Returns(members);

            // Act
            IEnumerable<Member> result = memberService.GetMembers();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(member1, result.ElementAt(0));
            Assert.Equal(member2, result.ElementAt(1));
        }

        [Fact]
        public void GetTrainingSessionsMember_WhenMemberIsNull_ThrowsServiceException()
        {
            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMember(null)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMember - member is null!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMember_WhenMemberDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                55,
                "Jane",
                "Doe",
                "jane.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Running" },
                TypeKlant.Silver
            );
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMember(member)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMember - member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMember_WhenMemberHasNoSessions_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                1,
                "Jane",
                "Doe",
                "jane.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Running" },
                TypeKlant.Silver
            );
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true);

            List<TrainingSession> trainingSessions = new List<TrainingSession>();

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMember(member)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMember - Deze member heeft nog geen TrainingSessions!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMember_WhenSessionsExist_ReturnsTrainingSessions()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Fitness" },
                TypeKlant.Gold
            );
            Cyclingsession cyclingSession = new Cyclingsession(
                1,
                DateTime.Now,
                60,
                10,
                300,
                80,
                100,
                "Endurance",
                null,
                member
            );

            Runningsession_main runningsession = new Runningsession_main(
                1,
                DateTime.Now,
                60,
                10,
                member
            );

            List<TrainingSession> trainingSessions = new List<TrainingSession>();
            trainingSessions.Add(cyclingSession);
            trainingSessions.Add(runningsession);

            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true);

            memberRepo
                .Setup(repo => repo.GetTrainingSessionsMember(member))
                .Returns(trainingSessions);

            // Act
            var result = memberService.GetTrainingSessionsMember(member);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(cyclingSession, result.ElementAt(0));
            Assert.Equal(runningsession, result.ElementAt(1));
        }

        [Fact]
        public void GetProgramListMember_WhenMemberIsNull_ThrowsServiceException()
        {
            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetProgramListMember(null)
            );
            Assert.Equal(
                "MemberService - GetProgramListMember - Member is null",
                exception.Message
            );
        }

        [Fact]
        public void GetProgramListMember_WhenMemberDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                55,
                "Jane",
                "Doe",
                "jane.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Running" },
                TypeKlant.Silver
            );
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetProgramListMember(member)
            );
            Assert.Equal(
                "MemberService - GetProgramListMember - Er bestaat geen member met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetProgramListMember_WhenMemberHasNoPrograms_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                1,
                "Jane",
                "Doe",
                "jane.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Running" },
                TypeKlant.Silver
            );
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true);

            List<Program> programs = new List<Program>();

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetProgramListMember(member)
            );
            Assert.Equal(
                "MemberService - GetProgramListMember - Deze member is nog voor geen enkel Program ingeschreven!",
                exception.Message
            );
        }

        [Fact]
        public void GetProgramListMember_WhenMemberHasPrograms_ReturnsPrograms()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Fitness" },
                TypeKlant.Gold
            );

            Program program1 = new Program(
                "Prog2",
                "Programmeren 2",
                "Graduaat studenten",
                new DateTime(2024, 9, 26),
                55
            );

            Program program2 = new Program(
                "Prog3",
                "Programmeren 3",
                "Graduaat studenten",
                new DateTime(2024, 9, 26),
                50
            );

            List<Program> programs = new List<Program>();
            programs.Add(program1);
            programs.Add(program2);

            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true);

            memberRepo.Setup(repo => repo.GetProgramListMember(member)).Returns(programs);

            // Act
            var result = memberService.GetProgramListMember(member);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(program1, result.ElementAt(0));
            Assert.Equal(program2, result.ElementAt(1));
        }

        [Fact]
        public void GetTrainingSessionsMemberInMaandInJaar_MemberIsNull_ThrowsServiceException()
        {
            // Arrange
            Member member = null;
            DateTime date = new DateTime(2024, 1, 1);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberInMaandInJaar(member, date)
            );
            Assert.Equal(
                "MemberService - TrainingSessionsMemberPerMaandInJaar - member is null!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberInMaandInJaar_MemberDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                55,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            DateTime date = new DateTime(2024, 1, 1);

            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(false); // Simuleer dat het lid niet bestaat

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberInMaandInJaar(member, date)
            );
            Assert.Equal(
                "MemberService - TrainingSessionsMemberPerMaandInJaar - member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberInMaandInJaar_NoTrainingSessions_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            DateTime date = new DateTime(2024, 1, 1);

            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true); // Simuleer dat het lid bestaat
            memberRepo
                .Setup(repo => repo.GetTrainingSessionsMemberInMaandInJaar(member, date))
                .Returns(new List<TrainingSession>()); // Simuleer dat er geen trainingssessies zijn

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberInMaandInJaar(member, date)
            );
            Assert.Equal(
                $"MemberService - TrainingSessionsMemberPerMaandInJaar - Deze member heeft geen TrainingSessions in maand {date.Month} in jaar {date.Year}!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberInMaandInJaar_ValidMember_ReturnsTrainingSessions()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            DateTime date = new DateTime(2024, 1, 1);
            List<TrainingSession> expectedSessions = new List<TrainingSession>
            {
                new Cyclingsession(
                    1,
                    date,
                    60,
                    200,
                    250,
                    80,
                    100,
                    "Endurance",
                    "Good session",
                    member
                ),
                new Runningsession_main(1, date, 45, 10.5f, member)
            };

            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true); // Simuleer dat het lid bestaat
            memberRepo
                .Setup(repo => repo.GetTrainingSessionsMemberInMaandInJaar(member, date))
                .Returns(expectedSessions); // Simuleer de trainingssessies

            // Act
            IEnumerable<TrainingSession> actualSessions =
                memberService.GetTrainingSessionsMemberInMaandInJaar(member, date);

            // Assert
            Assert.Equal(expectedSessions.Count, actualSessions.Count());
            Assert.Equal(expectedSessions, actualSessions);
            Assert.Equal(expectedSessions.ElementAt(0), actualSessions.ElementAt(0));
            Assert.Equal(expectedSessions.ElementAt(1), actualSessions.ElementAt(1));
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaar_MemberIsNull_ThrowsServiceException()
        {
            // Arrange
            DateTime date = DateTime.Now;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberAantalPerMaandInJaar(null, date)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - member is null!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaar_MemberDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                55,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            DateTime date = DateTime.Now;
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberAantalPerMaandInJaar(member, date)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaar_NoSessionsFound_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );

            DateTime date = new DateTime(2023, 5, 1);
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepo
                .Setup(repo => repo.GetTrainingSessionsMemberAantalPerMaandInJaar(member, date))
                .Returns(new Dictionary<int, int>());

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberAantalPerMaandInJaar(member, date)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - Deze member heeft geen TrainingSessions in maand 5 jaar 2023!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaar_SessionsFound_ReturnsDictionary()
        {
            // Arrange
            Member member = new Member(
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            DateTime date = new DateTime(2023, 5, 1);
            var expected = new Dictionary<int, int>
            {
                { 1, 10 }, // Januari heeft 10 sessies
                { 5, 3 }, // May 2023 has 3 sessions
            };

            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepo
                .Setup(repo => repo.GetTrainingSessionsMemberAantalPerMaandInJaar(member, date))
                .Returns(expected);

            // Act
            var result = memberService.GetTrainingSessionsMemberAantalPerMaandInJaar(member, date);

            // Assert
            Assert.Equal(expected, result);
            Assert.Equal(expected[1], result[1]);
            Assert.Equal(expected[5], result[5]);
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaarMetType_MemberIsNull_ThrowsServiceException()
        {
            // Arrange
            DateTime date = DateTime.Now;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(null, date)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaarMetType - member is null!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaarMetType_MemberDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                55,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            DateTime date = DateTime.Now;
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () =>
                    memberService.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(member, date)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaarMetType - member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaarMetType_NoSessionsFound_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            DateTime date = new DateTime(2023, 5, 1);
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepo
                .Setup(repo =>
                    repo.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(member, date)
                )
                .Returns(new Dictionary<string, Dictionary<int, int>>());

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () =>
                    memberService.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(member, date)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - Deze member heeft geen TrainingSessions in maand 5 jaar 2023!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaarMetType_SessionsFound_ReturnsDictionary()
        {
            // Arrange
            Member member = new Member(
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            DateTime date = new DateTime(2023, 5, 1);
            var expected = new Dictionary<string, Dictionary<int, int>>
            {
                {
                    "Cycling",
                    new Dictionary<int, int> { { 1, 10 }, { 5, 3 } }
                }, // Maand 1, heeft 10 sessiesMaand 5, heeft 3 sessies
                {
                    "Running",
                    new Dictionary<int, int> { { 2, 8 }, { 3, 5 } }
                } // Maand 2, heeft 8 sessies Maand 3, heeft 5 sessies
            };

            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepo
                .Setup(repo =>
                    repo.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(member, date)
                )
                .Returns(expected);

            // Act
            var result = memberService.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(
                member,
                date
            );

            // Assert
            Assert.Equal(expected, result);
            Assert.Equal(expected["Cycling"], result["Cycling"]);
            Assert.Equal(expected["Running"], result["Running"]);
        }

        [Fact]
        public void GetMemberId_MemberExists_ReturnsMember()
        {
            // Arrange
            int memberId = 1;
            Member expectedMember = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );

            memberRepo.Setup(repo => repo.GetMemberId(memberId)).Returns(expectedMember);

            // Act
            Member result = memberService.GetMemberId(memberId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMember, result);
        }

        [Fact]
        public void GetMemberId_MemberDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            int memberId = 1;

            memberRepo.Setup(repo => repo.GetMemberId(memberId)).Returns((Member)null); // Return null to simulate member not found

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetMemberId(memberId)
            );
            Assert.Equal(
                "MemberService - GetMemberId - Er is geen member met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void AddMember_WhenMemberIsNull_ThrowsServiceException()
        {
            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.AddMember(null)
            );
            Assert.Equal("MemberService - AddMember - Member is null", exception.Message);
        }

        [Fact]
        public void AddMember_WhenNameExists_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Fitness" },
                TypeKlant.Silver
            );
            memberRepo.Setup(repo => repo.IsMemberName(member)).Returns(true);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.AddMember(member)
            );
            Assert.Equal(
                "MemberService - AddMember - Member bestaat al (zelfde naam)!",
                exception.Message
            );
        }

        [Fact]
        public void AddMember_WhenEmailExists_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Fitness" },
                TypeKlant.Silver
            );
            memberRepo.Setup(repo => repo.IsMemberEmail(member)).Returns(true);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.AddMember(member)
            );
            Assert.Equal(
                "MemberService - AddMember - Dit email is al in gebruik!",
                exception.Message
            );
        }

        [Fact]
        public void AddMember_WhenMemberIsValid_ReturnsMember()
        {
            // Arrange
            Member member = new Member(
                1,
                "Jane",
                "Doe",
                "jane.doe@example.com",
                "Some Street 456",
                new DateTime(1995, 5, 15),
                new List<string> { "Yoga" },
                TypeKlant.Gold
            );
            memberRepo.Setup(repo => repo.IsMemberEmail(member)).Returns(false);
            memberRepo.Setup(repo => repo.IsMemberName(member)).Returns(false);
            memberRepo.Setup(repo => repo.AddMember(member)).Returns(member);

            // Act
            var result = memberService.AddMember(member);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Member_id);
            Assert.Equal("Jane", result.FirstName);
        }

        [Fact]
        public void UpdateMember_MemberIsNull_ThrowsServiceException()
        {
            // Arrange
            Member member = null;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.UpdateMember(member)
            );
            Assert.Equal("MemberService - UpdateMember - member is null!", exception.Message);
        }

        [Fact]
        public void UpdateMember_MemberDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(false); // Simuleer dat het lid niet bestaat

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.UpdateMember(member)
            );
            Assert.Equal(
                "MemberService - UpdateMember - Member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void UpdateMember_MemberExists_UpdatesAndReturnsMember()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true); // Simuleer dat het lid bestaat
            memberRepo.Setup(repo => repo.UpdateMember(member)).Verifiable(); // Verifieer dat UpdateMember wordt aangeroepen

            // Act
            Member result = memberService.UpdateMember(member);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(member, result);
        }

        [Fact]
        public void DeleteMember_MemberDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(false); // Simuleer dat het lid niet bestaat

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.DeleteMember(member)
            );
            Assert.Equal(
                "MemberService - DeleteMember - member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void DeleteMember_MemberExists_DeletesMember()
        {
            // Arrange
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true); // Simuleer dat het lid bestaat
            memberRepo.Setup(repo => repo.DeleteMember(member)).Verifiable(); // Verifieer dat DeleteMember wordt aangeroepen

            // Act
            memberService.DeleteMember(member);

            // Assert
            memberRepo.Verify(repo => repo.DeleteMember(member), Times.Once); // Verifieer dat DeleteMember slechts één keer werd aangeroepen
        }

        [Fact]
        public void GetAantalGeboekteTijdsloten_MemberIsNull_ThrowsServiceException()
        {
            // Arrange
            Member member = null;
            DateTime date = new DateTime(2024, 1, 1);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetAantalGeboekteTijdsloten(member, date)
            );
            Assert.Equal(
                "MemberService - GetAantalGeboekteTijdsloten - member is null!",
                exception.Message
            );
        }

        [Fact]
        public void GetAantalGeboekteTijdsloten_MemberDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Member member = new Member(
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            DateTime date = new DateTime(2024, 1, 1);

            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(false); // Simuleer dat het lid niet bestaat

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetAantalGeboekteTijdsloten(member, date)
            );
            Assert.Equal(
                "MemberService - GetAantalGeboekteTijdsloten - member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetAantalGeboekteTijdsloten_ValidMember_ReturnsAantalGeboekteTijdsloten()
        {
            // Arrange
            Member member = new Member(
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Cycling" },
                TypeKlant.Gold
            );
            DateTime date = new DateTime(2024, 1, 1);

            int expectedAantal = 4;

            memberRepo.Setup(repo => repo.IsMemberId(member)).Returns(true); // Simuleer dat het lid bestaat
            memberRepo
                .Setup(repo => repo.GetAantalGeboekteTijdsloten(date, member))
                .Returns(expectedAantal); // Simuleer het aantal geboekte tijdsloten

            // Act
            int actualAantal = memberService.GetAantalGeboekteTijdsloten(member, date);

            // Assert
            Assert.Equal(expectedAantal, actualAantal);
        }
    }
}
