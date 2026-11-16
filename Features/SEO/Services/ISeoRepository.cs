using System.Threading.Tasks;
using CACI_Life.Features.SEO.Models;

namespace CACI_Life.Features.SEO.Services
{
    public interface ISeoRepository
    {
        Task<PageSeoSettings?> GetSeoByPathAsync(string path);
    }
}
