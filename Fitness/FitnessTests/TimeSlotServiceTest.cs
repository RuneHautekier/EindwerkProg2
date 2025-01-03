using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using FitnessBL.Services;
using Moq;

namespace FitnessTests
{
    public class TimeSlotServiceTest
    {
        private readonly Mock<ITime_slotRepo> timeSlotRepo;
        private readonly Time_slotService timeSlotService;

        public TimeSlotServiceTest()
        {
            timeSlotRepo = new Mock<ITime_slotRepo>();
            timeSlotService = new Time_slotService(timeSlotRepo.Object);
        }

        [Fact]
        public void GetTime_slotId_IdOutOfRange_ThrowsServiceException()
        {
            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => timeSlotService.GetTime_slotId(15)
            );

            exception = Assert.Throws<ServiceException>(() => timeSlotService.GetTime_slotId(0));
            Assert.Equal(
                "Time_slotService - GetTime_slotId - Id moet tussen 1 en 14 liggen!",
                exception.Message
            );
        }

        [Fact]
        public void GetTime_slotId_ValidId_ReturnsTimeSlot()
        {
            // Arrange
            Time_slot timeSlot = new Time_slot(1, 8, 9, "Morning");

            // Mocks
            timeSlotRepo.Setup(repo => repo.GetTime_slotId(1)).Returns(timeSlot);

            // Act
            var result = timeSlotService.GetTime_slotId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(timeSlot, result);
        }
    }
}
