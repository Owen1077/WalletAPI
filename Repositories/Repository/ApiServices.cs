using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WalletTransaction.Model.DTO;
using WalletTransaction.Services.Interfaces;

namespace WalletTransaction.Services.Implementation
{
    public class ApiServices : IGetApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ApiServices(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<double> GetApiAsync(string currency)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                using (var response = await httpClient.GetAsync("https://open.er-api.com/v6/latest/NGN", HttpCompletionOption.ResponseHeadersRead))
                {
                    var con = currency.ToUpper();
                    response.EnsureSuccessStatusCode();
                    var stream = await response.Content.ReadAsStreamAsync();
                    var rates = await JsonSerializer.DeserializeAsync<ApiDto>(stream);
                    if (rates != null)
                        return rates.rates[con];
                }
            }
            catch (Exception)
            {
                throw;

            }
            return 0;
        }

        public async Task<double?> GetRateAsync(string currencyCode, double? amount)
        {
            try
            {
                var con = currencyCode.ToUpper();
                var httpClient = _httpClientFactory.CreateClient();
                var conversionStr = "https://open.er-api.com/v6/latest/NGN";
                using (var response = await httpClient.GetAsync(conversionStr, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    var stream = await response.Content.ReadAsStreamAsync();
                    var newRates = await JsonSerializer.DeserializeAsync<ApiDto>(stream);
                    var rate = newRates.rates[con];
                    var convAmount = rate * amount;



                    return convAmount;
                }



            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}