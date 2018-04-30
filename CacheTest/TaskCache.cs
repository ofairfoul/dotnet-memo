using System;
using System.Threading.Tasks;

namespace CacheTest {
  public class TaskCache {
    private Lazy<Task<string>> _lazy;

    public TaskCache(IRepository repository) {

      async Task<string> Factory() {
        try {
          return await repository.GetString();
        } catch (Exception e) {
          _lazy = new Lazy<Task<string>>(Factory);
          throw;
        }
      };

      _lazy = new Lazy<Task<string>>(Factory);
    }

    private async Task<string> Retry(Func<Task<string>> factory) {
      try {
        return await factory();
      } catch (Exception e) {
        _lazy = new Lazy<Task<string>>(Retry(factory));
        throw;
      }
    }


    private async Task<string> GetSomething() {
      return await Task.FromResult("Hi");
    }

    public async Task<string> CachedGetSomething (){
      return await _lazy.Value;
    }
  }
}