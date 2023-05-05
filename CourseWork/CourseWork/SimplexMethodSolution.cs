using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime;

namespace CourseWork
{
    public partial class SimplexMethodSolution : Form
    {
        public SimplexMethodSolution()
        {
            InitializeComponent();
        }

        List<List<double>> solution = new List<List<double>>();

        bool IsOptimum(List<double> functionValues, int cols)
        {
            for (int i = 0; i < cols; i++)
            {
                double num = functionValues[i];
                if (Math.Round(num, 6) > 0) 
                {
                    return false;
                }
            }
            return true;
        }


        int FindRowIndex(List<List<double>> limitValues, List<double> limitFreeValues, int rows, int colIndex)
        {
            int rowIndex = -1;
            for(int i = 0; i < rows; i++)
            {
                if(limitValues[i][colIndex] > 0)
                {
                    rowIndex = i;
                    break;
                }
            }

            if (rowIndex != rows - 1 && rowIndex != -1)
            {
                for (int i = rowIndex + 1; i < rows; i++)
                {
                    if (limitValues[i][colIndex] > 0 && ((limitFreeValues[i] / limitValues[i][colIndex]) < (limitFreeValues[rowIndex] / limitValues[rowIndex][colIndex])))
                    {
                        rowIndex = i;
                    }
                }
            }

            return rowIndex;
        }


        List<List<double>> MakeSolution(int cols, int rows, List<double> functionValues, List<List<double>> limitValues,
            List<double> limitFreeValues, List<int> basicValuesIndexes, double ResultFunctionF, bool min)
        {
            List<double> col = new List<double>();
            for (int i = 0; i < rows; i++)
            {
                col.Add(basicValuesIndexes[i] + 1);
            }
            col.Add(0);
            solution.Add(col);

            List<double> col1 = new List<double>();

            col1.AddRange(limitFreeValues);
            if (min)
                col1.Add(-ResultFunctionF);
            else
                col1.Add(ResultFunctionF);
            solution.Add(col1);

            for (int i = 0; i < cols; i++)
            {
                List<double> col2 = new List<double>();
                for (int j = 0; j < rows; j++)
                {
                    col2.Add(limitValues[j][i]);
                }
                if (min)
                    col2.Add(-functionValues[i]);
                else
                    col2.Add(functionValues[i]);
                solution.Add(col2);
            }

            return solution;
        }


        List<List<double>> SimplexSolveF(int cols, int rows,
            List<double> functionValues, List<List<double>> limitValues, List<double> limitFreeValues, List<int> basicValuesIndexes,
            double ResultFunctionF, bool min)
        {
            double resolvingElement;
            int colIndex;
            int rowIndex;
            bool optimum = IsOptimum(functionValues, cols);
            bool negative = false;
            int negIndex = -1;

            for (int i = 0; i < rows; i++)
            {
                if (limitFreeValues[i] < 0)
                {
                    optimum = false;
                    negative = true;
                    negIndex = i;
                }
            }

            while (!optimum)
            {
                colIndex = functionValues.IndexOf(functionValues.Max());

                if (negative)
                    rowIndex = negIndex;
                else rowIndex = FindRowIndex(limitValues, limitFreeValues, rows, colIndex);
                if (rowIndex == -1)
                    return solution;
                

                resolvingElement = limitValues[rowIndex][colIndex];
                basicValuesIndexes[rowIndex] = colIndex;

                //пересчитываем значение функции f
                ResultFunctionF -= limitFreeValues[rowIndex] * functionValues[colIndex] / resolvingElement;
                for (int i = 0; i < cols; i++)
                {
                    if (i != colIndex)
                        functionValues[i] -= limitValues[rowIndex][i] * functionValues[colIndex] / resolvingElement;
                }
                functionValues[colIndex] = 0;


                //пересчитываем значения свободных членов
                for (int i = 0; i < rows; i++)
                {
                    if (i != rowIndex)
                        limitFreeValues[i] -= limitValues[i][colIndex] * limitFreeValues[rowIndex] / resolvingElement;
                }
                limitFreeValues[rowIndex] /= resolvingElement;


                //пересчитываем симплекс-таблицу
                List<List<double>> solvingMatrix = new List<List<double>>();

                for (int i = 0; i < cols; i++)
                {
                    limitValues[rowIndex][i] /= resolvingElement;
                }
                for (int i = 0; i < rows; i++)
                {
                    List<double> rowSolution = new List<double>();
                    for (int j = 0; j < cols; j++)
                    {
                        if (i == rowIndex)
                        {
                            rowSolution.Add(limitValues[i][j]);
                        }
                        else
                            rowSolution.Add(limitValues[i][j] - limitValues[i][colIndex] * limitValues[rowIndex][j]);
                    }
                    solvingMatrix.Add(rowSolution);
                }

                limitValues.Clear();
                for (int i = 0; i < rows; i++)
                {
                    List<double> iteration = new List<double>();
                    iteration.AddRange(solvingMatrix[i]);
                    limitValues.Add(iteration);
                }
                solvingMatrix.Clear();

                //проверяем план на оптимальность
                optimum = IsOptimum(functionValues, cols);
            }

            return MakeSolution(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, ResultFunctionF, min);
        }

        List<List<double>> NoIOR(int cols, int rows)
        {
            for(int i = 0; i < rows; i++)
            {
                List<double> iteration = new List<double>();
                for (int j = 0; j < cols; j++)
                {
                    iteration.Add(0);
                }
                solution.Add(iteration);
            }
            return solution;
        }


        List<List<double>> SimplexFindIOR(int cols, int rows, 
            List<double> functionValues, List<List<double>> limitValues, List<double> limitFreeValues, List<int> basicValuesIndexes, List<double> functionT,
            double ResultFunctionT, int colsBeforeAdding, bool min)
        {
            double ResultFunctionF = 0;
            double resolvingElement;
            int colIndex;
            int rowIndex;
            bool optimum = false;

            while (!optimum)
            {
                colIndex = functionT.IndexOf(functionT.Max());

                rowIndex = FindRowIndex(limitValues, limitFreeValues, rows, colIndex);
                if (rowIndex == -1)
                    return solution = NoIOR(cols, rows);

                resolvingElement = limitValues[rowIndex][colIndex];
                basicValuesIndexes[rowIndex] = colIndex;

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


                //пересчитываем значения свободных членов
                for (int i = 0; i < rows; i++)
                {
                    if (i != rowIndex)
                        limitFreeValues[i] -= limitValues[i][colIndex] * limitFreeValues[rowIndex] / resolvingElement;
                }
                limitFreeValues[rowIndex] /= resolvingElement;


                //пересчитываем симплекс-таблицу
                List<List<double>> solvingMatrix = new List<List<double>>();

                for (int i = 0; i < cols; i++)
                {
                    limitValues[rowIndex][i] /= resolvingElement;
                }
                for (int i = 0; i < rows; i++)
                {
                    List<double> rowSolution = new List<double>();
                    for (int j = 0; j < cols; j++)
                    {
                        if (i == rowIndex)
                        {
                            rowSolution.Add(limitValues[i][j]);
                        }
                        else
                            rowSolution.Add(limitValues[i][j] - limitValues[i][colIndex] * limitValues[rowIndex][j]);
                    }
                    solvingMatrix.Add(rowSolution);
                }

                limitValues.Clear();
                for (int i = 0; i < rows; i++)
                {
                    List<double> iteration = new List<double>();
                    iteration.AddRange(solvingMatrix[i]);
                    limitValues.Add(iteration);
                }
                solvingMatrix.Clear();

                //проверяем план на оптимальность
                optimum = IsOptimum(functionT, colsBeforeAdding);
            }

            for(int i = 0; i < rows; i++)
            {
                if (basicValuesIndexes[i] >= colsBeforeAdding)
                    return solution = NoIOR(cols, rows);
            }

            cols = colsBeforeAdding;
            return SimplexSolveF(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, ResultFunctionF, min);
        }


        List<List<double>> SimplexMethod(int cols, int rows, List<double> functionValues, List<List<double>> limitValues,
            List<double> limitFreeValues, List<int> sign, bool min)
        {
            List<int> basicValuesIndexes = new List<int>();
            List<double> functionT = new List<double>();

            double ResultFunctionT = 0;
            int colsBeforeAdding;

            //если есть отрицательные b
            for (int i = 0; i < rows; i++)
            {
                if (limitFreeValues[i] < 0)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        limitValues[i][j] *= (-1);
                    }
                    limitFreeValues[i] *= (-1);
                    if (sign[i] == 0)
                        sign[i] = 2;
                    else if (sign[i] == 2)
                        sign[i] = 0;
                }
            }

            //добавление доп переменных, если есть неравенства
            for (int i = 0; i < rows; i++)
            {
                if (sign[i] == 0) // ">="
                {
                    for (int j = 0; j < rows; j++)
                    {
                        if (j == i)
                            limitValues[j].Add(-1);
                        else
                            limitValues[j].Add(0);
                    }
                    functionValues.Add(0);
                    cols++;
                }
            }

            //добавление доп переменных, если есть неравенства
            for (int i = 0; i < rows; i++)
            {
                if (sign[i] == 2) // "<="
                {
                    for (int j = 0; j < rows; j++)
                    {
                        if (j == i)
                            limitValues[j].Add(1);
                        else
                            limitValues[j].Add(0);
                    }
                    functionValues.Add(0);
                    cols++;
                }
            }

            colsBeforeAdding = cols;

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
                basicValuesIndexes.Add(cols);  //индексы базисных переменных
                functionValues.Add(0);
                cols++;
            }


            for (int j = 0; j < colsBeforeAdding; j++) //функция T
            {
                double sum = 0;
                for (int i = 0; i < rows; i++)
                {
                    sum += limitValues[i][j];
                }
                functionT.Add(sum);
            }
            for (int j = colsBeforeAdding; j < cols; j++)
            {
                functionT.Add(0);
            }
            for (int i = 0; i < rows; i++)
            {
                ResultFunctionT += limitFreeValues[i];
            }

            if (!min)
            {
                for (int i = 0; i < cols; i++) 
                {
                    functionValues[i] *= -1;
                }
            }

            return SimplexFindIOR(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, functionT, ResultFunctionT, colsBeforeAdding, min);
        }


        int MaxRowFractionIndex(List<List<double>> limitValues, List<double> fractions, int cols, int rows)
        {
            int maxFractionIndex = -1;
            for(int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if ((limitValues[i][j] - Math.Floor(limitValues[i][j]) != 0))
                    {
                        maxFractionIndex = i;
                        break;
                    }
                }
            }
            
            if (maxFractionIndex != -1 && maxFractionIndex != rows - 1)
            {
                for (int i = maxFractionIndex + 1; i < rows; i++)
                {
                    if (fractions[i] > fractions[maxFractionIndex])
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            if ((limitValues[i][j] - Math.Floor(limitValues[i][j]) != 0))
                            {
                                maxFractionIndex = i;
                            }
                        }
                    }
                }
            }

            return maxFractionIndex;
        }

        List<double> GetFractions(int rows, List<double> limitFreeValues, List<double> fractions)
        {
            for (int i = 0; i < rows; i++)
            {
                if ((limitFreeValues[i] - Math.Floor(limitFreeValues[i]) != 0)) //проверяем есть ли дробные части
                {
                    fractions.Add(limitFreeValues[i] - Math.Floor(limitFreeValues[i]));
                }
                else
                    fractions.Add(-1);
            }
            return fractions;
        }

        bool IsIntSolution(int rows, List<double> fractions)
        {
            for (int i = 0; i < rows; i++)
            {
                if (fractions[i] != -1)
                {
                    return true;
                }
            }
            return false;
        }

        List<List<double>> GomoryMethod(int cols, int rows, List<double> functionValues, List<List<double>> limitValues,
            List<double> limitFreeValues, List<int> basicValuesIndexes, double ResultFunctionF, bool min)
        {
            List<double> fractions = new List<double>();
            fractions = GetFractions(rows, limitFreeValues, fractions);
            //for (int i = 0; i < rows; i++)
            //{
            //    if ((limitFreeValues[i] - Math.Floor(limitFreeValues[i]) != 0)) //проверяем есть ли дробные части
            //    {
            //        fractions.Add(limitFreeValues[i] - Math.Floor(limitFreeValues[i]));
            //    }
            //    else
            //        fractions.Add(-1);
            //}

            //bool flag = false;
            //for(int i = 0; i < rows; i++)
            //{
            //    if (fractions[i] != -1)
            //    {
            //        flag = true;
            //        break;
            //    }
            //}

            bool flag = IsIntSolution(rows, fractions);

            if (!flag) //ответ целочисленный, РЕШЕНИЕ НЕ НУЖНО
            {
                return NoIOR(cols, rows);
            }


            while (flag) 
            {
                int maxFraction = MaxRowFractionIndex(limitValues, fractions, cols, rows);
                if (maxFraction == -1)
                {
                    List<List<double>> empty = new List<List<double>>();
                    return empty;  //если не нашлось ни одной строки с дробными значениями - ЦЕЛОЧИСЛЕННОГО РЕШЕНИЯ НЕТ
                }

                List<double> rowFractions = new List<double>();
                for (int j = 0; j < cols; j++)
                {
                    rowFractions.Add(limitValues[maxFraction][j] - Math.Floor(limitValues[maxFraction][j])); //дробные части чисел найденной строки
                    rowFractions[j] *= -1;
                }
                rowFractions.Add(1);

                //составим доп ограничение:
                for (int i = 0; i < rows; i++)
                {
                    limitValues[i].Add(0);
                }

                cols++;
                rows++;
                limitFreeValues.Add((limitFreeValues[maxFraction] - Math.Floor(limitFreeValues[maxFraction])) * (-1)); //дробная часть свободного члена
                limitValues.Add(rowFractions);
                functionValues.Add(0);
                basicValuesIndexes.Add(cols);

                solution = SimplexSolveF(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, ResultFunctionF, min);

                fractions.Clear();
                fractions = GetFractions(rows, limitFreeValues, fractions);
                flag = IsIntSolution(rows, fractions);
            }

            return solution;
        }


        internal SimplexMethodSolution(int cols, int rows, List<double> functionValues, List<List<double>> limitValues, 
            List<double> limitFreeValues, List<int> sign, bool min) 
        {
            this.AutoSize = true;
            this.Text = "Решение";
            this.StartPosition = FormStartPosition.CenterScreen;

            int num = 0;

            RichTextBox richTextBox1 = new RichTextBox
            {
                Location = new Point(10, 10),
                Font = new Font("Microsoft Sans Serif", 11),
                ReadOnly = true
            };
            richTextBox1.ContentsResized += (object sender, ContentsResizedEventArgs e) =>
            {
                var richTextBox = (RichTextBox)sender;
                richTextBox.Height = e.NewRectangle.Height;
            };
            richTextBox1.WordWrap = false;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.None;


            richTextBox1.Text += "Целевая функция: \r\n";
            for (int j = 0; j < cols; j++)
            {
                num = j;
                if (functionValues[j] == 0)
                {
                    if (j != cols - 1)
                    {
                        if (functionValues[j + 1] > 0)
                            richTextBox1.Text += "+ ";
                    }
                }
                else
                {
                    if (functionValues[j] == 1)
                        richTextBox1.Text += "x" + ++num + " ";
                    else if (functionValues[j] == -1)
                        richTextBox1.Text += "-x" + ++num + " ";
                    else
                        richTextBox1.Text += functionValues[j].ToString() + "x" + ++num + " ";
                    if (j != cols - 1)
                        if (functionValues[j + 1] > 0)
                            richTextBox1.Text += "+ ";
                }
            }
            if (min) 
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
                    if (limitValues[i][j] == 0)
                    {
                        if(j != cols - 1)
                        {
                            if (limitValues[i][j + 1] > 0)
                                richTextBox1.Text += "+ ";
                        }
                    }
                    else
                    {
                        if(limitValues[i][j] == 1)
                            richTextBox1.Text += "x" + ++num + " ";
                        else if (limitValues[i][j] == -1)
                            richTextBox1.Text += "-x" + ++num + " ";
                        else
                            richTextBox1.Text += limitValues[i][j].ToString() + "x" + ++num + " ";
                        if (j != cols - 1)
                            if(limitValues[i][j + 1] > 0)
                                richTextBox1.Text += "+ ";
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
            solution = SimplexMethod(cols, rows, functionValues, limitValues, limitFreeValues, sign, min);

            if (!solution.Any())
            {
                richTextBox1.Text += "\r\n";
                richTextBox1.Text += "Задача не имеет оптимальных решений. \r\n";
                richTextBox1.Width = 450;
                this.Controls.Add(richTextBox1);
            }
            else
            {
                cols = solution[0].Count();
                rows = solution.Count();
                bool flag = true;

                for(int i = 0; i < rows; i++)
                {
                    for(int j = 0; j < cols; j++)
                    {
                        if (solution[i][j] != 0)
                        {
                            flag = false;
                            break;
                        }                            
                    }
                }

                if (flag)
                {
                    richTextBox1.Text += "\r\n";
                    richTextBox1.Text += "Задача не имеет ИОР. Решений нет. \r\n";
                    richTextBox1.Width = 450;
                    this.Controls.Add(richTextBox1);
                }
                else
                {
                    DataGridView DGV = new DataGridView
                    {
                        Font = new Font("Microsoft Sans Serif", 11),
                        Location = new Point(10, richTextBox1.Height + 140),
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                        AutoSize = true,
                        ReadOnly = true,

                        ColumnCount = rows,
                        RowCount = cols
                    };

                    DGV.Columns[0].HeaderText = "Базис";
                    DGV.Columns[1].HeaderText = "Св. чл.";
                    int k = 1;
                    for (int i = 2; i < rows; i++)
                    {
                        DGV.Columns[i].HeaderText = "X" + k.ToString();
                        k++;

                    }

                    for (int i = 0; i < cols; i++)
                    {
                        for (int j = 0; j < rows; j++)
                        {
                            if (i < cols - 1 && j == 0)
                            {
                                DGV.Rows[i].Cells[j].Value = "X" + Math.Round(solution[j][i], 3).ToString();
                            }
                            else if (i == cols - 1 && j == 0)
                            {
                                DGV.Rows[i].Cells[j].Value = "F";
                            }
                            else
                            {
                                DGV.Rows[i].Cells[j].Value = Math.Round(solution[j][i], 2).ToString();
                            }
                        }
                    }

                    int dgv_width = DGV.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
                    int dgv_height = DGV.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
                    richTextBox1.Width = dgv_width / 2 + 100;

                    Button intSolutionButton = new Button
                    {
                        Location = new Point(10, richTextBox1.Height + dgv_height + 250),
                        Size = new Size(190, 50),
                        Name = "ButtonCount",
                        Font = new Font("Microsoft Sans Serif", 11),
                        Text = "Найти целочисленное решение"
                    };
                    this.Controls.Add(richTextBox1);
                    this.Controls.Add(DGV);
                    this.Controls.Add(intSolutionButton);


                    intSolutionButton.Click += (sender, eventArgs) =>
                    {
                        RichTextBox richTextBox2 = new RichTextBox
                        {
                            Location = new Point(10, richTextBox1.Height + dgv_height + 320),
                            Font = new Font("Microsoft Sans Serif", 11),
                            ReadOnly = true
                        };
                        richTextBox2.ContentsResized += (object senderRichTextBox, ContentsResizedEventArgs e) =>
                        {
                            var richTextBox = (RichTextBox)senderRichTextBox;
                            richTextBox.Height = e.NewRectangle.Height;
                        };
                        richTextBox2.WordWrap = false;
                        richTextBox2.ScrollBars = RichTextBoxScrollBars.None;


                        functionValues.Clear();
                        limitValues.Clear();
                        limitFreeValues.Clear();

                        List<int> basicValuesIndexes = new List<int>();
                        double ResultFunctionF = solution[rows - 1][1];

                        for(int i = 0; i < rows; i++)
                        {
                            List<double> limits = new List<double>();
                            for (int j = 0; j < cols; j++)
                            {
                                if (j == 0 && i != rows - 1)
                                {
                                    basicValuesIndexes.Add(Convert.ToInt32(solution[i][j]) - 1);
                                }
                                else if (j == 1 && i != rows - 1)
                                {
                                    limitFreeValues.Add(solution[i][j]);
                                }
                                else if (j > 1 && i != rows - 1)
                                {
                                    limits.Add(solution[i][j]);
                                }
                                else
                                {
                                    functionValues.Add(solution[i][j]);
                                }
                            }
                            limitValues.Add(limits);
                        }

                        solution.Clear();
                        solution = GomoryMethod(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, ResultFunctionF, min);

                        if (!solution.Any())
                        {
                            richTextBox2.Text += "Задача не имеет целочисленных решений. \r\n";
                            richTextBox2.Width = 450;
                            this.Controls.Add(richTextBox2);
                        }
                        else
                        {
                            richTextBox2.Text += "Решение задачи методом Гомори: нахождение целочисленного решения. \r\n";
                            cols = solution[0].Count();
                            rows = solution.Count();
                            bool intFlag = true;

                            for (int i = 0; i < rows; i++)
                            {
                                for (int j = 0; j < cols; j++)
                                {
                                    if (solution[i][j] != 0)
                                    {
                                        intFlag = false;
                                        break;
                                    }
                                }
                            }

                            if (intFlag)
                            {
                                richTextBox2.Text += "\r\n";
                                richTextBox2.Text += "Полученный ответ является целочисленным. Нет необходимости применять метод Гомори. \r\n";
                                richTextBox2.Width = 450;
                                this.Controls.Add(richTextBox2);
                            }
                            else
                            {
                                DataGridView DGV2 = new DataGridView
                                {
                                    Font = new Font("Microsoft Sans Serif", 11),
                                    Location = new Point(10, richTextBox1.Height + dgv_height + richTextBox2.Height + 140),
                                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                                    AutoSize = true,
                                    ReadOnly = true,

                                    ColumnCount = rows,
                                    RowCount = cols
                                };

                                DGV2.Columns[0].HeaderText = "Базис";
                                DGV2.Columns[1].HeaderText = "Св. чл.";
                                int k2 = 1;
                                for (int i = 2; i < rows; i++)
                                {
                                    DGV2.Columns[i].HeaderText = "X" + k2.ToString();
                                    k2++;

                                }

                                for (int i = 0; i < cols; i++)
                                {
                                    for (int j = 0; j < rows; j++)
                                    {
                                        if (i < cols - 1 && j == 0)
                                        {
                                            DGV2.Rows[i].Cells[j].Value = "X" + Math.Round(solution[j][i], 3).ToString();
                                        }
                                        else if (i == cols - 1 && j == 0)
                                        {
                                            DGV2.Rows[i].Cells[j].Value = "F";
                                        }
                                        else
                                        {
                                            DGV2.Rows[i].Cells[j].Value = Math.Round(solution[j][i], 2).ToString();
                                        }
                                    }
                                }

                                int dgvWidth = DGV2.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
                                int dgvHeight = DGV2.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
                                richTextBox2.Width = dgvWidth / 2 + 100;

                                this.Controls.Add(richTextBox2);
                                this.Controls.Add(DGV2);
                            };
                        }
                    };
                }                
            }
        }

    }
}
