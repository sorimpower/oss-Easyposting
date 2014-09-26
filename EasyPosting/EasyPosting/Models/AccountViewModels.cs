using System.ComponentModel.DataAnnotations;

namespace EasyPosting.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "이메일")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "현재 암호")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}은(는) {2}자 이상이어야 합니다.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "새 암호")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "새 암호 확인")]
        [Compare("NewPassword", ErrorMessage = "새 암호와 확인 암호가 일치하지 않습니다.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "이메일")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "암호")]
        public string Password { get; set; }

        [Display(Name = "사용자 이름 및 암호 저장")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "이메일")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}은(는) {2}자 이상이어야 합니다.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "암호")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "암호 확인")]
        [Compare("Password", ErrorMessage = "암호와 확인 암호가 일치하지 않습니다.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "이메일")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}은(는) {2}자 이상이어야 합니다.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "암호")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "암호 확인")]
        [Compare("Password", ErrorMessage = "암호와 확인 암호가 일치하지 않습니다.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "이메일")]
        public string Email { get; set; }
    }
}
