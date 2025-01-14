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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace DocIntel.Core.Models
{
    /// <summary>
    ///     Represents a role in the application.
    /// </summary>
    public class AppRole : IdentityRole<string>
    {
        /// <summary>
        ///     Initializes a new role.
        /// </summary>
        public AppRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Initializes a new role with the specified name.
        /// </summary>
        /// <param name="name">The name of the role</param>
        public AppRole(string name) : base(name)
        {
            Id = Guid.NewGuid().ToString();
            NormalizedName = name.ToUpperInvariant();
        }

        /// <summary>
        ///     An identifier for the urls
        /// </summary>
        /// <value>The identifier</value>
        [RegularExpression(@"[a-zA-Z0-9-_]*")]
        public string Slug { get; set; }

        /// <summary>
        ///     A short description of the role and its members
        /// </summary>
        /// <value>Description</value>
        public string Description { get; set; }

        /// <summary>
        ///     A string representing all permisssions. See Permissions for more details.
        /// </summary>
        /// <value>The permissions</value>
        public string PermissionList { get; set; }

        /// <summary>
        ///     The list of users included in the role
        /// </summary>
        /// <value>The users</value>
        public ICollection<AppUserRole> UserRoles { get; set; }

        /// <summary>
        ///     An array containing the string representation of the permissions. See IOperations* for exact strings for
        ///     each permission.
        /// </summary>
        /// <value>The permissions</value>
        [NotMapped]
        public string[] Permissions
        {
            get
            {
                if (string.IsNullOrEmpty(PermissionList))
                    return new string[] { };
                return PermissionList.Split(',').Select(x => x.Trim()).ToArray();
            }
            set => PermissionList = string.Join(",", value);
        }

        public DateTime CreationDate { get; internal set; }
        public DateTime ModificationDate { get; internal set; }
        public string CreatedById { get; internal set; }
        public AppUser CreatedBy { get; set; }
        public string LastModifiedById { get; internal set; }
        public AppUser LastModifiedBy { get; set; }
    }
}