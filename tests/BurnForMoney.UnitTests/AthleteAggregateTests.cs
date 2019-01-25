using System;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Infrastructure.Persistence;
using Xunit;

namespace BurnForMoney.UnitTests
{
    public class AthleteAggregateTests : AthleteBaseTests
    {
               
        [Fact]
        public async Task Can_CreateNewStravaAthlete()
        {
            var newAthleteId = Guid.NewGuid();
            var newExternalId = Guid.NewGuid().ToString();

            const string FirstName = "test_first_name";
            const string LastName = "test_last_name";
            const string ProfilePictureUrl = "https://test.com/img.png";

            await HandleCommand(new CreateAthleteCommand(newAthleteId, newExternalId, 
                FirstName, LastName, ProfilePictureUrl, Source.Strava));

            var newAthlete = await _athleteRepo.GetByIdAsync(newAthleteId);
            
            Assert.True(newAthlete.IsActive);
            Assert.Equal(newAthleteId, newAthlete.Id);
            Assert.Equal(newExternalId, newAthlete.ExternalId);
            Assert.Equal(FirstName, newAthlete.FirstName);
            Assert.Equal(LastName, newAthlete.LastName);
            Assert.Equal(ProfilePictureUrl, newAthlete.ProfilePictureUrl);
            Assert.Equal(Source.Strava, newAthlete.Source);
        }

        [Fact]
        public async Task Can_CreateAthlete_MinimumData()
        {
            var newAthleteId = Guid.NewGuid();
            const string FirstName = "test_first_name";
            
            await HandleCommand(new CreateAthleteCommand(newAthleteId, null, 
                FirstName, null, null, Source.None));

            var newAthlete = await GetAthleteAsync(newAthleteId);
            
            Assert.True(newAthlete.IsActive);
            Assert.Equal(newAthleteId, newAthlete.Id);
            Assert.Equal(FirstName, newAthlete.FirstName);
            Assert.Equal(Source.None, newAthlete.Source);
            Assert.Null(newAthlete.ExternalId);
            Assert.Null(newAthlete.LastName);
            Assert.Null(newAthlete.ProfilePictureUrl);
        }

        // [Fact] // Actually you can !
        // public async Task Cant_CreateTwoAthletes_WithSameId()
        // {
        //     var newAthleteId = Guid.NewGuid();
        //     var newExternalId = Guid.NewGuid().ToString();
            
        //     await HandleCommand(new CreateAthleteCommand(newAthleteId, newExternalId,
        //         "test_first_name", "test_last_name", "https://test.com/img.png", Source.Strava));

        //     await Assert.ThrowsAnyAsync<ConcurrencyException>(()=> 
        //         HandleCommand(new CreateAthleteCommand(newAthleteId, newExternalId,
        //         "test_first_name", "test_last_name", "https://test.com/img.png", Source.Strava)));
        // }

        [Fact]
        public async Task Cant_CreateAthlete_WithEmptyId()
        {
            var newAthleteId = Guid.Empty;
            var newExternalId = Guid.NewGuid().ToString();
            
            await Assert.ThrowsAsync<ArgumentNullException>(()=> 
                HandleCommand(new CreateAthleteCommand(newAthleteId, newExternalId,
                "test_first_name", "test_last_name", "https://test.com/img.png", Source.Strava)));
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
            await HandleCommand(new AddActivityCommand {
                Id = Guid.NewGuid(),
                ExternalId = "ex_id",
                AthleteId = athleteId,
                StartDate = new DateTime(2019,1,1),
                ActivityType = "sleeping",
                DistanceInMeters = 123,
                MovingTimeInMinutes = 61,
                Source = Source.Strava
            });
            
            var athlete = await GetAthleteAsync(athleteId);
            Assert.Single(athlete.Activities);
        }

        [Fact]
        public async Task Cant_AddActivity_ToNotExistingAthlete()
        {
            await Assert.ThrowsAnyAsync<Exception>(()=>
                HandleCommand(new AddActivityCommand {
                    Id = Guid.NewGuid(),
                    ExternalId = "ex_id",
                    AthleteId = Guid.NewGuid(),
                    StartDate = new DateTime(2019,1,1),
                    ActivityType = "sleeping",
                    DistanceInMeters = 123,
                    MovingTimeInMinutes = 61,
                    Source = Source.Strava
            }));
        }

         [Fact]
        public async Task Can_AddActivity_WithZeroDistance()
        {
            var athleteId = await CreateNewAthleteAsync();
            await HandleCommand(new AddActivityCommand {
                Id = Guid.NewGuid(),
                ExternalId = "ex_id",
                AthleteId = athleteId,
                StartDate = new DateTime(2019,1,1),
                ActivityType = "sleeping",
                DistanceInMeters = 0,
                MovingTimeInMinutes = 61,
                Source = Source.Strava
            });
            
            var athlete = await GetAthleteAsync(athleteId);
            Assert.Single(athlete.Activities);
        }

        [Fact]
        public async Task Cant_AddActivity_WithoutId()
        {
            var athleteId = await CreateNewAthleteAsync();
            
            await Assert.ThrowsAsync<ArgumentNullException>(()=>
                HandleCommand(new AddActivityCommand {
                    Id = Guid.Empty,
                    ExternalId = "ex_id",
                    AthleteId = athleteId,
                    StartDate = new DateTime(2019,1,1),
                    ActivityType = "sleeping",
                    DistanceInMeters = 123,
                    MovingTimeInMinutes = 61,
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
                    ExternalId = "ex_id",
                    AthleteId = athleteId,
                    StartDate = new DateTime(2019,1,1),
                    ActivityType = "sleeping",
                    DistanceInMeters = -123,
                    MovingTimeInMinutes = 61,
                    Source = Source.Strava
            }));   
        }

        [Fact]
        public async Task Cant_AddActivity_WithZeroMovingTime()
        {
            var athleteId = await CreateNewAthleteAsync();
            
            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new AddActivityCommand {
                    Id = Guid.NewGuid(),
                    ExternalId = "ex_id",
                    AthleteId = athleteId,
                    StartDate = new DateTime(2019,1,1),
                    ActivityType = "sleeping",
                    DistanceInMeters = 123,
                    MovingTimeInMinutes = 0,
                    Source = Source.Strava
            }));   
        }

         [Fact]
        public async Task Cant_AddActivity_Before2018()
        {
            var athleteId = await CreateNewAthleteAsync();
            
            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new AddActivityCommand {
                    Id = Guid.NewGuid(),
                    ExternalId = "ex_id",
                    AthleteId = athleteId,
                    StartDate = new DateTime(2017,12,31),
                    ActivityType = "sleeping",
                    DistanceInMeters = 123,
                    MovingTimeInMinutes = 60,
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
                ExternalId = "ex_id",
                AthleteId = athleteId,
                StartDate = new DateTime(2019,1,1),
                ActivityType = "sleeping",
                DistanceInMeters = 123,
                MovingTimeInMinutes = 61,
                Source = Source.Strava
            });

            await Assert.ThrowsAsync<InvalidOperationException>(()=>
                HandleCommand(new AddActivityCommand {
                    Id = activityId,
                    ExternalId = "ex_id",
                    AthleteId = athleteId,
                    StartDate = new DateTime(2019,1,1),
                    ActivityType = "sleeping",
                    DistanceInMeters = 123,
                    MovingTimeInMinutes = 61,
                    Source = Source.Strava
            }));   
        }
    }
}
