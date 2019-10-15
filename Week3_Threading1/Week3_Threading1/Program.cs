using System;
using System.Threading;

namespace Week3_Threading1 {
    public class ThreadWork {
        public static void DoWork(object curThread) {
            Console.WriteLine(curThread);
            Thread.Sleep(Convert.ToInt32(curThread) * 500);
            DoWork(curThread);
        }
    }

    class Program {
        static void Main(string[] args) {
            for(int i = 1; i < 6; i++) {
                Thread thread = new Thread(ThreadWork.DoWork);
                thread.Start(i);
            }
        }
    }
}
