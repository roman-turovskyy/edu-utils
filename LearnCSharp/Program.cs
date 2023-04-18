using LearnCSharp;

var waitAllTesting = new WaitAllTesting(raiseError: true);

await waitAllTesting.AsyncAsync();

// Just in case sleep before running the second test
Thread.Sleep(2000);
Console.WriteLine("-----");

await waitAllTesting.WaitALl();