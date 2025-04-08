using HRProContracts.ViewModels;

namespace HRProContracts.BindingModels
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserViewModel User { get; set; } = new();
    }
}
