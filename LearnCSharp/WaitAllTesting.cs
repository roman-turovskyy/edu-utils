using LearnCSharp.Exceptions;

namespace LearnCSharp;

public class WaitAllTesting
{
    private readonly bool _raiseError;

    public WaitAllTesting(bool raiseError = false)
    {
        _raiseError = raiseError;
    }

    private async Task Task1()
    {
        Console.WriteLine("Task1 start.");
        await Task.Delay(1000);
        Console.WriteLine("Task1 end.");
        if (_raiseError)
            throw new MyException("raiseError = true in Task1");
    }
    private async Task Task2()
    {
        Console.WriteLine("Task2 start.");
        await Task.Delay(2000);
        Console.WriteLine("Task2 end.");
        if (_raiseError)
            throw new InvalidOperationException("raiseError = true in Task2");
    }

    private async Task Task3()
    {
        Console.WriteLine("Task3 start.");
        await Task.Delay(0);
        Console.WriteLine("Task3 end.");
        if (_raiseError)
            throw new MyException("raiseError = true in Task3");
    }

    public async Task WaitALl()
    {
        try
        {
            Console.WriteLine("Task.WhenAll started.");
            Task t1 = Task1();
            Task t2 = Task2();
            Task t3 = Task3();
            await Task.WhenAll(t1, t2, t3).ContinueWith(x => { });
            Console.WriteLine("Task.WhenAll completed.");
        }
        catch (MyException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task AsyncAsync()
    {
        try
        {
            Console.WriteLine("AsyncAsync started.");
            Task t1 = Task1();
            Task t2 = Task2();
            Task t3 = Task3();
            await t1;
            await t2;
            await t3;
            Console.WriteLine("AsyncAsync completed.");
        }
        catch (MyException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}