public class RegisterModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public void VerifyPasswordsMatch()
    {
        if (Password != ConfirmPassword)
        {
            throw new ArgumentException("Passwords do not match.");
        }
    }
}