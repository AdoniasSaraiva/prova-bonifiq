namespace ProvaPub.Application.Interfaces.Services
{
    public interface IRandomService
    {
        Task<int> GetRandomNumber();
    }
}
