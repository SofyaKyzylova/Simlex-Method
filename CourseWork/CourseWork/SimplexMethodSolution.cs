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

        List<List<double>> solution = new List<List<double>>();

        List<List<double>> SimplexMethod(int cols, int rows, List<double> functionValues, List<List<double>> limitValues,
            List<double> limitFreeValues, List<int> sign, bool max)
        {
            //List<double> basicValues = new List<double>();
            List<int> basicValuesIndexes = new List<int>();
            List<double> functionT= new List<double>();

            double ResultFunctionT = 0;
            double ResultFunctionF = 0;
            double resolvingElement; 
            int colIndex;  
            int rowIndex;
            int colsBeforeAdding = cols;

            //добавление доп переменных, если есть неравенства
            for (int i = 0; i < rows; i++)
            {
                if (sign[i] == 0) // ">="
                {
                    for (int j = 0; j < rows; j++)
                    {
                        if (j == i)
                            limitValues[i].Add(-1);
                        else
                            limitValues[i].Add(0);
                    }
                    cols++;
                }
            } 

            //добавление доп переменных для двухэтапного метода
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (j == i)
                        limitValues[i].Add(1);
                    else
                        limitValues[i].Add(0);
                }
                basicValuesIndexes.Add(cols);  //запомнить индексы базисных переменных
                cols++;
            }


            // ИОР
            List<List<double>> solvingMatrix = new List<List<double>>(); //для пересчета таблицы

            double sum = 0;
            for (int j = 0; j < colsBeforeAdding; j++) //функция T
            {
                for (int i = 0; i < rows; i++)
                {
                    sum += limitValues[i][j];
                }
                functionT.Add(sum);
                sum = 0;
            }
            for (int j = colsBeforeAdding; j < cols; j++) 
            {
                functionT.Add(0);
            }
            for (int i = 0; i < rows; i++)
            {
                ResultFunctionT += limitFreeValues[i];
            }


            for (int i = 0; i < colsBeforeAdding; i++) //исходную функцию *(-1)
            {
                functionValues[i] *= -1;
            }
            for (int i = colsBeforeAdding; i < cols; i++) 
            {
                functionValues.Add(0);
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
                //разрешающий элемент
                List<double> rowElement = new List<double>();
                if (max)
                {
                    colIndex = functionT.IndexOf(functionT.Min());
                    for (int i = 0; i < rows; i++)
                    {
                        if (limitValues[i][colIndex] > 0)
                            rowElement.Add(limitFreeValues[i] / limitValues[i][colIndex]);
                        else
                            rowElement.Add(-1);
                    }
                    rowIndex = rowElement.IndexOf(rowElement.Max());
                    resolvingElement = limitValues[rowIndex][colIndex];
                }
                else
                {
                    colIndex = functionT.IndexOf(functionT.Max());
                    for(int i = 0; i < rows; i++)
                    {
                        if (limitValues[i][colIndex] > 0) //!!!!!
                            rowElement.Add(limitFreeValues[i] / limitValues[i][colIndex]);
                        else
                            rowElement.Add(100000);
                    }
                    rowIndex = rowElement.IndexOf(rowElement.Min());
                    resolvingElement = limitValues[rowIndex][colIndex];
                }

                basicValuesIndexes[rowIndex] = colIndex;


                //делим разрешающую строку на разрешающий элемент
                for (int i = 0; i < cols; i++) 
                {
                    limitValues[rowIndex][i] /= resolvingElement;
                }

                //разрешающий столбец "зануляем"
                for (int i = 0; i < rows; i++)  
                {
                    if(i == rowIndex)
                        limitValues[i][colIndex] = 1;
                    else
                        limitValues[i][colIndex] = 0;
                }

                //пересчитываем симплекс-таблицу
                for (int i = 0; i < rows; i++) 
                {
                    List<double> rowSolution = new List<double>();
                    for (int j = 0; j < cols; j++)
                    {
                        if (i == rowIndex || j == colIndex)
                        {
                            rowSolution.Add(limitValues[i][j]);
                        }
                        else
                            rowSolution.Add(limitValues[i][j] - limitValues[i][colIndex] * limitValues[rowIndex][j] / resolvingElement);
                    }
                    solvingMatrix.Add(rowSolution);
                }


                //пересчитываем значения свободных членов
                for (int i = 0; i < rows; i++) 
                {
                    if (i == rowIndex)
                        limitFreeValues[i] /= resolvingElement;
                    else
                        limitFreeValues[i] -= limitValues[i][colIndex] * limitFreeValues[rowIndex] / resolvingElement;
                }


                //пересчитываем значение функции T
                ResultFunctionT -= limitFreeValues[rowIndex] * functionT[colIndex] / resolvingElement;
                for (int i = 0; i < cols; i++) 
                {
                    if (i != colIndex)
                        functionT[i] -= limitValues[rowIndex][i] * functionT[colIndex] / resolvingElement;
                }
                functionT[colIndex] = 0;


                //пересчитываем значение функции f
                ResultFunctionF -= limitFreeValues[rowIndex] * functionValues[colIndex] / resolvingElement;
                for (int i = 0; i < cols; i++) 
                {
                    if (i != colIndex)
                        functionValues[i] -= limitValues[rowIndex][i] * functionValues[colIndex] / resolvingElement;
                }
                functionValues[colIndex] = 0;


                //проверяем план на оптимальность
                optimum = true;
                for (int i = 0; i < cols; i++)
                {
                    if (functionT[i] != 0 && ResultFunctionT != 0)
                    {
                        optimum = false;
                        break;
                    }
                }

                limitValues.Clear();
                limitValues = solvingMatrix;
            }

            // basicValuesIndexes - индексы базисных переменных
            // limitFreeValues - свободные члены (значения базисных переменных)
            // limitValues - симплекс-таблица
            // functionT, valFunctionT - значения функции Т
            // functionValues - значения функции f

            //??
            List<double> col = new List<double>();
            for (int i = 0; i < rows; i++)
            {
                col.Add(basicValuesIndexes[i] + 1);
            }
            col.Add(0);
            col.Add(0);
            solution.Add(col);

            List<double> col1 = new List<double>();
            for (int i = 0; i < rows; i++)
            {
                col1.Add(limitFreeValues[i]);
            }
            col1.Add(ResultFunctionT);
            col1.Add(ResultFunctionF);
            solution.Add(col1);

            for (int i = 0; i < cols; i++)
            {
                List<double> col2 = new List<double>();
                for (int j = 0; j < rows; j++)
                {
                    col2.Add(limitValues[j][i]);
                }
                col2.Add(functionT[i]);
                col2.Add(functionValues[i]);
                solution.Add(col2);
            }

            return solution;
        }



        internal SimplexMethodSolution(int cols, int rows, List<double> functionValues, List<List<double>> limitValues, 
            List<double> limitFreeValues, List<int> sign, bool max) 
        {
            this.AutoSize = true;
            int num = 0;

            RichTextBox richTextBox1 = new RichTextBox();
            richTextBox1.Location = new Point(10, 10);
            richTextBox1.Size = new Size(600, 400);
            richTextBox1.Font = new Font("Microsoft Sans Serif", 11);
            richTextBox1.ReadOnly = true;

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
            if (max) 
                richTextBox1.Text += " --> max \r\n";
            else 
                richTextBox1.Text += " --> min \r\n";


            richTextBox1.Text += "\r\n";
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
            
            richTextBox1.Text += "\r\n";
            richTextBox1.Text += "РЕШЕНИЕ: \r\n";
            solution = SimplexMethod(cols, rows, functionValues, limitValues, limitFreeValues, sign, max);
            cols = solution[0].Count();
            rows = solution.Count();

            DataGridView DGV = new DataGridView();
            DGV.Font = new Font("Microsoft Sans Serif", 11);
            DGV.Location = new Point(10, richTextBox1.Height + 20);
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DGV.ReadOnly = true;
            DGV.ColumnCount = rows;
            DGV.RowCount = cols;

            DGV.Columns[0].HeaderText = "Базис";
            DGV.Columns[1].HeaderText = "Св. чл.";
            int k = 1;
            for (int i = 2; i < rows; i++)
            {
                DGV.Columns[i].HeaderText = "X" + k.ToString();
                k++;
            }

            for(int i = 1; i < cols - 3; i++)
            {
                DGV.Rows[i].HeaderCell.Value = "X";
            }
            DGV.Rows[cols - 3].HeaderCell.Value = "T";
            DGV.Rows[cols - 2].HeaderCell.Value = "F";

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    DGV.Rows[i].Cells[j].Value = solution[j][i].ToString();
                }
            }

            //richTextBox1.Text += DGV;
            this.Controls.Add(richTextBox1);
            this.Controls.Add(DGV);
        }

    }
}
