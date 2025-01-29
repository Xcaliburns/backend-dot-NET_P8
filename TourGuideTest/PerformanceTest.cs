using GpsUtil.Location;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TourGuide.Utilities;
using Xunit.Abstractions;

namespace TourGuideTest
{
    public class PerformanceTest : IClassFixture<DependencyFixture>
    {
        

        private readonly DependencyFixture _fixture;

        private readonly ITestOutputHelper _output;

        public PerformanceTest(DependencyFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

     
        [Fact]
        public void HighVolumeTrackLocation()
        {
          
            _fixture.Initialize(1000);

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
           
            Parallel.ForEach(allUsers, user =>
            {
                _fixture.TourGuideService.TrackUserLocation(user);
            });

            stopWatch.Stop();
            _output.WriteLine($"highVolumeTrackLocation: Time Elapsed for getAllUsers: {stopWatch.Elapsed.TotalSeconds} seconds.");
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeTrackLocation: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");

            Assert.True(TimeSpan.FromMinutes(15).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }

     
        [Fact]
        public async Task HighVolumeGetRewardsAsync()
        {
           
            _fixture.Initialize(1000);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Attraction attraction = _fixture.GpsUtil.GetAttractions()[0];
            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            // Add visited locations for all users
            allUsers.ForEach(user =>
            {
                user.AddToVisitedLocations(new VisitedLocation(user.UserId, attraction, DateTime.Now));
            });

            
            var rewardTasks = allUsers.Select(user => _fixture.RewardsService.CalculateRewardsAsync(user)).ToArray();
            await Task.WhenAll(rewardTasks);

            // Check if all users have rewards
            foreach (var user in allUsers)
            {
                Assert.True(user.UserRewards.Count > 0, $"User {user.UserId} has no rewards.");
            }

            stopWatch.Stop();
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeGetRewards: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");
            Assert.True(TimeSpan.FromMinutes(20).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }

    }
}
