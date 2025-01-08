using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;

namespace TourGuide.LibrairiesWrappers;

public class GpsUtilWrapper : IGpsUtil
{
    private readonly GpsUtil.GpsUtil _gpsUtil;

    public GpsUtilWrapper()
    {
        _gpsUtil = new();
    }

    public Task<VisitedLocation> GetUserLocationAsync(Guid userId)
    {
        // Simulate asynchronous operation
        return Task.Run(() => _gpsUtil.GetUserLocation(userId));
    }

    //modification en async de la methode
    public async Task<List<Attraction>> GetAttractionsAsync()
    {
        return await _gpsUtil.GetAttractionsAsync();
    }
}
