using System;
using System.Collections.Generic;

class OriginalCode
{
    // Returns list of primes up to limit using Pritchard (wheel) sieve
    public static List<int> PrimesUpTo(int limit, bool verbose = false)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var members = new SortedSet<int> { 1 };
        int stp = 1, prime = 2, n, nxtpr, rtlim = 1 + (int)Math.Sqrt(limit), nl, ac = 2, rc = 1;
        List<int> primes = new List<int>(), tl = new List<int>();
        while (prime < rtlim)
        {
            nl = Math.Min(prime * stp, limit);
            if (stp < limit)
            {
                tl.Clear();
                foreach (var w in members)
                    for (n = w + stp; n <= nl; n += stp) tl.Add(n);
                members.UnionWith(tl); ac += tl.Count;
            }
            stp = nl; // update wheel size to wheel limit
            nxtpr = 5; // for obtaining the next prime
            tl.Clear();
            foreach (var w in members)
            {
                if (nxtpr == 5 && w > prime) nxtpr = w;
                if ((n = prime * w) > nl) break; else tl.Add(n);
            }
            foreach (var itm in tl) members.Remove(itm); rc += tl.Count;
            primes.Add(prime);
            prime = prime == 2 ? 3 : nxtpr;
        }
        members.Remove(1); primes.AddRange(members); sw.Stop();
        if (verbose) Console.WriteLine("Up to {0}, added:{1}, removed:{2}, primes counted:{3}, time:{4} ms", limit, ac, rc, primes.Count, sw.Elapsed.TotalMilliseconds);
        return primes;
    }
}