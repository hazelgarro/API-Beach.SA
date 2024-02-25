namespace APIHotelBeach.Models
{
    public class TipoCambioAPI
    {
        public HttpClient Inicial()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://apis.gometa.org");

            return client;
        }

    }
}
