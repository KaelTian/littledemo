using MethodTimer;

namespace MethodTimerDemo
{
    internal class TestClass1
    {
        [Time]
        public async Task SayHiAsync()
        {
            Console.WriteLine("Hi I am kael tian async method");
            await Task.Delay(1000);
        }

        [Time]
        public void SayHi()
        {
            Console.WriteLine("Hi I am kael tian sync method");
            Thread.Sleep(1000);
        }

        [Time("Current object: '{this}' | File name '{fileName}' with id '{id}'")]
        public async Task MethodWithAwaitAndThisAsync(string fileName, int id)
        {
            await Task.Delay(500);

            // Use so the compiler won't optimize
            Console.Write(fileName);
            Console.Write(id);
        }
    }
}
