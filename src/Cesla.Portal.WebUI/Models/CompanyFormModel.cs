using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cesla.Portal.WebUI.Models;

public class CompanyFormModel
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(255, ErrorMessage = "Company name is too long.")]
    [MinLength(5, ErrorMessage = "Company name is too short.")]    
    public string CompanyName { get; set; } = null!;
    public string FantasyName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}-?\d{2}$", 
        ErrorMessage = "Invalid CNPJ format.",
        MatchTimeoutInMilliseconds = 200)]
    public string Cnpj { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^(\+?\d{1,3})?[-. \(\)]?\(?\d{1,4}?\)?[-. \(\)]?\d{1,4}[-. \(\)]?\d{1,9}$", 
        ErrorMessage = "Invalid phone number format.",
        MatchTimeoutInMilliseconds = 200)]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "The email address is not valid")]
    public string EmailAddress { get; set; } = null!;

    [Required(ErrorMessage = "Address line is required")]
    public string AddressLine { get; set; } = null!;
    [Required]
    public string City { get; set; } = null!;
    [Required]
    public string State { get; set; } = null!;
    [Required]
    public string Country { get; set; } = null!;    
}
