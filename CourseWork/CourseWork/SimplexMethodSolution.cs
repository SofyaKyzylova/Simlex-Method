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
        bool closed = true;

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


        int FindColIndex(List<List<double>> limitValues, List<double> functionValues, int cols, int rowIndex)
        {
            int colIndex = -1;

            for (int i = 0; i < cols; i++)
            {
                if (limitValues[rowIndex][i] < 0) 
                {
                    colIndex = i;
                    break;
                }
            }

            if (colIndex != cols - 1 && colIndex != -1)
            {
                for (int i = colIndex + 1; i < cols; i++)
                {
                    if (limitValues[rowIndex][i] < 0) 
                    {
                        if (Math.Round(Math.Abs(functionValues[i] / limitValues[rowIndex][i]), 6) < Math.Round(Math.Abs(functionValues[colIndex] / limitValues[rowIndex][colIndex]), 6))
                        {
                            colIndex = i;
                        }
                    }
                }
            }

            return colIndex;
        }

        int FindColIndex(List<List<double>> limitValues, List<double> limitFreeValues, List<double> functionValues, int rows)
        {
            List<double> func = new List<double>();
            func.AddRange(functionValues);
            int colIndex = -1;

            while (func.Max() > 0)
            {
                colIndex = func.IndexOf(func.Max());
                int rowIndex = FindRowIndex(limitValues, limitFreeValues, rows, colIndex);

                if (rowIndex == -1)
                {
                    func[colIndex] = 0;
                    colIndex = func.IndexOf(func.Max());
                }
                else
                {
                    break;
                }
            }

            if (func.Max() <= 0)
            {
                colIndex = -1;
            }

            return colIndex;
        }

        List<List<double>> MakeSolution(int cols, int rows, List<double> functionValues, List<List<double>> limitValues,
            List<double> limitFreeValues, List<int> basicValuesIndexes, double ResultFunctionF, bool max)
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
            if (max)
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
                if (max)
                    col2.Add(-functionValues[i]);
                else
                    col2.Add(functionValues[i]);
                solution.Add(col2);
            }

            return solution;
        }


        List<List<double>> SimplexSolveF(int cols, int rows,
            List<double> functionValues, List<List<double>> limitValues, List<double> limitFreeValues, List<int> basicValuesIndexes,
            double ResultFunctionF, bool max)
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
                    break;
                }
            }

            while (!optimum)
            {
                if (negative)
                {
                    rowIndex = negIndex;
                    colIndex = FindColIndex(limitValues, functionValues, cols, rowIndex);
                    if (colIndex == -1)
                    {
                        List<List<double>> empty = new List<List<double>>();
                        return empty;
                    }
                }
                else
                {
                    colIndex = FindColIndex(limitValues, limitFreeValues, functionValues, rows);
                    if (colIndex == -1)
                        return solution = NoIOR(cols, rows);
                    rowIndex = FindRowIndex(limitValues, limitFreeValues, rows, colIndex);
                    if (rowIndex == -1)
                        return solution;
                }

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
                negative = false;

                for (int i = 0; i < rows; i++)
                {
                    if (limitFreeValues[i] < 0)
                    {
                        optimum = false;
                        negative = true;
                        negIndex = i;
                        break;
                    }
                }
            }

            return MakeSolution(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, ResultFunctionF, max);
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
            double ResultFunctionT, int colsBeforeAdding, bool max)
        {
            double ResultFunctionF = 0;
            double resolvingElement;
            int colIndex;
            int rowIndex;
            bool optimum = false;

            while (!optimum)
            {
                colIndex = FindColIndex(limitValues, limitFreeValues, functionT, rows);
                if (colIndex == -1)
                    return solution = NoIOR(cols, rows);
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

                optimum = IsOptimum(functionT, colsBeforeAdding);
            }

            for(int i = 0; i < rows; i++)
            {
                if (basicValuesIndexes[i] >= colsBeforeAdding)
                    return solution = NoIOR(cols, rows);
            }

            cols = colsBeforeAdding;
            return SimplexSolveF(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, ResultFunctionF, max);
        }


        List<List<double>> SimplexMethod(int cols, int rows, List<double> functionValues, List<List<double>> limitValues,
            List<double> limitFreeValues, List<int> sign, bool max)
        {
            List<int> basicValuesIndexes = new List<int>();
            List<double> functionT = new List<double>();

            double ResultFunctionT = 0;
            int colsBeforeAdding = cols;
            int colsAdded = 0;
            int countLess = 0;

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
                    basicValuesIndexes.Add(cols);
                    cols++;
                    colsAdded++;
                    countLess++;
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
                    colsAdded++;
                }
            }

            if (countLess != rows)
            {
                //добавление доп переменных для двухэтапного метода
                basicValuesIndexes.Clear();
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

            if (!max)
            {
                for (int i = 0; i < cols; i++) 
                {
                    functionValues[i] *= -1;
                }
            }

            return SimplexFindIOR(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, functionT, ResultFunctionT, colsBeforeAdding + colsAdded, max);
        }

        List<double> GetLimitFreeValues(int rows, int cols, List<List<double>> solution)
        {
            List<double> limitFreeValues = new List<double>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i == 1 && j != cols - 1)
                    {
                        limitFreeValues.Add(solution[i][j]);
                    }
                }
            }

            return limitFreeValues;
        }

        List<double> GetFunctionValues(int rows, int cols, bool max, List<List<double>> solution)
        {
            List<double> functionValues = new List<double>();
            if (max)
                functionValues.Add(-solution[1][cols - 1]);
            else
                functionValues.Add(solution[1][cols - 1]);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i > 1 && j == cols - 1)
                    {
                        if (max)
                            functionValues.Add(-solution[i][j]);
                        else
                            functionValues.Add(solution[i][j]);
                    }
                }
            }
            return functionValues;
        }

        List<int> GetBasicValuesIndexes(int rows, int cols, List<List<double>> solution)
        {
            List<int> basicValues = new List<int>();

            for (int i = 0; i < rows; i++)
            {
                List<double> limits = new List<double>();
                for (int j = 0; j < cols; j++)
                {
                    if (i == 0 && j != cols - 1)
                    {
                        basicValues.Add(Convert.ToInt32(solution[i][j]) - 1);
                    }
                }
            }
            return basicValues;
        }

        List<List<double>> GetLimitValues(int rows, int cols, List<List<double>> solution)
        {
            List<List<double>> limitValues = new List<List<double>>();
            List<List<double>> limitsAdd = new List<List<double>>();

            for (int i = 0; i < rows; i++)
            {
                List<double> limits = new List<double>();
                for (int j = 0; j < cols; j++)
                {
                    if (i > 1 && j != cols - 1)
                    {
                        limits.Add(solution[i][j]);
                    }
                }
                if (limits.Any())
                    limitsAdd.Add(limits);
            }

            cols -= 1;
            rows -= 2;

            for (int i = 0; i < cols; i++)
            {
                List<double> limits = new List<double>();
                for (int j = 0; j < rows; j++)
                {
                    limits.Add(limitsAdd[j][i]);
                }
                limitValues.Add(limits);
            }

            return limitValues;
        }

        int MaxRowFractionIndex(List<List<double>> limitValues, List<double> fractions, int cols, int rows)
        {
            int maxFractionIndex = -1;
            bool flag = false;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (Math.Round(limitValues[i][j] - Math.Floor(limitValues[i][j]), 6) != 0)
                    {
                        maxFractionIndex = i;
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    break;
            }

            if (maxFractionIndex != -1 && maxFractionIndex != rows - 1)
            {
                for (int i = maxFractionIndex + 1; i < rows; i++)
                {
                    if (fractions[i] > fractions[maxFractionIndex])
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            if (Math.Round(limitValues[i][j] - Math.Floor(limitValues[i][j]), 6) != 0)
                            {
                                maxFractionIndex = i;
                            }
                        }
                    }
                }
            }

            return maxFractionIndex;
        }

        List<double> GetFractions(int rows, List<double> limitFreeValues)
        {
            List<double> fractions = new List<double>();
            for (int i = 0; i < rows; i++)
            {
                double fract = limitFreeValues[i] - Math.Floor(limitFreeValues[i]);
                if (Math.Round((1.0 - fract), 6) == 0) 
                {
                    fractions.Add(0);
                }
                else
                {
                    fractions.Add(fract);
                }

            }
            return fractions;
        }

        bool IsIntSolution(int rows, List<double> fractions)
        {
            for (int i = 0; i < rows; i++)
            {
                if (Math.Round(fractions[i], 6) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        List<List<double>> GomoryMethod(int cols, int rows, List<double> functionValues, List<List<double>> limitValues,
            List<double> limitFreeValues, List<int> basicValuesIndexes, double ResultFunctionF, bool max)
        {
            List<double> fractions = new List<double>();
            fractions = GetFractions(rows, limitFreeValues);
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

                //доп ограничение
                for (int i = 0; i < rows; i++)
                {
                    limitValues[i].Add(0);
                }

                limitFreeValues.Add((limitFreeValues[maxFraction] - Math.Floor(limitFreeValues[maxFraction])) * (-1)); //дробная часть свободного члена
                limitValues.Add(rowFractions);
                functionValues.Add(0);
                basicValuesIndexes.Add(cols); 

                cols++;
                rows++;

                solution = SimplexSolveF(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, ResultFunctionF, max);
                if (!solution.Any())
                {
                    List<List<double>> empty = new List<List<double>>();
                    return empty;  //ЦЕЛОЧИСЛЕННОГО РЕШЕНИЯ НЕТ
                }
                cols = solution[0].Count();
                rows = solution.Count();

                fractions.Clear();
                basicValuesIndexes.Clear();
                limitFreeValues.Clear();
                functionValues.Clear();
                limitValues.Clear();

                basicValuesIndexes = GetBasicValuesIndexes(rows, cols, solution);
                limitFreeValues = GetLimitFreeValues(rows, cols, solution);
                functionValues = GetFunctionValues(rows, cols, max, solution);
                ResultFunctionF = functionValues[0];
                functionValues.RemoveAt(0);
                limitValues = GetLimitValues(rows, cols, solution);

                int c = cols;
                cols = rows - 2;
                rows = c - 1;

                fractions = GetFractions(rows, limitFreeValues);
                flag = IsIntSolution(rows, fractions);
            }

            return solution;
        }


        internal SimplexMethodSolution(int cols, int rows, List<double> functionValues, List<List<double>> limitValues, 
            List<double> limitFreeValues, List<int> sign, bool max) 
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
            richTextBox1.Width = 700;
            richTextBox1.WordWrap = false;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.None;


            richTextBox1.Text += "Целевая функция: \r\n";

            int funcNum = -1;
            for (int i = 0; i < cols; i++)
            {
                if (functionValues[i] != 0)
                {
                    funcNum = i;
                    break;
                }
            }

            for (int j = 0; j < cols; j++)
            {
                num = j;
                if (j >= funcNum)
                {
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
            }
            if (max) 
                richTextBox1.Text += " --> max \r\n";
            else 
                richTextBox1.Text += " --> min \r\n";


            richTextBox1.Text += "\r\n";
            richTextBox1.Text += "Ограничения: \r\n";
            num = 0;
            funcNum = -1;

            for (int i = 0; i < rows; i++)
            {
                for (int k = 0; k < cols; k++)
                {
                    if (limitValues[i][k] != 0)
                    {
                        funcNum = k;
                        break;
                    }
                }
                for (int j = 0; j < cols; j++)
                {
                    num = j;
                    if (j >= funcNum)
                    {
                        if (limitValues[i][j] == 0)
                        {
                            if (j != cols - 1)
                            {
                                if (limitValues[i][j + 1] > 0)
                                    richTextBox1.Text += "+ ";
                            }
                        }
                        else
                        {
                            if (limitValues[i][j] == 1)
                                richTextBox1.Text += "x" + ++num + " ";
                            else if (limitValues[i][j] == -1)
                                richTextBox1.Text += "-x" + ++num + " ";
                            else
                                richTextBox1.Text += limitValues[i][j].ToString() + "x" + ++num + " ";
                            if (j != cols - 1)
                                if (limitValues[i][j + 1] > 0)
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

            if (!solution.Any())
            {
                richTextBox1.Text += "\r\n";
                richTextBox1.Text += "Задача не имеет оптимальных решений. \r\n";
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
                    this.Controls.Add(richTextBox1);
                }
                else
                {
                    DataGridView DGV = new DataGridView
                    {
                        Font = new Font("Microsoft Sans Serif", 11),
                        Location = new Point(10, richTextBox1.Height + 120),
                        ReadOnly = true,
                        ColumnCount = rows,
                        RowCount = cols
                    };
                    DataGridViewElementStates states = DataGridViewElementStates.None;
                    DGV.ScrollBars = ScrollBars.None;
                    var totalHeight = DGV.Rows.GetRowsHeight(states) + DGV.ColumnHeadersHeight;
                    totalHeight += DGV.Rows.Count * 4 + 10;  

                    for(int i = 0; i < rows; i++)
                    {
                        DGV.Columns[i].Width = 70;
                    }

                    var totalWidth = (rows + 1) * 70;
                    DGV.ClientSize = new Size(totalWidth, totalHeight);

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
                                DGV.Rows[i].Cells[j].Value = Math.Round(solution[j][i], 3).ToString();
                            }
                        }
                    }

                    int dgv_height = DGV.Rows.GetRowsHeight(DataGridViewElementStates.Visible);

                    Button intSolutionButton = new Button
                    {
                        Location = new Point(10, richTextBox1.Height + dgv_height + 200),
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
                        if (closed)
                        {
                            RichTextBox richTextBox2 = new RichTextBox
                            {
                                Location = new Point(10, richTextBox1.Height + dgv_height + 180),
                                Font = new Font("Microsoft Sans Serif", 11),
                                ReadOnly = true
                            };
                            richTextBox2.ContentsResized += (object senderRichTextBox, ContentsResizedEventArgs e) =>
                            {
                                var richTextBox = (RichTextBox)senderRichTextBox;
                                richTextBox.Height = e.NewRectangle.Height;
                            };
                            richTextBox2.Width = 700;
                            richTextBox2.WordWrap = false;
                            richTextBox2.ScrollBars = RichTextBoxScrollBars.None;

                            functionValues.Clear();
                            limitValues.Clear();
                            limitFreeValues.Clear();

                            List<int> basicValuesIndexes = new List<int>();
                            List<List<double>> limitsAdd = new List<List<double>>();
                            double ResultFunctionF = solution[1][cols - 1];

                            basicValuesIndexes = GetBasicValuesIndexes(rows, cols, solution);
                            limitFreeValues = GetLimitFreeValues(rows, cols, solution);
                            functionValues = GetFunctionValues(rows, cols, max, solution);
                            ResultFunctionF = functionValues[0];
                            functionValues.RemoveAt(0);
                            limitValues = GetLimitValues(rows, cols, solution);

                            cols -= 1;
                            rows -= 2;

                            solution.Clear();
                            solution = GomoryMethod(rows, cols, functionValues, limitValues, limitFreeValues, basicValuesIndexes, ResultFunctionF, max);

                            if (!solution.Any())
                            {
                                richTextBox2.Text += "Задача не имеет целочисленных решений. \r\n";
                                this.Controls.Add(richTextBox2);
                            }
                            else
                            {
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
                                    this.Controls.Add(richTextBox2);
                                }
                                else
                                {
                                    richTextBox2.Text += "Решение задачи методом Гомори: нахождение целочисленного решения. \r\n";

                                    DataGridView DGV2 = new DataGridView
                                    {
                                        Font = new Font("Microsoft Sans Serif", 11),
                                        Location = new Point(10, richTextBox1.Height + dgv_height + richTextBox2.Height + 140),
                                        ReadOnly = true,
                                        ColumnCount = rows,
                                        RowCount = cols
                                    };

                                    DataGridViewElementStates states1 = DataGridViewElementStates.None;
                                    DGV2.ScrollBars = ScrollBars.None;
                                    var totalHeight1 = DGV2.Rows.GetRowsHeight(states1) + DGV2.ColumnHeadersHeight;
                                    totalHeight1 += DGV2.Rows.Count * 4 + 10;

                                    for (int i = 0; i < rows; i++)
                                    {
                                        DGV2.Columns[i].Width = 70;
                                    }

                                    var totalWidth1 = (rows + 1) * 70;
                                    DGV2.ClientSize = new Size(totalWidth1, totalHeight1);

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
                                                DGV2.Rows[i].Cells[j].Value = Math.Round(solution[j][i], 3).ToString();
                                            }
                                        }
                                    }

                                    this.Controls.Add(richTextBox2);
                                    this.Controls.Add(DGV2);
                                };
                            }

                            closed = false;
                        }
                        else
                        {
                            MessageBox.Show(
                            "Решение уже открыто!",
                             "Сообщение",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Information,
                             MessageBoxDefaultButton.Button1,
                             MessageBoxOptions.DefaultDesktopOnly);
                        }
                    };
                    
                }                
            }
        }

    }
}
