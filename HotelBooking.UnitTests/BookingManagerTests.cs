using System;
using System.Collections;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        private DateTime start;
        private DateTime end;
        private static int TenDays = 10;
        private static int TwentyDays = 20;

        public BookingManagerTests(){
            start = DateTime.Today.AddDays(TenDays);
            end = DateTime.Today.AddDays(TwentyDays);
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
