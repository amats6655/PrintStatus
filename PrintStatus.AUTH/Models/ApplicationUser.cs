// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace PrintStatus.AUTH.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
	//public List<BasePrinter> basePrinters {get; set;}
	// public int UserProfileId { get; set; }
	// public UserProfile UserProfile { get; set; }
}
