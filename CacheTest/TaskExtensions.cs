using System;
using System.Threading.Tasks;

public static class TaskExtensions {
  public static Func<Task<T>> CacheResult<T>(this Func<Task<T>> task) {
    Lazy<Task<T>> lazy;
    async Task<T> wrapper() {
        try {
          return await task();
        } catch (Exception) {
          lazy = new Lazy<Task<T>>(wrapper);
          throw;
        }
      };
    lazy = new Lazy<Task<T>>(wrapper);

    return async () => {
      return await lazy.Value;
    };
  }
}