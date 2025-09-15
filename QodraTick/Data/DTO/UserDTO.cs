using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO
{

    public class UserDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام نمایشی الزامی است")]
        [StringLength(200, ErrorMessage = "نام نمایشی نباید بیش از 200 کاراکتر باشد")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [StringLength(100, ErrorMessage = "نام کاربری نباید بیش از 100 کاراکتر باشد")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
        [StringLength(255, ErrorMessage = "ایمیل نباید بیش از 255 کاراکتر باشد")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید بین 6 تا 100 کاراکتر باشد")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "نقش کاربری الزامی است")]
        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        // Navigation properties for display
        public string RoleDisplayName { get; set; } = string.Empty;
        public int ActiveTicketsCount { get; set; }
        public int ResolvedTicketsCount { get; set; }
    }

    public class CreateUserDTO
    {
        [Required(ErrorMessage = "نام نمایشی الزامی است")]
        [StringLength(200, ErrorMessage = "نام نمایشی نباید بیش از 200 کاراکتر باشد")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [StringLength(100, ErrorMessage = "نام کاربری نباید بیش از 100 کاراکتر باشد")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
        [StringLength(255, ErrorMessage = "ایمیل نباید بیش از 255 کاراکتر باشد")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید بین 6 تا 100 کاراکتر باشد")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "نقش کاربری الزامی است")]
        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserDTO
    {
        [Required(ErrorMessage = "نام نمایشی الزامی است")]
        [StringLength(200, ErrorMessage = "نام نمایشی نباید بیش از 200 کاراکتر باشد")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
        [StringLength(255, ErrorMessage = "ایمیل نباید بیش از 255 کاراکتر باشد")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "نقش کاربری الزامی است")]
        public int RoleId { get; set; }

        public bool IsActive { get; set; }
    }

    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "رمز عبور فعلی الزامی است")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور جدید الزامی است")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور جدید باید بین 6 تا 100 کاراکتر باشد")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "تکرار رمز عبور الزامی است")]
        [Compare("NewPassword", ErrorMessage = "رمز عبور جدید و تکرار آن باید یکسان باشند")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginDTO
    {
        [Required(ErrorMessage = "نام کاربری الزامی است")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }

    public class UserProfileDTO
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string RoleDisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }

        // Statistics
        public int TotalTicketsCreated { get; set; }
        public int ActiveTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
    }
}

