namespace SurveyBasket.Abstractions.Consts;

public static class DefaultUsers
{
    public const string AdminId = "fd1f4b8e-3a54-4ade-84ff-348e64b415ca";
    public const string AdminEmail = "admin@clinic-system.com";
    //public const string AdminPassword = "P@ssword123";
    public const string PasswordHash = "AQAAAAIAAYagAAAAEFukf9We33opyXfRYXHo/PX7cg7XxyhxiM2MKvONm8wDOahbMFkU+u9Hdn8blVMbwQ==";
    public const string AdminSecurityStamp = "55BF92C9EF0249CDA210D85D1A851BC9";
    public const string AdminConcurrencyStamp = "99d2bbc6-bc54-4248-a172-a77de3ae4430";
    public static DateTime CreatedOn = new DateTime(2024, 11, 14, 20, 3, 24, 120, DateTimeKind.Utc).AddTicks(9402);
}