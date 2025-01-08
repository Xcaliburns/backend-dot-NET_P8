﻿using GpsUtil.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.Users;
using TourGuide.Utilities;

namespace TourGuideTest;

public class RewardServiceTest : IClassFixture<DependencyFixture>
{
    private readonly DependencyFixture _fixture;

    public RewardServiceTest(DependencyFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    //modification du test
    public async Task UserGetRewards()
    {
        _fixture.Initialize(0);
        var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        var attractions = await _fixture.GpsUtil.GetAttractionsAsync();
        var attraction = attractions.First();
        user.AddToVisitedLocations(new VisitedLocation(user.UserId, attraction, DateTime.Now));
        await _fixture.TourGuideService.TrackUserLocationAsync(user);
        var userRewards = user.UserRewards;
        _fixture.TourGuideService.Tracker.StopTracking();
        Assert.True(userRewards.Count == 1);
    }

    [Fact]
    //modification du test
    public async Task IsWithinAttractionProximity()
    {
        var attractions = await _fixture.GpsUtil.GetAttractionsAsync();
        var attraction = attractions.First();
        Assert.True(_fixture.RewardsService.IsWithinAttractionProximity(attraction, attraction));
    }

   // [Fact(Skip = ("Needs fixed - can throw InvalidOperationException"))]
    [Fact]
    public async Task NearAllAttractions()
    {
        _fixture.Initialize(1);
        _fixture.RewardsService.SetProximityBuffer(int.MaxValue);

        var user = _fixture.TourGuideService.GetAllUsers().First();
        await _fixture.RewardsService.CalculateRewardsAsync(user);
        var userRewards = _fixture.TourGuideService.GetUserRewards(user);
        _fixture.TourGuideService.Tracker.StopTracking();

        var attractions = await _fixture.GpsUtil.GetAttractionsAsync();
        Assert.Equal(attractions.Count, userRewards.Count);
    }

}
