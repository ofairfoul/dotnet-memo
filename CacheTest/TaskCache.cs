using System;
using System.Threading.Tasks;

namespace CacheTest {
  public class TaskCache {
    private readonly IRepository _repository;
    private Func<Task<string>> _cachedGetSomething;

    public TaskCache(IRepository repository) {
      _cachedGetSomething = TaskExtensions.CacheResult(repository.GetString);
    }

    public async Task<string> GetSomething(){
      return await _cachedGetSomething();
    }
  }
}