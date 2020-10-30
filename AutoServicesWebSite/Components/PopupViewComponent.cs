using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using AutoServicesWebSite.Models;
using System.Security.Claims;

namespace AutoServicesWebSite.Components
{
    [Authorize]
    public class PopupViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {           
            return View();
        }
    }

}
