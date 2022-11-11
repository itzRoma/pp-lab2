using System;
using static System.Environment;

namespace PPLab2
{
    public static class T4
    {
        public static void ThreadTask(Resources resources)
        {
            Console.Write("{0}Thread T4 started{1}", NewLine, NewLine);

            int begin = (4 - 1) * resources.H;
            int end = begin + resources.H;

            // 1. Введення: MD, d.
            for (int i = 0; i < resources.N; i++)
            {
                for (int j = 0; j < resources.N; j++)
                {
                    resources.MatrixMD[i, j] = 1;
                }
            }

            resources.ScalarD = 1;

            // 2. Очікувати на закінчення введення даних у інших задачах.
            Lab2.B.SignalAndWait();

            // 3. Обчислення 1: RH = C * MDH.
            for (int i = begin; i < end; i++)
            {
                for (int j = 0; j < resources.N; j++)
                {
                    resources.VectorR[i] += resources.VectorC[j] * resources.MatrixMD[j, i];
                }
            }

            // 4. Обчислення 2: MRH = MA * MBH.
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

            // 5. Обчислення 3: XH = E * MRH.
            for (int i = begin; i < end; i++)
            {
                for (int j = 0; j < resources.N; j++)
                {
                    resources.VectorX[i] += resources.VectorE[j] * resources.MatrixMR[j, i];
                }
            }

            // 6. Обчислення 4: m4 = max(RH).
            int scalarMi4 = resources.VectorR[begin];
            for (int i = begin; i < end; i++)
            {
                scalarMi4 = Math.Max(resources.VectorR[i], scalarMi4);
            }

            // 7. Обчислення 5: m = max(m, m4).
            Lab2.M.WaitOne();
            resources.ScalarM = Math.Max(resources.ScalarM, scalarMi4);
            Lab2.M.ReleaseMutex();

            // 8. Сигнал задачам T1, T2, T3 про обчислення m.
            Lab2.S4.Release(resources.P - 1);

            // 9. Чекати сигнал про обчислення m у задачах T1, T2, T3.
            Lab2.S1.WaitOne();
            Lab2.S2.WaitOne();
            Lab2.S3.WaitOne();

            // 10. Копіювання: m4 = m.
            Lab2.M.WaitOne();
            int scalarM4 = resources.ScalarM;
            Lab2.M.ReleaseMutex();

            // 11. Копіювання: d4 = d.
            Lab2.S.WaitOne();
            int scalarD4 = resources.ScalarD;
            Lab2.S.Release();

            // 12. Обчислення 6: WH = m4 * XH * d4.
            for (int i = begin; i < end; i++)
            {
                resources.VectorW[i] = scalarM4 * resources.VectorX[i] * scalarD4;
            }

            // 13. Сигнал задачі T3 про обчислення WH.
            Lab2.E4.Set();

            Console.Write("{0}Thread T4 finished{1}", NewLine, NewLine);
        }
    }
}