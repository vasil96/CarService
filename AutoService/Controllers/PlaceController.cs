using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AutoServiceWebSite.Controllers
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
            return View(places);
        }

        public IActionResult Edit(List<Place> places)
        {
            try
            {
                foreach (var place in places)
                {
                    Place placeO = _repoWrapper.Place.GetById(place.PlaceId);
                    placeO.IsBlock = place.IsBlock;
                    placeO.IsBusy = place.IsBusy;
                    if (!place.IsBusy)
                    {
                        placeO.UserId = null;
                    }
                    _repoWrapper.Place.Update(placeO);

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
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult ExtendEdit()
        {
            var places = _repoWrapper.Place.GetAllExtended();
            var placesUI = new List<PlaceUIRead>();
            foreach (var place in places)
            {
                var placeOptions = _repoWrapper.PlaceOption.Query().Where(p => p.PlaceId == place.PlaceId).ToList();
                var user = _repoWrapper.User.GetById(place.UserId.GetValueOrDefault());
                var placeUI = new PlaceUIRead()
                {
                    PlaceId = place.PlaceId,
                    IsBlock = place.IsBlock,
                    IsBusy = place.IsBusy,
                    User = user,
                    Option = new List<Option>()
                    {
                    }

                };

                foreach (var placeOption in placeOptions)
                {
                    var option = _repoWrapper.Option.GetById(placeOption.OptionId);
                    placeUI.Option.Add(option);
                }

                placesUI.Add(placeUI);
            }

            var options = _repoWrapper.Option.GetAll();
            ViewBag.Options = options;
            return View(placesUI);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExtendEdit(List<PlaceUIWrite> places)
        {
            try
            {
                var newPlaceOptions = new List<PlaceOption>();
                foreach (var place in places)
                {
                    Place placeO = _repoWrapper.Place.GetById(place.PlaceId);
                    placeO.IsBlock = place.IsBlock;
                    placeO.IsBusy = place.IsBusy;
                    if (!place.IsBusy)
                    {
                        placeO.UserId = null;
                    }
                    if (place.Option != null && place.Option.Length > 0)
                    {
                        foreach (var option in place.Option)
                        {
                            newPlaceOptions.Add(new PlaceOption()
                            {
                                PlaceId = place.PlaceId,
                                OptionId = option
                            });
                        }
                    }
                    _repoWrapper.PlaceOption.DeleteAll();
                    _repoWrapper.PlaceOption.AddRange(newPlaceOptions);
                    _repoWrapper.Place.Update(placeO);

                }
                _repoWrapper.PlaceOption.Save();
                _repoWrapper.Place.Save();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View(places);

            }

            return RedirectToAction("ExtendEdit", "Place");
        }
    }
}