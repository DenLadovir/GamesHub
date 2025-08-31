using System.ComponentModel.DataAnnotations;

public class ChangePasswordViewModel
{
    public string UserId { get; set; }
    public string UserEmail { get; set; }

    [Required(ErrorMessage = "Введите новый пароль")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Повторите пароль")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; }
}