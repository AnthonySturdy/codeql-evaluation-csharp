using System;
using System.Threading;

namespace Week3_Threading1 {
    public class ThreadWork {
        public static void DoWork() {
            for(int i = 100; i > 0; i--) {
                Console.WriteLine(i + "*");
            }
        }
    }

    class Program {
        static void Main(string[] args) {
            Thread thread1 = new Thread(ThreadWork.DoWork); // NEED TO DO EXCERSIZE 1.2
            thread1.Start();

            for(int i = 0; i < 100; i++) {
                Console.WriteLine(i);
            }

        }
    }
}
