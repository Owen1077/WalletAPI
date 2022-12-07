using System.Threading.Tasks;

namespace WalletTransaction.Services.Interfaces
{
    public interface IGetApi
    {
        Task <double > GetApiAsync (string currency);
        Task<double?> GetRateAsync(string currencyCode, double? amount);
    }
}
