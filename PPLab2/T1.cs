using System;
using static System.Environment;

namespace PPLab2
{
    public static class T1
    {
        public static void ThreadTask(Resources resources)
        {
            Console.Write("{0}Thread T1 started{1}", NewLine, NewLine);

            int begin = (1 - 1) * resources.H;
            int end = begin + resources.H;

            // 1. Очікувати на закінчення введення даних у інших задачах.
            Lab2.B.SignalAndWait();

            // 2. Обчислення 1: RH = C * MDH.
            for (int i = begin; i < end; i++)
            {
                for (int j = 0; j < resources.N; j++)
                {
                    resources.VectorR[i] += resources.VectorC[j] * resources.MatrixMD[j, i];
                }
            }

            // 3. Обчислення 2: MRH = MA * MBH.
            for (int i = 0; i < resources.N; i++)
            {
                for (int j = begin; j < end; j++)
                {
                    for (int k = 0; k < resources.MatrixMB.GetLength(1); k++)
                    {
                        resources.MatrixMR[i, j] += resources.MatrixMA[i, k] * resources.MatrixMB[k, j];
                    }
                }
            }

            // 4. Обчислення 3: XH = E * MRH.
            for (int i = begin; i < end; i++)
            {
                for (int j = 0; j < resources.N; j++)
                {
                    resources.VectorX[i] += resources.VectorE[j] * resources.MatrixMR[j, i];
                }
            }

            // 5. Обчислення 4: m1 = max(RH).
            int scalarMi1 = resources.VectorR[begin];
            for (int i = begin; i < end; i++)
            {
                scalarMi1 = Math.Max(resources.VectorR[i], scalarMi1);
            }

            // 6. Обчислення 5: m = max(m, m1).
            Lab2.M.WaitOne();
            resources.ScalarM = Math.Max(resources.ScalarM, scalarMi1);
            Lab2.M.ReleaseMutex();

            // 7. Сигнал задачам T2, T3, T4 про обчислення m.
            Lab2.S1.Release(resources.P - 1);

            // 8. Чекати сигнал про обчислення m у задачах T2, T3, T4.
            Lab2.S2.WaitOne();
            Lab2.S3.WaitOne();
            Lab2.S4.WaitOne();

            // 9. Копіювання: m1 = m.
            Lab2.M.WaitOne();
            int scalarM1 = resources.ScalarM;
            Lab2.M.ReleaseMutex();

            // 10. Копіювання: d1 = d.
            Lab2.S.WaitOne();
            int scalarD1 = resources.ScalarD;
            Lab2.S.Release();

            // 11. Обчислення 6: WH = m1 * XH * d1.
            for (int i = begin; i < end; i++)
            {
                resources.VectorW[i] = scalarM1 * resources.VectorX[i] * scalarD1;
            }

            // 12. Сигнал задачі T3 про обчислення WH.
            Lab2.E1.Set();

            Console.Write("{0}Thread T1 finished{1}", NewLine, NewLine);
        }
    }
}