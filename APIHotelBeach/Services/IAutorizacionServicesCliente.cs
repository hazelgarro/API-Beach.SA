using APIHotelBeach.Models;
using APIHotelBeach.Models.Custom;

namespace APIHotelBeach.Services
{
    public interface IAutorizacionServicesCliente
    {
            Task<AutorizacionResponse> DevolverToken(Cliente autorizacion);
    }
}
