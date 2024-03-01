namespace PrintStatus.DOM.Models;

public class UserRole
{
	public int Id { get; set; }
	public SystemRole? Role { get; set; }
	public int RoleId { get; set; }
	public ApplicationUser? User { get; set; }
	public int UserId { get; set; }
}