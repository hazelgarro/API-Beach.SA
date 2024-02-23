using APIHotelBeach.Models;
using APIHotelBeach.Models.Custom;
using APIHotelBeach.Context;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIHotelBeach.Services
{
    public class AutorizacionServices : IAutorizacionServices
    {

        private readonly IConfiguration _configuration;
        private readonly DbContextHotel _context;

        public AutorizacionServices(IConfiguration configuration, DbContextHotel context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<AutorizacionResponse> DevolverToken(Cliente autorizacion)
        {

            var temp = await _context.Clientes.FirstOrDefaultAsync(u => u.Email.Equals(autorizacion.Email) && u.Password.Equals(autorizacion.Password));

            if (temp == null)
            {

                return await Task.FromResult<AutorizacionResponse>(null);
            }

            string tokenCreado = GenerarToken(autorizacion.Email.ToString());

            return new AutorizacionResponse() { Token = tokenCreado, Resultado = true, Msj = "Ok" };
        }

        private string GenerarToken(string IdUsuario)
        {
            var key = _configuration.GetValue<string>("JwtSettings:Key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, IdUsuario));

            var credencialesToken = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes), 
                    SecurityAlgorithms.HmacSha256Signature 
                );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims, 
                Expires = DateTime.UtcNow.AddMinutes(10), 
                SigningCredentials = credencialesToken 
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            var tokenCreado = tokenHandler.WriteToken(tokenConfig);

            return tokenCreado;
        }
    }
}
