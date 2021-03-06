﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;

using CTIServices;
using Utilities;
using Models;
using MC = Models.Common;
using MN = Models.Node;
using Models.Node.Interface;
using Models.Search;
using CassCompetency = Models.Helpers.Cass.CassCompetencyV2;


namespace CTI.Directory.Controllers
{
	public class EditorController : BaseController
	{
		bool valid = true;
        AppUser user = AccountServices.GetCurrentUser();
        string status = "";

		//Testing
		public ActionResult V3()
		{
			return View( "~/Views/EditorV3/Editor.cshtml" );
		}
		//End Testing

		//Create or edit a Credential
		//[Authorize]
		public ActionResult Credential( int id = 0 )
		{
			return LoadEditor( typeof( MN.Credential ), id, EditorSettings.EditorType.CREDENTIAL );
		}
		//

		//Create or edit an Organization
		//[Authorize]
		public ActionResult Organization( int id = 0 )
		{
            if (id == 0 && user != null && user.Organizations != null && user.Organizations.Count > 0)
            {
                //do a get my org
                id = user.Organizations[ 0 ].Id;
            }
            if (id == -99)
                id = 0;
			return LoadEditor( typeof( MN.Organization ), id, EditorSettings.EditorType.ORGANIZATION );
		}

        [Authorize]
        public ActionResult OrganizationByCtid( string ctid = "")
        {
            MC.Organization org = OrganizationServices.GetByCtid( ctid );
            //return RedirectToAction( "Organization", "Editor" );
            return RedirectToAction( "Organization", "Editor", new { id = org.Id } );
            //return LoadEditor( typeof( MN.Organization ), org.Id, EditorSettings.EditorType.ORGANIZATION );
        }
        //
        /// <summary>
        /// Create or edit a QA Organization 
        /// - not sure if need a unique profile class??
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
		public ActionResult QAOrganization( int id = 0 )
		{
			return LoadEditor( typeof( MN.QAOrganization ), id, EditorSettings.EditorType.QA_ORGANIZATION );
		}
		//
		//Create or edit an Assessment
		//[Authorize]
		public ActionResult Assessment( int id = 0 )
		{
			return LoadEditor( typeof( MN.Assessment ), id, EditorSettings.EditorType.ASSESSMENT );
		}
		//

		//Create or edit a Learning Opportunity
		//[Authorize]
		public ActionResult LearningOpportunity( int id = 0 )
		{
			return LoadEditor( typeof( MN.LearningOpportunity ), id, EditorSettings.EditorType.LEARNINGOPPORTUNITY );
		}
		//

		//Standardized way of loading an editor
		private ActionResult LoadEditor( Type profileType, int profileID, EditorSettings.EditorType editorType )
		{
			if (!AccountServices.IsUserAuthenticated())
			{
				string nextUrl = string.Format( "/editor/{0}/{1}", editorType.ToString().ToLower(), profileID );
				return RedirectToAction( "CE_LoginRedirect", "Account", new { nextUrl = nextUrl } );
			}

			if ( !AuthorizationCheck( editorType, profileID ) )
			{
				return RedirectToAction( "Index", "Message" );
			}
			var parentProfile = new MN.ProfileLink();
			Guid owningAgentUid = new Guid();

			var parms = Request.QueryString.ToString();

			string lastProfileType = Request.Params[ "lastProfileType" ];
			string lastProfileRowId = Request.Params[ "lastProfileRowId" ];
			if ( ServiceHelper.IsValidGuid( lastProfileRowId ) )
			{
				if ( lastProfileType == "Credential" )
				{
					//get the credential as profile link, and include owning org

					MC.Credential cred = CredentialServices.GetBasicCredentialAsLink( new Guid( lastProfileRowId ) );
					owningAgentUid = cred.OwningAgentUid;
				}
				else if ( lastProfileType == "ConditionProfile" )
				{
					//get the credential for the condition, as profile link, and include owning org
					MN.ProfileLink plink = ConditionProfileServices.GetAsProfileLink( new Guid( lastProfileRowId ) );
					owningAgentUid = plink.OwningAgentUid;
				}
				else if ( lastProfileType == "ConditionManifest" )
				{
					//get the credential for the condition, as profile link, and include owning org
					//MN.ProfileLink plink = ConditionProfileServices.GetAsProfileLink( new Guid( lastProfileRowId ) );
					//owningAgentUid = plink.OwningAgentUid;
				}
				else if ( lastProfileType == "Organization"
					|| lastProfileType == "QAOrganization" )
				{
					owningAgentUid = new Guid( lastProfileRowId );
				}
			}


			var context = new ProfileContext()
			{
				IsTopLevel = true,
				Profile = new MN.ProfileLink()
				{
					Id = profileID,
					Type = profileType,
					OwningAgentUid = owningAgentUid
				}
			};

			var data = EditorServices.GetProfile( context, false, ref valid, ref status );

			if ( data.Id == 0 && profileID > 0 )
			{
				SetPopupErrorMessage(string.Format("ERROR - the requested {0} record was not found. " + status, editorType));
				return RedirectToAction( "Index", "Home" );
			}
			//if ( data.IsReferenceVersion )
			//{
			//	Session[ "siteMessage" ] = "The selected record is a Reference only, there is no detail to display";
			//	return RedirectToAction( "Index", "Message" );
			//}

			var settings = new EditorSettings()
			{
				MainProfile = new MN.ProfileLink()
				{
					Id = profileID,
					Type = profileType,
					Name = data.Name,
					RowId = data.RowId
				},
				Editor = editorType,
				Data = data
			};

			settings.UserOrganizations = OrganizationServices.OrganizationMember_OrgsAsCodeItems( user.Id );
			settings.ParentRequestType = Request.Params[ "prt" ];
			settings.LastProfileType = Request.Params[ "lastProfileType" ];
			settings.LastProfileRowId = Request.Params[ "lastProfileRowId" ];


			return View( "~/Views/Editor/EditorV2.cshtml", settings );
		}
		//

		//Standardized authorization/authentication checks
		private bool AuthorizationCheck( EditorSettings.EditorType type, int profileID )
		{
			//Initial data
			var canEdit = true;
			var name = "";

			//Check to see if the user has any edit permissions
			if ( AccountServices.AuthorizationCheck( "edit", true, ref status, ref user ) == false )
			{
				ConsoleMessageHelper.SetConsoleInfoMessage( "ERROR - NOT AUTHENTICATED. You will not be able to add or update content", "", false );
				Session[ "siteMessage" ] = "ERROR - NOT AUTHENTICATED. You will not be able to add or update content";
				canEdit = false;
			}

			//If the target is a new item, just return true/false depending on whether or not user can edit content
			if ( profileID == 0 )
			{
				return canEdit;
			}

			//Otherwise, if the item exists, check to see if the user can edit it specifically
			switch ( type )
			{
				case EditorSettings.EditorType.CREDENTIAL:
					canEdit = CredentialServices.CanUserUpdateCredential( profileID, user, ref status );
					name = "credential";
					break;
				case EditorSettings.EditorType.ORGANIZATION:
					canEdit = OrganizationServices.CanUserUpdateOrganization( user, profileID );
					name = "organization";
					break;
				case EditorSettings.EditorType.ASSESSMENT:
					canEdit = AssessmentServices.CanUserUpdateAssessment( profileID, user, ref status );
					name = "assessment";
					break;
				case EditorSettings.EditorType.LEARNINGOPPORTUNITY:
					canEdit = LearningOpportunityServices.CanUserUpdateLearningOpportunity( profileID, user, ref status );
					name = "learning opportunity";
					break;
				default:
					break;
			}

			//If the user can't edit, set a standardized message
			if ( !canEdit )
			{
				ConsoleMessageHelper.SetConsoleErrorMessage( "ERROR - you do not have authorization to edit this " + name, "", false );
				Session[ "siteMessage" ] = "ERROR - you do not have authorization to edit this " + name;
			}

			//Return the result of all the checks
			return canEdit;
		}

		#region Profile-based operations

		//Get a Profile
		public JsonResult GetProfile( ProfileContext context )
		{
			//Get the clientProfile based on the provided context
			var data = EditorServices.GetProfile( context, false, ref valid, ref status );

			//Return the data
			return JsonHelper.GetJsonWithWrapper( data, valid, status, null );
		}
		//

		//Save a Profile
		public JsonResult SaveProfile( ProfileContext context, string profile )
		{
			MN.ProfileLink link = null;
			if ( profile == null )
			{
				status = "Error a profile was not provided.";
				valid = false;
				return JsonHelper.GetJsonWithWrapper( null, valid, status, link );
			}

			if ( AccountServices.AuthorizationCheck( "updates", true, ref status, ref user ) == false )
			{
				return JsonHelper.GetJsonWithWrapper( null, false, status, null );
			}

			MN.BaseProfile result = new MN.BaseProfile();
			//Cast the clientProfile based on the provided context
			try
			{
				var data = JsonConvert.DeserializeObject( profile, context.Profile.Type, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore } );

				//Add or update the data
				result = EditorServices.SaveProfile( context, ( MN.BaseProfile ) data, ref valid, ref status, false );

				//Return the result
				if ( result == null )
					valid = false;
			} catch (Exception ex)
			{
				status = ex.Message;
				//status = ServiceHelper.FormatExceptions( ex );
				if ( status.IndexOf( ", line " ) > 0 )
					status = status.Substring( 0, status.IndexOf( ", line " ) );
				valid = false;
			}
			
	
			if ( valid  )
			{
				if ( context.Main.TypeName == "Credential" && result.Id == 0 )
				{
					//RedirectToAction( "Edit", new { id = model.Credential.Id } );
				}
				link = new MN.ProfileLink() { Id = result.Id, RowId = result.RowId, Name = result.Name, Type = context.Profile.Type };
			}
			return JsonHelper.GetJsonWithWrapper( result, valid, status, link );
		}
		//

		//Delete a Profile
		public JsonResult DeleteProfile( ProfileContext context )
		{
			//Check permission
			if ( AccountServices.AuthorizationCheck( "delete", true, ref status, ref user ) == false )
			{
				return JsonHelper.GetJsonWithWrapper( null, false, status, null );
			}
			
			//Delete the data
			EditorServices.DeleteProfile( context, ref valid, ref status );

			//Return the result
			return JsonHelper.GetJsonWithWrapper( null, valid, status, null );
		}
		#endregion

		#region registry
		//
		/// <summary>
		/// Register/update an object in the registry
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public JsonResult Register( ProfileContext context )
		{
			//Check permission
			if ( AccountServices.AuthorizationCheck( "publish", true, ref status, ref user ) == false )
			{
				return JsonHelper.GetJsonWithWrapper( null, false, status, null );
			}

			//publish to the registry
			EditorServices.RegisterEntity( context, ref valid, ref status );

			//Return the result
			return JsonHelper.GetJsonWithWrapper( null, valid, status, null );
		}

		/// <summary>
		/// Remove an object from the registry
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public JsonResult Unregister( ProfileContext context )
		{
			//Check permission
			if ( AccountServices.AuthorizationCheck( "publish", true, ref status, ref user ) == false )
			{
				return JsonHelper.GetJsonWithWrapper( null, false, status, null );
			}

			EditorServices.UnregisterEntity( context, ref valid, ref status );

			//Return the result
			return JsonHelper.GetJsonWithWrapper( null, valid, status, null );
		}
		#endregion

		#region Approvals
		//
		/// <summary>
		/// Approve an entity 
        /// - where is this called from - for now default to send email. 
        /// A: called from detail, so OK to send email
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public JsonResult Approve( ProfileContext context )
		{
			//Check permission
			if ( AccountServices.AuthorizationCheck( "publish", true, ref status, ref user ) == false )
			{
				return JsonHelper.GetJsonWithWrapper( null, false, status, null );
			}
            bool isPublished = false;
			EditorServices.ApproveEntity( context, ref valid, ref isPublished, ref status, true );

			//Return the result
			//return JsonHelper.GetJsonWithWrapper( null, valid, status, null );
            return JsonResponse( null, valid, status, new { IsApproved = valid, IsPublished = isPublished, StatusMessages = new List<string>() } );
        }

		/// <summary>
		/// Un-Approve an entity
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public JsonResult UnApprove( ProfileContext context )
		{
			//Check permission
			if ( AccountServices.AuthorizationCheck( "publish", true, ref status, ref user ) == false )
			{
				return JsonHelper.GetJsonWithWrapper( null, false, status, null );
			}

			EditorServices.UnApproveEntity( context, ref valid, ref status );

			//Return the result
			return JsonHelper.GetJsonWithWrapper( null, valid, status, null );
		}
		#endregion
		#region MicroSearchV2 methods

		//Get existing/saved MicroSearch results for initial display
		public JsonResult GetMicroProfiles( string searchType, List<MN.ProfileLink> items )
		{
			var data = MicroSearchServicesV2.GetMicroProfiles( searchType, items, ref valid, ref status );

			return JsonHelper.GetJsonWithWrapper( data, valid, status, null );
		}
		//

		public JsonResult RefreshMicroProfiles( string searchType, ProfileContext context, string propertyName )
		{
			var data = MicroSearchServicesV2.GetMicroProfiles( searchType, context, propertyName, ref valid, ref status );

			return JsonHelper.GetJsonWithWrapper( data, valid, status, null );
		}
		//

		//Do a MicroSearch
		public JsonResult DoMicroSearch( MicroSearchInputV2 query )
		{
			var totalResults = 0;
			var data = MicroSearchServicesV2.DoMicroSearch( query, ref totalResults, ref valid, ref status );

			return JsonHelper.GetJsonWithWrapper( data, valid, status, totalResults );
		}
		//

		//Add/Save a MicroProfile
		public JsonResult SaveMicroProfile( ProfileContext context, Dictionary<string, object> selectors, string searchType, string property, bool allowMultiple )
		{
			var data = new MicroSearchServicesV2().SaveMicroProfile( context, selectors, searchType, property, allowMultiple, ref valid, ref status );

			return JsonHelper.GetJsonWithWrapper( data, valid, status, null );
		}
		//

		//Delete a MicroProfile
		public JsonResult DeleteMicroProfile( ProfileContext context, Dictionary<string, object> selectors, string searchType, string property )
		{
			MicroSearchServicesV2.DeleteMicroProfile( context, selectors, searchType, property, ref valid, ref status );

			return JsonHelper.GetJsonWithWrapper( null, valid, status, null );
		}
		//

		//Create an organization reference
		public JsonResult SaveOrgReferenceProfile( ProfileContext context, MN.OrgReference profile, bool allowMultiple )
		{

			context.Profile.TypeName = profile.ProfileType;
            //set org references as QA
            //may want to force the TypeName to QAOrganization
            profile.IsAQAOrganization = true;
			var data = EditorServices.SaveProfile( context,
				( MN.BaseProfile ) profile, 
				ref valid, 
				ref status, true );

			MN.MicroProfile result = null;
			if ( valid )
			{
				//Update the context to work with the MicroSearch methods
				var selectors = new Dictionary<string, object>() 
				{
					{ "Id", data.Id },
					{ "Name", data.Name },
					{ "RowId", data.RowId.ToString() },
					{ "TypeName", profile.ProfileType }
				};
				//why is the profile set to the parent? - may be for use of the search type. for an start asmt, they are the same ProfileLink
				context.Profile = context.Parent;
				result = new MicroSearchServicesV2().SaveMicroProfile( context, selectors, profile.SearchType, context.Profile.Property, allowMultiple, ref valid, ref status );
			}
			
			return JsonHelper.GetJsonWithWrapper( result, valid, status, null );
		}

		//Create a Starter Profile
		public JsonResult SaveStarterProfile( ProfileContext context, MN.StarterProfile profile, bool allowMultiple )
		{

			context.Profile.TypeName = profile.ProfileType;
			var data = EditorServices.SaveProfile( context,
				( MN.BaseProfile ) profile,
				ref valid,
				ref status, 
				true ); //is reference

			MN.MicroProfile result = null;
			if ( valid && data != null)
			{
				//Update the context to work with the MicroSearch methods
				var selectors = new Dictionary<string, object>()
				{
					{ "Id", data.Id },
					{ "Name", data.Name },
					{ "RowId", data.RowId.ToString() },
					{ "TypeName", profile.ProfileType }
				};
				//why is the profile set to the parent? - may be for use of the search type. for an start asmt, they are the same ProfileLink
				context.Profile = context.Parent;
                //HMMM - this is being called from QA performed add reference, and shouldn't. Just want to return the 
				result = new MicroSearchServicesV2().SaveMicroProfile( context, selectors, profile.SearchType, context.Profile.Property, allowMultiple, ref valid, ref status );
			} else
			{
				if ( string.IsNullOrWhiteSpace( status ) )
					status = "Error encountered. Sorry the system was remiss in providing adequate details as to what failed!";
			}


			return JsonHelper.GetJsonWithWrapper( result, valid, status, null );
		}
		//
		//Copy a cost Profile
		public JsonResult CopyCostProfile( ProfileContext context, MN.StarterProfile profile, bool allowMultiple )
		{
			

			context.Profile.TypeName = profile.ProfileType;
			var data = EditorServices.SaveProfile( context,
				( MN.BaseProfile ) profile,
				ref valid,
				ref status,
				false );

			MN.MicroProfile result = null;
			if ( valid )
			{
				//Update the context to work with the MicroSearch methods
				var selectors = new Dictionary<string, object>() 
				{
					{ "Id", data.Id },
					{ "Name", data.Name },
					{ "RowId", data.RowId.ToString() },
					{ "TypeName", profile.ProfileType }
				};
				//why is the profile set to the parent? - may be for use of the search type. for an start asmt, they are the same ProfileLink
				context.Profile = context.Parent;
				result = new MicroSearchServicesV2().SaveMicroProfile( context, selectors, profile.SearchType, context.Profile.Property, allowMultiple, ref valid, ref status );
			}


			return JsonHelper.GetJsonWithWrapper( result, valid, status, null );
		}
		//
		public JsonResult SaveChildProfileLink( ProfileContext context, MN.StarterProfile profile, bool allowMultiple )
		{
			//appears the context.Profile is being set to the parent somewhere
			ProfileContext save = context;
			//????
			//is this necessary, as added a lopp and asmt, and profile.ProfileType = conditionProfile
			context.Profile.TypeName = profile.ProfileType;

			//var data = new MN.BaseProfile();
			
			MN.MicroProfile result = null;
			//Update the context to work with the MicroSearch methods
			//not sure about type name
			//for an lopp popup, the TypeName is credential, and the parent is ProfileLink??
			//shouldn't it be lopp? - actually works in microsearch - BUT POTENTIAL ISSUE
			var selectors = new Dictionary<string, object>() 
			{
				{ "Id", profile.Id },
				{ "Name", profile.Name },
				{ "RowId", profile.RowId.ToString() },
				{ "TypeName", context.Profile.TypeName }
			};
			var selectors2 = new Dictionary<string, object>() 
			{
				{ "Id", profile.Id },
				{ "Name", profile.Name },
				{ "RowId", profile.RowId.ToString() },
				{ "TypeName", profile.ProfileType }
			};
			//why is the profile set to the parent?
			//the parent is the credential, not condition
			//it seems to work if commented, but the whole process could be improved
			//context.Profile = context.Parent;

			//17-02-08 mparsons - if we know this was from a new 
			//context.Profile.RowId will contain the credentialRowId
			result = new MicroSearchServicesV2().SaveMicroProfile( context, selectors, profile.SearchType, context.Profile.Property, allowMultiple, ref valid, ref status );
            if (!string.IsNullOrWhiteSpace(status))
            {
                //if not a success message, force display
                //valid = false;
                status = "Successful with warning: " + status;
                ConsoleMessageHelper.SetConsoleInfoMessage( status );
            }

			return JsonHelper.GetJsonWithWrapper( result, valid, status, null );
		}
		//
		#endregion

		#region CASS Browser methods

		/// <summary>
		/// May have separate end points for competencies and frameworks, or determine if can be handled in one?
		/// called from editor, where invoked from cass.saveSelectedCompetencies
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public JsonResult SaveCassCompetencyList( MN.CassInput data )
		{
			MN.ProfileLink link = null;
			if ( data == null )
			{
				status = "Error a Competency profile was not provided.";
				valid = false;
				return JsonHelper.GetJsonWithWrapper( null, valid, status, link );
			}

			var result = CompetencyServices.SaveCassCompetencyList( data, ref valid, ref status );

			return JsonHelper.GetJsonWithWrapper( result, valid, status, null );
		}
		//
		
		public JsonResult LoadCassCompetencyList( MN.Interface.ProfileContext context )
		{
			//TODO: populate this
			var result = new List<CassCompetency>();
            //assuming context.Profile.RowId
            result = CompetencyServices.CassCompetency_GetAll( context.Profile.RowId );

            return JsonHelper.GetJsonWithWrapper( result, true, "", null );
		}
		//

		public JsonResult RemoveCassCompetencyFromList( int associationID )
		{
			bool isValid = CompetencyServices.RemoveCassCompetency( associationID, ref status );
			return JsonHelper.GetJsonWithWrapper( null, isValid, status, null );
		}
		//
		#endregion

		#region elastic loaders 
		//
		// 
		//[Authorize( Roles = "Administrator" )]
		//public ActionResult LoadCredentials( int maxRecords = 50 )
		//{
		//	if ( !User.Identity.IsAuthenticated
		//		|| ( User.Identity.Name != "mparsons" ) )
		//	{
		//		ConsoleMessageHelper.SetConsoleInfoMessage( "ERROR - NOT AUTHORIZED for that action. ", "", false );
		//		return RedirectToAction( "Index", "Home" );
		//	}

		//	ElasticIndexService eis = new ElasticIndexService();
		//	eis.CreateIndex();
		//	eis.LoadAllCredentials(ref  status);

		//	ConsoleMessageHelper.SetConsoleInfoMessage( status );
		//	//anything will do
		//	//return View( "ConfirmEmail" );
		//	return View( "~/Views/V2/Account/ConfirmEmail.cshtml" );
		//}


		#endregion

		#region Experimental

		public ActionResult EditorV4( string type )
		{
			switch ( type )
			{
				case "credential":
					return View( "~/views/editorv4/editorv4.cshtml", new Models.EditorModels.CredentialEditorList() );
				default:
					return new ContentResult() { Content = "Error Determining Editor Type" };
			}
		}

		#endregion
	}
}