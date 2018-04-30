using System.Threading.Tasks;

namespace CacheTest {
  public interface IRepository {
    Task<string> GetString();
  }
}