namespace Exam.Models
{
    public class ErrorViewModel //класс для ордер вью модель (не рабочий)
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
