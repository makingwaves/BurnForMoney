using System;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Infrastructure.Persistence;
using Xunit;

namespace BurnForMoney.Functions.UnitTests.Domain
{
    public class AthleteAggregateTests : AthleteBaseTests
    {
        private const string TestFirstName = "test_first_name";
        private const string TestLastName = "test_last_name";
        private const string TestProfilePictureUrl = "https://test.com/img.png";
        private const string TestExternalId = "ex_id";
        private const string TestActivityType = "sleeping";
        private const int PositiveDistanceInMeters = 123;
        private const int PositiveMovingTimeInMinutes = 61;

        private readonly DateTime _testStartDate  = new DateTime(2019,1,1);

        [Fact]
        public async Task Cant_CreateAthlete_WithEmptyId()
        {
            var newAthleteId = Guid.Empty;
            var newExternalId = Guid.NewGuid().ToString();

            await Assert.ThrowsAsync<ArgumentNullException>("Id", ()=>
                HandleCommand(new CreateAthleteCommand(newAthleteId, newExternalId,
                    TestFirstName, TestLastName, TestProfilePictureUrl, Source.Strava)));
        }

        [Fact]
        public async Task Cant_CreateAthlete_WithNoFirstName()
        {
            var newAthleteId = Guid.NewGuid();

            await Assert.ThrowsAsync<ArgumentNullException>("firstName", ()=>
                HandleCommand(new CreateAthleteCommand(newAthleteId, null,
                    null, null, null, Source.Strava)));
        }

        [Fact(Skip = "Logic bug, currently it's possible to add two athletes with the same id")]
        public async Task Cant_CreateTwoAthletes_WithSameId()
        {
            var newAthleteId = Guid.NewGuid();
            var newExternalId = Guid.NewGuid().ToString();
            
            await HandleCommand(new CreateAthleteCommand(newAthleteId, newExternalId,
                TestFirstName, TestLastName, TestProfilePictureUrl, Source.Strava));

            await Assert.ThrowsAnyAsync<ConcurrencyException>(()=>
                HandleCommand(new CreateAthleteCommand(newAthleteId, newExternalId,
                    TestFirstName, TestLastName, TestProfilePictureUrl, Source.Strava)));
        }

        [Fact]
        public async Task Can_CreateAthlete_MinimumData()
        {
            var newAthleteId = Guid.NewGuid();

            await HandleCommand(new CreateAthleteCommand(newAthleteId, null, 
                TestFirstName, null, null, Source.None));

            var newAthlete = await GetAthleteAsync(newAthleteId);
            
            Assert.True(newAthlete.IsActive);
            Assert.Equal(newAthleteId, newAthlete.Id);
            Assert.Equal(TestFirstName, newAthlete.FirstName);
            Assert.Equal(Source.None, newAthlete.Source);
            Assert.Null(newAthlete.ExternalId);
            Assert.Null(newAthlete.LastName);
            Assert.Null(newAthlete.ProfilePictureUrl);
        }

        [Fact]
        public async Task Can_CreateNewStravaAthlete()
        {
            var newAthleteId = Guid.NewGuid();
            var newExternalId = Guid.NewGuid().ToString();

            await HandleCommand(new CreateAthleteCommand(newAthleteId, newExternalId,
                TestFirstName, TestLastName, TestProfilePictureUrl, Source.Strava));

            var newAthlete = await _athleteRepo.GetByIdAsync(newAthleteId);

            Assert.True(newAthlete.IsActive);
            Assert.Equal(newAthleteId, newAthlete.Id);
            Assert.Equal(newExternalId, newAthlete.ExternalId);
            Assert.Equal(TestFirstName, newAthlete.FirstName);
            Assert.Equal(TestLastName, newAthlete.LastName);
            Assert.Equal(TestProfilePictureUrl, newAthlete.ProfilePictureUrl);
            Assert.Equal(Source.Strava, newAthlete.Source);
        }

        [Fact]
        public async Task Cant_AddActivity_WithoutId()
        {
            var athleteId = await CreateNewAthleteAsync();

            await Assert.ThrowsAsync<ArgumentNullException>("Id", () =>
                HandleCommand(new AddActivityCommand
                {
                    Id = Guid.Empty,
                    ExternalId = TestExternalId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                    Source = Source.Strava
                }));
        }

        [Fact]
        public async Task Cant_AddActivity_ToDeactivatedAthlete()
        {
            var athleteId = await CreateNewAthleteAsync();
            await HandleCommand(new DeactivateAthleteCommand(athleteId));
            var newActivityId = Guid.NewGuid();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                HandleCommand(new AddActivityCommand {
                    Id = newActivityId,
                    ExternalId = TestExternalId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                    Source = Source.Strava
                }));
        }

        [Fact]
        public async Task Cant_AddActivity_WithNoType()
        {
            var athleteId = await CreateNewAthleteAsync();
            var newActivityId = Guid.NewGuid();

            await Assert.ThrowsAsync<ArgumentNullException>("activityType", () =>
                HandleCommand(new AddActivityCommand
                {
                    Id = newActivityId,
                    ExternalId = TestExternalId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = string.Empty,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                    Source = Source.Strava
                }));
        }

        [Fact]
        public async Task Cant_AddActivity_WithNegativeDistance()
        {
            var athleteId = await CreateNewAthleteAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                HandleCommand(new AddActivityCommand
                {
                    Id = Guid.NewGuid(),
                    ExternalId = TestExternalId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = -PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                    Source = Source.Strava
                }));
        }

        [Fact]
        public async Task Cant_AddActivity_WithZeroMovingTime()
        {
            var athleteId = await CreateNewAthleteAsync();
            const int zeroMinutes = 0;

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                HandleCommand(new AddActivityCommand
                {
                    Id = Guid.NewGuid(),
                    ExternalId = TestExternalId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = zeroMinutes,
                    Source = Source.Strava
                }));
        }

        [Fact]
        public async Task Cant_AddActivity_Before2018()
        {
            var athleteId = await CreateNewAthleteAsync();
            var startDateBefore2018 = new DateTime(2017,12,31);

            await Assert.ThrowsAsync<InvalidOperationException>(() => HandleCommand(new AddActivityCommand
            {
                Id = Guid.NewGuid(),
                ExternalId = TestExternalId,
                AthleteId = athleteId,
                StartDate = startDateBefore2018,
                ActivityType = TestActivityType,
                DistanceInMeters = PositiveDistanceInMeters,
                MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                Source = Source.Strava
            }));
        }

        [Fact]
        public async Task Cant_AddActivity_WithTheSameId()
        {
            var athleteId = await CreateNewAthleteAsync();
            var activityId = Guid.NewGuid();

            await HandleCommand(new AddActivityCommand
            {
                Id = activityId,
                ExternalId = TestExternalId,
                AthleteId = athleteId,
                StartDate = _testStartDate,
                ActivityType = TestActivityType,
                DistanceInMeters = PositiveDistanceInMeters,
                MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                Source = Source.Strava
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                HandleCommand(new AddActivityCommand
                {
                    Id = activityId,
                    ExternalId = TestExternalId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                    Source = Source.Strava
                }));
        }

        [Fact]
        public async Task Cant_AddActivity_ToNotExistingAthlete()
        {
            await Assert.ThrowsAnyAsync<Exception>(()=>
                HandleCommand(new AddActivityCommand {
                    Id = Guid.NewGuid(),
                    ExternalId = TestExternalId,
                    AthleteId = Guid.NewGuid(),
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                    Source = Source.Strava
                }));
        }

        [Fact]
        public async Task Can_AddActivity_WithZeroDistance()
        {
            var athleteId = await CreateNewAthleteAsync();
            var newActivityId = Guid.NewGuid();
            const int zeroMeters = 0;

            await HandleCommand(new AddActivityCommand {
                Id = newActivityId,
                ExternalId = TestExternalId,
                AthleteId = athleteId,
                StartDate = _testStartDate,
                ActivityType = TestActivityType,
                DistanceInMeters = zeroMeters,
                MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                Source = Source.Strava
            });
            
            var athlete = await GetAthleteAsync(athleteId);
            Assert.Single(athlete.Activities);
            Assert.Equal(newActivityId, athlete.Activities[0].Id);
            Assert.Equal(zeroMeters, athlete.Activities[0].DistanceInMeters);
        }

        [Fact]
        public async Task Can_AddActivity_ToExistingAthlete()
        {
            var athleteId = await CreateNewAthleteAsync();
            var newActivityId = Guid.NewGuid();

            await HandleCommand(new AddActivityCommand {
                Id = newActivityId,
                ExternalId = TestExternalId,
                AthleteId = athleteId,
                StartDate = _testStartDate,
                ActivityType = TestActivityType,
                DistanceInMeters = PositiveDistanceInMeters,
                MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                Source = Source.Strava
            });
            
            var athlete = await GetAthleteAsync(athleteId);
            Assert.Single(athlete.Activities);
            Assert.Equal(newActivityId, athlete.Activities[0].Id);
        }

        [Fact]
        public async Task Cant_UpdateActivity_WithoutId()
        {
            var athleteId = await CreateNewAthleteAsync();
            await Assert.ThrowsAsync<ArgumentNullException>("Id", ()=>
                HandleCommand(new UpdateActivityCommand {
                    Id = Guid.Empty,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                }));
        }

        [Fact]
        public async Task Cant_UpdateActivity_OfDeactivatedAthlete()
        {
            var athleteId = await CreateNewAthleteAsync();
            await HandleCommand(new DeactivateAthleteCommand(athleteId));
            var newActivityId = Guid.NewGuid();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                HandleCommand(new UpdateActivityCommand {
                    Id = newActivityId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                }));
        }

        [Fact]
        public async Task Cant_UpdateActivity_WithNoType()
        {
            var athleteId = await CreateNewAthleteAsync();
            var newActivityId = Guid.NewGuid();

            await Assert.ThrowsAsync<ArgumentNullException>("activityType", () =>
                HandleCommand(new UpdateActivityCommand {
                    Id = newActivityId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = string.Empty,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                }));
        }

        [Fact]
        public async Task Cant_UpdateActivity_WithNegativeDistance()
        {
            var athleteId = await CreateNewAthleteAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new UpdateActivityCommand {
                    Id = Guid.NewGuid(),
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = -PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                }));
        }

        [Fact]
        public async Task Cant_UpdateActivity_WithZeroMovingTime()
        {
            var athleteId = await CreateNewAthleteAsync();
            const int zeroMinutes = 0;

            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new UpdateActivityCommand {
                    Id = Guid.NewGuid(),
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = zeroMinutes,
                }));
        }

        [Fact]
        public async Task Cant_UpdateActivity_ToDateBefore2018()
        {
            var athleteId = await CreateNewAthleteAsync();
            var startDateBefore2018 = new DateTime(2017,12,31);

            await Assert.ThrowsAsync<InvalidOperationException>(() => HandleCommand(new UpdateActivityCommand
            {
                Id = Guid.NewGuid(),
                AthleteId = athleteId,
                StartDate = startDateBefore2018,
                ActivityType = TestActivityType,
                DistanceInMeters = PositiveDistanceInMeters,
                MovingTimeInMinutes = PositiveMovingTimeInMinutes,
            }));
        }

        [Fact]
        public async Task Cant_UpdateActivity_WhichDoesNotExist()
        {
            var athleteId = await CreateNewAthleteAsync();
            var activityId = Guid.NewGuid();

            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new UpdateActivityCommand {
                    Id = activityId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                }));
        }

        [Fact]
        public async Task Can_UpdateActivity_WhichExists()
        {
            const int updatedDistanceInMeters = PositiveDistanceInMeters + 100;
            const int updatedMovingTimeInMinutes = PositiveMovingTimeInMinutes + 60;
            const string updatedActivityType = "Walk";
            var updatedStartDate = _testStartDate.Add(TimeSpan.FromDays(1));
            var athleteId = await CreateNewAthleteAsync();
            var activityId = Guid.NewGuid();
            await HandleCommand(new AddActivityCommand {
                Id = activityId,
                ExternalId = TestExternalId,
                AthleteId = athleteId,
                StartDate = _testStartDate,
                ActivityType = TestActivityType,
                DistanceInMeters = PositiveDistanceInMeters,
                MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                Source = Source.Strava
            });
            var originalAthlete = await GetAthleteAsync(athleteId);
            var originalActivity = originalAthlete.Activities.FirstOrDefault();
            var originalActivityId = originalActivity?.Id;
            var originalActivityExternalId = originalActivity?.ExternalId;
            var originalActivitySource = originalActivity?.Source;
            var originalActivityPoints = originalActivity?.Points;
            var originalActivityCategory = originalActivity?.Category;

            await HandleCommand(new UpdateActivityCommand
            {
                Id = activityId,
                AthleteId = athleteId,
                StartDate = updatedStartDate,
                ActivityType = updatedActivityType,
                DistanceInMeters = updatedDistanceInMeters,
                MovingTimeInMinutes = updatedMovingTimeInMinutes,
            });

            var updatedAthlete = await GetAthleteAsync(athleteId);
            var updatedActivity = updatedAthlete.Activities.FirstOrDefault();
            Assert.Equal(originalActivityId, updatedActivity?.Id);
            Assert.Equal(originalActivityExternalId, updatedActivity?.ExternalId);
            Assert.Equal(updatedDistanceInMeters, updatedActivity?.DistanceInMeters);
            Assert.Equal(updatedMovingTimeInMinutes, updatedActivity?.MovingTimeInMinutes);
            Assert.Equal(updatedActivityType, updatedActivity?.ActivityType);
            Assert.Equal(updatedStartDate, updatedActivity?.StartDate);
            Assert.Equal(originalActivitySource, updatedActivity?.Source);
            Assert.NotEqual(originalActivityCategory, updatedActivity?.Category);
            Assert.NotEqual(originalActivityPoints, updatedActivity?.Points);
        }

        [Fact]
        public async Task Cant_DeleteActivity_WithoutId()
        {
            var athleteId = await CreateNewAthleteAsync();
            await Assert.ThrowsAsync<ArgumentNullException>("Id", ()=>
                HandleCommand(new DeleteActivityCommand {
                    Id = Guid.Empty,
                    AthleteId = athleteId
                }));
        }

        [Fact]
        public async Task Cant_DeleteActivity_OfDeactivatedAthlete()
        {
            var athleteId = await CreateNewAthleteAsync();
            await HandleCommand(new DeactivateAthleteCommand(athleteId));
            var newActivityId = Guid.NewGuid();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                HandleCommand(new DeleteActivityCommand {
                    Id = newActivityId,
                    AthleteId = athleteId
                }));
        }

        [Fact]
        public async Task Cant_DeleteActivity_WhichDoesNotExist()
        {
            var athleteId = await CreateNewAthleteAsync();
            var activityId = Guid.NewGuid();

            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new DeleteActivityCommand {
                    Id = activityId,
                    AthleteId = athleteId
                }));
        }

        [Fact]
        public async Task Can_DeleteActivity_WhichExists()
        {
            var athleteId = await CreateNewAthleteAsync();
            var activityId = Guid.NewGuid();
            await HandleCommand(new AddActivityCommand {
                Id = activityId,
                ExternalId = TestExternalId,
                AthleteId = athleteId,
                StartDate = _testStartDate,
                ActivityType = TestActivityType,
                DistanceInMeters = PositiveDistanceInMeters,
                MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                Source = Source.Strava
            });
            var athleteBeforeActivityDeleted = await GetAthleteAsync(athleteId);

            await HandleCommand(new DeleteActivityCommand
            {
                Id = activityId,
                AthleteId = athleteId
            });

            var athleteAfterActivityDeleted = await GetAthleteAsync(athleteId);
            Assert.Single(athleteBeforeActivityDeleted.Activities);
            Assert.Empty(athleteAfterActivityDeleted.Activities);
        }

        [Fact]
        public async Task Cant_Activate_ActiveAthlete()
        {
            var athleteId = await CreateNewAthleteAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new ActivateAthleteCommand(athleteId)));
        }

        [Fact]
        public async Task Can_Deactivate_ActiveAthlete()
        {
            var athleteId = await CreateNewAthleteAsync();
            await HandleCommand(new DeactivateAthleteCommand(athleteId));

            var athlete = await GetAthleteAsync(athleteId);
            Assert.False(athlete.IsActive);
        }

        [Fact]
        public async Task Cant_Deactivate_DeactivatedAthlete()
        {
            var athleteId = await CreateNewAthleteAsync();
            await HandleCommand(new DeactivateAthleteCommand(athleteId));

            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new DeactivateAthleteCommand(athleteId)));
        }

        [Fact]
        public async Task Can_Activate_DeactivatedAthlete()
        {
            var athleteId = await CreateNewAthleteAsync();
            await HandleCommand(new DeactivateAthleteCommand(athleteId));
            await HandleCommand(new ActivateAthleteCommand(athleteId));

            var athlete = await GetAthleteAsync(athleteId);
            Assert.True(athlete.IsActive);
        }

        [Fact]
        public async Task Can_AddActivity_WithTheSameExternalId_IfSourceIsDifferent()
        {
            var athleteId = await CreateNewAthleteAsync();
            var newActivityId = Guid.NewGuid();

            var command1 = new AddActivityCommand {
                    Id = newActivityId,
                    ExternalId = TestExternalId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                    Source = Source.Strava
                };
            var command2 = new AddActivityCommand {
                    Id = Guid.NewGuid(),
                    ExternalId = TestExternalId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                    Source = Source.None
                };

            await HandleCommand(command1);
            await HandleCommand(command2);
        }

        [Fact]
        public async Task Cant_AddActivity_WithTheSameExternalId_IfSourceIsSame()
        {
            var athleteId = await CreateNewAthleteAsync();
            var newActivityId = Guid.NewGuid();

            var command1 = new AddActivityCommand {
                    Id = newActivityId,
                    ExternalId = TestExternalId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                    Source = Source.Strava
                };
            var command2 = new AddActivityCommand {
                    Id = Guid.NewGuid(),
                    ExternalId = TestExternalId,
                    AthleteId = athleteId,
                    StartDate = _testStartDate,
                    ActivityType = TestActivityType,
                    DistanceInMeters = PositiveDistanceInMeters,
                    MovingTimeInMinutes = PositiveMovingTimeInMinutes,
                    Source = Source.Strava
                };

            await HandleCommand(command1);
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                HandleCommand(command2));
        }
    }
}
