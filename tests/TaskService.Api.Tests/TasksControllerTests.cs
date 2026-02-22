using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskService.Api.Controllers;
using TaskService.Application.DTOs;
using TaskService.Application.Exceptions;
using TaskService.Application.Interfaces;
using TaskService.Domain.Enums;

namespace TaskService.Api.Tests
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskServices> mockTaskServices;
        private readonly Mock<ILogger<TasksController>> mockLogger;
        private readonly TasksController controller;

        public TasksControllerTests()
        {
            mockTaskServices = new Mock<ITaskServices>();
            mockLogger = new Mock<ILogger<TasksController>>();
            controller = new TasksController(mockTaskServices.Object, mockLogger.Object);
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenMissingDependencies()
        {
            Assert.Throws<ArgumentNullException>(() => new TasksController(null!, mockLogger.Object));
            Assert.Throws<ArgumentNullException>(() => new TasksController(mockTaskServices.Object, null!));
        }

        [Fact]
        public async Task Create_Should_Throw_InvalidInputException_If_DTO_Missing()
        {
            // Arrange 
            mockTaskServices.Setup(s => s.CreateAsync(null!, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidInputException());

            // Act
            await Assert.ThrowsAsync<InvalidInputException>(() => controller.Create(null!, CancellationToken.None));

            // Assert 

            mockTaskServices.Verify(s => s.CreateAsync(null!, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_Should_Throw_InvalidInputException_If_DTO_Is_Invalid()
        {
            // Arrange
            var invalidTitle = "<b>invalid</b>";
            var dto = new CreateTaskDto
            {
                Title = invalidTitle,
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo
            };

            mockTaskServices
                .Setup(s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == invalidTitle), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidInputException());

            // Act
            await Assert.ThrowsAsync<InvalidInputException>(() => controller.Create(dto, CancellationToken.None));

            // Assert
            mockTaskServices.Verify(
                s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == invalidTitle), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Create_Should_Throw_ValidationException_If_Title_Is_Null()
        {
            // Arrange 
            var dto = new CreateTaskDto
            {
                Title = null!,
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo
            };

            mockTaskServices
                .Setup(s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ValidationException.For("Title", "Title is required."));

            // Act
            var ex = await Assert.ThrowsAsync<ValidationException>(() => controller.Create(dto, CancellationToken.None));

            // Assert
            mockTaskServices.Verify(
                s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.Contains(ex.Errors.Keys, k => string.Equals(k, "Title", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Create_Should_Throw_ValidationException_If_Title_Is_Empty_String()
        {
            // Arrange 
            var dto = new CreateTaskDto
            {
                Title = "   ",
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo
            };

            mockTaskServices
                .Setup(s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ValidationException.For("Title", "Title is required."));

            // Act
            var ex = await Assert.ThrowsAsync<ValidationException>(() => controller.Create(dto, CancellationToken.None));

            // Assert
            mockTaskServices.Verify(
                s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.Contains(ex.Errors.Keys, k => string.Equals(k, "Title", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Create_Should_Throw_ValidationException_If_Title_Length_Above_200_Characters()
        {
            // Arrange 
            var dto = new CreateTaskDto
            {
                Title = new string('a', 201),
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo
            };

            mockTaskServices
                .Setup(s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ValidationException.For("Title", "Title is required."));

            // Act
            var ex = await Assert.ThrowsAsync<ValidationException>(() => controller.Create(dto, CancellationToken.None));

            // Assert
            mockTaskServices.Verify(
                s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.Contains(ex.Errors.Keys, k => string.Equals(k, "Title", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Create_Should_Throw_ValidationException_If_Description_Length_Above_2000_Characters()
        {
            // Arrange 
            var dto = new CreateTaskDto
            {
                Title = "test",
                Description = new string('a', 2001),
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo
            };

            mockTaskServices
                .Setup(s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Description == dto.Description), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ValidationException.For("Description", "Description is required."));

            // Act
            var ex = await Assert.ThrowsAsync<ValidationException>(() => controller.Create(dto, CancellationToken.None));

            // Assert
            mockTaskServices.Verify(
                s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Description == dto.Description), It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.Contains(ex.Errors.Keys, k => string.Equals(k, "Description", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Create_Should_Throw_ValidationException_If_originalEstimatedWork_Given_Negative_Value()
        {
            // Arrange 
            var dto = new CreateTaskDto
            {
                Title = "test",
                Description = "desc",
                OriginalEstimatedWork = -1,
                Status = TasksStatus.Todo
            };

            mockTaskServices
                .Setup(s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.OriginalEstimatedWork == dto.OriginalEstimatedWork), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ValidationException.For("OriginalEstimatedWork", "OriginalEstimatedWork is required."));

            // Act
            var ex = await Assert.ThrowsAsync<ValidationException>(() => controller.Create(dto, CancellationToken.None));

            // Assert
            mockTaskServices.Verify(
                s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.OriginalEstimatedWork == dto.OriginalEstimatedWork), It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.Contains(ex.Errors.Keys, k => string.Equals(k, "OriginalEstimatedWork", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Create_Should_Throw_ValidationException_If_Status_Given_Wrong_Value()
        {
            // Arrange 
            var dto = new CreateTaskDto
            {
                Title = "test",
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = (TasksStatus)999 // Invalid status value
            };

            mockTaskServices
                .Setup(s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Status == dto.Status), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ValidationException.For("Status", "Status is invalid."));

            // Act
            var ex = await Assert.ThrowsAsync<ValidationException>(() => controller.Create(dto, CancellationToken.None));

            // Assert
            mockTaskServices.Verify(
                s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Status == dto.Status), It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.Contains(ex.Errors.Keys, k => string.Equals(k, "Status", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Create_Should_Return_CreatedResult_If_Successful()
        {
            // Arrange 
            var dto = new CreateTaskDto
            {
                Title = "test",
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo
            };
            var responseDto = new TaskResponseDto
            {
                Id = 1,
                Title = dto.Title,
                Description = dto.Description,
                OriginalEstimatedWork = dto.OriginalEstimatedWork,
                Status = dto.Status.ToString()
            };
            mockTaskServices
                .Setup(s => s.CreateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseDto);
            // Act
            var result = await controller.Create(dto, CancellationToken.None);
            // Assert
            mockTaskServices.Verify(
                s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()),
                Times.Once);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(responseDto.Id, createdResult.RouteValues?["id"]);
            Assert.Equal(responseDto, createdResult.Value);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }

        [Fact]
        public async Task Create_Should_Throw_Conflict_If_Title_Already_Exists()
        {
            // Arrange 
            var dto = new CreateTaskDto
            {
                Title = "test",
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo
            };

            mockTaskServices
                .Setup(s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Conflict: a task with the same title already exists."));

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.Create(dto, CancellationToken.None));

            // Assert
            mockTaskServices.Verify(
                s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Create_Should_Throw_If_Task_Creating_With_Done_Status()
        {
            // Arrange 
            var dto = new CreateTaskDto
            {
                Title = "test",
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Done
            };

            mockTaskServices
                .Setup(s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Status == dto.Status), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ValidationException.For("Status", "Status is invalid."));

            // Act
            await Assert.ThrowsAsync<ValidationException>(() => controller.Create(dto, CancellationToken.None));

            // Assert
            mockTaskServices.Verify(
                s => s.CreateAsync(It.Is<CreateTaskDto>(d => d.Status == dto.Status), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetById_Should_Return_Ok_If_Found()
        {
            // Arrange
            var id = 1;
            var response = new TaskResponseDto
            {
                Id = id,
                Title = "test",
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            mockTaskServices
                .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await controller.GetById(id, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
            Assert.Equal(response, ok.Value);
            mockTaskServices.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_Should_Throw_NotFoundException_If_Task_Not_Exists()
        {
            // Arrange
            var id = 999;
            mockTaskServices
                .Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Task", id));

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetById(id, CancellationToken.None));
            mockTaskServices.Verify(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_Should_Return_Ok_With_Empty_List_If_No_Tasks_Exist()
        {
            // Arrange
            mockTaskServices
                .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var result = await controller.GetAll(CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
            var payload = Assert.IsAssignableFrom<IEnumerable<TaskResponseDto>>(ok.Value);
            Assert.Empty(payload);
            mockTaskServices.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_Should_Return_Ok_If_Successful()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateTaskDto
            {
                Title = "updated",
                Description = "desc",
                OriginalEstimatedWork = 2,
                Status = TasksStatus.InProgress
            };

            var response = new TaskResponseDto
            {
                Id = id,
                Title = dto.Title,
                Description = dto.Description,
                OriginalEstimatedWork = dto.OriginalEstimatedWork,
                Status = dto.Status.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            mockTaskServices
                .Setup(s => s.UpdateAsync(id, It.Is<UpdateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await controller.Update(id, dto, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
            Assert.Equal(response, ok.Value);
            mockTaskServices.Verify(
                s => s.UpdateAsync(id, It.Is<UpdateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Update_Should_Throw_ValidationException_If_Title_Is_Null()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateTaskDto
            {
                Title = null!,
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo
            };

            mockTaskServices
                .Setup(s => s.UpdateAsync(id, It.Is<UpdateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ValidationException.For("Title", "Title is required."));

            // Act
            var ex = await Assert.ThrowsAsync<ValidationException>(() => controller.Update(id, dto, CancellationToken.None));

            // Assert
            Assert.Contains(ex.Errors.Keys, k => string.Equals(k, "Title", StringComparison.OrdinalIgnoreCase));
            mockTaskServices.Verify(
                s => s.UpdateAsync(id, It.Is<UpdateTaskDto>(d => d.Title == dto.Title), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Update_Should_Throw_NotFoundException_If_Task_Not_Exists()
        {
            // Arrange
            var id = 999;
            var dto = new UpdateTaskDto
            {
                Title = "updated",
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo
            };

            mockTaskServices
                .Setup(s => s.UpdateAsync(id, It.IsAny<UpdateTaskDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Task", id));

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.Update(id, dto, CancellationToken.None));
            mockTaskServices.Verify(s => s.UpdateAsync(id, It.IsAny<UpdateTaskDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_Should_Throw_Conflict_If_Title_Already_Exists()
        {
            // Arrange
            var id = 1;
            var dto = new UpdateTaskDto
            {
                Title = "duplicate",
                Description = "desc",
                OriginalEstimatedWork = 1,
                Status = TasksStatus.Todo
            };

            mockTaskServices
                .Setup(s => s.UpdateAsync(id, It.IsAny<UpdateTaskDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Conflict: a task with the same title already exists."));

            // Act / Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.Update(id, dto, CancellationToken.None));
            mockTaskServices.Verify(s => s.UpdateAsync(id, It.IsAny<UpdateTaskDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_Should_Return_NoContent_If_Successful()
        {
            // Arrange
            var id = 1;
            mockTaskServices
                .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.Delete(id, CancellationToken.None);

            // Assert
            var noContent = Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, noContent.StatusCode);
            mockTaskServices.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_Should_Throw_NotFoundException_If_Task_Not_Exists()
        {
            // Arrange
            var id = 999;
            mockTaskServices
                .Setup(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Task", id));

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.Delete(id, CancellationToken.None));
            mockTaskServices.Verify(s => s.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}