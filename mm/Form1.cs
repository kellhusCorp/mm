using mm.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mm
{

    public partial class Form1 : Form
    {        
        /// <summary>
        /// Количество узлов в сетке по пространству (X).
        /// </summary>
        private readonly int N = 151;

        /// <summary>
        /// Количество узлов в сетке по времени (t). Пересчитается, если изменится величина во втором списке.
        /// </summary>
        private int M = 154;

        /// <summary>
        /// Матрица (каркас решения).
        /// </summary>
        private double[,] R;

        /// <summary>
        /// Матрица (каркас решения).
        /// </summary>
        private double[,] U;

        /// <summary>
        /// Длина интервала пространства.
        /// </summary>
        private readonly double L = 1;

        /// <summary>
        /// Массив для хранения сетки.
        /// </summary>
        private double[] x;

        /// <summary>
        /// Определяет шаг по времени, исходя из выбранного числа Куранта (тут не уверен, что те числа в comboBox это числа Куранта).
        /// </summary>
        private double GetKurant
        {
            get
            {
                switch (comboBox2.SelectedIndex)
                {
                    case 0: return 0.0014;
                    case 1: return 0.0065;
                    case 2: return 0.0130;
                    default: return 0.0065;
                }
            }
        }

        /// <summary>
        /// Определяет тип решения, исходя из выбранного типа импульса.
        /// </summary>
        private int GetTypeSolution
        {
            get
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0: return 1;
                    case 1: return 2;
                    case 2: return 3;
                    default: return 1;
                }
            }

        }

        /// <summary>
        /// Конструктор формы по умолч.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Генерирует сетку, проставляет начальную привязку comboBox'ов, <br></br>запускает расчет для синусоидального типа и шагом по сетке (2)-го типа + отрисовка результата.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            x = StaticFunc.GetMeshX(0, L, N);
            comboBox2.SelectedIndex = 1;
            comboBox1.SelectedIndex = 0;
            RunningSolution();
            PlotSolution(1);
        }

        /// <summary>
        /// Строит график текущего решения для заданного момента времени.
        /// </summary>
        /// <param name="n">Момент времени.</param>
        private void PlotSolution(int n)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            for (int i = 0; i < N; i++)
            {
                chart1.Series[0].Points.AddXY(x[i], R[n,i]);
                chart1.Series[1].Points.AddXY(x[i], U[n, i]);
            }                                                    
        }

        /// <summary>
        /// Запускает расчет для выбранного типа импульса и выбранным шагом по времени.
        /// </summary>
        private void RunningSolution()
        {                       
            double a = 0.5;
            double T = 1.0;
            M = (int)(T / GetKurant);
            U = StaticFunc.GenerateZerosMatrix(M, N);
            R = StaticFunc.GenerateZerosMatrix(M, N);

            StaticFunc.SolveTask(x, a, L, N, T, GetKurant, GetTypeSolution, ref U, ref R);
            trackBar1.Maximum = R.GetLength(0) - 2;
            
        }       

        /// <summary>
        /// Отрисовывает решение задачи для нужного момента времени (реагирует на изменение значения trackBar'a).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            PlotSolution(trackBar1.Value);
        }

        /// <summary>
        /// Запускает расчет при смене типа импульса или величины по времени. <br></br>
        /// Возвращает величину trackBar'a в изначальное положение + отрисовка графика решения в начальный момент времени.<br></br>
        /// <b>Аккуратно</b>, сюда подписано 2 control'a.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeEvent(object sender, EventArgs e)
        {
            RunningSolution();
            trackBar1.Value = 1;
            PlotSolution(1);
        }
    }


}
