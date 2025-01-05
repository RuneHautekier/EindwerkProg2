using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessAPI.Controllers;
using FitnessAPI.DTO;
using FitnessBL.Enums;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using FitnessBL.Services;
using FitnessEF.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Moq;

namespace FitnessTests
{
    public class MemberControllerTest
    {
        private readonly Mock<IMemberRepo> memberRepoMock;
        private readonly Mock<IReservationRepo> reservationRepoMock;
        private readonly Mock<IEquipmentRepo> equipmentRepoMock;

        private readonly MemberService memberService;
        private readonly ReservationService reservatieService;
        private readonly EquipmentService equipmentService;

        private readonly MemberController controller;

        private readonly Member member;
        private readonly List<TrainingSession> trainingSessions;

        public MemberControllerTest()
        {
            memberRepoMock = new Mock<IMemberRepo>();
            reservationRepoMock = new Mock<IReservationRepo>();
            equipmentRepoMock = new Mock<IEquipmentRepo>();

            memberService = new MemberService(memberRepoMock.Object);
            reservatieService = new ReservationService(
                reservationRepoMock.Object,
                memberService,
                equipmentService
            );

            controller = new MemberController(memberService);
            member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Fitness", "Swimming" },
                TypeKlant.Gold
            );

            DateTime date = new DateTime(2025, 1, 1);

            Runningsession_main rs = new Runningsession_main(1, date, 30, 12, member);
            Cyclingsession cs = new Cyclingsession(
                2,
                date,
                45,
                250,
                300,
                90,
                100,
                "Endurance",
                "Good session",
                member
            );

            trainingSessions = new List<TrainingSession>();

            trainingSessions.Add(rs);
            trainingSessions.Add(cs);
        }

        [Fact]
        public void GetMembers_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Mocks
            memberRepoMock.Setup(repo => repo.GetMembers()).Returns(new List<Member>());

            // Act
            var result = controller.GetMembers();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Er zitten nog geen members in de database!", badRequestResult.Value);
        }

        [Fact]
        public void GetMembers_ReturnsOk_WhenServiceReturnsMembers()
        {
            // Arrange
            var expectedMembers = new List<Member>
            {
                new Member
                {
                    Member_id = 1,
                    FirstName = "John",
                    LastName = "Doe"
                },
                new Member
                {
                    Member_id = 2,
                    FirstName = "Jane",
                    LastName = "Smith"
                }
            };

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMembers()).Returns(expectedMembers);

            // Act
            var result = controller.GetMembers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var members = Assert.IsAssignableFrom<IEnumerable<Member>>(okResult.Value);
            Assert.Equal(2, members.Count());
            Assert.Equal("John", members.First().FirstName);
        }

        [Fact]
        public void GetMemberNaam_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            string voornaam = "John";
            string achternaam = "Doe";

            // Mocks
            memberRepoMock
                .Setup(repo => repo.GetMemberNaam(voornaam, achternaam))
                .Returns(new List<Member>());

            // Act
            var result = controller.GetMemberNaam(voornaam, achternaam);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - GetMemberNaam - Er is geen member met deze naam!",
                badRequestResult.Value
            );
        }

        [Fact]
        public void GetMemberNaam_ReturnsOk_WhenServiceReturnsMembers()
        {
            // Arrange
            string voornaam = "John";
            string achternaam = "Doe";
            var expectedMembers = new List<Member>
            {
                new Member
                {
                    Member_id = 1,
                    FirstName = "John",
                    LastName = "Doe"
                },
                new Member
                {
                    Member_id = 2,
                    FirstName = "John",
                    LastName = "Doen"
                }
            };

            //Mocks
            memberRepoMock
                .Setup(repo => repo.GetMemberNaam(voornaam, achternaam))
                .Returns(expectedMembers);

            // Act
            var result = controller.GetMemberNaam(voornaam, achternaam);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var members = Assert.IsAssignableFrom<IEnumerable<Member>>(okResult.Value);
            Assert.Equal(2, members.Count());
            Assert.All(members, member => Assert.Contains("John", member.FirstName));
            Assert.All(members, member => Assert.Contains("Doe", member.LastName));
        }

        [Fact]
        public void GetTrainingSessionsMember_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Mocks
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(false);

            // Act
            var result = controller.GetTrainingSessionsMember(member.Member_id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - GetMemberId - Er is geen member met dit id!",
                badRequestResult.Value
            );
        }

        [Fact]
        public void GetTrainingSessionsMember_ReturnsOk_WhenTrainingSessionsAreFound()
        {
            // Arrange
            int memberId = 1;

            // Mocks
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock
                .Setup(repo => repo.GetTrainingSessionsMember(member))
                .Returns(trainingSessions);

            // Act
            var result = controller.GetTrainingSessionsMember(memberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var trainingSessionsDTOs = Assert.IsAssignableFrom<List<TrainingSessionDTO>>(
                okResult.Value
            );

            Assert.Equal(2, trainingSessionsDTOs.Count);

            // Check the first session (Running)
            var runningSession = trainingSessionsDTOs[0];
            Assert.Equal("Running", runningSession.SessionType);
            Assert.Equal(1, runningSession.Id);
            Assert.Equal(12, runningSession.AvgSpeed);

            // Check the second session (Cycling)
            var cyclingSession = trainingSessionsDTOs[1];
            Assert.Equal("Cycling", cyclingSession.SessionType);
            Assert.Equal(2, cyclingSession.Id);
            Assert.Equal(250, cyclingSession.AvgWatt);
            Assert.Equal("Good session", cyclingSession.Comment);
        }

        [Fact]
        public void GetProgramListMember_ReturnsBadRequest_WhenNoProgramsAreFound()
        {
            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(repo => repo.GetProgramListMember(member))
                .Returns(new List<Program>());

            // Act
            var result = controller.GetProgramListMember(member.Member_id);

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - GetProgramListMember - Deze member is nog voor geen enkel Program ingeschreven!",
                okResult.Value
            );
        }

        [Fact]
        public void GetProgramListMember_ReturnsOk_WhenProgramsAreFound()
        {
            // Arrange


            List<Program> programs = new List<Program>
            {
                new Program
                {
                    ProgramCode = "P001",
                    Name = "Running Program",
                    Target = "Endurance",
                    Startdate = DateTime.Now,
                    Max_members = 20
                },
                new Program
                {
                    ProgramCode = "P002",
                    Name = "Cycling Program",
                    Target = "Strength",
                    Startdate = DateTime.Now.AddDays(1),
                    Max_members = 15
                }
            };

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock.Setup(repo => repo.GetProgramListMember(member)).Returns(programs);

            // Act
            var result = controller.GetProgramListMember(member.Member_id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var programDTOs = Assert.IsAssignableFrom<List<ProgramDTO>>(okResult.Value);
            Assert.Equal(2, programDTOs.Count);

            // Controleer het eerste programma
            var program1 = programDTOs[0];
            Assert.Equal("P001", program1.ProgramCode);
            Assert.Equal("Running Program", program1.Name);
            Assert.Equal("Endurance", program1.Target);
            Assert.Equal(DateTime.Now.Date, program1.StartDate); // Vergelijk alleen de datum (geen tijd)
            Assert.Equal(20, program1.MaxMembers);

            // Controleer het tweede programma
            var program2 = programDTOs[1];
            Assert.Equal("P002", program2.ProgramCode);
            Assert.Equal("Cycling Program", program2.Name);
            Assert.Equal("Strength", program2.Target);
            Assert.Equal(DateTime.Now.AddDays(1).Date, program2.StartDate); // Vergelijk alleen de datum
            Assert.Equal(15, program2.MaxMembers);
        }

        [Fact]
        public void GetReservationsMember_ReturnsBadRequest_WhenNoReservationsAreFound()
        {
            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(repo => repo.GetReservationsMember(member))
                .Returns(new List<Reservation>());

            // Act
            var result = controller.GetReservationsMember(member.Member_id);

            // Assert
            var expectedResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - GetReservationsMember - Deze member heeft nog geen enkele reservations!",
                expectedResult.Value
            );
        }

        [Fact]
        public void TrainingSessionsMemberPerMaandInJaar_ReturnsBadRequest_WhenNoSessionsFound()
        {
            // Arrange
            int maand = 5; // Mei
            int jaar = 2025;

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(service =>
                    service.GetTrainingSessionsMemberInMaandInJaar(
                        member,
                        new DateTime(jaar, maand, 1)
                    )
                )
                .Returns(new List<TrainingSession>());

            // Act
            var result = controller.TrainingSessionsMemberPerMaandInJaar(
                member.Member_id,
                maand,
                jaar
            );

            // Assert
            var expectedResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - TrainingSessionsMemberPerMaandInJaar - Deze member heeft geen TrainingSessions in maand 5 in jaar 2025!",
                expectedResult.Value
            );
        }

        [Fact]
        public void TrainingSessionsMemberPerMaandInJaar_ReturnsSessions_WhenSessionsAreFound()
        {
            // Arrange
            int maand = 1; // Mei
            int jaar = 2025;
            DateTime date = new DateTime(2025, 1, 1);

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(service =>
                    service.GetTrainingSessionsMemberInMaandInJaar(
                        member,
                        new DateTime(jaar, maand, 1)
                    )
                )
                .Returns(trainingSessions);

            // Act
            var result = controller.TrainingSessionsMemberPerMaandInJaar(
                member.Member_id,
                maand,
                jaar
            );

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var trainingSessionDTOs = Assert.IsAssignableFrom<List<TrainingSessionDTO>>(
                okResult.Value
            );
            Assert.Equal(2, trainingSessionDTOs.Count); // Verifieer dat er twee sessies zijn

            // Verifieer de eerste sessie (Running)
            TrainingSessionDTO runningSessionDTO = trainingSessionDTOs[0];
            Assert.Equal("Running", runningSessionDTO.SessionType);
            Assert.Equal(1, runningSessionDTO.Id);
            Assert.Equal(30, runningSessionDTO.Duration);
            Assert.Equal(12, runningSessionDTO.AvgSpeed);

            // Verifieer de tweede sessie (Cycling)
            TrainingSessionDTO cyclingSessionDTO = trainingSessionDTOs[1];
            Assert.Equal("Cycling", cyclingSessionDTO.SessionType);
            Assert.Equal(2, cyclingSessionDTO.Id);
            Assert.Equal(45, cyclingSessionDTO.Duration);
            Assert.Equal(250, cyclingSessionDTO.AvgWatt);
            Assert.Equal(300, cyclingSessionDTO.MaxWatt);
            Assert.Equal(90, cyclingSessionDTO.AvgCadence);
            Assert.Equal(100, cyclingSessionDTO.MaxCadence);
            Assert.Equal("Endurance", cyclingSessionDTO.TrainingsType);
            Assert.Equal("Good session", cyclingSessionDTO.Comment);
        }

        [Fact]
        public void GetTrainingSessionsMemberGegevensTijd_ReturnsBadRequest_WhenNoSessionsFound()
        {
            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(repo => repo.GetTrainingSessionsMember(member))
                .Returns(new List<TrainingSession>());

            // Act
            var result = controller.GetTrainingSessionsMemberGegevensTijd(member.Member_id);

            // Assert
            var expectedResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - GetTrainingSessionsMember - Deze member heeft nog geen TrainingSessions!",
                expectedResult.Value
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberGegevensTijd_ReturnsCorrectData_WhenSessionsFound()
        {
            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(repo => repo.GetTrainingSessionsMember(member))
                .Returns(trainingSessions);

            // Act
            var result = controller.GetTrainingSessionsMemberGegevensTijd(member.Member_id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var tsDTO = Assert.IsAssignableFrom<TrainingSessionMetLangsteEnKortsteDTO>(
                okResult.Value
            );

            Assert.Equal(2, tsDTO.AantalTrainingSessions); // Verifieer dat er twee sessies zijn
            Assert.Equal(1.25m, tsDTO.AantalUren); // Verifieer het aantal uren (75 minuten / 60 = 1.25 uur)
            Assert.NotNull(tsDTO.LangsteTrainingSession); // Verifieer dat de langste sessie niet null is
            Assert.NotNull(tsDTO.KortsteTrainingSession); // Verifieer dat de kortste sessie niet null is
            Assert.Equal(38, tsDTO.GemiddeldeDuur); // Verifieer de gemiddelde duur (75 minuten / 2 sessies)

            // Verifieer de kortste sessie (Cycling)
            var kortsteTsDTO = tsDTO.KortsteTrainingSession;
            Assert.Equal("Cycling", kortsteTsDTO.SessionType);
            Assert.Equal(2, kortsteTsDTO.Id);
            Assert.Equal(new DateTime(2025, 1, 1), kortsteTsDTO.Date);
            Assert.Equal(45, kortsteTsDTO.Duration);
            Assert.Equal(250, kortsteTsDTO.AvgWatt);
            Assert.Equal(300, kortsteTsDTO.MaxWatt);
            Assert.Equal(90, kortsteTsDTO.AvgCadence);
            Assert.Equal(100, kortsteTsDTO.MaxCadence);
            Assert.Equal("Endurance", kortsteTsDTO.TrainingsType);
            Assert.Equal("Good session", kortsteTsDTO.Comment);

            // Verifieer de langste sessie (Running)
            var langsteTsDTO = tsDTO.LangsteTrainingSession;
            Assert.Equal("Running", langsteTsDTO.SessionType);
            Assert.Equal(1, langsteTsDTO.Id);
            Assert.Equal(new DateTime(2025, 1, 1), langsteTsDTO.Date);
            Assert.Equal(30, langsteTsDTO.Duration);
            Assert.Equal(12, langsteTsDTO.AvgSpeed);
        }

        [Fact]
        public void GetTrainingSessionsMemberInMaandInJaar_ReturnsBadRequest_WhenNoSessionsFound()
        {
            // Arrange
            int maand = 1;
            int jaar = 2025; // Jaar 2025

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(repo =>
                    repo.GetTrainingSessionsMemberInMaandInJaar(
                        member,
                        new DateTime(jaar, maand, 1)
                    )
                )
                .Returns(new List<TrainingSession>());

            // Act
            var result = controller.GetTrainingSessionsMemberInMaandInJaar(
                member.Member_id,
                maand,
                jaar
            );

            // Assert
            var expectedResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - TrainingSessionsMemberPerMaandInJaar - Deze member heeft geen TrainingSessions in maand 1 in jaar 2025!",
                expectedResult.Value
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberInMaandInJaar_ReturnsCorrectData_WhenSessionsFound()
        {
            // Arrange
            int maand = 1; // Mei
            int jaar = 2025; // Jaar 2025

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(repo =>
                    repo.GetTrainingSessionsMemberInMaandInJaar(
                        member,
                        new DateTime(jaar, maand, 1)
                    )
                )
                .Returns(trainingSessions);

            // Act
            var result = controller.GetTrainingSessionsMemberInMaandInJaar(
                member.Member_id,
                maand,
                jaar
            );

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tsDTOs = Assert.IsAssignableFrom<List<TrainingSessionDTO>>(okResult.Value);
            Assert.Equal(2, tsDTOs.Count); // Verifieer dat er twee sessies zijn

            // Verifieer de RunningSession DTO
            TrainingSessionDTO runningSessionDTO = tsDTOs[0];
            Assert.Equal("Running", runningSessionDTO.SessionType);
            Assert.Equal(1, runningSessionDTO.Id);
            Assert.Equal(30, runningSessionDTO.Duration);
            Assert.Equal(12, runningSessionDTO.AvgSpeed);

            // Verifieer de CyclingSession DTO
            TrainingSessionDTO cyclingSessionDTO = tsDTOs[1];
            Assert.Equal("Cycling", cyclingSessionDTO.SessionType);
            Assert.Equal(2, cyclingSessionDTO.Id);
            Assert.Equal(45, cyclingSessionDTO.Duration);
            Assert.Equal(250, cyclingSessionDTO.AvgWatt);
            Assert.Equal(300, cyclingSessionDTO.MaxWatt);
            Assert.Equal(90, cyclingSessionDTO.AvgCadence);
            Assert.Equal(100, cyclingSessionDTO.MaxCadence);
            Assert.Equal("Endurance", cyclingSessionDTO.TrainingsType);
            Assert.Equal("Good session", cyclingSessionDTO.Comment);
            Assert.Equal("High", cyclingSessionDTO.TrainingsImpact); // Verifieer de berekende trainingsimpact
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaar_ReturnsBadResult_WhenNoSessionsFound()
        {
            // Arrange
            int jaar = 2025;

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(repo =>
                    repo.GetTrainingSessionsMemberAantalPerMaandInJaar(
                        member,
                        new DateTime(jaar, 1, 1)
                    )
                )
                .Returns(new Dictionary<int, int>());

            // Act
            var result = controller.GetTrainingSessionsMemberAantalPerMaandInJaar(
                member.Member_id,
                jaar
            );

            // Assert
            var expectedResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - Deze member heeft geen TrainingSessions in maand 1 jaar 2025!",
                expectedResult.Value
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaar_ReturnsCorrectData_WhenSessionsFound()
        {
            // Arrange
            int jaar = 2025;

            // Stel een dictionary van sessies per maand in
            var sessiesPerMaand = new Dictionary<int, int>
            {
                { 1, 5 }, // 5 sessies in januari
                { 3, 2 }, // 2 sessies in maart
                { 5, 8 }, // 8 sessies in mei
            };

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(repo =>
                    repo.GetTrainingSessionsMemberAantalPerMaandInJaar(
                        member,
                        new DateTime(jaar, 1, 1)
                    )
                )
                .Returns(sessiesPerMaand);

            // Act
            var result = controller.GetTrainingSessionsMemberAantalPerMaandInJaar(
                member.Member_id,
                jaar
            );

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            TrainingSessionMemberSessiesAantalPerMaandDTO aantalPerMaandDTO =
                Assert.IsType<TrainingSessionMemberSessiesAantalPerMaandDTO>(okResult.Value);
            Assert.Equal(3, aantalPerMaandDTO.SessiesPerMaand.Count); // Er zouden 3 maanden met sessies moeten zijn

            // Verifieer de maand en het aantal sessies
            Assert.Contains(
                aantalPerMaandDTO.SessiesPerMaand,
                kvp => kvp.Key == "Maand 1" && kvp.Value == "Aantal Sessies: 5"
            );
            Assert.Contains(
                aantalPerMaandDTO.SessiesPerMaand,
                kvp => kvp.Key == "Maand 3" && kvp.Value == "Aantal Sessies: 2"
            );
            Assert.Contains(
                aantalPerMaandDTO.SessiesPerMaand,
                kvp => kvp.Key == "Maand 5" && kvp.Value == "Aantal Sessies: 8"
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaarMetType_ReturnsBadRequest_WhenNoSessionsFound()
        {
            // Arrange
            int jaar = 2025;

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(repo =>
                    repo.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(
                        member,
                        new DateTime(jaar, 1, 1)
                    )
                )
                .Returns(new Dictionary<string, Dictionary<int, int>>());

            // Act
            var result = controller.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(
                member.Member_id,
                jaar
            );

            // Assert
            var expectedResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - GetTrainingSessionsMemberAantalPerMaandInJaar - Deze member heeft geen TrainingSessions in maand 1 jaar 2025!",
                expectedResult.Value
            );
        }

        [Fact]
        public void GetTrainingSessionsMemberAantalPerMaandInJaarMetType_ReturnsCorrectData_WhenSessionsFound()
        {
            // Arrange
            int jaar = 2025;

            // Stel een dictionary van sessies per maand per type in
            var sessiesPerMaand = new Dictionary<string, Dictionary<int, int>>
            {
                {
                    "Running",
                    new Dictionary<int, int>
                    {
                        { 1, 5 }, // 5 sessies in januari voor Running
                        { 3, 2 } // 2 sessies in maart voor Running
                    }
                },
                {
                    "Cycling",
                    new Dictionary<int, int>
                    {
                        { 2, 3 }, // 3 sessies in februari voor Cycling
                        { 4, 6 } // 6 sessies in april voor Cycling
                    }
                }
            };

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock
                .Setup(repo =>
                    repo.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(
                        member,
                        new DateTime(jaar, 1, 1)
                    )
                )
                .Returns(sessiesPerMaand);

            // Act
            var result = controller.GetTrainingSessionsMemberAantalPerMaandInJaarMetType(
                member.Member_id,
                jaar
            );

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            TrainingSessionMemberAantalPerMaandInJaarMetTypeDTO aantalPerMaandDTO =
                Assert.IsType<TrainingSessionMemberAantalPerMaandInJaarMetTypeDTO>(okResult.Value);
            Assert.Equal(2, aantalPerMaandDTO.SessiesPerMaand.Count); // Er zouden 2 types van sessies moeten zijn (Running, Cycling)

            // Verifieer de sessies per maand voor "Running"
            var runningSessions = aantalPerMaandDTO.SessiesPerMaand["Running"];
            Assert.Contains(
                runningSessions,
                kvp => kvp.Key == "Maand 1" && kvp.Value == "Aantal Sessies: 5"
            );
            Assert.Contains(
                runningSessions,
                kvp => kvp.Key == "Maand 3" && kvp.Value == "Aantal Sessies: 2"
            );

            // Verifieer de sessies per maand voor "Cycling"
            var cyclingSessions = aantalPerMaandDTO.SessiesPerMaand["Cycling"];
            Assert.Contains(
                cyclingSessions,
                kvp => kvp.Key == "Maand 2" && kvp.Value == "Aantal Sessies: 3"
            );
            Assert.Contains(
                cyclingSessions,
                kvp => kvp.Key == "Maand 4" && kvp.Value == "Aantal Sessies: 6"
            );
        }

        [Fact]
        public void GetMemberId_ReturnsBadRequest_WhenMemberDoesNotExist()
        {
            // Arrange
            int memberId = 1;

            // Mocks
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(false);

            // Act
            var result = controller.GetMemberId(memberId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); // Controleer of de response BadRequest is
            Assert.Equal(
                "MemberService - GetMemberId - Er is geen member met dit id!",
                badRequestResult.Value
            );
        }

        [Fact]
        public void GetMemberId_ReturnsOk_WhenMemberExists()
        {
            // Mocks
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);

            // Act
            var result = controller.GetMemberId(member.Member_id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedMember = Assert.IsType<Member>(okResult.Value);
            Assert.Equal(member.Member_id, returnedMember.Member_id);
            Assert.Equal("John", returnedMember.FirstName);
            Assert.Equal("Doe", returnedMember.LastName);
        }

        [Fact]
        public void AddMember_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            MemberAanmakenDTO memberDTO = new MemberAanmakenDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@gmail.com",
                Address = "123 Main St",
                Birthday = new DateTime(1990, 1, 1),
                Intrests = new List<string> { "Cycling", "Running" },
                TypeMember = TypeKlant.Gold
            };

            // Mocks

            memberRepoMock
                .Setup(repo =>
                    repo.IsMemberName(
                        It.Is<Member>(m => m.FirstName == "John" && m.LastName == "Doe")
                    )
                )
                .Returns(true);

            // Act
            var result = controller.AddMember(memberDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - AddMember - Member bestaat al (zelfde naam)!",
                badRequestResult.Value
            );
        }

        [Fact]
        public void AddMember_ReturnsCreatedAtAction_WhenMemberIsSuccessfullyAdded()
        {
            // Arrange
            MemberAanmakenDTO memberDTO = new MemberAanmakenDTO
            {
                FirstName = "John",
                LastName = "Banger",
                Email = "john.doe@example.com",
                Address = "123 Main St",
                Birthday = new DateTime(1990, 1, 1),
                Intrests = new List<string> { "Cycling", "Running" },
                TypeMember = TypeKlant.Bronze
            };

            // Mocks
            memberRepoMock.Setup(repo => repo.AddMember(It.IsAny<Member>()));

            // Act
            var result = controller.AddMember(memberDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Member returnedMember = Assert.IsType<Member>(createdResult.Value);
            Assert.Equal("John", returnedMember.FirstName);
            Assert.Equal("Banger", returnedMember.LastName);
        }

        [Fact]
        public void UpdateMember_NoMember_ReturnsBadRequest()
        {
            // Arrange

            // Mocks
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(false);

            // Act
            var result = controller.UpdateMember(
                member.Member_id,
                new DateTime(1990, 01, 01),
                TypeKlant.Gold,
                "John",
                "Doe",
                "john.doe@example.com",
                "123 Main St",
                new List<string> { "Cycling", "Running" }
            );

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - GetMemberId - Er is geen member met dit id!",
                badRequestResult.Value
            );
        }

        [Fact]
        public void UpdateMember_ValidMember_ReturnsNoContent()
        {
            // Arrange
            string voornaam = "Jing";

            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);

            // Act
            var result = controller.UpdateMember(
                member.Member_id,
                member.Birthday,
                member.MemberType,
                voornaam,
                member.LastName,
                member.Email,
                member.Address,
                member.Interests
            );

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Member returnedMember = Assert.IsType<Member>(createdResult.Value);
            Assert.Equal("Jing", returnedMember.FirstName);
            Assert.Equal(member.LastName, returnedMember.LastName);
        }

        [Fact]
        public void DeleteMember_MemberNotFound_ReturnsBadRequest()
        {
            // Arrange
            int memberId = 1;

            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(memberId)).Returns((Member)null);

            // Act
            var result = controller.DeleteMember(memberId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "MemberService - GetMemberId - Er is geen member met dit id!",
                badRequestResult.Value
            );
        }

        [Fact]
        public void DeleteMember_ValidMember_ReturnsOk()
        {
            // Mocks
            memberRepoMock.Setup(repo => repo.GetMemberId(member.Member_id)).Returns(member);
            memberRepoMock.Setup(repo => repo.IsMemberId(member)).Returns(true);

            // Act
            var result = controller.DeleteMember(member.Member_id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("De member is succesvol verwijderd!", okResult.Value);
        }
    }
}
