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
    public class EquipmentServiceTest
    {
        private readonly Mock<IEquipmentRepo> equipmentRepo;
        private readonly EquipmentService equipmentService;

        private Equipment equipment1;
        private Equipment equipment2;
        private Member member;
        private Reservation reservation1;
        private Reservation reservation2;
        private Dictionary<Time_slot, Equipment> dic;

        public EquipmentServiceTest()
        {
            equipmentRepo = new Mock<IEquipmentRepo>();
            equipmentService = new EquipmentService(equipmentRepo.Object);

            equipment1 = new Equipment(1, "Treadmill");
            equipment2 = new Equipment(2, "Dumbbell");
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
            dic = new Dictionary<Time_slot, Equipment>();
            reservation1 = new Reservation(1, DateTime.Now, member, dic);
            reservation2 = new Reservation(2, DateTime.Now, member, dic);
        }

        [Fact]
        public void GetEquipment_NoEquipment_ThrowsServiceException()
        {
            // Arrange
            List<Equipment> emptyList = new List<Equipment>();

            // Mocks
            equipmentRepo.Setup(repo => repo.GetEquipment()).Returns(emptyList);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.GetEquipment()
            );
            Assert.Equal("Er zit nog geen equipment in de database!", exception.Message);
        }

        [Fact]
        public void GetEquipment_EquipmentExists_ReturnsEquipmentList()
        {
            // Arrange
            List<Equipment> equipmentList = new List<Equipment>();

            equipmentList.Add(equipment1);
            equipmentList.Add(equipment2);

            // Mocks
            equipmentRepo.Setup(repo => repo.GetEquipment()).Returns(equipmentList);

            // Act
            IEnumerable<Equipment> result = equipmentService.GetEquipment();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(equipment1, result.ElementAt(0));
            Assert.Equal(equipment2, result.ElementAt(1));
            Assert.Contains(result, e => e.Device_type == "Treadmill");
            Assert.Contains(result, e => e.Device_type == "Dumbbell");
        }

        [Fact]
        public void GetEquipmentId_InvalidId_ThrowsServiceException()
        {
            // Mocks
            equipmentRepo
                .Setup(repo => repo.GetEquipmentId(It.IsAny<int>()))
                .Returns((Equipment)null);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.GetEquipmentId(999)
            );
            Assert.Equal(
                "EquipmentService - GetEquipmentId - Er is geen equipment met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void GetEquipmentId_ValidId_ReturnsEquipment()
        {
            // Mocks
            equipmentRepo.Setup(repo => repo.GetEquipmentId(1)).Returns(equipment1);

            // Act
            Equipment result = equipmentService.GetEquipmentId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(equipment1, result);
        }

        [Fact]
        public void AddEquipment_NullEquipment_ThrowsServiceException()
        {
            // Arrange
            Equipment nullEquipment = null;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.AddEquipment(nullEquipment)
            );
            Assert.Equal("EquipmentService - AddEquipment - Equipment is null", exception.Message);
        }

        [Fact]
        public void AddEquipment_DeviceTypeAsString_ThrowsServiceException()
        {
            // Arrange
            Equipment equipmentWithInvalidType = new Equipment(1, "string");

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.AddEquipment(equipmentWithInvalidType)
            );
            Assert.Equal(
                "EquipmentService - AddEquipment - Gelieve het type van het equipment in te vullen!",
                exception.Message
            );
        }

        [Fact]
        public void AddEquipment_ValidEquipment_ReturnsEquipment()
        {
            // Mocks
            equipmentRepo.Setup(repo => repo.AddEquipment(equipment1));

            // Act
            Equipment result = equipmentService.AddEquipment(equipment1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(equipment1, result);
        }

        [Fact]
        public void DeleteEquipment_EquipmentIsNull_ThrowsServiceException()
        {
            // Arrange
            Equipment equipment = null;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.DeleteEquipment(equipment)
            );
            Assert.Equal(
                "EquipmentService - DeleteEquipment - equipment is null",
                exception.Message
            );
        }

        [Fact]
        public void DeleteEquipment_EquipmentDoesNotExist_ThrowsServiceException()
        {
            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment1)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.DeleteEquipment(equipment1)
            );
            Assert.Equal(
                "EquipmentService - DeleteEquipment - equipment bestaat niet op id",
                exception.Message
            );
        }

        [Fact]
        public void DeleteEquipment_HasFutureReservations_ThrowsServiceException()
        {
            // Mocks
            List<Reservation> futureReservations = new List<Reservation> { reservation1 };

            equipmentRepo
                .Setup(repo => repo.GetFutureReservationsForEquipment(equipment1))
                .Returns(futureReservations);

            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment1)).Returns(true);
            equipmentRepo.Setup(repo => repo.DeleteEquipment(equipment1));

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.DeleteEquipment(equipment1)
            );
            Assert.Equal(
                "EquipmentService - DeleteEquipment - equipment kan niet verwijderd worden want heeft nog reservations!",
                exception.Message
            );
        }

        [Fact]
        public void DeleteEquipment_ValidEquipment_DeletesEquipment()
        {
            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment1)).Returns(true);
            equipmentRepo.Setup(repo => repo.DeleteEquipment(equipment1));

            equipmentRepo
                .Setup(repo => repo.GetFutureReservationsForEquipment(equipment1))
                .Returns(new List<Reservation>());

            // Act
            equipmentService.DeleteEquipment(equipment1);

            // Assert
            equipmentRepo.Verify(repo => repo.DeleteEquipment(equipment1), Times.Once);
        }

        [Fact]
        public void GetFutureReservationsForEquipment_EquipmentIsNull_ThrowsServiceException()
        {
            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.GetFutureReservationsForEquipment(null)
            );
            Assert.Equal(
                "EquipmentService - GetFutureReservationsForEquipment - equipment is null",
                exception.Message
            );
        }

        [Fact]
        public void GetFutureReservationsForEquipment_EquipmentDoesNotExist_ThrowsServiceException()
        {
            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment1)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.GetFutureReservationsForEquipment(equipment1)
            );
            Assert.Equal(
                "EquipmentService - GetFutureReservationsForEquipment - equipment bestaat niet op id",
                exception.Message
            );
        }

        [Fact]
        public void GetFutureReservationsForEquipment_NoFutureReservations_ReturnsEmptyList()
        {
            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment1)).Returns(true);
            equipmentRepo
                .Setup(repo => repo.GetFutureReservationsForEquipment(equipment1))
                .Returns(Enumerable.Empty<Reservation>());

            // Act
            IEnumerable<Reservation> result = equipmentService.GetFutureReservationsForEquipment(
                equipment1
            );

            // Assert
            Assert.NotNull(result); // Zorg ervoor dat het resultaat niet null is
            Assert.Empty(result); // Verifieer dat de lijst leeg is
        }

        [Fact]
        public void GetFutureReservationsForEquipment_WithFutureReservations_ReturnsReservationList()
        {
            // Arrange
            List<Reservation> reservations = new List<Reservation> { reservation1, reservation2 };

            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment1)).Returns(true);
            equipmentRepo
                .Setup(repo => repo.GetFutureReservationsForEquipment(equipment1))
                .Returns(reservations);

            // Act
            IEnumerable<Reservation> result = equipmentService.GetFutureReservationsForEquipment(
                equipment1
            );

            // Assert
            Assert.NotNull(result); // Zorg ervoor dat het resultaat niet null is
            Assert.Equal(2, result.Count()); // Verifieer dat er twee reserveringen zijn
            Assert.Contains(result, r => r.Reservation_id == 1); // Verifieer dat de eerste reservering in de lijst zit
            Assert.Contains(result, r => r.Reservation_id == 2); // Verifieer dat de tweede reservering in de lijst zit
        }

        [Fact]
        public void GetAvailableEquipment_DateInThePast_ThrowsServiceException()
        {
            // Arrange
            DateTime pastDate = DateTime.Now.AddDays(-1); // Een datum in het verleden
            Time_slot timeSlot = new Time_slot(1, 8, 9, "Morning"); // Stel een geldige Time_slot in
            string deviceType = "Cardio"; // Stel een geldig apparaat type in

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.GetAvailableEquipment(pastDate, timeSlot, deviceType)
            );

            // Assert
            Assert.Equal(
                "EquipmentServie - GetAvailableEquipment - Date moet in de toekomst liggen om te zien of dit equipment in de toekomst al gebruikt wordt!",
                exception.Message
            );
        }

        [Fact]
        public void GetAvailableEquipment_TimeSlotIsNull_ThrowsServiceException()
        {
            // Arrange
            DateTime futureDate = DateTime.Now.AddDays(1); // Een datum in de toekomst
            Time_slot timeSlot = null; // Stel Time_slot in als null
            string deviceType = "Cardio"; // Stel een geldig apparaat type in

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.GetAvailableEquipment(futureDate, timeSlot, deviceType)
            );

            // Assert
            Assert.Equal(
                "EquipmentServie - GetAvailableEquipment - TimeSlot is null!",
                exception.Message
            );
        }

        [Fact]
        public void GetAvailableEquipment_DeviceTypeIsNull_ThrowsServiceException()
        {
            // Arrange
            DateTime futureDate = DateTime.Now.AddDays(1); // Een datum in de toekomst
            Time_slot timeSlot = new Time_slot(1, 8, 9, "Morning"); // Stel een geldige Time_slot in
            string deviceType = null; // Stel DeviceType in als null

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.GetAvailableEquipment(futureDate, timeSlot, deviceType)
            );

            // Assert
            Assert.Equal(
                "EquipmentServie - GetAvailableEquipment - DeviceType is null!",
                exception.Message
            );
        }

        [Fact]
        public void GetAvailableEquipment_ValidParameters_ReturnsAvailableEquipment()
        {
            // Arrange
            DateTime futureDate = DateTime.Now.AddDays(1); // Een datum in de toekomst
            Time_slot timeSlot = new Time_slot(1, 8, 9, "Morning"); // Stel een geldige Time_slot in
            string deviceType = "Cardio"; // Stel een geldig apparaat type in
            Equipment expectedEquipment = new Equipment(1, "Cardio"); // Stel het verwachte equipment in

            // Mocks
            equipmentRepo
                .Setup(repo => repo.GetAvailableEquipment(futureDate, timeSlot, deviceType))
                .Returns(expectedEquipment);

            // Act
            Equipment result = equipmentService.GetAvailableEquipment(
                futureDate,
                timeSlot,
                deviceType
            );

            // Assert
            Assert.NotNull(result); // Controleer of het resultaat niet null is
            Assert.Equal(expectedEquipment, result); // Controleer of het resultaat gelijk is aan het verwachte equipment
        }

        [Fact]
        public void GetAllAvailableEquipment_DateInPast_ThrowsServiceException()
        {
            // Arrange
            DateTime pastDate = DateTime.Now.AddDays(-1);
            int validTimeSlotId = 5;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.GetAllAvailableEquipment(pastDate, validTimeSlotId)
            );

            Assert.Equal(
                "EquipmentService - GetAvailableEquipment - Date moet in de toekomst liggen om te zien of dit equipment in de toekomst al gebruikt wordt!",
                exception.Message
            );
        }

        [Theory]
        [InlineData(0)] // TimeSlot ID te laag
        [InlineData(15)] // TimeSlot ID te hoog
        public void GetAllAvailableEquipment_InvalidTimeSlotId_ThrowsServiceException(
            int invalidTimeSlotId
        )
        {
            // Arrange
            DateTime futureDate = DateTime.Now.AddDays(1);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.GetAllAvailableEquipment(futureDate, invalidTimeSlotId)
            );

            Assert.Equal(
                "EquipmentService - GetAvailableEquipment - TimeSlot id ligt niet in de range!",
                exception.Message
            );
        }

        [Fact]
        public void GetAllAvailableEquipment_ValidInputs_ReturnsAvailableEquipment()
        {
            // Arrange
            DateTime futureDate = DateTime.Now.AddDays(1);
            int validTimeSlotId = 5;

            List<Equipment> expectedEquipment = new List<Equipment>
            {
                new Equipment(1, "Treadmill"),
                new Equipment(2, "Dumbbell")
            };

            // Mocks
            equipmentRepo
                .Setup(repo => repo.GetAllAvailableEquipment(futureDate, validTimeSlotId))
                .Returns(expectedEquipment);

            // Act
            var result = equipmentService.GetAllAvailableEquipment(futureDate, validTimeSlotId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEquipment, result);
        }

        [Fact]
        public void EquipmentPlaatsOnderhoud_EquipmentIsNull_ThrowsServiceException()
        {
            // Arrange
            Equipment equipment = null;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.EquipmentPlaatsOnderhoud(equipment)
            );

            Assert.Equal(
                "EquipmentService - EquipmentPlaatsOnderhoud - equipment is null",
                exception.Message
            );
        }

        [Fact]
        public void EquipmentPlaatsOnderhoud_EquipmentDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");

            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.EquipmentPlaatsOnderhoud(equipment)
            );

            Assert.Equal(
                "EquipmentService - EquipmentPlaatsOnderhoud - equipment bestaat niet op id",
                exception.Message
            );
        }

        [Fact]
        public void EquipmentPlaatsOnderhoud_EquipmentAlreadyInOnderhoud_ThrowsServiceException()
        {
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");

            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);

            equipmentRepo.Setup(repo => repo.EquipmentInOnderhoud(equipment)).Returns(true);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.EquipmentPlaatsOnderhoud(equipment)
            );

            Assert.Equal(
                "EquipmentService - EquipmentPlaatsOnderhoud - Equipment zit al in onderhoud!",
                exception.Message
            );
        }

        [Fact]
        public void EquipmentPlaatsOnderhoud_ValidEquipment_PlacesInOnderhoud()
        {
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");

            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);

            equipmentRepo.Setup(repo => repo.EquipmentInOnderhoud(equipment)).Returns(false);

            // Act
            equipmentService.EquipmentPlaatsOnderhoud(equipment);

            // Assert
            equipmentRepo.Verify(repo => repo.EquipmentPlaatsOnderhoud(equipment), Times.Once);
        }

        [Fact]
        public void EquipmentVerwijderOnderhoud_EquipmentIsNull_ThrowsServiceException()
        {
            // Arrange
            Equipment equipment = null;
            DateTime startDate = DateTime.Now;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.EquipmentVerwijderOnderhoud(equipment, startDate)
            );

            Assert.Equal(
                "EquipmentService - EquipmentVerwijderOnderhoud - equipment is null",
                exception.Message
            );
        }

        [Fact]
        public void EquipmentVerwijderOnderhoud_EquipmentDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");
            DateTime startDate = DateTime.Now;

            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.EquipmentVerwijderOnderhoud(equipment, startDate)
            );

            Assert.Equal(
                "EquipmentService - EquipmentVerwijderOnderhoud - equipment bestaat niet op id",
                exception.Message
            );
        }

        [Fact]
        public void EquipmentVerwijderOnderhoud_EquipmentNotInOnderhoud_ThrowsServiceException()
        {
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");
            DateTime startDate = DateTime.Now;

            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);

            equipmentRepo.Setup(repo => repo.EquipmentInOnderhoud(equipment)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.EquipmentVerwijderOnderhoud(equipment, startDate)
            );

            Assert.Equal(
                "EquipmentService - EquipmentVerwijderOnderhoud - Equipment zit niet in onderhoud!",
                exception.Message
            );
        }

        [Fact]
        public void EquipmentVerwijderOnderhoud_ValidEquipment_RemovesOnderhoud()
        {
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");
            DateTime startDate = DateTime.Now;

            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);
            equipmentRepo.Setup(repo => repo.EquipmentInOnderhoud(equipment)).Returns(true);

            // Act
            equipmentService.EquipmentVerwijderOnderhoud(equipment, startDate);

            // Assert
            equipmentRepo.Verify(repo => repo.EquipmentVerwijderOnderhoud(equipment), Times.Once);
        }

        [Fact]
        public void EquipmentInOnderhoud_EquipmentIsNull_ThrowsServiceException()
        {
            // Arrange
            Equipment equipment = null;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.EquipmentInOnderhoud(equipment)
            );

            Assert.Equal(
                "EquipmentService - EquipmentInOnderhoud - equipment is null",
                exception.Message
            );
        }

        [Fact]
        public void EquipmentInOnderhoud_EquipmentDoesNotExist_ThrowsServiceException()
        {
            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment1)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.EquipmentInOnderhoud(equipment1)
            );

            Assert.Equal(
                "EquipmentService - EquipmentInOnderhoud - equipment bestaat niet op id",
                exception.Message
            );
        }

        [Fact]
        public void EquipmentInOnderhoud_EquipmentIsInOnderhoud_ReturnsTrue()
        {
            // Arrange

            // Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment1)).Returns(true);

            equipmentRepo.Setup(repo => repo.EquipmentInOnderhoud(equipment1)).Returns(true);

            // Act
            bool result = equipmentService.EquipmentInOnderhoud(equipment1);

            // Assert
            Assert.True(result);
            equipmentRepo.Verify(repo => repo.IsEquipmentId(equipment1), Times.Once);
            equipmentRepo.Verify(repo => repo.EquipmentInOnderhoud(equipment1), Times.Once);
        }

        [Fact]
        public void EquipmentInOnderhoud_EquipmentIsNotInOnderhoud_ReturnsFalse()
        {
            // Arrange

            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment1)).Returns(true);

            equipmentRepo.Setup(repo => repo.EquipmentInOnderhoud(equipment1)).Returns(false);

            // Act
            bool result = equipmentService.EquipmentInOnderhoud(equipment1);

            // Assert
            Assert.False(result);
            equipmentRepo.Verify(repo => repo.IsEquipmentId(equipment1), Times.Once);
            equipmentRepo.Verify(repo => repo.EquipmentInOnderhoud(equipment1), Times.Once);
        }
    }
}
