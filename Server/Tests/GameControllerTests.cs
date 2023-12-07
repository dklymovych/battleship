using Xunit;
using Moq;
using Server.Controllers;
using Server.Database;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Server.Dto;


namespace Server.Tests
{
    public class GameControllerTests
    {
        private readonly Mock<DataContext> _mockContext;
        private readonly GameController _controller;
        private readonly ClaimsPrincipal _user;

        public GameControllerTests()
        {
            _mockContext = new Mock<DataContext>();
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "TestUser")
            }));

            _controller = new GameController(_mockContext.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _user }
                }
            };
        }

        [Fact]
        public void GenerateGameCode_ReturnsCorrectLengthString()
        {
            var gameCode = _controller.GenerateGameCode();
            Assert.Equal(6, gameCode.Length); 
        }

        [Fact]
        public void FindRoom_ReturnsRoom_IfExists()
        {
            var existingRoom = new Room
            {
                GameCode = "ABC123",
                Player1 = new Player { Username = "TestUser" }
            };
            var rooms = new List<Room> { existingRoom }.AsQueryable();
            var mockSet = new Mock<DbSet<Room>>();
            mockSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
            mockSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
            mockSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
            mockSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());
            _mockContext.Setup(ctx => ctx.Rooms).Returns(mockSet.Object);
            var result = _controller.FindRoom("ABC123");
            Assert.NotNull(result);
            Assert.Equal("ABC123", result.GameCode);
        }

        [Fact]
        public void FindRoom_ReturnsNull_IfRoomDoesNotExist()
        {
            var rooms = new List<Room>().AsQueryable();
            var mockSet = new Mock<DbSet<Room>>();
            mockSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
            mockSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
            mockSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
            mockSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());
            _mockContext.Setup(ctx => ctx.Rooms).Returns(mockSet.Object);
            var result = _controller.FindRoom("NonExistingCode");
            Assert.Null(result);
        }

        [Fact]
        public void JoinRoom_ReturnsNotFound_IfRoomDoesNotExist()
        {
            var rooms = new List<Room>().AsQueryable();
            var mockSet = new Mock<DbSet<Room>>();
            mockSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
            mockSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
            mockSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
            mockSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());
            _mockContext.Setup(ctx => ctx.Rooms).Returns(mockSet.Object);
            var actionResult = _controller.JoinRoom("NonExistingCode", null);
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public void CancelWaitForGame_ReturnsNotFound_IfGameDoesNotExist()
        {
            var rooms = new List<Room>().AsQueryable();
            var mockSet = new Mock<DbSet<Room>>();
            mockSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
            mockSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
            mockSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
            mockSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());
            _mockContext.Setup(ctx => ctx.Rooms).Returns(mockSet.Object);
            var result = _controller.CancelWaitForGame("NonExistingGameCode");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Surrender_ReturnsNotFound_IfGameDoesNotExist()
        {
            var rooms = new List<Room>().AsQueryable();
            var mockSet = new Mock<DbSet<Room>>();
            mockSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
            mockSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
            mockSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
            mockSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());
            _mockContext.Setup(ctx => ctx.Rooms).Returns(mockSet.Object);
            var result = _controller.Surrender("NonExistingGameCode");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void WaitForGame_ReturnsNotFound_IfGameDoesNotExist()
        {
            var rooms = new List<Room>().AsQueryable();
            var mockSet = new Mock<DbSet<Room>>();
            mockSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
            mockSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
            mockSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
            mockSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());
            _mockContext.Setup(ctx => ctx.Rooms).Returns(mockSet.Object);
            var result = _controller.WaitForGame("NonExistingGameCode");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void MakeMove_ReturnsNotFound_IfGameDoesNotExist()
        {
            var rooms = new List<Room>().AsQueryable();
            var mockSet = new Mock<DbSet<Room>>();
            mockSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
            mockSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
            mockSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
            mockSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());
            _mockContext.Setup(ctx => ctx.Rooms).Returns(mockSet.Object);
            var result = _controller.MakeMove("NonExistingGameCode", new CoordinateDto());
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void WaitForMove_ReturnsNotFound_IfGameDoesNotExist()
        {
            var rooms = new List<Room>().AsQueryable();
            var mockSet = new Mock<DbSet<Room>>();
            mockSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.Provider);
            mockSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.Expression);
            mockSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.ElementType);
            mockSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());
            _mockContext.Setup(ctx => ctx.Rooms).Returns(mockSet.Object);
            var result = _controller.WaitForMove("NonExistingGameCode");
            Assert.IsType<NotFoundResult>(result);
        }
    }
}