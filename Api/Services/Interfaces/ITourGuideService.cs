﻿using GpsUtil.Location;

using TourGuide.Users;
using TourGuide.Utilities;
using TripPricer;

namespace TourGuide.Services.Interfaces
{
    public interface ITourGuideService
    {
        Tracker Tracker { get; }
        void AddUser(User user);
        List<User> GetAllUsers();
        Task <List<Attraction>> GetNearByAttractionsAsync(VisitedLocation visitedLocation);
        List<Provider> GetTripDeals(User user);
        User GetUser(string userName);
        Task<VisitedLocation> GetUserLocationAsync(User user); // Modification vers async
        List<UserReward> GetUserRewards(User user);
        Task<VisitedLocation> TrackUserLocationAsync(User user); // Modification vers async
    }
}