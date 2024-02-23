using APIHotelBeach.Models;
using APIHotelBeach.Models.Custom;

namespace APIHotelBeach.Services
{
    public interface IAutorizacionServices
    {
            Task<AutorizacionResponse> DevolverToken(Cliente autorizacion);
    }
}
