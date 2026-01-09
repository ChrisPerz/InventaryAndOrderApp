public class AddRoleModel
{
    public string Email { get; set; }
    public Roles Role { get; set; }
}

public enum Roles
{
    Admin,
    Manager,
    User
}