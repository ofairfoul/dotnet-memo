using System.Threading.Tasks;
using CacheTest;
using NUnit.Framework;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class Tests
    {
    private IRepository _repo;
    private TaskCache _cache;

        [SetUp]
        public void Setup()
        {
            _repo = A.Fake<IRepository>();
            _cache = new TaskCache(_repo);
        }

        [Test]
        public async Task BasicTest()
        {

            A.CallTo(() => _repo.GetString())
                .ReturnsNextFromSequence(new [] {
                    Task.FromException<string>(new Exception()),
                    Task.FromResult("2"),
                    Task.FromResult("3")
                });

            Assert.ThrowsAsync<Exception>(async () => {
                await _cache.GetSomething();
            });

            var result = await _cache.GetSomething();
            Assert.AreEqual(result, "2");

            var nextResult = await _cache.GetSomething();
            Assert.AreEqual(nextResult, "2");
        }


        [Test]
        public async Task MultithreadingTask()
        {

            var callCount = 0;

            Func<Task<string>> func = async () => {
                callCount++;
                await Task.Delay(100);
                if(callCount == 1) {
                    throw new Exception();
                }
                return "Hi!";
            };

            A.CallTo(() => _repo.GetString())
                .ReturnsLazily(x => func());

            var tasks = new [] { 0, 99, 105, 140 }
                .Select<int, Task<string>>(t => {
                    return Task.Factory.StartNew(async () => {
                        await Task.Delay(t);
                        return await _cache.GetSomething();
                    }).Unwrap();
                }).ToArray();

            

            try {
                Task.WaitAll(tasks);
            } catch (Exception) {

            }

            Assert.IsTrue(tasks[0].IsFaulted);
            Assert.IsTrue(tasks[1].IsFaulted);
            Assert.IsFalse(tasks[2].IsFaulted);
            Assert.IsFalse(tasks[3].IsFaulted);

            Assert.AreEqual(tasks[2].Result, "Hi!");
            Assert.AreEqual(tasks[3].Result, "Hi!");

            Assert.AreEqual(callCount, 2);

        }
    }
}