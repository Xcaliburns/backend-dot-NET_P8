﻿using GpsUtil.Location;
using System.Collections.Concurrent;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private const double StatuteMilesPerNauticalMile = 1.15077945;
    private readonly int _defaultProximityBuffer = 10;
    private int _proximityBuffer;
    private readonly int _attractionProximityRange = 200;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardCentral _rewardsCentral;
    private static int count = 0;

    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral = rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    public void SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    public void SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }

  

    public async Task CalculateRewardsAsync(User user)
    {
        // Utilise Task.Run pour lancer la méthode sur un thread d'arrière-plan
        await Task.Run(() =>
        {
            lock (user)
            {
                List<VisitedLocation> userLocations = user.VisitedLocations.ToList();
                List<Attraction> attractions = _gpsUtil.GetAttractions();

                foreach (var visitedLocation in userLocations)
                {
                    foreach (var attraction in attractions)
                    {
                        if (!user.UserRewards.Any(r => r.Attraction.AttractionName == attraction.AttractionName))
                        {
                            if (NearAttraction(visitedLocation, attraction))
                            {
                                int rewardPoints = GetRewardPoints(attraction, user);
                                user.AddUserReward(new UserReward(visitedLocation, attraction, rewardPoints));
                            }
                        }
                    }
                }
            }
        });
    }


    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        Console.WriteLine(GetDistance(attraction, location));
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    // ajout de la methode  GetClosestAttractions pour les tests
    public List<Attraction> GetClosestAttractions(Locations location)
    {
        var attractions = _gpsUtil.GetAttractions();
        var closestAttractions = attractions
            .Select(attraction => new
            {
                Attraction = attraction,
                Distance = GetDistance(attraction, location)
            })
            .OrderBy(attractionWithDistance => attractionWithDistance.Distance)
            .Take(5)
            .Select(attractionWithDistance => attractionWithDistance.Attraction)
            .ToList();

        return closestAttractions;
    }

    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        return GetDistance(attraction, visitedLocation.Location) <= _proximityBuffer;
    }

    public int GetRewardPoints(Attraction attraction, User user)
    {
        return _rewardsCentral.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);
    }

    public double GetDistance(Locations loc1, Locations loc2)
    {
        double lat1 = Math.PI * loc1.Latitude / 180.0;
        double lon1 = Math.PI * loc1.Longitude / 180.0;
        double lat2 = Math.PI * loc2.Latitude / 180.0;
        double lon2 = Math.PI * loc2.Longitude / 180.0;

        double angle = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2)
                                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2));

        double nauticalMiles = 60.0 * angle * 180.0 / Math.PI;
        return StatuteMilesPerNauticalMile * nauticalMiles;
    }
}
