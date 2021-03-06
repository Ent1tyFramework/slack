﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Slack.Data.Entities;
using Slack.Data.Interfaces;
using Slack.Identity.Contexts;
using Slack.Identity.Entities;
using Slack.Identity.Managers;
using Slack.Services.Interfaces;
using System.Threading.Tasks;
using System.Web;
using static Slack.Services.SubService;

namespace Slack.Hubs
{
    [Authorize]
    public class SubHub : Hub
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IServicesManager servicesManager;

        public SubHub(ApplicationUserManager userManager, IServicesManager servicesManager)
        {
            this.userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            this.servicesManager = servicesManager;
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public async Task Subscribe(string toId)
        {
            string currentId = Context.User.Identity.GetUserId();

            var user = await userManager.FindByIdAsync(toId);

            if (user != null)
            {
                SubStatus subStatus = await servicesManager.SubService.AddAsync(currentId, user.Id);

                if (subStatus == SubStatus.Added)
                    Clients.Caller.subscribe();
                else
                    Clients.Caller.unsubscribe();
            }
            //Later i will add notifications
        }
    }
}