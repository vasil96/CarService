using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AutoServicesWebSite.Controllers
{
    [Authorize(Roles = "Administrator,Technician")]
    public class PlaceController : Controller
    {
        private IRepositoryWrapper _repoWrapper;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlaceController));
        public PlaceController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var places = _repoWrapper.Place.GetAllExtended().ToList();
            var userplace = _repoWrapper.UserPlace.GetAll();
            ViewBag.UserPlace = userplace;

            return View(places);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(List<Place> places)
        {
            try
            {
                foreach (var place in places)
                {
                    Place entity = _repoWrapper.Place.GetById(place.PlaceId);
                    entity.IsBlock = place.IsBlock;
                    entity.IsBusy = place.IsBusy;
                    if (!place.IsBusy)
                    {
                        entity.UserId = null;
                    }
                    _repoWrapper.Place.Update(entity);

                }
                _repoWrapper.Place.Save();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View(places);

            }

            return RedirectToAction("Edit", "Place");
        }
    }
}