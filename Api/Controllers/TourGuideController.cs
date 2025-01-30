using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.Models;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;
    private readonly IRewardsService _rewardsService;

    public TourGuideController(ITourGuideService tourGuideService, IRewardsService rewardsService)
    {
        _tourGuideService = tourGuideService;
        _rewardsService = rewardsService;
    }

    [HttpGet("getLocation")]
    public ActionResult<VisitedLocation> GetLocation([FromQuery] string userName)
    {
        var location = _tourGuideService.GetUserLocation(GetUser(userName));
        return Ok(location);
    }

    
    [HttpGet("getNearbyAttractions")]
    public ActionResult<List<NearbyAttraction>> GetNearbyAttractions([FromQuery] string userName)
    {
        var user = GetUser(userName);
        var visitedLocation = _tourGuideService.GetUserLocation(user);

        var closestAttractions = _tourGuideService.GetNearByAttractions(visitedLocation)
            .Select(attraction => new NearbyAttraction
            {
                AttractionName = attraction.AttractionName,
                AttractionLatitude = attraction.Latitude,
                AttractionLongitude = attraction.Longitude,
                UserLatitude = visitedLocation.Location.Latitude,
                UserLongitude = visitedLocation.Location.Longitude,
                Distance = _rewardsService.GetDistance(attraction, visitedLocation.Location),
                RewardPoints = _rewardsService.GetRewardPoints(attraction, user)
            });
            

        return Ok(closestAttractions);
    }


    [HttpGet("getRewards")]
    public ActionResult<List<UserReward>> GetRewards([FromQuery] string userName)
    {
        var rewards = _tourGuideService.GetUserRewards(GetUser(userName));
        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public ActionResult<List<Provider>> GetTripDeals([FromQuery] string userName)
    {
        var deals = _tourGuideService.GetTripDeals(GetUser(userName));
        return Ok(deals);
    }

    private User GetUser(string userName)
    {
        return _tourGuideService.GetUser(userName);
    }
}
