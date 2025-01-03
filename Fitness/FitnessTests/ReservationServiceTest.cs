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

        private Time_slot timeSlot1;
        private Time_slot timeSlot2;
        private Equipment equipment;
        private Equipment equipmentInOnderhoud;
        private Equipment alternativeEquipment;
        private Member member1;
        private Member member;
        private Dictionary<Time_slot, Equipment> timeslotEquipment;
        private Reservation reservation;
        private Reservation updatedReservation;

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

            // Initialiseer objecten die in meerdere tests gebruikt worden.
            timeSlot1 = new Time_slot(4, 11, 12, "morning");
            timeSlot2 = new Time_slot(5, 12, 13, "morning");
            equipment = new Equipment(1, "Treadmill");

            equipmentInOnderhoud = new Equipment(1, "Treadmill");
            alternativeEquipment = new Equipment(2, "Alternative Treadmill");

            member1 = new Member(
                2,
                "John",
                "Doe",
                "john.doe@example.com",
                "Some Street 123",
                new DateTime(1990, 1, 1),
                new List<string> { "Fitness", "Swimming" },
                TypeKlant.Gold
            );

            member = new Member(
                1,
                "Jane",
                "Doe",
                "jane.doe@example.com",
                "Another Street 456",
                new DateTime(1995, 5, 20),
                new List<string> { "Yoga", "Running" },
                TypeKlant.Silver
            );

            timeslotEquipment = new Dictionary<Time_slot, Equipment>
            {
                { timeSlot1, equipment },
                { timeSlot2, equipment }
            };

            reservation = new Reservation(1, DateTime.Now.AddDays(1), member1, timeslotEquipment);

            updatedReservation = new Reservation(
                2,
                DateTime.Now.AddDays(1),
                member1,
                new Dictionary<Time_slot, Equipment>
                {
                    { timeSlot1, alternativeEquipment },
                    { timeSlot2, alternativeEquipment }
                }
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

            // Mocks
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

            // Mocks
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

            // Mocks
            reservationRepo.Setup(repo => repo.GetNieuwReservationId()).Returns(expectedId); // De mock returnt het verwachte ID

            // Act
            int result = reservationService.GetNieuwReservationId();

            // Assert
            Assert.Equal(expectedId, result); // Controleer of het resultaat gelijk is aan het verwachte ID
            reservationRepo.Verify(repo => repo.GetNieuwReservationId(), Times.Once); // Verifieer dat de repository-methode werd aangeroepen
        }

        [Fact]
        public void AddReservation_NullReservation_ThrowsServiceException()
        {
            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => reservationService.AddReservation(null)
            );
            Assert.Equal("Reservation - Reservatie is null", exception.Message);
        }

        [Fact]
        public void AddReservation_InvalidTimeslotCount_ThrowsReservationException()
        {
            // Arrange
            Reservation reservation = new Reservation(
                1,
                DateTime.Now,
                member1,
                new Dictionary<Time_slot, Equipment>()
            );

            // Act & Assert
            ReservationException exception = Assert.Throws<ReservationException>(
                () => reservationService.AddReservation(reservation)
            );
            Assert.Equal(
                "Je moet minimaal 1 tijdslot en maximaal 2 tijdsloten reserveren!",
                exception.Message
            );
        }

        [Fact]
        public void AddReservation_TimeslotsNotAdjacent_ThrowsTimeSlotException()
        {
            // Arrange
            Time_slot timeSlot1 = new Time_slot(1, 8, 9, "Morning");
            Time_slot timeSlot2 = new Time_slot(3, 10, 11, "Morning");
            Dictionary<Time_slot, Equipment> timeslotEquipment = new Dictionary<
                Time_slot,
                Equipment
            >
            {
                { timeSlot1, equipment },
                { timeSlot2, equipment }
            };

            Reservation reservation = new Reservation(1, DateTime.Now, member1, timeslotEquipment);

            // Act & Assert
            Time_SlotException exception = Assert.Throws<Time_SlotException>(
                () => reservationService.AddReservation(reservation)
            );
            Assert.Equal("De tijdsloten moeten na elkaar liggen!", exception.Message);
        }

        [Fact]
        public void AddReservation_EquipmentInMaintenance_ThrowsEquipmentException()
        {
            // Mocks

            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);
            equipmentRepo.Setup(service => service.EquipmentInOnderhoud(equipment)).Returns(true);

            // Act & Assert
            EquipmentException exception = Assert.Throws<EquipmentException>(
                () => reservationService.AddReservation(reservation)
            );
            Assert.Equal(
                $"Equipment met id {equipment.Equipment_id} zit momenteel in onderhoud!",
                exception.Message
            );
        }

        [Fact]
        public void AddReservation_MemberExceedsTimeSlotLimit_ThrowsMemberException()
        {
            /// Mocks
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);

            memberRepo
                .Setup(service => service.GetAantalGeboekteTijdsloten(reservation.Date, member1))
                .Returns(4);

            // Act & Assert
            MemberException exception = Assert.Throws<MemberException>(
                () => reservationService.AddReservation(reservation)
            );
            Assert.Equal(
                "Een member mag maximaal 4 TimeSlots per dag reserveren.",
                exception.Message
            );
        }

        [Fact]
        public void AddReservation_TimeSlotsDifferentSessions_ThrowsTimeSlotException()
        {
            // Arrange
            Time_slot timeSlot1 = new Time_slot(4, 11, 12, "morning");
            Time_slot timeSlot2 = new Time_slot(5, 12, 13, "afternoon");

            Dictionary<Time_slot, Equipment> timeslotEquipment = new Dictionary<
                Time_slot,
                Equipment
            >
            {
                { timeSlot1, equipment },
                { timeSlot2, equipment }
            };
            Reservation reservation = new Reservation(1, DateTime.Now, member1, timeslotEquipment);

            // Mocks

            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);

            // Act & Assert
            Time_SlotException exception = Assert.Throws<Time_SlotException>(
                () => reservationService.AddReservation(reservation)
            );
            Assert.Equal(
                "Alle Time_slots moeten in dezelfde dagindeling vallen.",
                exception.Message
            );
        }

        [Fact]
        public void AddReservation_ReservationAlreadyExists_ThrowsServiceException()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);

            reservationRepo
                .Setup(repo => repo.CheckIfReservationExists(reservation))
                .Throws(new ServiceException("Reservatie bestaat al!"));

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => reservationService.AddReservation(reservation)
            );
            Assert.Equal("Reservatie bestaat al!", exception.Message);
        }

        [Fact]
        public void AddReservation_ValidReservation_ReturnsReservation()
        {
            // Mocks
            memberRepo.Setup(repo => repo.IsMemberId(member1)).Returns(true);
            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipment)).Returns(true);
            equipmentRepo.Setup(service => service.EquipmentInOnderhoud(equipment)).Returns(false);
            memberRepo
                .Setup(service => service.GetAantalGeboekteTijdsloten(reservation.Date, member1))
                .Returns(2);
            reservationRepo.Setup(repo => repo.CheckIfReservationExists(reservation)).Verifiable();
            reservationRepo.Setup(repo => repo.AddReservation(reservation)).Verifiable();

            // Act
            Reservation result = reservationService.AddReservation(reservation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservation, result);
        }

        [Fact]
        public void UpdateReservationsWithNewEquipment_NoAlternativeEquipment_ThrowsServiceException()
        {
            // Arrange

            Dictionary<Time_slot, Equipment> timeslotEquipment = new Dictionary<
                Time_slot,
                Equipment
            >
            {
                { timeSlot1, equipmentInOnderhoud }
            };

            Reservation reservation = new Reservation(
                1,
                DateTime.Now.AddDays(1),
                member,
                timeslotEquipment
            );

            List<Reservation> reservations = new List<Reservation> { reservation };

            // Mocks

            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipmentInOnderhoud)).Returns(true);

            equipmentRepo
                .Setup(service => service.GetFutureReservationsForEquipment(equipmentInOnderhoud))
                .Returns(reservations);

            equipmentRepo
                .Setup(service => service.GetEquipmentId(equipmentInOnderhoud.Equipment_id))
                .Returns(equipmentInOnderhoud);

            equipmentRepo
                .Setup(service =>
                    service.GetAvailableEquipment(
                        reservation.Date,
                        timeSlot1,
                        equipmentInOnderhoud.Device_type
                    )
                )
                .Returns((Equipment)null); // Geen alternatief beschikbaar

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => reservationService.UpdateReservationsWithNewEquipment(equipmentInOnderhoud)
            );

            Assert.Equal(
                $"Geen beschikbaar alternatief equipment gevonden voor tijdslot {timeSlot1}!",
                exception.Message
            );
        }

        [Fact]
        public void UpdateReservationsWithNewEquipment_ValidScenario_UpdatesReservations()
        {
            // Arrange

            Dictionary<Time_slot, Equipment> timeslotEquipment = new Dictionary<
                Time_slot,
                Equipment
            >
            {
                { timeSlot1, equipmentInOnderhoud },
                { timeSlot2, equipmentInOnderhoud }
            };

            Reservation reservation = new Reservation(
                1,
                DateTime.Now.AddDays(1),
                member,
                timeslotEquipment
            );

            List<Reservation> reservations = new List<Reservation> { reservation };

            // Mocks

            equipmentRepo.Setup(repo => repo.IsEquipmentId(equipmentInOnderhoud)).Returns(true);

            equipmentRepo
                .Setup(service => service.GetFutureReservationsForEquipment(equipmentInOnderhoud))
                .Returns(reservations);

            equipmentRepo
                .Setup(service => service.GetEquipmentId(equipmentInOnderhoud.Equipment_id))
                .Returns(equipmentInOnderhoud);

            equipmentRepo
                .Setup(service =>
                    service.GetAvailableEquipment(
                        reservation.Date,
                        timeSlot1,
                        equipmentInOnderhoud.Device_type
                    )
                )
                .Returns(alternativeEquipment);

            equipmentRepo
                .Setup(service =>
                    service.GetAvailableEquipment(
                        reservation.Date,
                        timeSlot2,
                        equipmentInOnderhoud.Device_type
                    )
                )
                .Returns(alternativeEquipment);

            reservationRepo.Setup(repo => repo.DeleteReservation(reservation)).Verifiable();
            reservationRepo
                .Setup(repo => repo.AddReservation(It.IsAny<Reservation>()))
                .Verifiable();

            // Act
            reservationService.UpdateReservationsWithNewEquipment(equipmentInOnderhoud);

            Assert.All(
                reservation.TimeslotEquipment.Values,
                e => Assert.Equal(alternativeEquipment, e)
            );
        }

        [Fact]
        public void UpdateReservationTimeSlots_ReservationDoesNotExist_ThrowsServiceException()
        {
            // Arrange
            Dictionary<Time_slot, Equipment> timeslotEquipmentRes1 = new Dictionary<
                Time_slot,
                Equipment
            >
            {
                { timeSlot1, equipment },
            };

            Dictionary<Time_slot, Equipment> timeslotEquipmentRes2 = new Dictionary<
                Time_slot,
                Equipment
            >
            {
                { timeSlot1, equipment },
                { timeSlot2, equipment },
            };

            Reservation nietBestaandeReservation = new Reservation(
                1,
                DateTime.Now.AddDays(1),
                member,
                timeslotEquipmentRes1
            );

            Reservation updatedReservation = new Reservation(
                2,
                DateTime.Now,
                member,
                timeslotEquipmentRes2
            );

            // Mocks

            reservationRepo
                .Setup(repo => repo.IsReservationId(nietBestaandeReservation))
                .Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () =>
                    reservationService.UpdateReservationTimeSlots(
                        nietBestaandeReservation,
                        updatedReservation
                    )
            );

            Assert.Equal(
                "ReservationService - UpdateReservationTimeSlots - Er is geen reservation met dit id!",
                exception.Message
            );
        }

        [Fact]
        public void UpdateReservationTimeSlots_ValidReservations_Success()
        {
            // Arrange
            Time_slot oldTimeSlot = new Time_slot(1, 10, 11, "morning");
            Equipment oldEquipment = new Equipment(1, "Treadmill");

            Time_slot newTimeSlot = new Time_slot(2, 11, 12, "morning");
            Equipment newEquipment = new Equipment(2, "Dumbbell");

            Dictionary<Time_slot, Equipment> oldTimeslotEquipment = new Dictionary<
                Time_slot,
                Equipment
            >
            {
                { oldTimeSlot, oldEquipment }
            };

            Dictionary<Time_slot, Equipment> newTimeslotEquipment = new Dictionary<
                Time_slot,
                Equipment
            >
            {
                { newTimeSlot, newEquipment }
            };

            Reservation oldReservation = new Reservation(
                1,
                DateTime.Now.AddDays(1),
                member,
                oldTimeslotEquipment
            );
            Reservation updatedReservation = new Reservation(
                1,
                DateTime.Now.AddDays(1),
                member,
                newTimeslotEquipment
            );

            // Mocks

            reservationRepo.Setup(repo => repo.IsReservationId(oldReservation)).Returns(true);

            // Act
            reservationService.UpdateReservationTimeSlots(oldReservation, updatedReservation);

            // Assert
            reservationRepo.Verify(repo => repo.DeleteReservation(oldReservation), Times.Once);
            reservationRepo.Verify(repo => repo.AddReservation(updatedReservation), Times.Once);
        }

        [Fact]
        public void DeleteReservation_ReservationDoesNotExist_ThrowsServiceException()
        {
            // Arrange

            Reservation reservation = new Reservation(
                1,
                DateTime.Now.AddDays(1),
                member,
                timeslotEquipment
            );

            // Mocks

            reservationRepo.Setup(repo => repo.IsReservationId(reservation)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => reservationService.DeleteReservation(reservation)
            );

            Assert.Equal(
                "ReservationService - DeleteReservation - Reservation bestaat niet met dit id!",
                exception.Message
            );

            reservationRepo.Verify(
                repo => repo.DeleteReservation(It.IsAny<Reservation>()),
                Times.Never
            );
        }

        [Fact]
        public void DeleteReservation_ValidReservation_Success()
        {
            // Mocks
            reservationRepo.Setup(repo => repo.IsReservationId(reservation)).Returns(true);

            // Act
            reservationService.DeleteReservation(reservation);

            // Assert
            reservationRepo.Verify(repo => repo.DeleteReservation(reservation), Times.Once);
        }
    }
}
