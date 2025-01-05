using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessAPI.Controllers;
using FitnessAPI.DTO;
using FitnessBL.Exceptions;
using FitnessBL.Interfaces;
using FitnessBL.Model;
using FitnessBL.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FitnessTests
{
    public class ProgramControllerTest
    {
        private readonly Mock<IProgramRepo> programRepoMock;
        private readonly ProgramService programService;

        private readonly ProgramController controller;

        private Program program;

        public ProgramControllerTest()
        {
            programRepoMock = new Mock<IProgramRepo>();
            programService = new ProgramService(programRepoMock.Object);
            controller = new ProgramController(programService);

            DateTime date = DateTime.Now.AddDays(5);
            Dictionary<int, Member> dic = new Dictionary<int, Member>();
            program = new Program("P1", "Program 1 voor beginners", "Beginners", date, 50, dic);
        }

        [Fact]
        public void GetProgramCode_ProgramNotFound_ReturnsBadRequest()
        {
            // Arrange
            string programCode = "ABC123";

            programRepoMock.Setup(repo => repo.GetProgramCode(programCode)).Returns((Program)null);

            // Act
            var result = controller.GetProgramCode(programCode);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "ProgramService - GetProgramId - Er is geen Program met deze programCode!",
                badRequestResult.Value
            );
        }

        [Fact]
        public void GetProgramCode_ValidProgramCode_ReturnsOk()
        {
            // Mocks
            programRepoMock
                .Setup(repo => repo.GetProgramCode(program.ProgramCode))
                .Returns(program);

            // Act
            var result = controller.GetProgramCode(program.ProgramCode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Program>(okResult.Value);
            Assert.Equal(program.ProgramCode, returnValue.ProgramCode);
            Assert.Equal(program.Name, returnValue.Name);
        }

        [Fact]
        public void AddProgram_ThrowsProgramException_ReturnsBadRequest()
        {
            // Arrange
            ProgramAanmakenDTO programDTO = new ProgramAanmakenDTO
            {
                ProgramCode = "ABC123",
                Name = "Cycling Program",
                Target = "Cycling enthusiasts",
                Startdate = new DateTime(2025, 1, 1),
                Max_members = 30
            };

            Program program = new Program(
                programDTO.ProgramCode,
                programDTO.Name,
                programDTO.Target,
                programDTO.Startdate,
                programDTO.Max_members
            );

            programRepoMock
                .Setup(repo =>
                    repo.BestaatProgram(It.Is<Program>(p => p.ProgramCode == program.ProgramCode))
                )
                .Returns(true);

            // Act
            var result = controller.AddProgram(programDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "ProgramService - AddProgram - Dit programma bestaat al (zelfde programmaCode)",
                badRequestResult.Value
            );
        }

        [Fact]
        public void AddProgram_ValidProgram_ReturnsCreatedAtAction()
        {
            // Arrange
            ProgramAanmakenDTO programDTO = new ProgramAanmakenDTO
            {
                ProgramCode = "ABC123",
                Name = "Cycling Program",
                Target = "Cycling enthusiasts",
                Startdate = new DateTime(2025, 1, 1),
                Max_members = 30
            };

            Program program = new Program(
                programDTO.ProgramCode,
                programDTO.Name,
                programDTO.Target,
                programDTO.Startdate,
                programDTO.Max_members
            );

            programRepoMock.Setup(repo => repo.AddProgram(program)).Verifiable();

            // Act
            var result = controller.AddProgram(programDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetProgramCode", createdResult.ActionName);
            Assert.Equal(program.ProgramCode, createdResult.RouteValues["programCode"]);
            var returnValue = Assert.IsType<Program>(createdResult.Value);
            Assert.Equal(program.ProgramCode, returnValue.ProgramCode);
        }

        [Fact]
        public void UpdateProgram_ValidUpdate_ReturnsCreatedAtAction()
        {
            // Arrange


            ProgramAanmakenDTO programDTO = new ProgramAanmakenDTO
            {
                Startdate = new DateTime(2025, 6, 1),
                Name = "Updated Program Name",
                Target = "Updated Target",
                Max_members = 100
            };

            // Mocks
            programRepoMock
                .Setup(repo => repo.GetProgramCode(program.ProgramCode))
                .Returns(program);
            programRepoMock.Setup(repo => repo.BestaatProgram(program)).Returns(true);
            programRepoMock.Setup(repo => repo.UpdateProgram(program)).Verifiable();

            // Act
            var result = controller.UpdateMember(
                program.ProgramCode,
                programDTO.Startdate,
                programDTO.Name,
                programDTO.Target,
                programDTO.Max_members
            );

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetProgramCode", createdResult.ActionName);
            Assert.Equal(program.ProgramCode, createdResult.RouteValues["programCode"]);
            var returnValue = Assert.IsType<Program>(createdResult.Value);
            Assert.Equal(programDTO.Name, returnValue.Name);
            Assert.Equal(programDTO.Target, returnValue.Target);
            Assert.Equal(programDTO.Startdate, returnValue.Startdate);
            Assert.Equal(programDTO.Max_members, returnValue.Max_members);
        }

        [Fact]
        public void UpdateProgram_ProgramNotFound_ReturnsBadRequest()
        {
            // Arrange


            ProgramAanmakenDTO programDTO = new ProgramAanmakenDTO
            {
                Startdate = new DateTime(2025, 6, 1),
                Name = "Updated Program Name",
                Target = "Updated Target",
                Max_members = 100
            };

            // Mocks
            programRepoMock
                .Setup(repo => repo.GetProgramCode(program.ProgramCode))
                .Returns((Program)null);
            programRepoMock.Setup(repo => repo.BestaatProgram(program)).Returns(false);
            programRepoMock.Setup(repo => repo.UpdateProgram(program)).Verifiable();

            // Act
            var result = controller.UpdateMember(
                program.ProgramCode,
                programDTO.Startdate,
                programDTO.Name,
                programDTO.Target,
                programDTO.Max_members
            );

            // Assert
            var expectedResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(
                "ProgramService - GetProgramId - Er is geen Program met deze programCode!",
                expectedResult.Value
            );
        }
    }
}
