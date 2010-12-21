using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class generator_params : Form
    {
        public generator_params()
        {
            InitializeComponent();

            Z_comboBox.SelectedIndex = 0;
            Z_param1_label.Text = "a";
            Z_param2_label.Text = "b";
            Z_param1_textBox.Text = "0";
            Z_param2_textBox.Text = "0";
        }

        public generator_params(int iGeneratorType, double fGeneratorParamFirst, double fGeneratorParamSecond)
        {
            InitializeComponent();

            Z_comboBox.SelectedIndex = iGeneratorType;
            //Z_param1_label.Text = "a";
            //Z_param2_label.Text = "b";
            Z_param1_textBox.Text = fGeneratorParamFirst.ToString();
            Z_param2_textBox.Text = fGeneratorParamSecond.ToString();

            fGeneratorParamFirst_BASE = fGeneratorParamFirst;
            fGeneratorParamSecond_BASE = fGeneratorParamSecond;
            
            
        }

        //protected override ShowDialog

        private void Z_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Z_comboBox.SelectedIndex)
            {
                case 0:
                    Z_param1_label.Text = "a";
                    Z_param2_label.Text = "b";
                    Z_param2_label.Visible = true;
                    this.Z_param2_textBox.Visible = true;
                    break;

                case 1:
                    Z_param1_label.Text = "mu";
                    Z_param2_label.Text = "sigma";
                    Z_param2_label.Visible = true;
                    this.Z_param2_textBox.Visible = true;
                    break;

                case 2:
                    Z_param1_label.Text = "sigma";
                    Z_param2_label.Visible = false;
                    this.Z_param2_textBox.Visible = false;
                    break;

                case 3:
                    Z_param1_label.Text = "k";
                    Z_param2_label.Text = "tetta";
                    Z_param2_label.Visible = true;
                    this.Z_param2_textBox.Visible = true;
                    break;

                case 4:
                    Z_param1_label.Text = "lambda";
                    Z_param2_label.Visible = false;
                    this.Z_param2_textBox.Visible = false;
                    break;


                default:
                    MessageBox.Show("EXTERMINATE!!!!");
                    break;
            }
        }
        /*
        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            fGeneratorParamFirst = double.Parse(Z_param1_textBox.Text);
            fGeneratorParamSecond = double.Parse(Z_param2_textBox.Text);

            Close();
        }
        */

        private void buttonOK_Click(object sender, EventArgs e)
        {


            double fTempParamFirst = 0.0;
            double fTempParamSecond = 0.0;
            bool bResultOK = true;

            if (!(Form1.ConvertStrToDouble(Z_param1_textBox.Text, out fTempParamFirst) 
                && Form1.ConvertStrToDouble(Z_param2_textBox.Text, out fTempParamSecond)))
            {
                bResultOK = false;
            }
          

            if (bResultOK)
            {
                if (Z_comboBox.SelectedIndex == 0)
                {
                    if (fTempParamFirst < fTempParamSecond)
                    {

                        DialogResult = DialogResult.OK;
                        fGeneratorParamFirst = fTempParamFirst;
                        fGeneratorParamSecond = fTempParamSecond;
                        Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Первый параметр должен быть строго меньше второго!");

                        Z_param1_textBox.Text = fGeneratorParamFirst_BASE.ToString();
                        Z_param2_textBox.Text = fGeneratorParamSecond_BASE.ToString();
                    }

                }

                if (Z_comboBox.SelectedIndex == 1)
                {
                    if (fTempParamSecond > 0)
                    {

                        DialogResult = DialogResult.OK;
                        fGeneratorParamFirst = fTempParamFirst;
                        fGeneratorParamSecond = fTempParamSecond;
                        Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Второй параметр должен быть больше нуля!");

                        //Z_param1_textBox.Text = fGeneratorParamFirst_BASE.ToString();
                        Z_param2_textBox.Text = fGeneratorParamSecond_BASE.ToString();
                    }

                }

                if ((Z_comboBox.SelectedIndex == 2)||(Z_comboBox.SelectedIndex == 4))
                {
                    if (fTempParamFirst > 0)
                    {

                        DialogResult = DialogResult.OK;
                        fGeneratorParamFirst = fTempParamFirst;
                        fGeneratorParamSecond = fTempParamSecond;
                        Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Параметр должен быть больше нуля!");

                        Z_param1_textBox.Text = fGeneratorParamFirst_BASE.ToString();
                        //Z_param2_textBox.Text = fGeneratorParamSecond_BASE.ToString();
                    }

                }

                if (Z_comboBox.SelectedIndex == 3)
                {
                    if ((fTempParamFirst > 0) && (fTempParamSecond > 0))
                    {

                        DialogResult = DialogResult.OK;
                        fGeneratorParamFirst = fTempParamFirst;
                        fGeneratorParamSecond = fTempParamSecond;
                        Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Параметры должны быть положительными!");

                        Z_param1_textBox.Text = fGeneratorParamFirst_BASE.ToString();
                        Z_param2_textBox.Text = fGeneratorParamSecond_BASE.ToString();
                    }

                }

            }
            else
            {
                MessageBox.Show("Не удается преобразовать значения параметров генеатора к float");

                Z_param1_textBox.Text = fGeneratorParamFirst_BASE.ToString();
                Z_param2_textBox.Text = fGeneratorParamSecond_BASE.ToString();
            }


        }


    }
}
