using Microsoft.EntityFrameworkCore;
using APIHotelBeach.Models;

namespace APIHotelBeach.Context
{
    public class DbContextHotel : DbContext
    {
        //constructor
        public DbContextHotel(DbContextOptions<DbContextHotel> options) : base(options){ }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Paquete> Paquetes { get; set; }

    }
}
