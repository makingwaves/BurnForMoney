﻿using System;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using Xunit;

namespace BurnForMoney.UnitTests
{
    public class StravaPointsCalculationTests : AthleteBaseTests
    {
        private Guid _athleteId;

        public StravaPointsCalculationTests()
        {
            _athleteId = CreateNewAthleteAsync().Result;
        }

        [Theory]
        [InlineData("Walk")]
        [InlineData("Ride")]        
        [InlineData("EBikeRide")]
        [InlineData("Handcycle")]
        [InlineData("VirtualRide")]
        public async Task CheckDistanceBasedPoints_x1(string sportType)
        {
            await RecordActivity(sportType, 1200, 100);
            await AssertActivityPoints(1.2);

            await RecordActivity(sportType, 1000, 1);
            await AssertActivityPoints(1.0);
        }

        [Theory]
        [InlineData("Run")]
        [InlineData("Elliptical")]
        [InlineData("VirtualRun")]
        public async Task CheckDistanceBasedPoints_x2(string sportType)
        {
            await RecordActivity(sportType, 1500, 100);
            await AssertActivityPoints(3.0);

            await RecordActivity(sportType, 1000, 1);
            await AssertActivityPoints(2.0);

        }

        [Theory]
        [InlineData("Snowboard")]
        [InlineData("AlpineSki")]
        [InlineData("BackcountrySki")]
        [InlineData("IceSkate")]
        [InlineData("NordicSki")]
        [InlineData("Canoeing")]
        [InlineData("Kayaking")]
        [InlineData("Kitesurf")]
        [InlineData("Rowing")]
        [InlineData("StandUpPaddling")]
        [InlineData("Surfing")]
        [InlineData("Swim")]
        [InlineData("Windsurf")]
        [InlineData("Crossfit")]
        [InlineData("WeightTraining")]
        [InlineData("Hike")]
        [InlineData("RockClimbing")]
        [InlineData("Snowshoe")]
        [InlineData("InlineSkate")]
        [InlineData("RollerSki")]
        [InlineData("StairStepper")]
        [InlineData("Wheelchair")]
        [InlineData("Workout")]
        [InlineData("Yoga")]
        public async Task CheckTimeBasedPoints(string sportType)
        {
            await RecordActivity(sportType, 0, 25);
            await AssertActivityPoints(2.5);

            await RecordActivity(sportType, 1000, 25);
            await AssertActivityPoints(2.5);
        }

        private async Task AssertActivityPoints(double expectedAmount)
        {
            Assert.Equal(expectedAmount, (await GetAthleteAsync(_athleteId)).Activities.Last().Points);
        }

        private async Task RecordActivity(string type, double distance, double duration)
        {
            await HandleCommand(new AddActivityCommand {
                Id = Guid.NewGuid(),
                ExternalId = "ex_id",
                AthleteId = (await GetAthleteAsync(_athleteId)).Id,
                StartDate = new DateTime(2018,1,1),
                ActivityType = type,
                DistanceInMeters = distance,
                MovingTimeInMinutes = duration,
                Source = Source.Strava
            });
        }
    }
}