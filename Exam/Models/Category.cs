namespace Exam.Models
{
    public class Category //класс для категории
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<Vehicle>? Vehicles { get; set; }
    }
}
