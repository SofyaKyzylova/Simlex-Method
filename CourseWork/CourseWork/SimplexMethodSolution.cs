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
            List<double> basicValues = new List<double>();
            List<double> basicValuesIndexes = new List<double>();
            List<double> functionT= new List<double>(); 

            double resolvingElement; 
            int colIndex;  
            int rowIndex; 


            //добавление дополнительных переменных, если есть неравенства
            for (int i = 0; i < rows; i++)
            {
                List<double> limitValues1 = new List<double>();
                if (sign[i] == 0) // ">="
                {
                    limitValues1.Add(-1.0);
                    limitValues.Add(limitValues1);
                    limitValues1.Clear();
                    
                    int k = i;
                    for(int g = 0; g < k; g++)
                    {
                        limitValues1.Add(0);
                        limitValues.Add(limitValues1);
                        limitValues1.Clear();
                    }
                    for (int g = k + 1; g < rows; g++)
                    {
                        limitValues1.Add(0);
                        limitValues.Add(limitValues1);
                        limitValues1.Clear();
                    }
                    cols++;
                }
            }

            //добавление доп переменных для двухэтапного метода
            for (int i = 0; i < rows; i++)
            {
                List<double> limitValues1 = new List<double>();
                limitValues1.Add(1.0);
                limitValues.Add(limitValues1);
                limitValues1.Clear();

                int k = i;
                for (int g = 0; g < k; g++)
                {
                    limitValues1.Add(0);
                    limitValues.Add(limitValues1);
                    limitValues1.Clear();
                }
                for (int g = k + 1; g < rows; g++)
                {
                    limitValues1.Add(0);
                    limitValues.Add(limitValues1);
                    limitValues1.Clear();
                }
                cols++;
            }

            //базисные переменные
            for (int j = 0; j < cols; j++)
            {
                int counter = 0;
                for (int i = 0; i < rows; i++)
                {
                    if (limitValues[i][j] == 0)
                        counter += 1;
                }
                for (int i = 0; i < rows; i++)
                {
                    if ((i == j) && (limitValues[i][j] == 1) && (counter == rows - 1))
                    {
                        basicValues.Add(limitValues[i][j]);
                    }
                }
            }

            //запомнить индексы базисных переменных //?
            //basicValuesIndexes

            //БАЗИСНЫМИ ПЕРЕМЕННЫМИ ДОЛЖНЫ БЫТЬ ВВЕДЕННЫЕ ДОП ПЕРЕМЕННЫЕ !!
            // if(basicValues.Count() != rows) error;

            // ИОР
            List<List<double>> solvingMatrix = new List<List<double>>(); //для пересчета таблицы
            for(int i = 0; i < rows; i++)
            {
                List<double> sMatrix = new List<double>();
                for (int j = 0; j < cols; j++)
                {
                    sMatrix.Add(0);
                }
                solvingMatrix.Add(sMatrix);
            }

            double sum = 0;
            for (int j = 0; j < cols; j++) //функция T
            {
                for (int i = 0; i < rows; i++)
                {
                    sum += limitValues[i][j];
                }
                functionT.Add(sum);
                sum = 0;
            }
            double valFunctionT = 0;
            for(int i = 0; i < rows; i++)
            {
                valFunctionT += limitFreeValues[i];
            }


            bool optimum = true;
            if (max) //проверка на оптимальность для max
            {
                for (int i = 0; i < cols; i++)
                {
                    if (functionT[i] < 0)
                    {
                        optimum = false;
                        break;
                    }
                }
            }
            else //проверка на оптимальность для mim
            {
                for (int i = 0; i < cols; i++)
                {
                    if (functionT[i] > 0)
                    {
                        optimum = false;
                        break;
                    }
                }
            }


            while (!optimum) 
            {
                for(int i = 0; i < cols; i++) //исходную функцию *(-1)
                {
                    functionValues[i] *= -1;
                }

                //разрешающий элемент
                List<double> rowElement = new List<double>();
                if (max)
                {
                    colIndex = functionT.IndexOf(functionT.Min());
                    for (int i = 0; i < rows; i++)
                    {
                        rowElement.Add(limitFreeValues[i] / limitValues[i][colIndex]);
                    }
                    rowIndex = rowElement.IndexOf(rowElement.Max());
                    resolvingElement = limitValues[rowIndex][colIndex];
                }
                else
                {
                    colIndex = functionT.IndexOf(functionT.Max());
                    for(int i = 0; i < rows; i++)
                    {
                        rowElement.Add(limitFreeValues[i] / limitValues[i][colIndex]);
                    }
                    rowIndex = rowElement.IndexOf(rowElement.Min());
                    resolvingElement = limitValues[rowIndex][colIndex];
                }
                //ПРОВЕРКА, ЧТО ЭЛЕМЕНТ НЕ ОТРИЦАТЕЛЬНЫЙ!!!!!


                //remove element with rowIndex
                basicValues.Add(resolvingElement); //переменную разрешающего стролбца включаем в базис 


                for(int i = 0; i < cols; i++) //делим разрешающую строку на разрешающий элемент
                {
                    limitValues[rowIndex][i] /= resolvingElement;
                }
                limitFreeValues[rowIndex] /= resolvingElement;

                for (int i = 0; i < rows; i++)  //разрешающий столбец "зануляем"
                {
                    if(i==colIndex)
                        limitValues[i][colIndex] = 1;
                    else
                        limitValues[i][colIndex] = 0;
                }

                /*
                for (int i = 0; i < rowIndex; i++) //пересчитываем симплекс-таблицу
                {
                    List<double> solution = new List<double>();
                    for (int j = 0; j < colIndex; j++)
                    {
                        solution.Add(limitValues[i][j] - limitValues[i][colIndex] * limitValues[rowIndex][j] / resolvingElement);
                    }
                    solvingMatrix.Add(solution);
                }
                //разрешающий столбец и строка? //add
                for (int i = rowIndex + 1; i < rows; i++) 
                {
                    List<double> solution = new List<double>();
                    for (int j = colIndex + 1; j < cols; j++)
                    {
                        solution.Add(limitValues[i][j] - limitValues[i][colIndex] * limitValues[rowIndex][j] / resolvingElement);
                    }
                    solvingMatrix.Add(solution);
                }*/

                for (int i = 0; i < rows; i++) //пересчитываем симплекс-таблицу
                {
                    List<double> solution = new List<double>();
                    for (int j = 0; j < cols; j++)
                    {
                        if (i == rowIndex && j == colIndex)
                        {
                            solution.Add(limitValues[i][j]);
                        }
                        else
                            solution.Add(limitValues[i][j] - limitValues[i][colIndex] * limitValues[rowIndex][j] / resolvingElement);
                    }
                    solvingMatrix.Add(solution);
                }


                for (int i = 0; i < rowIndex; i++) //пересчитываем значения свободных членов
                {
                    for (int j = 0; j < colIndex; j++)
                    {

                    }
                }
                //разрешающий столбец и строка? //add
                for (int i = rowIndex + 1; i < rows; i++)
                {
                    for (int j = colIndex + 1; j < cols; j++)
                    {
                    }
                }

                //пересчитываем значение функции


                //проверяем план на оптимальность
                optimum = true;
                if (max) //проверка на оптимальность для max
                {
                    for (int i = 0; i < cols; i++)
                    {
                        if (functionT[i] < 0)
                        {
                            optimum = false;
                            break;
                        }
                    }
                }
                else //проверка на оптимальность для mim
                {
                    for (int i = 0; i < cols; i++)
                    {
                        if (functionT[i] > 0)
                        {
                            optimum = false;
                            break;
                        }
                    }
                }
            }

            // solution = basicValues + limitFreeValues +  solvingMatrix + valFunctionT + functionT

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
