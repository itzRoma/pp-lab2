using System;
using static System.Environment;

namespace PPLab2
{
    public static class T2
    {
        public static void ThreadTask(Resources resources)
        {
            Console.Write("{0}Thread T2 started{1}", NewLine, NewLine);

            int begin = (2 - 1) * resources.H;
            int end = begin + resources.H;

            // 1. Введення: MB, E.
            for (int i = 0; i < resources.N; i++)
            {
                for (int j = 0; j < resources.N; j++)
                {
                    resources.MatrixMB[i, j] = 1;
                }

                resources.VectorE[i] = 1;
            }

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

            // 6. Обчислення 4: m2 = max(RH).
            int scalarMi2 = resources.VectorR[begin];
            for (int i = begin; i < end; i++)
            {
                scalarMi2 = Math.Max(resources.VectorR[i], scalarMi2);
            }

            // 7. Обчислення 5: m = max(m, m2).
            Lab2.M.WaitOne();
            resources.ScalarM = Math.Max(resources.ScalarM, scalarMi2);
            Lab2.M.ReleaseMutex();

            // 8. Сигнал задачам T1, T3, T4 про обчислення m.
            Lab2.S2.Release(resources.P - 1);

            // 9. Чекати сигнал про обчислення m у задачах T1, T3, T4.
            Lab2.S1.WaitOne();
            Lab2.S3.WaitOne();
            Lab2.S4.WaitOne();

            // 10. Копіювання: m2 = m.
            Lab2.M.WaitOne();
            int scalarM2 = resources.ScalarM;
            Lab2.M.ReleaseMutex();

            // 11. Копіювання: d2 = d.
            Lab2.S.WaitOne();
            int scalarD2 = resources.ScalarD;
            Lab2.S.Release();

            // 12. Обчислення 6: WH = m2 * XH * d2.
            for (int i = begin; i < end; i++)
            {
                resources.VectorW[i] = scalarM2 * resources.VectorX[i] * scalarD2;
            }

            // 13. Сигнал задачі T3 про обчислення WH.
            Lab2.E2.Set();

            Console.Write("{0}Thread T2 finished{1}", NewLine, NewLine);
        }
    }
}