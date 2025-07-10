namespace Exam.Models
{
    public class Vehicle //класс машинок
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }


        public string? Brand { get; set; }
        
        public string? Model { get; set; }
        
        public int Mileage { get; set; }

        public int Years { get; set; }

        public int Price { get; set; }

        public int CategoryId { get; set; }
        
        public Category? Category { get; set; }

        public bool IsSold { get; set; } = false;

    }
}
