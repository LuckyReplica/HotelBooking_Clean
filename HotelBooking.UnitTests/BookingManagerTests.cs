using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HotelBooking.Core;
using HotelBooking.SpecflowTests.Fakes;
using HotelBooking.WebApi.Controllers;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace HotelBooking.SpecflowTests
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

        public BookingManagerTests()
        {
            start = DateTime.Today.AddDays(TenDays);
            end = DateTime.Today.AddDays(TwentyDays);

            var bookingList = new Booking[] { new Booking() { StartDate = start.AddDays(-1), EndDate = end, RoomId = 1, CustomerId = 1, IsActive = true, Id = 1 }, new Booking() { StartDate = start, EndDate = end, RoomId = 2, CustomerId = 2, IsActive = true, Id = 2 } };
            var roomsList = new Room[] { new Room() { Description = "1", Id = 1 }, new Room() { Description = "2", Id = 2 } };

            mockBookingRepository = new Mock<IRepository<Booking>>();
            mockRoomRepository = new Mock<IRepository<Room>>();
            mockCustomerRepository = new Mock<IRepository<Customer>>();
            mockBookingManager = new Mock<IBookingManager>();

            mockRoomRepository.Setup(x => x.GetAll()).Returns(() => roomsList);

            mockBookingRepository.Setup(x => x.GetAll()).Returns(() => bookingList);

            fakeBookingManager = new BookingManager(mockBookingRepository.Object, mockRoomRepository.Object);


            //Dates are fully occupied in the FakeBookingRepository
            IRepository<Booking> bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);

        }

        public static IEnumerable<object[]> CreateBooking_TestCases()
        {
            DateTime start = DateTime.Today.AddDays(TenDays);
            DateTime end = DateTime.Today.AddDays(TwentyDays);

            var list = new List<object[]>();
            object[] case_CreateBookingInOccupiedTime_Fail = {start, end, 1, 1, false };
            object[] case_CreateBookingInAvailableTime_Succeed = { start.AddDays(-2), start.AddDays(-1), 1, 1, true };
            object[] case_CreateBookingInAvailableRoom_Succeed = { start.AddDays(-2), end.AddDays(2), 1, 1, false };

            list.Add(case_CreateBookingInOccupiedTime_Fail);
            list.Add(case_CreateBookingInAvailableTime_Succeed);
            list.Add(case_CreateBookingInAvailableRoom_Succeed);


            return list;
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
            object[] case_IsNotFullyOccupied = { start.AddDays(-3), end, true };

            list.Add(case_IsOccupied);
            list.Add(case_IsNotOccupied);
            list.Add(case_IsNotFullyOccupied);

            return list;
        }

        #region "Moq tests"

        [Fact]
        public void GetAvailableRoomForPeriod_ReturnRoomNoTwo()
        {
            //ARRANGE
            //ACT
            var roomNo = fakeBookingManager.FindAvailableRoom(start.AddDays(-3), start.AddDays(-1));

            //ASSERT
            Assert.Equal(2, roomNo);
        }

        [Fact]
        public void GetAvailableRoomForPeriod_ReturnARoomNo()
        {
            //ARRANGE
            //ACT
            var roomNo = fakeBookingManager.FindAvailableRoom(start.AddDays(-3), start.AddDays(-2));

            //ASSERT
            Assert.True(roomNo > 0);
        }

        [Theory]
        [MemberData(nameof(CreateBooking_TestCases))]
        public void CreateBooking_IsPossibleMoq_Succeed(DateTime start, DateTime end, int roomID, int customerId, bool expectedResult)
        {
            //ARRANGE
            Booking booking = new Booking() { CustomerId = customerId, StartDate = start, EndDate =end, RoomId = roomID, Id=2 };

            //ACT
            var result = fakeBookingManager.CreateBooking(booking);

            Assert.Equal(expectedResult, result);
        }

        #endregion

        #region "Original tests"
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
        #endregion

        #region "Data-driven tests"

        [Theory]
        [MemberData(nameof(GetFullyOccupiedDates_TestCases))]
        public void GetFullyOccupiedDates_DoesExist_Success(DateTime startDate, DateTime endDate, bool expectedResult)
        {
            //ARRANGE
            //ACT
            var listDates = this.bookingManager.GetFullyOccupiedDates(startDate, endDate);

            //ASSERT
            Assert.Equal(expectedResult, listDates.Count()>0);
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

        [Theory]
        [MemberData(nameof(CreateBooking_TestCases))]
        public void CreateBooking_IsPossible_Success(DateTime start, DateTime end, int roomId, int customerId, bool expectedResult)
        {
            //ARRANGE
            var booking = new Booking { StartDate = start, EndDate = end, CustomerId = customerId, Id = 2, RoomId = roomId };

            //ACT
            bool result = this.bookingManager.CreateBooking(booking);

            //ASSERT
            Assert.Equal(expectedResult, result);
        }

        #endregion
    }
}
