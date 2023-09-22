using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    // Returns list of primes up to limit using Pritchard (wheel) sieve
    static List<int> PrimesUpTo(int limit, bool verbose = false)
    {
        var stop_watch = System.Diagnostics.Stopwatch.StartNew();
        var wheel = new SortedSet<int> { 1 };
        int current_prime = 2;
        int limit_to_run = 1 + (int)Math.Sqrt(limit);
        int next_prime;
        int added_count = 2;
        int removed_count = 1;
        int next_wheel_length;
        int wheel_length = 1, n;
        List<int> primes = new List<int>();
        List<int> temp_list = new List<int>();
        
        int biggest_prime = (int)GetSecondBiggestPrimeNumber((ulong)limit, 1);
        int second_biggest_prime = (int)GetSecondBiggestPrimeNumber((ulong)limit, 2);

        while (current_prime < limit_to_run)
        {
            next_wheel_length = Math.Min(current_prime * wheel_length, limit);

            // Extend Step
            if (wheel_length < limit)
            {
                temp_list.Clear();
                foreach (int number_in_wheel in wheel)
                {
                    for (n = number_in_wheel + wheel_length; n <= next_wheel_length; n += wheel_length)
                    {
                        temp_list.Add(n);
                    }
                }
                wheel.UnionWith(temp_list); added_count += temp_list.Count;
            }

            if (second_biggest_prime == current_prime)
                break;

            wheel_length = next_wheel_length; // update wheel size to wheel limit
            temp_list.Clear();

            next_prime = 5; // for obtaining the next prime
            foreach (int number_in_wheel in wheel)
            {
                if (next_prime == 5 && number_in_wheel > current_prime)
                {
                    next_prime = number_in_wheel;
                }
                if ((n = current_prime * number_in_wheel) > next_wheel_length)
                    break;
                else
                    temp_list.Add(n);
            }
            // Remove Step
            foreach (var itm in temp_list)
            {
                wheel.Remove(itm);
                removed_count += temp_list.Count;
            }
            primes.Add(current_prime);
            current_prime = current_prime == 2 ? 3 : next_prime;
        }

        temp_list.Clear();

        int threadsNumber = 6;
        Parallel.For(0, threadsNumber, threadIndex =>
        {
            List<int> sectionList = new List<int>();
            SortedSet<int>.Enumerator wheelEnum = wheel.GetEnumerator();
            for (int i = 0; i < threadIndex; i++)
            {
                wheelEnum.MoveNext();
            }
            while (wheelEnum.MoveNext())
            {
                int number_in_wheel = wheelEnum.Current;
                if (number_in_wheel % biggest_prime == 0 || number_in_wheel % second_biggest_prime == 0)
                    sectionList.Add(number_in_wheel);
                for (int i = 0; i < threadsNumber - 1; i++)
                {
                    wheelEnum.MoveNext();
                }
            }
            lock (temp_list)
            {
                temp_list.AddRange(sectionList);
            }
        });


        // Remove Step
        foreach (var itm in temp_list)
        {
            wheel.Remove(itm);
            removed_count += temp_list.Count;
        }
        primes.Add(current_prime);

        wheel.Remove(1);
        primes.AddRange(wheel);
        stop_watch.Stop();
        if (verbose)
            Console.WriteLine("Up to {0}, added:{1}, removed:{2}, primes counted:{3}, time:{4} ms", limit, added_count, removed_count, primes.Count, stop_watch.Elapsed.TotalMilliseconds);
        return primes;
    }

    public static ulong GetSecondBiggestPrimeNumber(ulong limit, int amountToGoBack)
    {
        ulong limit_to_run = (ulong)Math.Sqrt(limit);

        for (ulong i = limit_to_run; i > 0; i--)
        {
            if (PrimeTest.MillerRabin(i))
            {
                if (amountToGoBack > 1)
                {
                    amountToGoBack--;
                }
                else
                {
                    return i;
                }
            }
        }
        return 1;
    }

    static void Main(string[] args)
    {
        // First Implementation 'should' be more memory efficient
        PrimesUpTo(1000000,true);
        OriginalCode.PrimesUpTo(1000000, true);
    }
}