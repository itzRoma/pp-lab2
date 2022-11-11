using System;
using System.Threading;
using static System.Environment;

namespace PPLab2
{
    /// <summary>
    /// Паралельне програмування: Лабораторна робота №2 (ЛР2) <br/>
    /// Варіант: 22 <br/>
    /// Функція: W = max(C * MD) * E * (MA * MB) * d <br/>
    /// Автор: Бондаренко Роман Ігорович, група ІО-03 <br/>
    /// Дата: 11/11/2022
    /// </summary>
    internal static class Lab2
    {
        private const int AmountOfProcessors = 4;

        /// <summary>
        /// Barrier for controlling input from all processors (threads)
        /// </summary>
        public static readonly Barrier B = new Barrier(
            AmountOfProcessors,
            barrier => Console.Write(
                "{0}---> All the required data was provided, calculation started <---{1}", NewLine, NewLine
            )
        );

        /// <summary>
        /// Mutex for controlling access to the shared resource 'm'
        /// </summary>
        public static readonly Mutex M = new Mutex();

        /// <summary>
        /// Semaphore to synchronize T2, T3, T4 with the completion of the calculation of 'm' in T1
        /// </summary>
        public static readonly Semaphore S1 = new Semaphore(0, AmountOfProcessors - 1);

        /// <summary>
        /// Semaphore to synchronize T1, T3, T4 with the completion of the calculation of 'm' in T2
        /// </summary>
        public static readonly Semaphore S2 = new Semaphore(0, AmountOfProcessors - 1);

        /// <summary>
        /// Semaphore to synchronize T1, T2, T4 with the completion of the calculation of 'm' in T3
        /// </summary>
        public static readonly Semaphore S3 = new Semaphore(0, AmountOfProcessors - 1);

        /// <summary>
        /// Semaphore to synchronize T1, T2, T3 with the completion of the calculation of 'm' in T4
        /// </summary>
        public static readonly Semaphore S4 = new Semaphore(0, AmountOfProcessors - 1);

        /// <summary>
        /// Semaphore for controlling access to the shared resource 'd'
        /// </summary>
        public static readonly Semaphore S = new Semaphore(1, 1);

        /// <summary>
        /// Event to synchronize T3 with completion of 'WH' calculation in T1
        /// </summary>
        public static readonly EventWaitHandle E1 = new AutoResetEvent(true);

        /// <summary>
        /// Event to synchronize T3 with completion of 'WH' calculation in T2
        /// </summary>
        public static readonly EventWaitHandle E2 = new AutoResetEvent(true);

        /// <summary>
        /// Event to synchronize T3 with completion of 'WH' calculation in T4
        /// </summary>
        public static readonly EventWaitHandle E4 = new AutoResetEvent(true);

        public static void Main(string[] args)
        {
            Console.Write("{0}Provide N (the size of vectors and matrices): ", NewLine);
            int n = int.Parse(Console.ReadLine());

            Resources resources = new Resources(n, AmountOfProcessors);

            Thread t1 = new Thread(() => T1.ThreadTask(resources));
            Thread t2 = new Thread(() => T2.ThreadTask(resources));
            Thread t3 = new Thread(() => T3.ThreadTask(resources));
            Thread t4 = new Thread(() => T4.ThreadTask(resources));

            Console.Write("{0}Starting threads...{1}", NewLine, NewLine);

            int start = DateTime.Now.Millisecond;

            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();

            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();

            int end = DateTime.Now.Millisecond;

            Console.Write("{0}All thread finished, execution time is {1} ms{2}", NewLine, end - start, NewLine);
        }
    }
}