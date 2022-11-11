using System;
using static System.Environment;

namespace PPLab2
{
    public static class T3
    {
        public static void ThreadTask(Resources resources)
        {
            Console.Write("{0}Thread T3 started{1}", NewLine, NewLine);

            int begin = (3 - 1) * resources.H;
            int end = begin + resources.H;

            // 1. Введення: C, MA.
            for (int i = 0; i < resources.N; i++)
            {
                resources.VectorC[i] = 1;

                for (int j = 0; j < resources.N; j++)
                {
                    resources.MatrixMA[i, j] = 1;
                }
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

            // 6. Обчислення 4: m3 = max(RH).
            int scalarMi3 = resources.VectorR[begin];
            for (int i = begin; i < end; i++)
            {
                scalarMi3 = Math.Max(resources.VectorR[i], scalarMi3);
            }

            // 7. Обчислення 5: m = max(m, m3).
            Lab2.M.WaitOne();
            resources.ScalarM = Math.Max(resources.ScalarM, scalarMi3);
            Lab2.M.ReleaseMutex();

            // 8. Сигнал задачам T1, T2, T4 про обчислення m.
            Lab2.S3.Release(resources.P - 1);

            // 9. Чекати сигнал про обчислення m у задачах T1, T2, T4.
            Lab2.S1.WaitOne();
            Lab2.S2.WaitOne();
            Lab2.S4.WaitOne();

            // 10. Копіювання: m3 = m.
            Lab2.M.WaitOne();
            int scalarM3 = resources.ScalarM;
            Lab2.M.ReleaseMutex();

            // 11. Копіювання: d3 = d.
            Lab2.S.WaitOne();
            int scalarD3 = resources.ScalarD;
            Lab2.S.Release();

            // 12. Обчислення 6: WH = m3 * XH * d3.
            for (int i = begin; i < end; i++)
            {
                resources.VectorW[i] = scalarM3 * resources.VectorX[i] * scalarD3;
            }

            // 13. Чекати сигнал про обчислення WH у задачах T1, T2, T4.
            Lab2.E1.WaitOne();
            Lab2.E2.WaitOne();
            Lab2.E4.WaitOne();

            // 14. Виведення результату W.
            Console.Write(
                "{0}T3 - Answer W{1}[{2}]{3}", NewLine, NewLine, string.Join(", ", resources.VectorW), NewLine
            );

            Console.Write("{0}Thread T3 finished{1}", NewLine, NewLine);
        }
    }
}