using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Week3_2 {
    class Program {
        static int numThreads = 0;
        static int curNumber = 0; //I tried setting each thread to count 10 from the number they were given, but I ran out of memory at about 130. So now using global variable for the count

        public static void DoWork() {
            numThreads++;

            for(int i = 0; i < 10; i++) {
                curNumber++;
                Console.WriteLine(curNumber);

                if(curNumber >= 500) {
                    break;
                }
            }

            if(curNumber < 500) {
                for (int i = 0; i < 2; i++) {
                    Thread thread = new Thread(DoWork);
                    thread.Start();
                }
            } else {
                Console.WriteLine("number of threads: " + numThreads);
            }
        }

        static void Main(string[] args) {
            Thread thread = new Thread(DoWork);
            thread.Start();

        }
    }
}
