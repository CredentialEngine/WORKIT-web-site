﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models.Common;

namespace Models
{
	public class AppUser : BaseObject
	{
		public AppUser()
		{
			IsValid = true;
			UserRoles = new List<string>();
			Roles = "";
            OrgMbrs = "";
			PublishByOrgCtid = "";
		}
		//public int Id { get; set; }
		//public Guid RowId { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string SortName { get; set; }
		public string AspNetUserId { get; set; }
		//
		public string CEAccountIdentifier { get; set; } = "";
		public bool IsActive { get; set; }
		public bool IsValid { get; set; }

		public List<string> UserRoles { get; set; }
		public string Roles { get; set; }
        public string OrgMbrs { get; set; }
		//public List<CodeItem> Organizations1 { get; set; }
        public List<Organization> Organizations { get; set; } = new List<Organization>();
		public string lastLogon { get; set; }

		public int PrimaryOrgId { get; set; }
		public string PublishByOrgCtid { get; set; }
		public int DefaultRoleId { get; set; }



        /// <summary>
        /// Returns full name of user
        /// </summary>
        /// <returns></returns>
        public string FullName()
		{
			if ( string.IsNullOrWhiteSpace( FirstName ) )
				return "Incomplete - Update Profile";
			else
				return this.FirstName + " " + this.LastName;
		}
	}
}
