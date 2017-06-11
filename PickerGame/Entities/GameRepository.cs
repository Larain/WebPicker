using PickerGameModel.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PickerGameModel.Entities.Services;
using PickerGameModel.Interfaces.Services;

namespace PickerGameModel.Entities
{
    public class GameRepository : IRepository<IGameService>
    {
        private List<IGameService> Games { get; set; }
        public GameRepository()
        {
            Games = new List<IGameService> {new GameService()};
        }

        public List<IGameService> Get()
        {
            return Games;
        }

        public Task<List<IGameService>> GetAsync()
        {
            throw new System.NotImplementedException();
        }

        public IGameService Get(int id)
        {
            return Games.First(x => x.Game.GameId == id);
        }

        public Task<IGameService> GetAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public int Update(IGameService item)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> UpdateAsync(IGameService item)
        {
            throw new System.NotImplementedException();
        }

        public int Insert(IGameService item)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> InsertAsync(IGameService item)
        {
            throw new System.NotImplementedException();
        }

        public int Delete(IGameService item)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> DeleteAsync(IGameService item)
        {
            throw new System.NotImplementedException();
        }

        public int Delete(int itemId)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> DeleteAsync(int itemId)
        {
            throw new System.NotImplementedException();
        }
    }
}