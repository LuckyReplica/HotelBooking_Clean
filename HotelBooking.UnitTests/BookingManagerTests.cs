using System;
using System.Collections;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private Mock<IBookingManager> bookingManager;
        private Mock<IRepository<Customer>> customerRepository;
        private Mock<IRepository<Room>> roomRepository;
        private Mock<IRepository<Booking>> bookingRepository;
        private DateTime start = DateTime.Today;
        private DateTime end = DateTime.Today;
        private static int TenDays = 10;
        private static int TwentyDays = 20;

        public BookingManagerTests()
        {
            var rooms = new List<Room>
            {
                new Room { Id=1, Description="a" },
                new Room { Id=2, Description="b" },
                new Room { Id=3, Description="c" },
            };
            var customers = new List<Customer>
            {
                new Customer{ Id=1, Name="Karsten", Email="karsten@gmial.com" },
                new Customer{ Id=2, Name="Holgert", Email="holgert@gmial.com" }
            };
            var bookings = new List<Booking>
            {
                new Booking{ Id=1, StartDate=start, EndDate=end.AddDays(12), Customer=customers[0], Room=rooms[0], IsActive=true },
                new Booking{ Id=1, StartDate=start.AddDays(5), EndDate=end.AddDays(12), Customer=customers[1], Room=rooms[2], IsActive=true }
            };

            bookingRepository = new Mock<IRepository<Booking>>(start, end);
            roomRepository = new Mock<IRepository<Room>>();
            bookingManager = new Mock<IBookingManager>(bookingRepository, roomRepository);

        }

        public static IEnumerable<object[]> FindAvailableRoom_TestCases()
        {
            DateTime start = DateTime.Today.AddDays(TenDays);
            DateTime end = DateTime.Today.AddDays(TwentyDays);

            var list = new List<object[]>();
            object[] case_NoAvailableRooms = { start, end, false };
            object[] case_HasAvailableRoom = { start.AddDays(-3), start.AddDays(-2), true };
            object[] case_StartDateOutOccupied_EndDateInOuccupied = { start.AddDays(-1), end.AddDays(-1), false };
            object[] case_StartDateBeforeOccupied_EndDateAfterOccupied = { start.AddDays(-3), end.AddDays(+2), false };

            list.Add(case_NoAvailableRooms);
            list.Add(case_HasAvailableRoom);
            list.Add(case_StartDateOutOccupied_EndDateInOuccupied);
            list.Add(case_StartDateBeforeOccupied_EndDateAfterOccupied);

            return list;
        }

        [Fact]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            DateTime date = DateTime.Today;
            Assert.Throws<ArgumentException>(() => bookingManager.FindAvailableRoom(date, date));
        }

        [Fact]
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }

        [Fact]
        public void GetFullyOccupiedDates_DoesExist_Success()
        {
            //ARRANGE
            bool isNotNull = false;

            //ACT
            isNotNull = this.bookingManager.GetFullyOccupiedDates(start, end) != null;

            //ASSERT
            Assert.True(isNotNull);
        }


        [Theory]
        [MemberData(nameof(FindAvailableRoom_TestCases))]
        public void FindAvailableRoom_IsAvailable_Success(DateTime startDate, DateTime endDate, bool expectedResult)
        {
            //ARRANGE
            //ACT
            int roomNo = this.bookingManager.FindAvailableRoom(startDate, endDate);

            //ASSERT
            Assert.Equal(expectedResult, roomNo > 0);
        }

    }
}
