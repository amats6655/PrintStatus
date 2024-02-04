// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;
using PrintStatus.DOM.Models;

namespace PrintStatus.AUTH.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
	public List<BasePrinterUser> PrinterUsers { get; set; }
}
