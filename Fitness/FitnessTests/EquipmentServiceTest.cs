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

        public EquipmentServiceTest()
        {
            equipmentRepo = new Mock<IEquipmentRepo>();
            equipmentService = new EquipmentService(equipmentRepo.Object);
        }

        [Fact]
        public void GetEquipment_NoEquipment_ThrowsServiceException()
        {
            // Arrange
            List<Equipment> emptyList = new List<Equipment>();
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
            Equipment equipment1 = new Equipment(1, "Treadmill");
            Equipment equipment2 = new Equipment(2, "Dumbbell");

            List<Equipment> equipmentList = new List<Equipment>();

            equipmentList.Add(equipment1);
            equipmentList.Add(equipment2);

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
            // Arrange
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
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");
            equipmentRepo.Setup(repo => repo.GetEquipmentId(1)).Returns(equipment);

            // Act
            Equipment result = equipmentService.GetEquipmentId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(equipment, result);
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
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");
            equipmentRepo.Setup(repo => repo.AddEquipment(equipment));

            // Act
            Equipment result = equipmentService.AddEquipment(equipment);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(equipment, result);
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
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.DeleteEquipment(equipment)
            );
            Assert.Equal(
                "EquipmentService - DeleteEquipment - equipment bestaat niet op id",
                exception.Message
            );
        }

        [Fact]
        public void DeleteEquipment_HasFutureReservations_ThrowsServiceException()
        {
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);
            equipmentRepo.Setup(repo => repo.DeleteEquipment(equipment));

            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Fitness", "Swimming" },
                TypeKlant.Gold
            );

            Dictionary<Time_slot, Equipment> dic = new Dictionary<Time_slot, Equipment>();

            // Simuleer dat er toekomstige reserveringen zijn
            List<Reservation> futureReservations = new List<Reservation>
            {
                new Reservation(1, DateTime.Now, member, dic)
            };

            equipmentRepo
                .Setup(repo => repo.GetFutureReservationsForEquipment(equipment))
                .Returns(futureReservations);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.DeleteEquipment(equipment)
            );
            Assert.Equal(
                "EquipmentService - DeleteEquipment - equipment kan niet verwijderd worden want heeft nog reservations!",
                exception.Message
            );
        }

        [Fact]
        public void DeleteEquipment_ValidEquipment_DeletesEquipment()
        {
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);
            equipmentRepo.Setup(repo => repo.DeleteEquipment(equipment));

            // Simuleer geen toekomstige reserveringen
            equipmentRepo
                .Setup(repo => repo.GetFutureReservationsForEquipment(equipment))
                .Returns(new List<Reservation>());

            // Act
            equipmentService.DeleteEquipment(equipment);

            // Assert
            equipmentRepo.Verify(repo => repo.DeleteEquipment(equipment), Times.Once);
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
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");

            // Mock de repository om te controleren dat het equipment niet bestaat
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => equipmentService.GetFutureReservationsForEquipment(equipment)
            );
            Assert.Equal(
                "EquipmentService - GetFutureReservationsForEquipment - equipment bestaat niet op id",
                exception.Message
            );
        }

        [Fact]
        public void GetFutureReservationsForEquipment_NoFutureReservations_ReturnsEmptyList()
        {
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");

            // Mock de repository om te zorgen dat er geen toekomstige reserveringen zijn
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);
            equipmentRepo
                .Setup(repo => repo.GetFutureReservationsForEquipment(equipment))
                .Returns(Enumerable.Empty<Reservation>());

            // Act
            IEnumerable<Reservation> result = equipmentService.GetFutureReservationsForEquipment(
                equipment
            );

            // Assert
            Assert.NotNull(result); // Zorg ervoor dat het resultaat niet null is
            Assert.Empty(result); // Verifieer dat de lijst leeg is
        }

        [Fact]
        public void GetFutureReservationsForEquipment_WithFutureReservations_ReturnsReservationList()
        {
            // Arrange
            Equipment equipment = new Equipment(1, "Treadmill");
            Member member = new Member(
                1,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Fitness", "Swimming" },
                TypeKlant.Gold
            );

            Dictionary<Time_slot, Equipment> dic = new Dictionary<Time_slot, Equipment>();

            List<Reservation> reservations = new List<Reservation>
            {
                new Reservation(1, DateTime.Now, member, dic),
                new Reservation(2, DateTime.Now, member, dic)
            };

            // Mock de repository om te zorgen dat er toekomstige reserveringen zijn
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);
            equipmentRepo
                .Setup(repo => repo.GetFutureReservationsForEquipment(equipment))
                .Returns(reservations);

            // Act
            IEnumerable<Reservation> result = equipmentService.GetFutureReservationsForEquipment(
                equipment
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

            // Mock de repository om het verwachte equipment te retourneren
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
    }
}
