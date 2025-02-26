// See https://aka.ms/new-console-template for more information
using MethodTimerDemo;

Console.WriteLine("Hello, World!");
var testClass1 = new TestClass1();
testClass1.SayHi();
await testClass1.SayHiAsync();
await testClass1.MethodWithAwaitAndThisAsync("IT狂人", 10086);
//ClassWithAsyncMethod classWithAsyncMethod = new ClassWithAsyncMethod();
//await classWithAsyncMethod.MethodWithAwaitAsync("field1", 111);
Console.WriteLine("Press any key");
Console.ReadKey();

