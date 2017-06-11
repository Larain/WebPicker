using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickerGameModel.Interfaces
{
    public interface IRepository<T>
    {
        List<T> Get();
        Task<List<T>> GetAsync();
        T Get(int id);
        Task<T> GetAsync(int id);
        int Update(T item);
        Task<int> UpdateAsync(T item);
        int Insert(T item);
        Task<int> InsertAsync(T item);
        int Delete(T item);
        Task<int> DeleteAsync(T item);
        int Delete(int itemId);
        Task<int> DeleteAsync(int itemId);
    }
}