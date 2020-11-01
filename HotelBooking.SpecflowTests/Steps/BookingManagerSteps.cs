using HotelBooking.Core;
using System;
using TechTalk.SpecFlow;
using HotelBooking.SpecflowTests.Fakes;
using Moq;
using Xunit;

namespace HotelBooking.SpecflowTests.Steps
{
    [Binding]
    public class BookingManagerSteps
    {
        private IBookingManager bookingManager;
        private DateTime start;
        private DateTime end;
        private static int TenDays = 10;
        private static int TwentyDays = 20;
        private static int customerId = 1;
        private static int roomID = 1;
        private static int id = 2;
        private bool result;

        private Mock<IRepository<Booking>> mockBookingRepository;
        private Mock<IRepository<Room>> mockRoomRepository;
        private Mock<IRepository<Customer>> mockCustomerRepository;
        private Mock<IBookingManager> mockBookingManager;

        private IBookingManager fakeBookingManager;

        public BookingManagerSteps()
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

        [Given(@"start date before occupied time")]
        public void GivenStartDateBeforeOccupiedTime()
        {
            start = DateTime.Today.AddDays(TenDays - 3);
        }
        
        [Given(@"end date before occupied time")]
        public void GivenEndDateBeforeOccupiedTime()
        {
            end = DateTime.Today.AddDays(TenDays - 2);
        }
        
        [When(@"creating a booking")]
        public void WhenCreatingABooking()
        {
            Booking booking = new Booking() { CustomerId = customerId, StartDate = start, EndDate = end, RoomId = roomID, Id = id };

            result = fakeBookingManager.CreateBooking(booking);
        }
        
        [Then(@"the booking should be created")]
        public void ThenTheBookingShouldBeCreated()
        {
            Assert.True(result);
        }

        [Given(@"start date after occupied time")]
        public void GivenStartDateAfterOccupiedTime()
        {
            start = DateTime.Today.AddDays(TwentyDays + 2);
        }

        [Given(@"end date after occupied time")]
        public void GivenEndDateAfterOccupiedTime()
        {
            end = DateTime.Today.AddDays(TwentyDays + 3);
        }

        [Then(@"the booking should not be created")]
        public void ThenTheBookingShouldNotBeCreated()
        {
            Assert.False(result);
        }

        [Given(@"start date in occupied time")]
        public void GivenStartDateInOccupiedTime()
        {
            start = DateTime.Today.AddDays(TenDays);
        }

        [Given(@"end date in occupied time")]
        public void GivenEndDateInOccupiedTime()
        {
            end = DateTime.Today.AddDays(TwentyDays);
        }

        [Given(@"start date after, and > end date")]
        public void GivenStartDateAfterAndEndDate()
        {
            start = DateTime.Today.AddDays(TenDays + 4);
        }

    }
}
