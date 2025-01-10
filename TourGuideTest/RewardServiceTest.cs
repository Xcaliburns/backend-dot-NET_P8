using GpsUtil.Location;
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
    public async void UserGetRewards()
    {
        _fixture.Initialize(0);
        var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        var attraction = _fixture.GpsUtil.GetAttractions().First();
        user.AddToVisitedLocations(new VisitedLocation(user.UserId, attraction, DateTime.Now));
        _fixture.TourGuideService.TrackUserLocation(user); // Ensure this method is awaited
        await _fixture.RewardsService.CalculateRewardsAsync(user); // Calculate rewards for the user
        var userRewards = user.UserRewards;
        _fixture.TourGuideService.Tracker.StopTracking();
        Assert.True(userRewards.Count == 1);
    }

    [Fact]
    public void IsWithinAttractionProximity()
    {
        var attraction = _fixture.GpsUtil.GetAttractions().First();
        Assert.True(_fixture.RewardsService.IsWithinAttractionProximity(attraction, attraction));
    }

   // [Fact(Skip = ("Needs fixed - can throw InvalidOperationException"))]
    [Fact]
    //ajout de await
    public async Task NearAllAttractions()
    {
        _fixture.Initialize(1);
        _fixture.RewardsService.SetProximityBuffer(int.MaxValue);

        var user = _fixture.TourGuideService.GetAllUsers().First();
        await _fixture.RewardsService.CalculateRewardsAsync(user);
        var userRewards = _fixture.TourGuideService.GetUserRewards(user);
        _fixture.TourGuideService.Tracker.StopTracking();

        Assert.Equal(_fixture.GpsUtil.GetAttractions().Count, userRewards.Count);
    }

}
