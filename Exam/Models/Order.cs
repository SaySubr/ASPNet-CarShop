using Exam.Models;
using System;
namespace Exam.Models
{
    public class Order //класс заказов
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
        public DateTime OrderDate { get; set; }

        public Vehicle? Vehicle { get; set; }

        public string? UserId { get; set; }
    }
}