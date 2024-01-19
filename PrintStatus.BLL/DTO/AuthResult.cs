using Microsoft.AspNetCore.Identity;

namespace PrintStatus.BLL.DTO
{
	public class AuthResult
	{
		public bool IsAuthenticated { get; set; }
		public IEnumerable<string> Roles { get; set; } = [];
		public IEnumerable<IdentityError> Errors { get; set; } = [];
		public string IdentityUserId { get; set; }
	}
}
