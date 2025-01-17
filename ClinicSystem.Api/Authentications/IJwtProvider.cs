﻿namespace SurveyBasket.Api.Authentications
{
    public interface IJwtProvider
    {
        (string token, int expireIn) GenerateToken(ApplicationUser user);
        string? ValidateToken(string token);
    }
}
