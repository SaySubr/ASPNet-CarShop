using Exam.Data;
using Exam.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Exam.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Сохраняет заказ в базе
        public async Task<bool> SaveOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        // Удаляет машину по id (для админа)
        public async Task<bool> RemoveVehicleAsync(int vehicleId)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null)
                return false;

            _context.Vehicles.Remove(vehicle);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        //Получает машину по айди (смотря на категорию)
        public async Task<Vehicle?> GetVehicleByIdAsync(int id) 
        {
            return await _context.Vehicles
                .Include(v => v.Category)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
    }
}
