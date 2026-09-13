using ProvaPub.Domain;
using ProvaPub.Application.Interfaces.Repositories;
using ProvaPub.Infra.Data.Context;

namespace ProvaPub.Infra.Data.Repositories
{
    public class RandomRepository : IRandomRepository
    {
        private readonly TestDbContext _ctx;
        
        public RandomRepository(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<RandomNumber> InsertRandomNumber(int number)
        {
            var randomNumber = _ctx.Numbers.Add(new RandomNumber() { Number = number });
            _ctx.SaveChanges();

            return Task.FromResult(randomNumber.Entity);
        }

        public Task<bool> RandomNumberExists(int number)
        {
            bool exists = _ctx.Numbers.Any(n => n.Number == number);
            return Task.FromResult(exists);
        }
    }
}
