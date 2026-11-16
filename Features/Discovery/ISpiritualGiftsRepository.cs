using System.Collections.Generic;
using System.Threading.Tasks;

namespace CACI_Life.Features.Discovery
{
    public interface ISpiritualGiftsRepository
    {
        Task InitializeDatabaseAsync();
        Task<int> SaveEntryAsync(SpiritualGiftsEntry entry);
        Task<IEnumerable<SpiritualGiftsEntry>> GetAllEntriesAsync();
        Task<SpiritualGiftsEntry?> GetEntryByIdAsync(int id);
    }
}
