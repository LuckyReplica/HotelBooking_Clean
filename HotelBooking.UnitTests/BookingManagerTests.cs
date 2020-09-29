using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using HotelBooking.WebApi.Controllers;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        private DateTime start;
        private DateTime end;
        private static int TenDays = 10;
        private static int TwentyDays = 20;

        private Mock<IRepository<Booking>> mockBookingRepository;
        private Mock<IRepository<Room>> mockRoomRepository;
        private Mock<IRepository<Customer>> mockCustomerRepository;
        private Mock<IBookingManager> mockBookingManager;

        private IBookingManager fakeBookingManager;
        private BookingsController controller;

        public BookingManagerTests()
        {
            start = DateTime.Today.AddDays(TenDays);
            end = DateTime.Today.AddDays(TwentyDays);

            var bookingList = new Booking[] { new Booking() { StartDate = start, EndDate = end } };
            var roomsList = new Room[] { new Room() { Description = "1", Id = 1 }, new Room() { Description = "2", Id = 2 } };

            mockBookingRepository = new Mock<IRepository<Booking>>();
            mockRoomRepository = new Mock<IRepository<Room>>();
            mockCustomerRepository = new Mock<IRepository<Customer>>();
            mockBookingManager = new Mock<IBookingManager>();

            mockRoomRepository.Setup(x => x.GetAll()).Returns(() => roomsList);

            mockBookingRepository.Setup(x => x.GetAll()).Returns(() => bookingList);
            mockBookingManager.Setup(x => x.FindAvailableRoom(start, end)).Returns(() => 1);

            controller = new BookingsController(mockBookingRepository.Object, mockRoomRepository.Object, mockCustomerRepository.Object, mockBookingManager.Object);
            fakeBookingManager = mockBookingManager.Object;



            IRepository<Booking> bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);



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

        public static IEnumerable<object[]> GetFullyOccupiedDates_TestCases()
        {
            DateTime start = DateTime.Today.AddDays(TenDays);
            DateTime end = DateTime.Today.AddDays(TwentyDays);
            var list = new List<object[]>();

            object[] case_IsOccupied = { start, end, true };
            object[] case_IsNotOccupied = { start.AddDays(-3), start.AddDays(-2), false };
            object[] case_IsNotFullyOccupied = { start.AddDays(-3), end, false };

            list.Add(case_IsOccupied);
            list.Add(case_IsNotOccupied);
            list.Add(case_IsNotFullyOccupied);

            return list;
        }

        #region "Moq tests"

        [Fact]
        public void GetAllBookings_ReturnsOneBooking()
        {
            //ARRANGE
            //ACT
            var bookings = controller.Get();

            //ASSERT
            Assert.Single(bookings);
        }

        [Fact]
        public void GetAllAvailableRoomForPeriod_ReturnsARoomNo()
        {
            //ARRANGE
            //ACT
            var roomNo = fakeBookingManager.FindAvailableRoom(start, end);

            //ASSERT
            Assert.Equal(1, roomNo);
        }

        #endregion


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


        #region "Data-driven tests"

        [Theory]
        [MemberData(nameof(GetFullyOccupiedDates_TestCases))]
        public void GetFullyOccupiedDates_DoesExist_Success(DateTime startDate, DateTime endDate, bool expectedResult)
        {
            //ARRANGE
            bool isNotNull = false;

            //ACT
            isNotNull = this.bookingManager.GetFullyOccupiedDates(startDate, endDate) != null;

            //ASSERT
            Assert.Equal(expectedResult, isNotNull);
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

        #endregion






    }
}
