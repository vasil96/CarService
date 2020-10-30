using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using AutoServicesWebSite.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace AutoServicesWebSite.Components
{
    [Authorize]
    public class HeaderViewComponent : ViewComponent
    {
        private IRepositoryWrapper _repoWrapper;

        public HeaderViewComponent(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal currentUser = (ClaimsPrincipal)User;
            var loggedUserId = Int32.Parse(currentUser.FindFirstValue(ClaimTypes.NameIdentifier));

            var placeId = HttpContext.Session.GetInt32("PlaceId") ?? (_repoWrapper.Place.Query().Where(p => !p.IsBlock && p.UserId == loggedUserId).Any() ? (int?)_repoWrapper.Place.Query().Where(p => !p.IsBlock && p.UserId == loggedUserId).FirstOrDefault().PlaceId : null);
            if (!HttpContext.Session.GetInt32("PlaceId").HasValue && placeId.HasValue)
            {
                HttpContext.Session.SetInt32("PlaceId", placeId.Value);
            }
            var model = new Header()
            {
                IsAdministrator = currentUser.IsInRole("Administrator"),
                IsTechnician = currentUser.IsInRole("Technician"),
                IsOffice = currentUser.IsInRole("Office"),
                IsMechanic = currentUser.IsInRole("Mechanic"),
                IncomleteQueueId = placeId.HasValue ? _repoWrapper.Queue.GetIncomplete(placeId.Value) != null ? (int?)(_repoWrapper.Queue.GetIncomplete(placeId.Value)).QueueId : null : null
            };
            return View( model);

        }
    }

}
