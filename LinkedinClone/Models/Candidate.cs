using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinkedinClone.Models
{
    public class Candidate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Display(Name = "Phone Number")]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Education level is required.")]
        [Display(Name = "Education Level")]
        public string EducationLevel { get; set; }

        [Required(ErrorMessage = "Years of experience is required.")]
        [Display(Name = "Years of Experience")]
        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50.")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Last employer is required.")]
        [Display(Name = "Last Employer")]
        [StringLength(100)]
        public string LastEmployer { get; set; }


        [Display(Name = "CV File Path")]
        public string? CvFilePath { get; set; }

        //[Display(Name = "CV File")]
        [NotMapped]
        public FormFile? CvFile { get; set; }

    }
}
