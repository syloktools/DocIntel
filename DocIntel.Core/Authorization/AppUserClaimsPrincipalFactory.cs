/* DocIntel
 * Copyright (C) 2018-2021 Belgian Defense, Antoine Cailliau
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DocIntel.Core.Models;
using DocIntel.Core.Repositories;
using DocIntel.Core.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DocIntel.Core.Authorization
{
    public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, AppRole>
    {
        private readonly IServiceProvider _serviceProvider;
        
        
        public AppUserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IOptions<IdentityOptions> options, IServiceProvider serviceProvider)
            : base(userManager, roleManager, options)
        {
            _serviceProvider = serviceProvider;
        }
        
        // TODO Refactor, code duplication
        private DocIntelContext GetContext()
        {
            var dbContextOptions = _serviceProvider.GetRequiredService<DbContextOptions<DocIntelContext>>();
            var dbContextLogger = _serviceProvider.GetRequiredService<ILogger<DocIntelContext>>();
            var dbContext = new DocIntelContext(dbContextOptions, dbContextLogger);
            return dbContext;
        }
        
        public override async Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var principal = await base.CreateAsync(user);
            var claimsIdentity = (ClaimsIdentity) principal.Identity;
            try
            {   
                var context = GetContext();
                PopulateClaims(context, claimsIdentity, user);
                await context.DisposeAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error in AppUserClaimsPrincipalFactory");
            }
            return principal;
        }

        private void AddParentGroup(DocIntelContext context, ClaimsIdentity claimsIdentity, Group group)
        {
            if (group.ParentGroupId != default)
            {
                var parentGroup = context.Groups.SingleOrDefault(_ => _.GroupId == group.ParentGroupId);
                if (parentGroup != null)
                {
                    claimsIdentity.AddClaim(new Claim("Group", parentGroup.GroupId.ToString()));
                    AddParentGroup(context, claimsIdentity, parentGroup);
                }
            }
        }


        private void PopulateClaims(DocIntelContext context, ClaimsIdentity claimsIdentity, AppUser user)
        {
            if (claimsIdentity != null)
            {
                claimsIdentity.AddClaim(new Claim("UserId", user.Id));
                if (user.Bot)
                    claimsIdentity.AddClaim(new Claim("Bot", user.Bot.ToString()));

                var permissions = context.UserRoles.AsNoTracking()
                    .Where(x => x.UserId == user.Id)
                    .Select(_ => _.Role).ToList()
                    .SelectMany(x => x.Permissions).ToList();
                foreach (var permission in permissions) 
                    claimsIdentity.AddClaim(new Claim("Permission", permission));

                var groups = context.Members.AsNoTracking().Include(_ => _.Group)
                    .Where(x => x.UserId == user.Id)
                    .Select(_ => _.Group).ToList();
                foreach (var group in groups)
                {
                    claimsIdentity.AddClaim(new Claim("Group", @group.GroupId.ToString()));
                    AddParentGroup(context, claimsIdentity, @group);
                }
            }
        }
        
        public async Task<ClaimsPrincipal> CreateAsync(DocIntelContext context, AppUser user)
        {
            ClaimsPrincipal principal = null;
            try
            {
                var id = new ClaimsIdentity("Identity.Application",
                    Options.ClaimsIdentity.UserNameClaimType,
                    Options.ClaimsIdentity.RoleClaimType);
                id.AddClaim(new Claim(Options.ClaimsIdentity.UserIdClaimType, user.Id));
                id.AddClaim(new Claim(Options.ClaimsIdentity.UserNameClaimType, user.UserName));
                if (!string.IsNullOrEmpty(user.Email))
                    id.AddClaim(new Claim(Options.ClaimsIdentity.EmailClaimType, user.Email));
                
                principal = new ClaimsPrincipal(id);
            }
            catch
            {
                Console.WriteLine("Error in AppUserClaimsPrincipalFactory.1");
                return null;
            }

            var claimsIdentity = (ClaimsIdentity) principal.Identity;
            try
            {
                PopulateClaims(context, claimsIdentity, user);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in AppUserClaimsPrincipalFactory.2");
            }
            return principal;
        }
    }
}