using System.ComponentModel.DataAnnotations;

namespace Games.Models
{
    public class Feedback
    {
        [Display(Name = "Имя")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Некорректная длина имени!")]
        [Required(ErrorMessage = "Поле необходимо заполнить!")]
        public string Name { get; set; }

        [Display(Name = "Фамилия")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Некорректная длина фамилии!")]
        [Required(ErrorMessage = "Поле необходимо заполнить!")]
        public string Surname { get; set;}

        [Display(Name = "Возраст")]
        [Required(ErrorMessage = "Поле необходимо заполнить!")]
        public int Age { get; set;}

        [Display(Name = "Электронная почта")]
        [StringLength(70, MinimumLength = 2, ErrorMessage = "Некорректная длина адреса электронной почты!")]
        [Required(ErrorMessage = "Поле необходимо заполнить!")]
        public string Email { get; set;}

        [Display(Name = "Сообщение")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Некорректная длина Сообщения!")]
        [Required(ErrorMessage = "Поле необходимо заполнить!")]
        public string Message { get; set;}
    }
}
