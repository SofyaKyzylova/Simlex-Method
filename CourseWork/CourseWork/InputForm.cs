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
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
        }

        List<double> functionValues = new List<double>();
        List<List<double>> limitValues = new List<List<double>>();
        List<double> limitFreeValues = new List<double>();
        List<int> sign = new List<int>();
        bool min = true;
        bool isClosed = true;

        internal InputForm(int cols, int rows)
        {
            InitializeComponent();
            this.AutoSize = true;

            Label label1 = new Label();
            label1.Location = new Point(10, 10);
            label1.Name = "label2";
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 10);
            label1.Text = "Введите коэффициеты целевой функции";
            this.Controls.Add(label1);


            int x = 10, y = 40, xNum, yNum;

            for (int j = 0; j < cols; j++)
            {
                xNum = j;

                TextBox textBox = new TextBox();
                textBox.Location = new Point(x, y);
                textBox.Name = j.ToString();
                textBox.Font = new Font("Microsoft Sans Serif", 10);
                textBox.Height = 20;
                textBox.Width = 40;
                textBox.KeyPress += (sender, eventArgs) => {
                    if (!char.IsControl(eventArgs.KeyChar) && !char.IsDigit(eventArgs.KeyChar) &&
                    (eventArgs.KeyChar != '-') && (eventArgs.KeyChar != ','))
                    {
                        eventArgs.Handled = true;
                    }
                    if ((eventArgs.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1) &&
                    (eventArgs.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
                    {
                        eventArgs.Handled = true;
                    }
                    if ((eventArgs.KeyChar == '-') && ((sender as TextBox).SelectionStart != 0 || (sender as TextBox).Text.Contains("-")))
                    {
                        eventArgs.Handled = true;
                    }
                };
                textBox.Click += (sender, eventArgs) =>
                {
                    (sender as TextBox).Clear();
                };
                this.Controls.Add(textBox);

                Label label = new Label();
                label.Location = new Point(x + 45, y);
                label.Name = "labelFunc" + j;
                label.AutoSize = true;
                label.Font = new Font("Microsoft Sans Serif", 10);
                label.Text = "x" + ++xNum;
                this.Controls.Add(label);
                x += 145;
            }

            
            x = 10 + 145 * cols;

            ComboBox comboBoxMaxMin = new ComboBox();
            comboBoxMaxMin.Location = new Point(x, y);  
            comboBoxMaxMin.Size = new Size(50, 100);
            comboBoxMaxMin.Name = "ComboBoxMaxMin";
            comboBoxMaxMin.Font = new Font("Microsoft Sans Serif", 10);
            comboBoxMaxMin.Items.Add("max");
            comboBoxMaxMin.Items.Add("min");
            comboBoxMaxMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Controls.Add(comboBoxMaxMin);


            Label label2 = new Label();
            label2.Location = new Point(10, 100);
            label2.Name = "label1";
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 10);
            label2.Text = "Введите коэффициеты ограничений";
            this.Controls.Add(label2);

            x = 10; y = 140;

            for (int i = 0; i < rows; i++)
            {
                xNum = i;
                xNum++;
                for (int j = 0; j < cols; j++)
                {
                    yNum = j;

                    TextBox textBoxLim = new TextBox();
                    textBoxLim.Location = new Point(x, y);
                    textBoxLim.Name = i.ToString() + j.ToString();
                    textBoxLim.Font = new Font("Microsoft Sans Serif", 10);
                    textBoxLim.Height = 20;
                    textBoxLim.Width = 40;
                    textBoxLim.KeyPress += (sender, eventArgs) => {
                        if (!char.IsControl(eventArgs.KeyChar) && !char.IsDigit(eventArgs.KeyChar) &&
                        (eventArgs.KeyChar != '-') && (eventArgs.KeyChar != ','))
                        {
                            eventArgs.Handled = true;
                        }
                        if ((eventArgs.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1) &&
                        (eventArgs.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
                        {
                            eventArgs.Handled = true;
                        }
                        if ((eventArgs.KeyChar == '-') && ((sender as TextBox).SelectionStart != 0 || (sender as TextBox).Text.Contains("-")))
                        {
                            eventArgs.Handled = true;
                        }
                    };
                    textBoxLim.Click += (sender, eventArgs) =>
                    {
                        (sender as TextBox).Clear();
                    };
                    this.Controls.Add(textBoxLim);

                    Label labelLim = new Label();
                    labelLim.Location = new Point(x + 45, y);
                    labelLim.Name = "label" + i + j;
                    labelLim.AutoSize = true;
                    labelLim.Font = new Font("Microsoft Sans Serif", 10);
                    labelLim.Text = "x" + ++yNum; 
                    this.Controls.Add(labelLim);

                    x += 145; 
                }
                x = 10; y += 25;
            }


            y = 140; 
            for (int j = 0; j < rows; j++)
            {
                ComboBox comboBox = new ComboBox();
                comboBox.Location = new Point(10 + 145 * cols, y);
                comboBox.Size = new Size(50, 35); 
                comboBox.Name = "comboBox" + j.ToString();
                comboBox.Font = new Font("Microsoft Sans Serif", 10);
                comboBox.Items.Add(">=");
                comboBox.Items.Add("=");
                comboBox.Items.Add("<=");
                comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                this.Controls.Add(comboBox);

                y += 25;
            }


            y = 140; 
            for (int j = 0; j < rows; j++)
            {
                xNum = j;

                TextBox textBoxB = new TextBox();
                textBoxB.Location = new Point(80 + 150 * cols, y);
                textBoxB.Name = "textBox" + j.ToString();
                textBoxB.Font = new Font("Microsoft Sans Serif", 10);
                textBoxB.Height = 20;
                textBoxB.Width = 40;
                textBoxB.KeyPress += (sender, eventArgs) => {
                    if (!char.IsControl(eventArgs.KeyChar) && !char.IsDigit(eventArgs.KeyChar) && 
                    (eventArgs.KeyChar != '-') && (eventArgs.KeyChar != ','))
                    {
                        eventArgs.Handled = true;
                    }
                    if ((eventArgs.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1) && 
                    (eventArgs.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
                    {
                        eventArgs.Handled = true;
                    }
                    if ((eventArgs.KeyChar == '-') && ((sender as TextBox).SelectionStart != 0 || (sender as TextBox).Text.Contains("-")))
                    {
                        eventArgs.Handled = true;
                    }
                };
                textBoxB.Click += (sender, eventArgs) =>
                {
                    (sender as TextBox).Clear();
                };
                    this.Controls.Add(textBoxB);

                Label labelB = new Label();
                labelB.Location = new Point(125 + 150 * cols, y);
                labelB.Name = "label" + j;
                labelB.AutoSize = true;
                labelB.Font = new Font("Microsoft Sans Serif", 10);
                labelB.Text = "b" + ++xNum;
                this.Controls.Add(labelB);
                y += 25;
            }


            Button buttonSolve = new Button();
            buttonSolve.Location = new Point(10, y + 25);
            buttonSolve.Size = new Size(110, 45);
            buttonSolve.Name = "ButtonCount";
            buttonSolve.Font = new Font("Microsoft Sans Serif", 10);
            buttonSolve.Text = "Рассчитать";
            this.Controls.Add(buttonSolve);


            buttonSolve.Click += (sender, eventArgs) =>
            {
                bool textBoxesFilled = this.Controls.OfType<TextBox>().Any(textBox => textBox.Text == "");
                bool textIncorrect = this.Controls.OfType<TextBox>().Any(textBox => textBox.Text == "-");
                bool comboBoxesFilled = this.Controls.OfType<ComboBox>().Any(comboBox => comboBox.SelectedIndex == -1);

                if (textBoxesFilled || textIncorrect || comboBoxesFilled) 
                {
                    MessageBox.Show(
                        "Пожалуйста, заполните все поля ввода",
                         "Сообщение",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Warning,
                         MessageBoxDefaultButton.Button1,
                         MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                {
                    for (int j = 0; j < cols; j++)
                    {
                        TextBox textbox = (TextBox)(from t in this.Controls.OfType<TextBox>() where t.Name == j.ToString() select t).First();
                        functionValues.Add(Convert.ToDouble(textbox.Text));
                    }
                    for (int i = 0; i < rows; i++)
                    {
                        List<double> limitValues1 = new List<double>();
                        for (int j = 0; j < cols; j++)
                        {
                            TextBox textbox = (TextBox)(from t in this.Controls.OfType<TextBox>() where t.Name == i.ToString() + j.ToString() select t).First();
                            limitValues1.Add(Convert.ToDouble(textbox.Text));
                        }
                        limitValues.Add(limitValues1);
                    }
                    for (int j = 0; j < rows; j++)
                    {
                        TextBox textbox = (TextBox)(from t in this.Controls.OfType<TextBox>() where t.Name == "textBox" + j.ToString() select t).First();
                        limitFreeValues.Add(Convert.ToDouble(textbox.Text));
                    }
                    for (int j = 0; j < rows; j++)
                    {
                        ComboBox comboBox = (ComboBox)(from t in this.Controls.OfType<ComboBox>() where t.Name == "comboBox" + j.ToString() select t).First();
                        if (comboBox.SelectedItem.ToString() == ">=")
                            sign.Add(0);
                        else if (comboBox.SelectedItem.ToString() == "<=")
                            sign.Add(2);
                        else sign.Add(1);
                    }

                    string extremum = comboBoxMaxMin.Text;
                    if (extremum == "min")
                        min = false;

                    if (isClosed)
                    {
                        SimplexMethodSolution simplexMethodSolution = new SimplexMethodSolution(cols, rows, functionValues, limitValues, limitFreeValues, sign, min);
                        simplexMethodSolution.Show(this);
                        isClosed = false;
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
                }
                
            };
        }
    }
}
