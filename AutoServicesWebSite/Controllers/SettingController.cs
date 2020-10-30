using System;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace AutoServicesWebSite.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SettingController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SettingController));

        private IRepositoryWrapper _repoWrapper;

        public SettingController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        // GET: Setting/Edit/5
        public ActionResult Edit()
        {
            var model = _repoWrapper.Setting.Setting;
            return View(model);
        }

        // POST: Setting/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Setting model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                _repoWrapper.Setting.Update(model);
                _repoWrapper.Setting.Save();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);

            }
            return View(model);
        }        
    }
}