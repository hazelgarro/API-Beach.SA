using APIHotelBeach.Models;
using APIHotelBeach.Models.Custom;

namespace APIHotelBeach.Services
{
    public interface IAutorizacionServicesEmpleado
    {
            Task<AutorizacionResponse> DevolverTokenEmpleado(Empleado autorizacion);
    }
}
