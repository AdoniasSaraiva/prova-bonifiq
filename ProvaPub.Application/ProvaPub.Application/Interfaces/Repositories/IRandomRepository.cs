using ProvaPub.Domain;

namespace ProvaPub.Application.Interfaces.Repositories
{
    public interface IRandomRepository
    {
        Task<RandomNumber> InsertRandomNumber(int number);

        Task<bool> RandomNumberExists(int number);
    }
}
