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
    public class ReservationServiceTest
    {
        private readonly Mock<IReservationRepo> reservationRepo;
        private readonly Mock<IMemberRepo> memberRepo;
        private readonly Mock<IEquipmentRepo> equipmentRepo;

        private readonly EquipmentService equipmentService;
        private readonly ReservationService reservationService;
        private readonly MemberService memberService;

        public ReservationServiceTest()
        {
            equipmentRepo = new Mock<IEquipmentRepo>();
            equipmentService = new EquipmentService(equipmentRepo.Object);

            memberRepo = new Mock<IMemberRepo>();
            memberService = new MemberService(memberRepo.Object);

            reservationRepo = new Mock<IReservationRepo>();
            reservationService = new ReservationService(
                reservationRepo.Object,
                memberService,
                equipmentService
            );
        }

        [Fact]
        public void GetReservationId_InvalidId_ThrowsServiceException()
        {
            // Arrange
            int invalidId = -1; // Ongeldig id, kleiner dan 1

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => reservationService.GetReservationId(invalidId)
            );
            Assert.Equal(
                "ReservationService - GetReservationId - Voer een geldig id in >0!",
                exception.Message
            );
        }

        [Fact]
        public void GetReservationId_ReservationNotFound_ThrowsServiceException()
        {
            // Arrange
            int validId = 1; // Geldig id
            reservationRepo
                .Setup(repo => repo.GetReservationId(validId))
                .Returns((Reservation)null); // Geen reservation gevonden

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => reservationService.GetReservationId(validId)
            );
            Assert.Equal(
                "ReservationService - GetReservationId - Er is geen Reservation met dit Id ",
                exception.Message
            );
        }

        [Fact]
        public void GetReservationId_ValidId_ReturnsReservation()
        {
            // Arrange
            int validId = 1; // Geldig id
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

            Reservation reservation = new Reservation(
                1,
                DateTime.Now,
                member,
                new Dictionary<Time_slot, Equipment>()
            );
            reservationRepo.Setup(repo => repo.GetReservationId(validId)).Returns(reservation); // Reservation gevonden

            // Act
            Reservation result = reservationService.GetReservationId(validId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservation, result);
        }

        [Fact]
        public void GetNieuwReservationId_ReturnsNewId()
        {
            // Arrange
            int expectedId = 1; // Het verwachte nieuwe ID
            reservationRepo.Setup(repo => repo.GetNieuwReservationId()).Returns(expectedId); // De mock returnt het verwachte ID

            // Act
            int result = reservationService.GetNieuwReservationId();

            // Assert
            Assert.Equal(expectedId, result); // Controleer of het resultaat gelijk is aan het verwachte ID
            reservationRepo.Verify(repo => repo.GetNieuwReservationId(), Times.Once); // Verifieer dat de repository-methode werd aangeroepen
        }
    }
}
