namespace PresentationLayer.Mvc;

public static class Constants
{
    public static string JwtToken = "JWT_TOKEN";

    public static string DefaultArea = Areas.Customer;
    public static string DefaultController = "Home";
    public static string DefaultAction = "Index";

    public static class Areas
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
    }

    public static class Views
    {
        public const string BadRequest = "BadRequest";
        public const string NotFound = "NotFound";
    }

    public static class Keys
    {
        public const string Title = "Title";
        public const string Action = "Action";
        public const string ButtonText = "ButtonText";
        public const string ErrorMessage = "ErrorMessage";
    }
}
