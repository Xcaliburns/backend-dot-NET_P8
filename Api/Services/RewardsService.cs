using GpsUtil.Location;
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
        _rewardsCentral =rewardCentral;
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

    //public async Task CalculateRewardsAsync(User user)
    //{
    //    count++;
    //    List<VisitedLocation> userLocations = user.VisitedLocations;
    //    List<Attraction> attractions = await _gpsUtil.GetAttractionsAsync();

    //    var rewardsToAdd = new ConcurrentBag<UserReward>();
    //    var userRewardNames = new HashSet<string>(user.UserRewards.Select(r => r.Attraction.AttractionName));

    //    Parallel.ForEach(userLocations, visitedLocation =>
    //    {
    //        foreach (var attraction in attractions)
    //        {
    //            if (!userRewardNames.Contains(attraction.AttractionName))
    //            {
    //                if (NearAttraction(visitedLocation, attraction))
    //                {
    //                    var reward = new UserReward(visitedLocation, attraction, GetRewardPoints(attraction, user));
    //                    rewardsToAdd.Add(reward);
    //                }
    //            }
    //        }
    //    });

    //    // Add collected rewards to the user's rewards
    //    foreach (var reward in rewardsToAdd)
    //    {
    //        user.AddUserReward(reward);
    //    }
    //}

    public async Task CalculateRewardsAsync(User user)
    {
        List<VisitedLocation> userLocations = user.VisitedLocations;
        var attractions = await _gpsUtil.GetAttractionsAsync();

        var userRewardNames = new HashSet<string>(user.UserRewards.Select(r => r.Attraction.AttractionName));
        var rewardsToAdd = new ConcurrentBag<UserReward>();

        Parallel.ForEach(userLocations, visitedLocation =>
        {
            foreach (var attraction in attractions)
            {
                if (!userRewardNames.Contains(attraction.AttractionName) && NearAttraction(visitedLocation, attraction))
                {
                    var reward = new UserReward(visitedLocation, attraction, GetRewardPoints(attraction, user));
                    rewardsToAdd.Add(reward);
                }
            }
        });

        foreach (var reward in rewardsToAdd)
        {
            user.AddUserReward(reward);
        }
    }




    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        Console.WriteLine(GetDistance(attraction, location));
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    // Méthode pour obtenir les attractions les plus proches d'une localisation donnée
    public async Task<List<Attraction>> GetClosestAttractionsAsync(Locations location)
    {
        // Récupération de toutes les attractions disponibles de manière asynchrone
        var attractions = await _gpsUtil.GetAttractionsAsync();

        // Vérification si la liste des attractions est null
        if (attractions == null)
        {
            return new List<Attraction>();
        }

        // Calcul des distances entre chaque attraction et la localisation donnée
        var closestAttractions = attractions
            .Select(attraction => new
            {
                Attraction = attraction,
                Distance = GetDistance(attraction, location) // Calcul de la distance
            })
            .OrderBy(attractionWithDistance => attractionWithDistance.Distance) // Tri par distance croissante
            .Take(5) // Sélection des 5 attractions les plus proches
            .Select(attractionWithDistance => attractionWithDistance.Attraction) // Extraction des attractions
            .ToList(); // Conversion en liste

        // Retourne la liste des attractions les plus proches
        return closestAttractions;
    }

    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        if (visitedLocation == null || visitedLocation.Location == null || attraction == null)
        {
            return true;
        }
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
