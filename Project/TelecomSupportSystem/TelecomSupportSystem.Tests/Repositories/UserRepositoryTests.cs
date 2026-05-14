using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;

namespace TelecomSupportSystem.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            // Arrange
            using var context = CreateDbContext();
            var user = new User
            {
                UserId = 1,
                Email = "test@example.com",
                PasswordHash = "hash",
                FirstName = "Test",
                LastName = "User",
                Role = Role.CLIENT,
                Location = Location.SARAJEVO
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetByEmailAsync("test@example.com");

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("test@example.com");
            result.FirstName.Should().Be("Test");
            result.LastName.Should().Be("User");
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAvailableAgentsByTeamIdAsync_ShouldReturnOnlyAvailableAgents()
        {
            // Arrange
            using var context = CreateDbContext();
            
            var team = new Team { TeamId = 1, TeamName = "Team A", TeamStatus = TeamStatus.ACTIVE, TeamType = TeamType.AGENTS };
            context.Teams.Add(team);

            context.Users.AddRange(
                new User
                {
                    UserId = 1,
                    Email = "agent1@example.com",
                    PasswordHash = "hash",
                    FirstName = "Agent",
                    LastName = "One",
                    Role = Role.AGENT,
                    TeamId = 1,
                    Location = Location.SARAJEVO,
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE
                },
                new User
                {
                    UserId = 2,
                    Email = "agent2@example.com",
                    PasswordHash = "hash",
                    FirstName = "Agent",
                    LastName = "Two",
                    Role = Role.AGENT,
                    TeamId = 1,
                    Location = Location.SARAJEVO,
                    AvailabilityStatus = AvailabilityStatus.BUSY
                },
                new User
                {
                    UserId = 3,
                    Email = "agent3@example.com",
                    PasswordHash = "hash",
                    FirstName = "Agent",
                    LastName = "Three",
                    Role = Role.CLIENT,
                    TeamId = 1,
                    Location = Location.SARAJEVO,
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE
                },
                new User
                {
                    UserId = 4,
                    Email = "agent4@example.com",
                    PasswordHash = "hash",
                    FirstName = "Agent",
                    LastName = "Four",
                    Role = Role.AGENT,
                    TeamId = 2,
                    Location = Location.SARAJEVO,
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE
                }
            );
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetAvailableAgentsByTeamIdAsync(1);

            // Assert
            result.Should().HaveCount(1);
            result.First().UserId.Should().Be(1);
            result.First().Role.Should().Be(Role.AGENT);
            result.First().AvailabilityStatus.Should().Be(AvailabilityStatus.AVAILABLE);
        }

        [Fact]
        public async Task GetAvailableAgentsForForwardingAsync_ShouldExcludeSpecifiedUser()
        {
            // Arrange
            using var context = CreateDbContext();
            
            context.Users.AddRange(
                new User
                {
                    UserId = 1,
                    Email = "agent1@example.com",
                    PasswordHash = "hash",
                    FirstName = "Agent",
                    LastName = "One",
                    Role = Role.AGENT,
                    Location = Location.SARAJEVO,
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                    AccountStatus = AccountStatus.ACTIVE
                },
                new User
                {
                    UserId = 2,
                    Email = "agent2@example.com",
                    PasswordHash = "hash",
                    FirstName = "Agent",
                    LastName = "Two",
                    Role = Role.AGENT,
                    Location = Location.SARAJEVO,
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                    AccountStatus = AccountStatus.ACTIVE
                }
            );
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetAvailableAgentsForForwardingAsync(1);

            // Assert
            result.Should().HaveCount(1);
            result.First().UserId.Should().Be(2);
        }

        [Fact]
        public async Task GetAvailableAgentsForForwardingAsync_ShouldReturnOnlyActiveAndAvailable()
        {
            // Arrange
            using var context = CreateDbContext();
            
            context.Users.AddRange(
                new User
                {
                    UserId = 1,
                    Email = "agent1@example.com",
                    PasswordHash = "hash",
                    FirstName = "Agent",
                    LastName = "One",
                    Role = Role.AGENT,
                    Location = Location.SARAJEVO,
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                    AccountStatus = AccountStatus.ACTIVE
                },
                new User
                {
                    UserId = 2,
                    Email = "agent2@example.com",
                    PasswordHash = "hash",
                    FirstName = "Agent",
                    LastName = "Two",
                    Role = Role.AGENT,
                    Location = Location.SARAJEVO,
                    AvailabilityStatus = AvailabilityStatus.BUSY,
                    AccountStatus = AccountStatus.ACTIVE
                },
                new User
                {
                    UserId = 3,
                    Email = "agent3@example.com",
                    PasswordHash = "hash",
                    FirstName = "Agent",
                    LastName = "Three",
                    Role = Role.AGENT,
                    Location = Location.SARAJEVO,
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                    AccountStatus = AccountStatus.INACTIVE
                }
            );
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetAvailableAgentsForForwardingAsync(99);

            // Assert
            result.Should().HaveCount(1);
            result.First().UserId.Should().Be(1);
        }

        [Fact]
        public async Task GetTechniciansByLocationAsync_ShouldReturnOnlyTechniciansAtLocation()
        {
            // Arrange
            using var context = CreateDbContext();
            
            context.Users.AddRange(
                new User
                {
                    UserId = 1,
                    Email = "tech1@example.com",
                    PasswordHash = "hash",
                    FirstName = "Technician",
                    LastName = "One",
                    Role = Role.TECHNICIAN,
                    Location = Location.SARAJEVO,
                    AccountStatus = AccountStatus.ACTIVE
                },
                new User
                {
                    UserId = 2,
                    Email = "tech2@example.com",
                    PasswordHash = "hash",
                    FirstName = "Technician",
                    LastName = "Two",
                    Role = Role.TECHNICIAN,
                    Location = Location.TUZLA,
                    AccountStatus = AccountStatus.ACTIVE
                },
                new User
                {
                    UserId = 3,
                    Email = "agent1@example.com",
                    PasswordHash = "hash",
                    FirstName = "Agent",
                    LastName = "One",
                    Role = Role.AGENT,
                    Location = Location.SARAJEVO,
                    AccountStatus = AccountStatus.ACTIVE
                }
            );
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetTechniciansByLocationAsync(Location.SARAJEVO);

            // Assert
            result.Should().HaveCount(1);
            result.First().UserId.Should().Be(1);
            result.First().Role.Should().Be(Role.TECHNICIAN);
            result.First().Location.Should().Be(Location.SARAJEVO);
        }

        [Fact]
        public async Task GetTechniciansByLocationAsync_ShouldIncludeTicketAssignments()
        {
            // Arrange
            using var context = CreateDbContext();
            
            var user = new User
            {
                UserId = 1,
                Email = "tech1@example.com",
                PasswordHash = "hash",
                FirstName = "Technician",
                LastName = "One",
                Role = Role.TECHNICIAN,
                Location = Location.SARAJEVO,
                AccountStatus = AccountStatus.ACTIVE
            };
            
            var ticket = new Ticket
            {
                TicketId = 1,
                Title = "Test Ticket",
                Description = "Test",
                CreatorId = 2,
                Status = TicketStatus.OPEN,
                Priority = Priority.LOW,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow
            };
            
            context.Users.Add(user);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var ticketUser = new TicketUser
            {
                TicketId = 1,
                UserId = 1,
                AssignmentType = AssignmentType.AUTOMATIC
            };
            context.TicketUsers.Add(ticketUser);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetTechniciansByLocationAsync(Location.SARAJEVO);

            // Assert
            result.Should().HaveCount(1);
            var tech = result.First();
            tech.TicketAssignments.Should().HaveCount(1);
            tech.TicketAssignments.First().Ticket.Should().NotBeNull();
        }
    }
}
