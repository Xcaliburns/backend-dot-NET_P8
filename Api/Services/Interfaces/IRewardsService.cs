using GpsUtil.Location;
using TourGuide.Users;

namespace TourGuide.Services.Interfaces
{
    public interface IRewardsService
    {
        Task CalculateRewardsAsync(User user);//modification de la methode en async
        double GetDistance(Locations loc1, Locations loc2);
        bool IsWithinAttractionProximity(Attraction attraction, Locations location);
        void SetDefaultProximityBuffer();
        void SetProximityBuffer(int proximityBuffer);
       Task<List<Attraction>>  GetClosestAttractionsAsync(Locations location); // nouvelle méthode
        int GetRewardPoints(Attraction attraction, User user);// methode qui n etait pas dans l interface
    }
}