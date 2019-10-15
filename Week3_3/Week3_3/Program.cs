using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Week3_3 {
    class Program {
        static uint n = 0;

        public static void isPrime(object d) {
            Stopwatch s = new Stopwatch();
            s.Start();


            while (s.Elapsed < TimeSpan.FromSeconds(180)) {

                bool isPrime = true;
                for (int i = 2; i <= n / 2; ++i) {
                    if (n % i == 0) {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime)
                    Console.WriteLine(n + "-" + Convert.ToString(Convert.ToInt32(d)));

                n++;
            }

            s.Stop();
        }

        static void Main(string[] args) {
            for(int i = 0; i < 4; i++) {
                Thread t = new Thread(isPrime);
                t.Start(i);
            }
        }
    }
}
