using System.ComponentModel.DataAnnotations;


//не работает оставил чтоб не потерять и по другому сделать
public class OrderViewModel
{
    public int VehicleId { get; set; }

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Введите корректный email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Телефон обязателен")]
    [RegularExpression(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$", ErrorMessage = "Введите корректный номер телефона")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Номер карты обязателен")]
    [RegularExpression(@"^\d{4} \d{4} \d{4} \d{4}$", ErrorMessage = "Введите 16 цифр в формате 0000 0000 0000 0000")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Дата окончания карты обязательна")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Введите срок действия в формате MM/YY")]
    public string ExpiryDate { get; set; }

    [Required(ErrorMessage = "CVV обязателен")]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV должен содержать 3 цифры")]
    public string CVV { get; set; }
}
