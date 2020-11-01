using HotelBooking.Core;
using HotelBooking.WebApi.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HotelBooking.SpecflowTests
{
    public class BookingsControllerTests
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

        public BookingsControllerTests()
        {
            start = DateTime.Today.AddDays(TenDays);
            end = DateTime.Today.AddDays(TwentyDays);

            var bookingList = new Booking[] { new Booking() { StartDate = start, EndDate = end } };
            var roomsList = new Room[] { new Room() { Description = "1", Id = 1 }, new Room() { Description = "2", Id = 2 } };

            mockBookingRepository = new Mock<IRepository<Booking>>();
            mockRoomRepository = new Mock<IRepository<Room>>();
            mockCustomerRepository = new Mock<IRepository<Customer>>();
            mockBookingManager = new Mock<IBookingManager>();

            //mockRoomRepository.Setup(x => x.GetAll()).Returns(() => roomsList);

            mockBookingRepository.Setup(x => x.GetAll()).Returns(() => bookingList);
            //mockBookingManager.Setup(x => x.FindAvailableRoom(start, end)).Returns(() => 1);

            controller = new BookingsController(mockBookingRepository.Object, mockRoomRepository.Object, mockCustomerRepository.Object, mockBookingManager.Object);
        }

        [Fact]
        public void GetAllBookings_ReturnsOneBooking()
        {
            //ARRANGE
            //ACT
            var bookings = controller.Get();

            //ASSERT
            Assert.Single(bookings);
        }
    }
}
