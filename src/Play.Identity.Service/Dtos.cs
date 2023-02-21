using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service
{
    public record UserDto(
        Guid id,
        string Username,
        string Email,
        decimal Gil,
        DateTimeOffset CreatedDate            
    );

    public record UpdateUserDto(
        [Required] [EmailAddress] string Email,
        [Range(0, 1_000_000)] decimal Gil     
    );
}