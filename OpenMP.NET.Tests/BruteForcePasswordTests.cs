using OpenMP.NET.BruteForcePassword;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace OpenMP.NET.Tests
{
    public class BruteForcePasswordTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        string password = "54321";

        public BruteForcePasswordTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            DictionaryPasswordHelper.CreateDictionaryPassword(password.Length);
        }

        [Fact]
        public void TestMethod_BruteForce()
        {
            //DictionaryPasswordHelper.CreateDictionaryPassword(password.Length);

            var sw = new Stopwatch();
            sw.Start();

            var tt = new BruteForceService(_testOutputHelper);
            tt.BruteForce(password, DictionaryPasswordHelper.DictionaryPassword);

            sw.Stop();
            _testOutputHelper.WriteLine(sw.Elapsed.ToString()); // Здесь логируем
        }

        [Fact]
        public void TestMethod_BruteForceParallel()
        {
            //DictionaryPasswordHelper.CreateDictionaryPassword(password.Length);

            //Summary result = BenchmarkRunner.Run<TestBruteForceBenchmark1>();
            var sw = new Stopwatch();
            sw.Start();

            var tt = new BruteForceService(_testOutputHelper);
            tt.BruteForceParallel(password, DictionaryPasswordHelper.DictionaryPassword);

            sw.Stop();
            _testOutputHelper.WriteLine(sw.Elapsed.ToString()); // Здесь логируем
        }

        [Fact]
        public void TestMethod_BruteForceOpenMP()
        {
            //DictionaryPasswordHelper.CreateDictionaryPassword(password.Length);

            var sw = new Stopwatch();
            sw.Start();

            var tt = new BruteForceService(_testOutputHelper);
            tt.BruteForceOpenMP(password, DictionaryPasswordHelper.DictionaryPassword);

            sw.Stop();
            _testOutputHelper.WriteLine(sw.Elapsed.ToString());
        }

        [Fact]
        public void TestMethod_ParallelLinQ()
        {
            //DictionaryPasswordHelper.CreateDictionaryPassword(password.Length);

            var sw = new Stopwatch();
            sw.Start();

            var tt = new BruteForceService(_testOutputHelper);
            tt.ParallelLinQ(password, DictionaryPasswordHelper.DictionaryPassword);

            sw.Stop();
            _testOutputHelper.WriteLine(sw.Elapsed.ToString()); // Здесь логируем
        }
    }
}
