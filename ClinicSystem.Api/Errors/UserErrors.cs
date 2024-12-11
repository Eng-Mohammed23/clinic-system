﻿using ClinicSystem.Api.Abstructions;

namespace ClinicSystem.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);

    public static readonly Error NotFound =
        new("User.NotFound", "User is not found", StatusCodes.Status404NotFound);

    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);
    
    public static readonly Error DuplicatedAppointment =
        new("User.DuplicatedAppointment", "This Time is booked", StatusCodes.Status409Conflict);

    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidCode =
         new("User.InvalidCode", "Invalid code", StatusCodes.Status401Unauthorized);
}
