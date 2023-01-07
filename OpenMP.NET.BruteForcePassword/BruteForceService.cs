using Xunit.Abstractions;

namespace OpenMP.NET.BruteForcePassword
{
    public class BruteForceService
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BruteForceService(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void BruteForce(string password, List<string> dictionaryPassword)
        {
            foreach (var temp in dictionaryPassword)
            {
                //Console.WriteLine($"Выполняется задача {Task.CurrentId}, | {temp} = {password}");
                if (temp.Equals(password))
                {
                    Console.WriteLine($"Найден пароль {Task.CurrentId}, | {temp} = {password}");
                    break;
                }
            }
        }

        public void BruteForceParallel(string password, List<string> dictionaryPassword)
        {
            System.Threading.Tasks.Parallel.ForEach(dictionaryPassword, (i, state) =>
            {
                //_testOutputHelper.WriteLine($"Выполняется задача {Task.CurrentId}, | {i} = {password}");
                if (i.Equals(password))
                {
                    _testOutputHelper.WriteLine($"Найден пароль {Task.CurrentId}, | {i} = {password}");
                    state.Stop();
                }
            });
        }

        public void BruteForceOpenMP(string password, List<string> dictionaryPassword)
        {
            uint threads = 8;
            int end = dictionaryPassword.Count;

                OpenMP.Parallel.ParallelFor(0, end, schedule: OpenMP.Parallel.Schedule.Static,
                num_threads: threads, action: i =>
                {
                    //_testOutputHelper.WriteLine($"Выполняется задача {Thread.CurrentThread.Name}, | {dictionaryPassword[i]} = {password}");
                    if (dictionaryPassword[i].Equals(password))
                    {
                        OpenMP.Parallel.Brack();
                        _testOutputHelper.WriteLine($"Найден пароль {Thread.CurrentThread.Name}, | {dictionaryPassword[i]} = {password}");
                    }
                });
        }

        public void ParallelLinQ(string password, List<string> dictionaryPassword)
        {
            var result = dictionaryPassword
                .AsParallel()
                .SingleOrDefault(x => x.Equals(password));

            _testOutputHelper.WriteLine($"Найден пароль {Task.CurrentId}, | {result} = {password}");
        }
    }

    
}
