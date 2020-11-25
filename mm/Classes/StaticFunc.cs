using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mm.Classes
{

    /// <summary>
    /// Класс синглтон для вычисления начальных условий задачи, построения сеток, генерации нулевых матриц.
    /// </summary>
    public static class StaticFunc
    {

        /// <summary>
        /// Вычисляет значение функции - начальное условие для задачи прямоугольного типа импульса.
        /// </summary>
        /// <returns></returns>
        public static double GetU0(double x, double x0, double l, double Z)
        {
            if (x > x0 && x < x0 + l)
            {
                return Z;
            }
            else return 0;
        }

        /// <summary>
        /// Вычисляет значение функции - начальное условие для задачи треугольного типа импульса.
        /// </summary>
        /// <returns></returns>
        public static double GetU1(double x, double x0, double l, double Z)
        {
            double x1 = x0;
            double x2 = x0 + l;
            double center = 0.5 * (x1 + x2);
            if (x >= x1 && x < center)
            {
                return Z * (x - x1) / (center - x1);
            }
            else if (x <= x2 && x >= center)
            {
                return Z - Z * (x - center) / (x2 - center);
            }
            else return 0;
        }

        /// <summary>
        /// Вычисляет значение функции - начальное условие для задачи синусоидального типа импульса.
        /// </summary>
        /// <returns></returns>
        public static double GetU2(double x, double x0, double l, double Z)
        {
            if (x > x0 && x < x0 + l)
            {
                return Z * Math.Pow(Math.Sin((x - x0) * Math.PI / l), 2);
            }
            else return 0;
        }

        /// <summary>
        /// Генерирует сетку X.
        /// </summary>
        /// <param name="a">Левая граница сегмента.</param>
        /// <param name="b">Правая граница сегмента.</param>
        /// <param name="N">Необходимое количество узлов в сетке.</param>
        /// <returns></returns>
        public static double[] GetMeshX(double a, double b, int N)
        {
            double h = (b - a) / N;
            double[] Mesh = new double[N];
            Mesh[0] = a;
            for (int i = 1; i < N; i++)
            {
                Mesh[i] = Mesh[i - 1] + h;
            }
            return Mesh;
        }

        /// <summary>
        /// Генерирует нулевую матрицу N x M.
        /// </summary>
        /// <returns></returns>
        public static double[,] GenerateZerosMatrix(int N, int M)
        {
            var Matrix = new double[N + 1, M + 1];
            for (int i = 0; i < N + 1; i++)
            {
                for (int j = 0; j < M + 1; j++)
                {
                    Matrix[i, j] = 0.0;
                }
            }
            return Matrix;
        }

        /// <summary>
        /// Осуществляет расчет задачи с указанными параметрами.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="L"></param>
        /// <param name="N"></param>
        /// <param name="T"></param>
        /// <param name="Kurant"></param>
        /// <param name="TypeSolution"></param>
        /// <param name="U"></param>
        /// <param name="R"></param>
        public static void SolveTask(double[] x, double a, double L, int N, double T, double Kurant, int TypeSolution, ref double[,] U, ref double[,] R)
        {
            double h = L / N;
            int M = (int)(T / Kurant);
            U = GenerateZerosMatrix(M, N);
            R = GenerateZerosMatrix(M, N);
            var u0 = GenerateZerosMatrix(1, N);
            var x0 = 0.1;
            var l = 0.2;
            var Z = 1.0;

            if (TypeSolution == 1)
            {
                for (int i = 1; i < N; i++)
                {
                    u0[0, i] = GetU2(x[i], x0, l, Z);
                }
            }
            else if (TypeSolution == 2)
            {                
                for (int i = 1; i < N; i++)
                {
                    u0[0, i] = GetU0(x[i], x0, l, Z);
                }
            }
            else if (TypeSolution == 3)
            {
                for (int i = 1; i < N; i++)
                {
                    u0[0, i] = GetU1(x[i], x0, l, Z);
                }
            }

            for (int i = 0; i < N; i++)
            {
                U[1, i] = u0[0, i];
            }

            for (int n = 1; n <= M - 1; n++)
            {
                for (int j = 2; j < N; j++)
                {
                    double W = a * Kurant / h;
                    U[n + 1, j] = U[n, j] - (U[n, j] - U[n, j - 1]) * W;
                    double k = x[j] - a * Kurant * (n - 1);
                    while (k <= 0)
                    {
                        k += L - h;
                    }
                    if (TypeSolution == 1) u0[0, j] = GetU2(k, x0, l, Z);
                    else if (TypeSolution == 2) u0[0, j] = GetU0(k, x0, l, Z);
                    else if (TypeSolution == 3) u0[0, j] = GetU1(k, x0, l, Z);
                }
                U[n + 1, 1] = U[n + 1, N];
                u0[0, 1] = u0[0, N];

                for (int i = 0; i < N; i++)
                {
                    R[n, i] = u0[0, i];
                }
            }
        }
    }
}
