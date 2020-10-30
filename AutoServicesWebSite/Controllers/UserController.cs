using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoServicesWebSite.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Models;
using DataAccess.Repositories;
using AutoServicesWebSite.Code;
using System.Linq;

namespace AutoServicesWebSite.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UserController));
        private IRepositoryWrapper _repoWrapper;

        public UserController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }
        // GET: User
        [HttpGet]
        public ActionResult All()
        {
            IEnumerable<User> model = _repoWrapper.User.GetAll();
            return View(model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.Role = _repoWrapper.Role.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(User model)
        {
            ViewBag.Role = _repoWrapper.Role.GetAll();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (_repoWrapper.User.Query().Where(u => u.UserName == model.UserName).FirstOrDefault() != null)
            {
                ModelState.AddModelError("Внимание", "Съществува потребител с такъв \"Псевдоним\"");
                return View(model);
            }

            try
            {
                _repoWrapper.User.Add(model);
                _repoWrapper.User.Save();

                //update place

                return RedirectToAction("All", "User");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);

            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = _repoWrapper.User.GetById(id);
            ViewBag.Role = _repoWrapper.Role.GetAll();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User model)
        {
            ViewBag.Role = _repoWrapper.Role.GetAll();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _repoWrapper.User.Update(model);
                _repoWrapper.User.Save();

                //update place
                var loggedUserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (loggedUserId == model.UserId)
                {
                    SignOut();
                    SignInUser(model);
                }

                return RedirectToAction("All", "User");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);

            }
            return View(model);
        }
        // GET: User/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            Login loginModel = new Login
            {
                Places = _repoWrapper.Place.GetActive()
            };
            return View(loginModel);
        }

        // POST: User/Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            model.Places = _repoWrapper.Place.GetActive();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _repoWrapper.User.GetAuthenticatedUser(model.UserName, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("Неразрешен достъп", "Невалидна парола или потребител.");
                return View(model);
            }
            if (user.RoleId == (int)UserRole.Mechanic)
            {
                var userIsPlacedOn = _repoWrapper.Place.Query().Where(p => p.UserId == user.UserId).FirstOrDefault();
                if (userIsPlacedOn == null)
                {
                    var place = _repoWrapper.Place.GetById(model.PlaceId);
                    place.IsBusy = true;
                    place.UserId = user.UserId;
                    try
                    {
                        _repoWrapper.Place.Update(place);
                        _repoWrapper.Place.Save();
                        HttpContext.Session.SetInt32("PlaceId", model.PlaceId); ;

                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                    }
                }
                else
                {
                    HttpContext.Session.SetInt32("PlaceId", userIsPlacedOn.PlaceId); ;
                }
            }

            return await SignInUser(user);
        }

        // GET: User/Edit/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var user = _repoWrapper.User.GetById(id);
            try
            {
                _repoWrapper.User.Delete(user);
                _repoWrapper.User.Save();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return RedirectToAction("All", "User");
        }

        public ActionResult Cancel()
        {
            return RedirectToAction("All", "User");
        }

        public async Task<IActionResult> LogOut()
        {
            var loggedUserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var placeId = HttpContext.Session.GetInt32("PlaceId") ?? (_repoWrapper.Place.Query().Where(p => !p.IsBlock && p.UserId == loggedUserId).Any() ? (int?)_repoWrapper.Place.Query().Where(p => !p.IsBlock && p.UserId == loggedUserId).FirstOrDefault().PlaceId : null);
            if (placeId != null)
            {
                try
                {
                    //_repoWrapper.UserPlace.Delete(up);
                    //_repoWrapper.UserPlace.Save();
                    var place = _repoWrapper.Place.GetById((int)placeId);
                    place.IsBusy = false;
                    place.UserId = null;
                    _repoWrapper.Place.Update(place);
                    _repoWrapper.Place.Save();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            }
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "User");
        }

        private async Task<IActionResult> SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Authentication, GetRoleName(user.RoleId)),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.MiddleName + " " + user.LastName),
                new Claim(ClaimTypes.Role, user.RoleId == (int)UserRole.Administrator ? "Administrator": user.RoleId == (int)UserRole.Technician ? "Technician" : user.RoleId == (int)UserRole.Office ? "Office" : "Mechanic")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddYears(1),
                });

            return RedirectToAction("Index", "Home");
        }
        public static string GetRoleName(int RoleId)
        {
            switch (RoleId)
            {
                case (int)UserRole.Administrator:
                    return "Администратор";
                case (int)UserRole.Technician:
                    return "Технически";
                case (int)UserRole.Office:
                    return "Офис";
                case (int)UserRole.Mechanic:
                    return "Механик";
                default:
                    return "Анонимен";
            }

        }
    }
}