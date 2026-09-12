using ProvaPub.Models;

namespace ProvaPub.Repository.Inteface
{
    public interface IRandomRepository
    {
        Task<RandomNumber> InsertRandomNumber(int number);

        Task<bool> RandomNumberExists(int number);
    }
}
