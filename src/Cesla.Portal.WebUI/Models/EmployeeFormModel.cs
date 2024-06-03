using System.ComponentModel.DataAnnotations;
using Cesla.Portal.WebUI.Validation;

namespace Cesla.Portal.WebUI.Models;

public class EmployeeFormModel
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(255, ErrorMessage = "First name is too long.")]    
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(255, ErrorMessage = "Last name is too long.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Date of birth is required")]
    [PastDate]
    public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "The email address is not valid")]
    public string EmailAddress { get; set; } = null!;

    [Required(ErrorMessage = "Job title is required")]
    public string JobTitle { get; set; } = null!;

    [Required(ErrorMessage = "Department is required")]
    public string Department { get; set; } = null!;
}
