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
    public class ProgramServiceTest
    {
        private readonly Mock<IProgramRepo> programRepo;
        private readonly ProgramService programService;
        private Program program;

        public ProgramServiceTest()
        {
            programRepo = new Mock<IProgramRepo>();
            programService = new ProgramService(programRepo.Object);
            program = new Program
            {
                ProgramCode = "P999",
                Name = "Yoga Basics",
                Target = "Beginners",
                Startdate = DateTime.Now,
                Max_members = 10
            };
        }

        [Fact]
        public void GetProgramCode_InvalidProgramCode_ThrowsServiceException()
        {
            // Arrange
            string programCode = "P998";

            // Mocks
            programRepo.Setup(repo => repo.GetProgramCode(programCode)).Returns((Program)null);

            // Act
            ServiceException exception = Assert.Throws<ServiceException>(
                () => programService.GetProgramCode(programCode)
            );

            // Assert
            Assert.Equal(
                "ProgramService - GetProgramId - Er is geen Program met deze programCode!",
                exception.Message
            );
        }

        [Fact]
        public void GetProgramCode_ValidProgramCode_ReturnsProgram()
        {
            // Arrange
            string programCode = "P999";

            // Mocks
            programRepo.Setup(repo => repo.GetProgramCode(programCode)).Returns(program);

            // Act
            Program result = programService.GetProgramCode(programCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(program, result);
        }

        [Fact]
        public void AddProgram_NullProgram_ThrowsServiceException()
        {
            // Arrange
            Program newProgram = null;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => programService.AddProgram(newProgram)
            );

            Assert.Equal("ProgramService - AddProgram - Program is null", exception.Message);
        }

        [Fact]
        public void AddProgram_ExistingProgram_ThrowsServiceException()
        {
            // Mocks
            programRepo.Setup(repo => repo.BestaatProgram(program)).Returns(true);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => programService.AddProgram(program)
            );

            Assert.Equal(
                "ProgramService - AddProgram - Dit programma bestaat al (zelfde programmaCode)",
                exception.Message
            );
        }

        [Fact]
        public void AddProgram_ValidProgram_AddsProgramAndReturnsIt()
        {
            // Arrange
            Program newProgram = new Program
            {
                ProgramCode = "P123",
                Name = "Yoga Basics",
                Target = "Beginners",
                Startdate = DateTime.Now,
                Max_members = 10
            };

            // Mocks
            programRepo.Setup(repo => repo.BestaatProgram(newProgram)).Returns(false);
            programRepo.Setup(repo => repo.AddProgram(newProgram));

            // Act
            Program result = programService.AddProgram(newProgram);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newProgram, result);
        }

        [Fact]
        public void UpdateProgram_NullProgram_ThrowsServiceException()
        {
            // Arrange
            Program programToUpdate = null;

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => programService.UpdateProgram(programToUpdate)
            );

            Assert.Equal("ProgramService - UpdateProgram - Program is null!", exception.Message);
        }

        [Fact]
        public void UpdateProgram_NonExistingProgram_ThrowsServiceException()
        {
            // Arrange
            Program nonExistingProgram = new Program
            {
                ProgramCode = "P123",
                Name = "Yoga Basics",
                Target = "Beginners",
                Startdate = DateTime.Now,
                Max_members = 10
            };

            // Mocks
            programRepo.Setup(repo => repo.BestaatProgram(nonExistingProgram)).Returns(false);

            // Act & Assert
            ServiceException exception = Assert.Throws<ServiceException>(
                () => programService.UpdateProgram(nonExistingProgram)
            );

            Assert.Equal(
                "ProgramService - UpdateProgram - Program bestaat niet met deze programCode!",
                exception.Message
            );
        }

        [Fact]
        public void UpdateProgram_ValidProgram_UpdatesAndReturnsProgram()
        {
            Program updatedProgram = new Program
            {
                ProgramCode = "P999",
                Name = "Advanced Yoga",
                Target = "Intermediate",
                Startdate = program.Startdate,
                Max_members = 15
            };

            // Mocks
            programRepo
                .Setup(repo => repo.BestaatProgram(It.Is<Program>(p => p.ProgramCode == "P999")))
                .Returns(true);

            programRepo
                .Setup(repo => repo.UpdateProgram(It.IsAny<Program>()))
                .Callback<Program>(p =>
                {
                    // Simuleer de update van het programma
                    program.Name = p.Name;
                    program.Target = p.Target;
                    program.Max_members = p.Max_members;
                });

            // Act
            Program result = programService.UpdateProgram(updatedProgram);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Advanced Yoga", program.Name);
            Assert.Equal("Intermediate", program.Target);
            Assert.Equal(15, program.Max_members);
        }
    }
}
