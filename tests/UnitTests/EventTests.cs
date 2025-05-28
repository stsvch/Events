using Events.Domain.Entities;
using Events.Domain.Exceptions;
using Events.Infrastructure.Persistence;
using Events.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.UnitTests
{
    public class EventTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void Constructor_Should_Create_Event_When_Valid_Data()
        {
            // Arrange
            var id = Guid.NewGuid();
            var title = "Conference";
            var description = "Conference";
            var date = DateTimeOffset.UtcNow.AddDays(10);
            var venue = "Main Hall";
            var categoryId = Guid.NewGuid();
            var capacity = 10;

            // Act
            var ev = new Event(id, title, description, date, venue, categoryId, capacity);

            // Assert
            Assert.Equal(id, ev.Id);
            Assert.Equal(title, ev.Title);
            Assert.Equal(description, ev.Description);
            Assert.Equal(date, ev.Date);
            Assert.Equal(venue, ev.Venue);
            Assert.Equal(categoryId, ev.CategoryId);
            Assert.Equal(capacity, ev.Capacity);
            Assert.Empty(ev.Participants);
        }

        [Fact]
        public void Constructor_ShouldThrow_When_Title_Is_Empty()
        {
            Assert.Throws<InvariantViolationException>(() =>
                new Event(Guid.NewGuid(), "", "desc", DateTimeOffset.UtcNow, "venue", Guid.NewGuid(), 10)
            );
        }

        [Fact]
        public void Constructor_ShouldThrow_When_Description_Is_Empty()
        {
            Assert.Throws<InvariantViolationException>(() =>
                new Event(Guid.NewGuid(), "Title", "", DateTimeOffset.UtcNow, "venue", Guid.NewGuid(), 10)
            );
        }

        [Fact]
        public void Constructor_ShouldThrow_When_Capacity_Is_Non_Positive()
        {
            Assert.Throws<InvariantViolationException>(() =>
                new Event(Guid.NewGuid(), "Title", "Desc", DateTimeOffset.UtcNow, "venue", Guid.NewGuid(), 0)
            );
        }

        [Fact]
        public void RemoveParticipant_Should_Remove_If_Exists()
        {
            // Arrange
            var ev = new Event(Guid.NewGuid(), "Event", "desc", DateTimeOffset.UtcNow, "venue", Guid.NewGuid(), 2);
            var participantId = Guid.NewGuid();
            ev.AddParticipant(participantId);

            // Act
            ev.RemoveParticipant(participantId);

            // Assert
            Assert.Empty(ev.Participants);
        }


        [Fact]
        public void AddParticipant_Should_Add_When_Space_Available_And_Not_Registered()
        {
            // Arrange
            var ev = new Event(Guid.NewGuid(), "Event", "desc", DateTimeOffset.UtcNow, "venue", Guid.NewGuid(), 2);
            var participantId = Guid.NewGuid();

            // Act
            ev.AddParticipant(participantId);

            // Assert
            Assert.Single(ev.Participants);
            Assert.Equal(participantId, ev.Participants.First().ParticipantId);
        }

        [Fact]
        public void RemoveParticipant_ShouldThrow_When_Not_Found()
        {
            // Arrange
            var ev = new Event(Guid.NewGuid(), "Event", "desc", DateTimeOffset.UtcNow, "venue", Guid.NewGuid(), 2);

            // Act & Assert
            Assert.Throws<EntityNotFoundException>(() => ev.RemoveParticipant(Guid.NewGuid()));
        }

        [Fact]
        public void UpdateDetails_Should_Update_Event_Properties()
        {
            // Arrange
            var ev = new Event(Guid.NewGuid(), "OldTitle", "OldDesc", DateTimeOffset.UtcNow, "OldVenue", Guid.NewGuid(), 10);

            var newTitle = "NewTitle";
            var newDesc = "NewDesc";
            var newDate = DateTimeOffset.UtcNow.AddMonths(1);
            var newVenue = "NewVenue";
            var newCategoryId = Guid.NewGuid();
            var newCapacity = 20;

            // Act
            ev.UpdateDetails(newTitle, newDesc, newDate, newVenue, newCategoryId, newCapacity);

            // Assert
            Assert.Equal(newTitle, ev.Title);
            Assert.Equal(newDesc, ev.Description);
            Assert.Equal(newDate, ev.Date);
            Assert.Equal(newVenue, ev.Venue);
            Assert.Equal(newCategoryId, ev.CategoryId);
            Assert.Equal(newCapacity, ev.Capacity);
        }

        [Fact]
        public void UpdateDetails_ShouldThrow_If_Capacity_Less_Than_Participants()
        {
            // Arrange
            var ev = new Event(Guid.NewGuid(), "Event", "desc", DateTimeOffset.UtcNow, "venue", Guid.NewGuid(), 2);
            ev.AddParticipant(Guid.NewGuid());
            ev.AddParticipant(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<InvariantViolationException>(() =>
                ev.UpdateDetails("Title", "Desc", DateTimeOffset.UtcNow, "venue", Guid.NewGuid(), 1));
        }



        [Fact]
        public void AddParticipant_ShouldThrow_When_Already_Registered()
        {
            // Arrange
            var ev = new Event(Guid.NewGuid(), "Event", "desc", DateTimeOffset.UtcNow, "venue", Guid.NewGuid(), 2);
            var participantId = Guid.NewGuid();
            ev.AddParticipant(participantId);

            // Act & Assert
            Assert.Throws<InvariantViolationException>(() => ev.AddParticipant(participantId));
        }


        [Fact]
        public async Task AddAsync_Should_Add_Event()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EventRepository(context);

            var @event = new Event(
                Guid.NewGuid(),
                "Title",
                "Description",
                DateTimeOffset.UtcNow,
                "Venue",
                Guid.NewGuid(),
                10
            );

            // Act
            await repository.AddAsync(@event);

            // Assert
            Assert.Single(context.Events);
            Assert.Equal("Title", context.Events.First().Title);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Event()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EventRepository(context);

            var id = Guid.NewGuid();
            var @event = new Event(
                id,
                "Title2",
                "Description2",
                DateTimeOffset.UtcNow,
                "Venue2",
                Guid.NewGuid(),
                15
            );

            context.Events.Add(@event);
            await context.SaveChangesAsync();

            // Act
            var found = await repository.GetByIdAsync(id);

            // Assert
            Assert.NotNull(found);
            Assert.Equal("Title2", found.Title);
        }
    }
}
