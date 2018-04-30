using System.Threading.Tasks;

public static class TaskExtensions {
  public static async Task<T> CacheResult<T>(this Task<T> task) {
    return default(T);
  }
}