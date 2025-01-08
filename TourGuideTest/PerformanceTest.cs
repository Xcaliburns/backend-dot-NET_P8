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
        /*
         * Note on performance improvements:
         * 
         * The number of generated users for high-volume tests can be easily adjusted using this method:
         * 
         *_fixture.Initialize(100000); (for example)
         * 
         * 
         * These tests can be modified to fit new solutions, as long as the performance metrics at the end of the tests remain consistent.
         * 
         * These are the performance metrics we aim to achieve:
         * 
         * highVolumeTrackLocation: 100,000 users within 15 minutes:
         * Assert.True(TimeSpan.FromMinutes(15).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
         *
         * highVolumeGetRewards: 100,000 users within 20 minutes:
         * Assert.True(TimeSpan.FromMinutes(20).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        */

        private readonly DependencyFixture _fixture;

        private readonly ITestOutputHelper _output;

        public PerformanceTest(DependencyFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

       // [Fact(Skip = ("Delete Skip when you want to pass the test"))]
        [Fact]
        public async Task HighVolumeTrackLocation()
        {
            // On peut ici augmenter le nombre d'utilisateurs pour tester les performances
            _fixture.Initialize(100);

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //remplacement du foreach par un Parallel.ForEach pour améliorer les performances
            var tasks = allUsers.Select(user => _fixture.TourGuideService.TrackUserLocationAsync(user)).ToArray();
            await Task.WhenAll(tasks);
            _output.WriteLine($"highVolumeTrackLocation: Time Elapsed TrackUserLocationAsync: {stopWatch.Elapsed.TotalSeconds} seconds.");

            stopWatch.Stop();
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeTrackLocation: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");

            Assert.True(TimeSpan.FromMinutes(15).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }

        // [Fact(Skip = ("Delete Skip when you want to pass the test"))]
        [Fact]
        public async Task HighVolumeGetRewards()
        {
            // On peut ici augmenter le nombre d'utilisateurs pour tester les performances
            _fixture.Initialize(10000);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Appel de la méthode asynchrone GetAttractionsAsync pour obtenir la liste des attractions
            List<Attraction> attractions = await _fixture.GpsUtil.GetAttractionsAsync();
            _output.WriteLine($"highVolumeGetRewards GetAttractionsAsync(): Time Elapsed : {stopWatch.Elapsed.TotalSeconds} seconds.");
            // Sélection de la première attraction de la liste
            Attraction attraction = attractions[0];
            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();
            _output.WriteLine($"highVolumeGetRewards GetAllUsers(): Time Elapsed : {stopWatch.Elapsed.TotalSeconds} seconds.");

            allUsers.ForEach(u => u.AddToVisitedLocations(new VisitedLocation(u.UserId, attraction, DateTime.Now)));
            _output.WriteLine($"highVolumeGetRewards AddToVisitedLocations: Time Elapsed : {stopWatch.Elapsed.TotalSeconds} seconds.");

            // Utilisation de la méthode asynchrone pour améliorer les performances
            //var tasks = allUsers.Select(u => _fixture.RewardsService.CalculateRewardsAsync(u)).ToArray();
            //await Task.WhenAll(tasks);
            //allUsers.ForEach(u => _fixture.RewardsService.CalculateRewardsAsync(u));
            foreach (var user in allUsers)
            {
                await _fixture.RewardsService.CalculateRewardsAsync(user);
            }
            _output.WriteLine($"highVolumeGetRewards CalculateRewardsAsync: Time Elapsed : {stopWatch.Elapsed.TotalSeconds} seconds.");

            foreach (var user in allUsers)
            {
                Assert.True(user.UserRewards.Count > 0);
            }
            stopWatch.Stop();
            _fixture.TourGuideService.Tracker.StopTracking();

           
            _output.WriteLine($"highVolumeGetRewards: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");

            Assert.True(TimeSpan.FromMinutes(20).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }
    }
}
