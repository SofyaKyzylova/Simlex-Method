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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        bool isClosed = true;
        InputForm inputForm;
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" )
            {
                MessageBox.Show("Пожалуйста, заполните все поля ввода");
            }
            else
            {
                if(int.Parse(textBox1.Text) == 0 || int.Parse(textBox2.Text) == 0)
                {
                    MessageBox.Show("Введено неверное число!");
                }
                else if(int.Parse(textBox1.Text) > 15)
                {
                    MessageBox.Show("Введено слишком большое число!");
                }
                else
                {
                    int n = int.Parse(textBox1.Text);
                    int m = int.Parse(textBox2.Text);
                    
                    if (isClosed)
                    {
                        inputForm = new InputForm(n, m);
                        inputForm.Show(this);
                        isClosed = false;
                    }
                    else
                    {
                        inputForm.Close();
                        isClosed = true;
                    }
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char a = e.KeyChar;
            if (a != '\b' && !Char.IsDigit(a))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char a = e.KeyChar;
            if (a != '\b' && !Char.IsDigit(a))
            {
                e.Handled = true;
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            (sender as TextBox).Clear();
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            (sender as TextBox).Clear();
        }
    }
}
