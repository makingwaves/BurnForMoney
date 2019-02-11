using System;
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
                HandleCommand(new AddActivityCommand {
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
        public async Task Cant_AddActivity_WithoutId()
        {
            var athleteId = await CreateNewAthleteAsync();
            
            await Assert.ThrowsAsync<ArgumentNullException>("Id", ()=>
                HandleCommand(new AddActivityCommand {
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
        public async Task Cant_AddActivity_WithNegativeDistance()
        {
            var athleteId = await CreateNewAthleteAsync();
            
            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new AddActivityCommand {
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

            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new AddActivityCommand {
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

            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new AddActivityCommand {
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
    }
}
