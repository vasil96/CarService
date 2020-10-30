using DataAccess.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;

namespace AutoServiceWebSite.Controllers
{
    public class QueueController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(QueueController));
        private readonly IRepositoryWrapper _repoWrapper;
        public QueueController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        [HttpGet]
        public int QueueCount()
        {
            int queueCount = 0;
            var loggedUserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var placeId = HttpContext.Session.GetInt32("PlaceId") ?? 
                (_repoWrapper.Place.Query().Where(p => !p.IsBlock && p.UserId == loggedUserId).Any() ? 
                (int?)_repoWrapper.Place.Query().Where(p => !p.IsBlock && p.UserId == loggedUserId).FirstOrDefault().PlaceId : null);
            if (!HttpContext.Session.GetInt32("PlaceId").HasValue && placeId.HasValue)
            {
                HttpContext.Session.SetInt32("PlaceId", placeId.Value);
            }
            if (placeId != null && _repoWrapper.Place.IsBlocked((int)placeId))
            {
                return 0;
            }
            try
            {
                queueCount = _repoWrapper.Queue.GetCountByPlace(placeId.GetValueOrDefault());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return queueCount;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public bool Remove()
        {
            bool result;
            try
            {
                _repoWrapper.Queue.Clean();
                result = true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                result = false;
            }
            return result;
        }
    }
}