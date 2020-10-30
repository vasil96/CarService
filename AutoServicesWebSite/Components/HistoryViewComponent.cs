using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using AutoServicesWebSite.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Models;
using System.Collections.Generic;

namespace AutoServicesWebSite.Components
{
    [Authorize]
    public class HistoryViewComponent : ViewComponent
    {
        private IRepositoryWrapper _repoWrapper;

        public HistoryViewComponent(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }
        public async Task<IViewComponentResult> InvokeAsync(IList<Protocol> model)
        {
            
            return View(model);

        }
    }

}
