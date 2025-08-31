using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Games.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Display(Name = "Название")]
        [MaxAndMinSize(2, 100)]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [NotMapped]
        public List<int> SelectedGenreIds { get; set; } = new();
        public List<GameGenre> GameGenres { get; set; } = new();

        [Display(Name = "Дата выхода")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
    }

    public class MaxAndMinSize : ValidationAttribute
    {
        private readonly int _max;
        private readonly int _min;

        public MaxAndMinSize(int min, int max)
        {
            _min = min;
            _max = max;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return new ValidationResult("Значение не может быть null.");
            }

            if(value is not string strValue)
            {
                return new ValidationResult("Некорректный тип данных. Ожидается строка.");
            }

            if(strValue.Length < _min || strValue.Length > _max)
            {
                return new ValidationResult($"Длина должна быть от {_min} до {_max} символов.");
            }

            return ValidationResult.Success;
        }
    }
}
