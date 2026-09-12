using ProvaPub.Repository.Inteface;
using ProvaPub.Services.Interface;

namespace ProvaPub.Services
{
    public class RandomService : IRandomService
    {
        private readonly IRandomRepository _randomRepository;

        public RandomService(IRandomRepository randomRepository)
        {
            _randomRepository = randomRepository;
        }

        public async Task<int> GetRandomNumber()
        {
            int seed = Guid.NewGuid().GetHashCode();

            var number = new Random(seed).Next(100);

            while (true)
            {
                bool exists = await _randomRepository.RandomNumberExists(number);
                if (!exists)
                    break;

                number = new Random(Guid.NewGuid().GetHashCode()).Next(100);
            }
            await _randomRepository.InsertRandomNumber(number);
            return number;
        }

    }
}
