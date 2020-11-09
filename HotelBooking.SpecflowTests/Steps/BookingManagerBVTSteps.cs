using HotelBooking.Core;
using HotelBooking.SpecflowTests.Fakes;
using Moq;
using System;
using TechTalk.SpecFlow;
using Xunit;

namespace HotelBooking.SpecflowTests.Steps
{
    [Binding]
    public class BookingManagerBVTSteps
    {
        private IBookingManager bookingManager;
        private DateTime start;
        private DateTime end;
        private static int TenDays = 10;
        private static int TwentyDays = 20;
        private static int customerId = 1;
        private static int roomID = 1;
        private static int id = 2;

        private Mock<IRepository<Booking>> mockBookingRepository;
        private Mock<IRepository<Room>> mockRoomRepository;

        private IBookingManager bookingManager_mock;

        public BookingManagerBVTSteps()
        {
            start = DateTime.Today.AddDays(TenDays);
            end = DateTime.Today.AddDays(TwentyDays);

            var bookingList = new Booking[] { new Booking() { StartDate = start.AddDays(-1), EndDate = end, RoomId = 1, CustomerId = 1, IsActive = true, Id = 1 }, new Booking() { StartDate = start, EndDate = end, RoomId = 2, CustomerId = 2, IsActive = true, Id = 2 } };
            var roomsList = new Room[] { new Room() { Description = "1", Id = 1 }, new Room() { Description = "2", Id = 2 } };

            mockBookingRepository = new Mock<IRepository<Booking>>();
            mockRoomRepository = new Mock<IRepository<Room>>();

            mockRoomRepository.Setup(x => x.GetAll()).Returns(() => roomsList);

            mockBookingRepository.Setup(x => x.GetAll()).Returns(() => bookingList);

            bookingManager_mock = new BookingManager(mockBookingRepository.Object, mockRoomRepository.Object);

            //Dates are fully occupied in the FakeBookingRepository
            IRepository<Booking> bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);
        }

        private DateTime startDateTestcase;
        private DateTime endDateTestcase;
        private bool result = false;

        [Given(@"the start date is (.*) from now")]
        public void GivenTheStartDateIsFromNow(int startDays)
        {
            start = DateTime.Now.AddDays(startDays);
        }
        
        [Given(@"the end date is (.*) from now")]
        public void GivenTheEndDateIsFromNow(int endDays)
        {
            end = DateTime.Now.AddDays(endDays);
        }
        
        [When(@"the booking is created")]
        public void WhenTheBookingIsCreated()
        {
            Booking booking = new Booking() { CustomerId = customerId, StartDate = start, EndDate = end, RoomId = roomID, Id = id };

            result = bookingManager_mock.CreateBooking(booking);
        }
        
        [Then(@"the booking should return ""(.*)""")]
        public void ThenTheBookingShouldReturn(string expectedResult)
        {
            bool expResult;
            bool.TryParse(expectedResult,out expResult);

            Assert.Equal(expResult, result);

        }
    }
}
