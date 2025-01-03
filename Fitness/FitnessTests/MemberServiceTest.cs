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

        private Member member1;
        private Cyclingsession cyclingSession;
        private Runningsession_main runningsession;
        private DateTime date;

        public MemberServiceTest()
        {
            memberRepo = new Mock<IMemberRepo>();
            memberService = new MemberService(memberRepo.Object);

            member1 = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Fitness", "Swimming" },
                TypeKlant.Gold
            );

            cyclingSession = new Cyclingsession(
                1,
                DateTime.Now,
                60,
                10,
                300,
                80,
                100,
                "Endurance",
                null,
                member1
            );

            runningsession = new Runningsession_main(1, DateTime.Now, 60, 10, member1);

            date = new DateTime(2024, 1, 1);
        }

        [Fact]
        public void GetMembers_WhenNoMembers_ReturnsServiceException()
        {
            // Mocks
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
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMember(member1)
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
            List<TrainingSession> trainingSessions = new List<TrainingSession>();

            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMember(member1)
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
            List<TrainingSession> trainingSessions = new List<TrainingSession>();
            trainingSessions.Add(cyclingSession);
            trainingSessions.Add(runningsession);

            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);

            memberRepo
                .Setup(repo => repo.GetTrainingSessionsMember(member1))
                .Returns(trainingSessions);

            // Act
            IEnumerable<TrainingSession> result = memberService.GetTrainingSessionsMember(member1);

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
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetProgramListMember(member1)
            );
            Assert.Equal(
                "MemberService - GetProgramListMember - Er bestaat geen member met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetProgramListMember_WhenMemberHasNoPrograms_ThrowsServiceException()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);

            List<Program> programs = new List<Program>();

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetProgramListMember(member1)
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

            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);

            memberRepo.Setup(repo => repo.GetProgramListMember(member1)).Returns(programs);

            // Act
            IEnumerable<Program> result = memberService.GetProgramListMember(member1);

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
            DateTime date = new DateTime(2024, 1, 1);

            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(false); // Simuleer dat het lid niet bestaat

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberInMaandInJaar(member1, date)
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
            DateTime date = new DateTime(2024, 1, 1);

            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true); // Simuleer dat het lid bestaat
            memberRepo
                .Setup(repo => repo.GetTrainingSessionsMemberInMaandInJaar(member1, date))
                .Returns(new List<TrainingSession>()); // Simuleer dat er geen trainingssessies zijn

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberInMaandInJaar(member1, date)
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
            List<TrainingSession> expectedSessions = new List<TrainingSession>
            {
                cyclingSession,
                runningsession
            };

            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true); // Simuleer dat het lid bestaat
            memberRepo
                .Setup(repo => repo.GetTrainingSessionsMemberInMaandInJaar(member1, date))
                .Returns(expectedSessions); // Simuleer de trainingssessies

            // Act
            IEnumerable<TrainingSession> actualSessions =
                memberService.GetTrainingSessionsMemberInMaandInJaar(member1, date);

            // Assert
            Assert.Equal(expectedSessions.Count, actualSessions.Count());
            Assert.Equal(expectedSessions, actualSessions);
            Assert.Equal(expectedSessions.ElementAt(0), actualSessions.ElementAt(0));
            Assert.Equal(expectedSessions.ElementAt(1), actualSessions.ElementAt(1));
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaar_MemberIsNull_ThrowsServiceException()
        {
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
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberAantalPerMaandInJaar(member1, date)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaar_NoSessionsFound_ThrowsServiceException()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);
            memberRepo
                .Setup(repo => repo.GetTrainingSessionsMemberAantalPerMaandInJaar(member1, date))
                .Returns(new Dictionary<int, int>());

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetTrainingSessionsMemberAantalPerMaandInJaar(member1, date)
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - Deze member heeft geen TrainingSessions in maand 1 jaar 2024!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaar_SessionsFound_ReturnsDictionary()
        {
            // Arrange
            Dictionary<int, int> expected = new Dictionary<int, int>
            {
                { 1, 10 }, // Januari heeft 10 sessies
                { 5, 3 }, // May 2023 has 3 sessions
            };

            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);
            memberRepo
                .Setup(repo => repo.GetTrainingSessionsMemberAantalPerMaandInJaar(member1, date))
                .Returns(expected);

            // Act
            Dictionary<int, int> result =
                memberService.GetTrainingSessionsMemberAantalPerMaandInJaar(member1, date);

            // Assert
            Assert.Equal(expected, result);
            Assert.Equal(expected[1], result[1]);
            Assert.Equal(expected[5], result[5]);
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaarMetType_MemberIsNull_ThrowsServiceException()
        {
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
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () =>
                    memberService.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(
                        member1,
                        date
                    )
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaarMetType - member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaarMetType_NoSessionsFound_ThrowsServiceException()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);
            memberRepo
                .Setup(repo =>
                    repo.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(member1, date)
                )
                .Returns(new Dictionary<string, Dictionary<int, int>>());

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () =>
                    memberService.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(
                        member1,
                        date
                    )
            );
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - Deze member heeft geen TrainingSessions in maand 1 jaar 2024!",
                exception.Message
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaarMetType_SessionsFound_ReturnsDictionary()
        {
            Dictionary<string, Dictionary<int, int>> expected = new Dictionary<
                string,
                Dictionary<int, int>
            >
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

            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);
            memberRepo
                .Setup(repo =>
                    repo.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(member1, date)
                )
                .Returns(expected);

            // Act
            var result = memberService.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(
                member1,
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

            // Mocks
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

            // Mocks
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
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberName(member1)).Returns(true);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.AddMember(member1)
            );
            Assert.Equal(
                "MemberService - AddMember - Member bestaat al (zelfde naam)!",
                exception.Message
            );
        }

        [Fact]
        public void AddMember_WhenEmailExists_ThrowsServiceException()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberEmail(member1)).Returns(true);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.AddMember(member1)
            );
            Assert.Equal(
                "MemberService - AddMember - Dit email is al in gebruik!",
                exception.Message
            );
        }

        [Fact]
        public void AddMember_WhenMemberIsValid_ReturnsMember()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberEmail(member1)).Returns(false);
            memberRepo.Setup(repo => repo.IsMemberName(member1)).Returns(false);
            memberRepo.Setup(repo => repo.AddMember(member1)).Returns(member1);

            // Act
            var result = memberService.AddMember(member1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Member_id);
            Assert.Equal("John", result.FirstName);
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
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(false); // Simuleer dat het lid niet bestaat

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.UpdateMember(member1)
            );
            Assert.Equal(
                "MemberService - UpdateMember - Member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void UpdateMember_MemberExists_UpdatesAndReturnsMember()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true); // Simuleer dat het lid bestaat
            memberRepo.Setup(repo => repo.UpdateMember(member1)).Verifiable(); // Verifieer dat UpdateMember wordt aangeroepen

            // Act
            Member result = memberService.UpdateMember(member1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(member1, result);
        }

        [Fact]
        public void DeleteMember_MemberDoesNotExist_ThrowsServiceException()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(false); // Simuleer dat het lid niet bestaat

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.DeleteMember(member1)
            );
            Assert.Equal(
                "MemberService - DeleteMember - member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void DeleteMember_MemberExists_DeletesMember()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true); // Simuleer dat het lid bestaat
            memberRepo.Setup(repo => repo.DeleteMember(member1)).Verifiable(); // Verifieer dat DeleteMember wordt aangeroepen

            // Act
            memberService.DeleteMember(member1);

            // Assert
            memberRepo.Verify(repo => repo.DeleteMember(member1), Times.Once); // Verifieer dat DeleteMember slechts één keer werd aangeroepen
        }

        [Fact]
        public void GetAantalGeboekteTijdsloten_MemberIsNull_ThrowsServiceException()
        {
            // Arrange
            Member member = null;

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
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(false); // Simuleer dat het lid niet bestaat

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetAantalGeboekteTijdsloten(member1, date)
            );
            Assert.Equal(
                "MemberService - GetAantalGeboekteTijdsloten - member bestaat niet met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetAantalGeboekteTijdsloten_ValidMember_ReturnsAantalGeboekteTijdsloten()
        {
            int expectedAantal = 4;

            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true); // Simuleer dat het lid bestaat
            memberRepo
                .Setup(repo => repo.GetAantalGeboekteTijdsloten(date, member1))
                .Returns(expectedAantal); // Simuleer het aantal geboekte tijdsloten

            // Act
            int actualAantal = memberService.GetAantalGeboekteTijdsloten(member1, date);

            // Assert
            Assert.Equal(expectedAantal, actualAantal);
        }

        [Fact]
        public void GetReservationsMember_MemberIsNull_ThrowsServiceException()
        {
            // Arrange
            Member member = null;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetReservationsMember(member)
            );

            Assert.Equal(
                "MemberService - GetProgramListMember - Member is null",
                exception.Message
            );
        }

        [Fact]
        public void GetReservationsMember_MemberDoesNotExist_ThrowsServiceException()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetReservationsMember(member1)
            );

            Assert.Equal(
                "MemberService - GetProgramListMember - Er bestaat geen member met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetReservationsMember_NoReservations_ThrowsServiceException()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);

            memberRepo
                .Setup(repo => repo.GetReservationsMember(member1))
                .Returns(new List<Reservation>());

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetReservationsMember(member1)
            );

            Assert.Equal(
                "MemberService - GetReservationsMember - Deze member heeft nog geen enkele reservations!",
                exception.Message
            );
        }

        [Fact]
        public void GetReservationsMember_ValidMemberWithReservations_ReturnsReservations()
        {
            List<Reservation> reservations = new List<Reservation>
            {
                new Reservation(
                    1,
                    DateTime.Now.AddDays(1),
                    member1,
                    new Dictionary<Time_slot, Equipment>()
                ),
                new Reservation(
                    2,
                    DateTime.Now.AddDays(2),
                    member1,
                    new Dictionary<Time_slot, Equipment>()
                )
            };

            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);

            memberRepo.Setup(repo => repo.GetReservationsMember(member1)).Returns(reservations);

            // Act
            IEnumerable<Reservation> result = memberService.GetReservationsMember(member1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(reservations, result);
            memberRepo.Verify(repo => repo.IsMemberId(member1), Times.Once);
            memberRepo.Verify(repo => repo.GetReservationsMember(member1), Times.Once);
        }

        [Fact]
        public void GetMemberNaam_NoMembersFound_ThrowsServiceException()
        {
            // Arrange
            string voornaam = "Johnaton";
            string achternaam = "Doe";

            // Mocks
            memberRepo
                .Setup(repo => repo.GetMemberNaam(voornaam, achternaam))
                .Returns(new List<Member>());

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => memberService.GetMemberNaam(voornaam, achternaam)
            );

            Assert.Equal(
                "MemberService - GetMemberNaam - Er is geen member met deze naam!",
                exception.Message
            );
        }

        [Fact]
        public void GetMemberNaam_MembersFound_ReturnsMembers()
        {
            // Arrange
            string voornaam = "John";
            string achternaam = "Doe";

            List<Member> members = new List<Member>
            {
                new Member(
                    1,
                    "Johnatanial",
                    "Doe",
                    "Johnatanial.doe@example.com",
                    "Street 123",
                    DateTime.Now,
                    new List<string> { "Yoga" },
                    TypeKlant.Silver
                ),
                new Member(
                    2,
                    "John",
                    "Doe",
                    "john.doe2@example.com",
                    "Street 456",
                    DateTime.Now,
                    new List<string> { "Pilates" },
                    TypeKlant.Gold
                )
            };

            // Mocks
            memberRepo.Setup(repo => repo.GetMemberNaam(voornaam, achternaam)).Returns(members);

            // Act
            IEnumerable<Member> result = memberService.GetMemberNaam(voornaam, achternaam);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(members, result);
            memberRepo.Verify(repo => repo.GetMemberNaam(voornaam, achternaam), Times.Once);
        }
    }
}
