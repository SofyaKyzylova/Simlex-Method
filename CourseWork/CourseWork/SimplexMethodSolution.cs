using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class SimplexMethodSolution : Form
    {
        public SimplexMethodSolution()
        {
            InitializeComponent();
        }


        List<List<double>> solution;

        List<List<double>> SimplexMethod(int cols, int rows, List<double> functionValues, List<List<double>> limitValues,
            List<double> limitFreeValues, List<int> sign, bool max)
        {
            //добавление дополнительных переменных
            for (int i = 0; i < rows; i++)
            {
                List<double> limitValues1 = new List<double>();
                if (sign[i] == 0) // ">="
                {
                    limitValues1.Add(-1.0);
                    limitValues.Add(limitValues1);
                    limitValues1.Clear();
                    cols++;

                    int k = i;
                    for(int g = 0; g < k; g++)
                    {
                        limitValues1.Add(0);
                        limitValues.Add(limitValues1);
                        limitValues1.Clear();
                    }
                    for (int g = k; g < rows; g++)
                    {
                        limitValues1.Add(0);
                        limitValues.Add(limitValues1);
                        limitValues1.Clear();
                    }
                }
            }
            for (int i = 0; i < rows; i++)
            {
                List<double> limitValues1 = new List<double>();
                limitValues1.Add(1.0);
                limitValues.Add(limitValues1);
                limitValues1.Clear();
            }


            // ИОР
            List<List<double>> solvingMatrix = new List<List<double>>();

            for(int i = 0; i < rows; i++)
            {
                List<double> sMatrix = new List<double>();
                for (int j = 0; j < cols; j++)
                {
                    sMatrix.Add(0);
                }
                solvingMatrix.Add(sMatrix);
            }


            

            List<List<double>> basicValues;
            //List<double> freeValues; ??
            // массив для последней строки Симплекс-таблицы

            double resolvingElement; // разрешающий элемент
            int colIndex;  // индекс выбранного столбца
            int rowIndex;  // индекс выбранной строки

            return solution;
        }

        internal SimplexMethodSolution(int cols, int rows, List<double> functionValues, List<List<double>> limitValues, 
            List<double> limitFreeValues, List<int> sign, bool max) 
        {
            this.AutoSize = true;
            int num = 0;

            RichTextBox richTextBox1 = new RichTextBox();
            richTextBox1.Location = new Point(10, 10);
            richTextBox1.Size = new Size(800, 600);
            richTextBox1.Font = new Font("Microsoft Sans Serif", 11);

            richTextBox1.Text += "Целевая функция: \r\n";
            for (int j = 0; j < cols; j++)
            {
                num = j;
                richTextBox1.Text += functionValues[j].ToString() + "x" + ++num + " ";
                if(j != cols -1)
                {
                    if (functionValues[j + 1] >= 0.0)
                    {
                        richTextBox1.Text += "+ ";
                    }
                }
            }
            richTextBox1.Text += "\r\n";

            richTextBox1.Text += "Направление экстремума: ";
            if (max) richTextBox1.Text += "max \r\n";
            else richTextBox1.Text += "min \r\n";


            richTextBox1.Text += "Ограничения: \r\n";
            num = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    num = j;
                    richTextBox1.Text += limitValues[i][j].ToString() + "x" + ++num + " ";
                    if (j != cols - 1)
                    {
                        if (limitValues[i][j + 1] >= 0.0) 
                        {
                            richTextBox1.Text += "+ ";
                        }
                    }
                }
                if(sign[i] == 0)
                    richTextBox1.Text += " >= ";
                else if(sign[i] == 1)
                    richTextBox1.Text += " = ";
                else richTextBox1.Text += " <= ";
                
                richTextBox1.Text += limitFreeValues[i].ToString() + "\r\n";
            }

            this.Controls.Add(richTextBox1);
        }

    }
}
