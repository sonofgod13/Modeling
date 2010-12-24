using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ModelingDataTypes;
using GeneratorSubsystem;
using System.Threading;
using System.Net;
using System.Configuration;
using System.Diagnostics;


namespace Modeling
{
    public partial class ImitationGUIForm : Form
    {
        private bool modelFlag;
        private bool pauseModelFlag;
        private bool frontOfficeFlag;
        private Imitation imitator;

        private FrontOffice.FrontOfficeImitationHelper frontOffice;

        private string frontOfficeUrl;

        private Dictionary<Button, Func<GeneratedElement>> BindedButtons = new Dictionary<Button, Func<GeneratedElement>>();

        private void setStringButton(Button button, string text)
        {
            if (button.InvokeRequired)
            {
                button.Invoke(
                    new Action<Button, string>(setStringButton), 
                    new object[] { button, text }
                );
            }
            else
            {
                button.Text = text;
            }

        }

        private void setEnableButton(Button button, bool enabled)
        {
            if (button.InvokeRequired)
            {
                button.Invoke(
                    new Action<Button, bool>(setEnableButton), 
                    new object[] { button, enabled }
                );
            }
            else
            {
                button.Enabled = enabled;
            }

        }

        private void setLabelText(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(
                    new Action<Label, string>(setLabelText), 
                    new object[] { label, text }
                );
            }
            else
            {
                label.Text = text;
            }
        }


        public ImitationGUIForm()  //при загрузке формы основных параметров
        {
            Params.Initialization();   //инициализация начальных параметров
            InitializeComponent();
            this.BindButtons();

            /////////  Это чтоб нельзя было задать параметры изменений срочности заявок
            label4.Visible = false;
            label3.Visible = false;
            button4.Visible = false;
            button3.Visible = false;
            ///////////////////////////////////////

            modelFlag = false;
            pauseModelFlag = false;
            frontOfficeFlag = true;
            button_start.Text = "Старт";
            button_stop.Enabled = false;
            front_office_button.Enabled = false;
            materials_button.Enabled = false;
            idle_button.Enabled = false;
            averageDelay_button.Enabled = false;
            finish_button.Enabled = false;
            //cancel_button.Enabled = false;
            frontOffice = new Modeling.FrontOffice.FrontOfficeImitationHelper();
            frontOfficeUrl = ConfigurationSettings.AppSettings["FrontOfficeUrl"].ToString();

            dUrg_BASE = Params.fUrgencyPropabilityDemand;    //вероятность срочности заявки
            dRef_BASE = Params.fRefusePropabilityDemand;     //вероятность отказа от заявки
            this.textBox_UrgencyPropabilityDemand.Text = dUrg_BASE.ToString();
            //вероятность срочности заявки (поле ввода)
            this.textBox_RefusePropabilityDemand.Text = dRef_BASE.ToString();
            //вероятность отказа от заявки (поле ввода)

            ModelingDayToWork_BASE = Params.ModelingDayToWork;  //время моделирования в днях
            this.textBox_ModelingDayCount.Text = ModelingDayToWork_BASE.ToString();
            //время моделирования в днях (поле ввода)
        }

        private void BindButtons()
        {
            BindedButtons.Add(this.Z_g_time_button, () => Params.GeneratorDemandsTime);

            BindedButtons.Add(this.Z_g_p1_button, () => Params.Products[1]);
            BindedButtons.Add(this.Z_g_p2_button, () => Params.Products[2]);
            BindedButtons.Add(this.Z_g_p3_button, () => Params.Products[3]);

            BindedButtons.Add(this.M_g_m1_button, () => Params.Materials[1]);
            BindedButtons.Add(this.M_g_m2_button, () => Params.Materials[2]);
            BindedButtons.Add(this.M_g_m3_button, () => Params.Materials[3]);
            BindedButtons.Add(this.M_g_m4_button, () => Params.Materials[4]);
            BindedButtons.Add(this.M_g_m5_button, () => Params.Materials[5]);
            BindedButtons.Add(this.M_g_m6_button, () => Params.Materials[6]);
            BindedButtons.Add(this.M_g_m7_button, () => Params.Materials[7]);
            BindedButtons.Add(this.M_g_m8_button, () => Params.Materials[8]);
            BindedButtons.Add(this.M_g_m9_button, () => Params.Materials[9]);
            BindedButtons.Add(this.M_g_m10_button, () => Params.Materials[10]);
            BindedButtons.Add(this.M_g_m11_button, () => Params.Materials[11]);
            BindedButtons.Add(this.M_g_m12_button, () => Params.Materials[12]);

            BindedButtons.Add(this.button_deliveryDelayGenerator, () => Params.DeliveryDelayGenerator);
            BindedButtons.Add(this.button_demandModifyTime, () => Params.DemandModifyTime);
            BindedButtons.Add(this.button8, () => Params.ArticlesModify);

            BindedButtons.Add(this.button7, () => Params.Products[1]);
            BindedButtons.Add(this.button6, () => Params.Products[2]);
            BindedButtons.Add(this.button5, () => Params.Products[3]);

            BindedButtons.Add(this.button4, () => Params.UgrToStandModify);
            BindedButtons.Add(this.button3, () => Params.StandToUrgModify);
        }

        private void StartClickThreadWorkerStart()
        {
            bool end = imitator.Start(this.modellingTimeLabel);
            if (end)
            {
                setStringButton(this.button_start, "Старт");
                modelFlag = false;
                setEnableButton(this.button_stop, false);


                double demandAverageDelay = this.imitator.GetDemandAverageDelay();

                var demandAverageDelayLabelText = (demandAverageDelay == -1)
                    ? "нет выполненных заказов"
                    : String.Format("{0} дней", Math.Round(demandAverageDelay, 3));

                setLabelText(this.demandAverageDelayLabel, demandAverageDelayLabelText);

                setLabelText(this.activityFactorLabel, Math.Round(imitator.GetActivityFactor(), 5).ToString());
                setLabelText(this.retargetTimePercentLabel, Math.Round(imitator.GetRetargetTimePercent(), 5).ToString());
                setLabelText(this.refuseNumLabel, imitator.GetRefusesNum().ToString());
                setLabelText(this.acceptedNumLabel, imitator.GetAcceptedDemandsNum().ToString());
                setLabelText(this.finishedNumLabel, imitator.GetFinishedDemandsNum().ToString());
                
                setEnableButton(this.materials_button, true);
                setEnableButton(this.idle_button, true);
                setEnableButton(this.averageDelay_button, true);
                setEnableButton(this.finish_button, true);
                
                setEnableButton(this.front_office_button, true);
            }
        }

        private void StartClickThreadWorkerStop()
        {
            bool pauseDone = imitator.Stop();

            setEnableButton(this.button_start, true);
            setEnableButton(this.button_stop, true);

            if (pauseDone)
            {
                double demandAverageDelay = this.imitator.GetDemandAverageDelay();

                var demandAverageDelayLabelText = (demandAverageDelay == -1)
                    ? "нет выполненных заказов"
                    : String.Format("{0} дней", Math.Round(demandAverageDelay, 3));

                setLabelText(this.demandAverageDelayLabel, demandAverageDelayLabelText);

                setLabelText(this.activityFactorLabel, Math.Round(imitator.GetActivityFactor(), 5).ToString());
                setLabelText(this.retargetTimePercentLabel, Math.Round(imitator.GetRetargetTimePercent(), 5).ToString());
                setLabelText(this.refuseNumLabel, imitator.GetRefusesNum().ToString());
                setLabelText(this.acceptedNumLabel, imitator.GetAcceptedDemandsNum().ToString());
                setLabelText(this.finishedNumLabel, imitator.GetFinishedDemandsNum().ToString());
                
                setEnableButton(this.materials_button, true);
                setEnableButton(this.idle_button, true);
                setEnableButton(this.averageDelay_button, true);
                setEnableButton(this.finish_button, true);
                
                setEnableButton(this.front_office_button, true);
            }
        }

        private void StartClickThreadWorkerContinue()
        {
            bool end = false;
            end = imitator.Continue(this.modellingTimeLabel);
            if (end == true)
            {
                setStringButton(this.button_start, "Старт");
                modelFlag = false;
                setEnableButton(this.button_stop, false);
                double demandAverageDelay = this.imitator.GetDemandAverageDelay();

                var demandAverageDelayLabelText = (demandAverageDelay == -1)
                    ? "нет выполненных заказов"
                    : String.Format("{0} дней", Math.Round(demandAverageDelay, 3));

                setLabelText(this.demandAverageDelayLabel, demandAverageDelayLabelText);

                setLabelText(this.activityFactorLabel, Math.Round(imitator.GetActivityFactor(), 5).ToString());
                setLabelText(this.retargetTimePercentLabel, Math.Round(imitator.GetRetargetTimePercent(), 5).ToString());
                setLabelText(this.refuseNumLabel, imitator.GetRefusesNum().ToString());
                setLabelText(this.acceptedNumLabel, imitator.GetAcceptedDemandsNum().ToString());
                setLabelText(this.finishedNumLabel, imitator.GetFinishedDemandsNum().ToString());
                
                setEnableButton(this.materials_button, true);
                setEnableButton(this.idle_button, true);
                setEnableButton(this.averageDelay_button, true);
                setEnableButton(this.finish_button, true);
                
                setEnableButton(this.front_office_button, true);
            }
        }

        private void StopClickThreadWorkerStop()
        {
            bool stopDone = imitator.Stop();
            setEnableButton(this.button_start, true);
            if (stopDone)
            {
                double demandAverageDelay = this.imitator.GetDemandAverageDelay();

                var demandAverageDelayLabelText = (demandAverageDelay == -1)
                    ? "нет выполненных заказов"
                    : String.Format("{0} дней", Math.Round(demandAverageDelay, 3));

                setLabelText(this.demandAverageDelayLabel, demandAverageDelayLabelText);

                setLabelText(this.activityFactorLabel, Math.Round(imitator.GetActivityFactor(), 5).ToString());
                setLabelText(this.retargetTimePercentLabel, Math.Round(imitator.GetRetargetTimePercent(), 5).ToString());
                setLabelText(this.refuseNumLabel, imitator.GetRefusesNum().ToString());
                setLabelText(this.acceptedNumLabel, imitator.GetAcceptedDemandsNum().ToString());
                setLabelText(this.finishedNumLabel, imitator.GetFinishedDemandsNum().ToString());
                
                setEnableButton(materials_button, true);
                setEnableButton(idle_button, true);
                setEnableButton(averageDelay_button, true);
                setEnableButton(finish_button, true);
                
                setEnableButton(front_office_button, true);
            }
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            if (!modelFlag)
            {
                if (!CheckUrgPropabilities())
                    return;

                button_start.Text = "Пауза";
                this.modelFlag = true;
                this.frontOfficeFlag = true;

                setLabelText(this.activityFactorLabel, "NaN");
                setLabelText(this.modellingTimeLabel, "NaN");
                setLabelText(this.demandAverageDelayLabel, "NaN");
                setLabelText(this.retargetTimePercentLabel, "NaN");
                setLabelText(this.refuseNumLabel, "NaN");
                setLabelText(this.acceptedNumLabel, "NaN");
                setLabelText(this.finishedNumLabel, "NaN");
                
                this.imitator = new Imitation();
                button_stop.Enabled = true;
                front_office_button.Enabled = false;

                var workThread = new Thread(StartClickThreadWorkerStart)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Highest
                };

                workThread.Start();


            }
            else
            {
                if (!pauseModelFlag)
                {
                    setEnableButton(this.button_start, false);
                    setEnableButton(this.button_stop, false);
                    button_start.Text = "Продолжить";
                    pauseModelFlag = true;

                    var workThread = new Thread(StartClickThreadWorkerStop)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Normal
                    };

                    workThread.Start();
                }
                else
                {
                    button_start.Text = "Пауза";
                    materials_button.Enabled = false;
                    idle_button.Enabled = false;
                    averageDelay_button.Enabled = false;
                    finish_button.Enabled = false;
                    
                    pauseModelFlag = false;
                    front_office_button.Enabled = false;
                    this.imitator.SaveNewFrontDemands(frontOffice.GetNewAtDate(this.imitator.GetModelingTime()));
                    this.imitator.SaveChangedFrontDemands(frontOffice.GetChangedAtDate(this.imitator.GetModelingTime()));

                    var workThread = new Thread(StartClickThreadWorkerContinue)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    };
                    workThread.Start();

                }
            }
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            modelFlag = false;
            pauseModelFlag = false;
            setEnableButton(this.button_start, false);
            setEnableButton(this.button_stop, false);

            var workThread = new Thread(StopClickThreadWorkerStop)
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            workThread.Start();

            button_start.Text = "Старт";
        }

        /// <summary>
        /// Функция установки параметров генератора
        /// </summary>
        /// <param name="iGeneratorType"></param>
        /// <param name="fGeneratorParamFirst"></param>
        /// <param name="fGeneratorParamSecond"></param>
        /// <returns></returns>
        private bool SetGeneratorParameters(ref GeneratorType iGeneratorType, ref double fGeneratorParamFirst, ref double fGeneratorParamSecond)
        {
            bool bSetResult = false;
            //iGeneratorType = 0;
            //fGeneratorParamFirst = 0.0;
            //fGeneratorParamSecond = 0.0;

            using (var generatorForm = new GeneratorParamsForm(iGeneratorType, fGeneratorParamFirst, fGeneratorParamSecond))
            {
                if (DialogResult.OK == generatorForm.ShowDialog())
                {
                    iGeneratorType = (GeneratorType)generatorForm.Z_comboBox.SelectedIndex;
                    fGeneratorParamFirst = generatorForm.fGeneratorParamFirst;
                    fGeneratorParamSecond = generatorForm.fGeneratorParamSecond;
                    bSetResult = true;
                }
            }

            return bSetResult;
        }

        private void GenericSetGeneratorParametersClick(object sender, EventArgs e)
        {
            var button = sender as Button;

            if (button == null || !this.BindedButtons.ContainsKey(button))
                return;

            var generatorParameters = this.BindedButtons[button]();

            SetGeneratorParameters(
                ref generatorParameters.GeneratorType,
                ref generatorParameters.fA,
                ref generatorParameters.fB
            );
        }

        private void materials_button_Click(object sender, EventArgs e)
        {
            var x = new int[Params.MATERIALS_NUMBER][];
            var days = new List<int>();

            for (int i = 0; i < imitator.GetMaterialsPerDayStatistic()[0].Length; i++)
            {
                days.Add(i + 1);
            }

            for (int i = 0; i < Params.MATERIALS_NUMBER; i++)
            {
                x[i] = days.ToArray();
            }

            var lines = new string[12];
            for (var i = 0; i < 12; i++)
                lines[i] = String.Format("Материал {0}", i + 1);

            var gr = new GraphForm(
                imitator.GetMaterialsPerDayStatistic(),
                x,
                lines,
                "Дни",
                "Количество",
                "Изменение количества материалов на складе"
            );

            gr.ShowDialog();
        }

        private bool CheckUrgPropabilities()
        {
            double dUrg = 0.0;
            double dRef = 0.0;
            bool bResultUrg = false;
            bool bResultRef = false;

            bResultUrg = ConvertStrToDouble(textBox_UrgencyPropabilityDemand.Text, out dUrg);
            bResultRef = ConvertStrToDouble(textBox_RefusePropabilityDemand.Text, out dRef);

            if (!bResultUrg || !bResultRef)
            {
                string str = "Не удается преобразовать значения параметра к float:";
                if (!(bResultUrg))
                {
                    str = str + "\nВероятность срочности заявки";
                    textBox_UrgencyPropabilityDemand.Text = dUrg_BASE.ToString();
                }
                if (!(bResultRef))
                {
                    str = str + "\nВероятность отаза от заявки";
                    textBox_RefusePropabilityDemand.Text = dRef_BASE.ToString();
                }
                MessageBox.Show(str);
                return false;
            }

            if ((dUrg < 0) || (dRef < 0))
            {
                string str = "Параметр должен быть больше или равен нулю:";
                if (dUrg < 0)
                {
                    str = str + "\nВероятность срочности заявки";
                    textBox_UrgencyPropabilityDemand.Text = dUrg_BASE.ToString();
                }
                if (dRef < 0)
                {
                    str = str + "\nВероятность отаза от заявки";
                    textBox_RefusePropabilityDemand.Text = dRef_BASE.ToString();
                }
                MessageBox.Show(str);
                return false;
            }
            if (dUrg + dRef > 1)
            {
                textBox_UrgencyPropabilityDemand.Text = dUrg_BASE.ToString();
                textBox_RefusePropabilityDemand.Text = dRef_BASE.ToString();

                MessageBox.Show("Сумаа вероятностей срочности заявки и отаза от заявки не должна быть больше единицы!");
                return false;
            }
            Params.fRefusePropabilityDemand = dRef;
            Params.fUrgencyPropabilityDemand = dUrg;
            return true;
        }

        /// <summary>
        /// Преобразует строку в double иначе возвращает false
        /// </summary>
        /// <param name="sIn"></param>
        /// <param name="dOut"></param>
        /// <returns></returns>
        public static bool ConvertStrToDouble(string sIn, out double dOut)
        {
            dOut = 0.0;

            try
            {
                dOut = double.Parse(sIn);
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }

        private void textBox_ModelingDayCount_Leave(object sender, EventArgs e)
        {
            double dModelDays = 0.0;
            bool bResultModelDays = false;
            int iModelDays = 0;

            bResultModelDays = ConvertStrToDouble(textBox_ModelingDayCount.Text, out dModelDays);


            if (bResultModelDays)
            {
                iModelDays = (int)dModelDays;
                if (iModelDays.ToString() == textBox_ModelingDayCount.Text)
                {
                    if (iModelDays > 0)
                    {
                        Params.ModelingDayToWork = iModelDays;
                        return;
                    }
                    else bResultModelDays = false;
                }
                else
                {
                    bResultModelDays = false;
                }
            }

            if (!bResultModelDays)
            {
                MessageBox.Show("Введено некорректное значение!");
                textBox_ModelingDayCount.Text = ModelingDayToWork_BASE.ToString();
            }


            iModelDays = 5;


            /*
                        if (!bResultUrg || !bResultRef)
                        {
                            string str = "Не удается преобразовать значения параметра к float:";
                            if (!(bResultUrg))
                            {
                                str = str + "\nВероятность срочности заявки";
                                textBox2.Text = dUrg_BASE.ToString();
                            }
                            if (!(bResultRef))
                            {
                                str = str + "\nВероятность отаза от заявки";
                                textBox3.Text = dRef_BASE.ToString();
                            }
                            MessageBox.Show(str);
                            return false;
                        }
             * */
        }

        private void idle_button_Click(object sender, EventArgs e)
        {
            var x = new int[1][];
            var y = new Dictionary<int, double[]>();
            var days = new List<int>();

            for (int i = 0; i < imitator.GetIdlePerDayStatistic().Length; i++)
            {
                days.Add(i + 1);
            }
            x[0] = days.ToArray();
            y[0] = imitator.GetIdlePerDayStatistic();

            var lines = new[] { "Доля времени простоя" };

            GraphForm gr = new GraphForm(y, x, lines, "Дни", "Доля", "Изменение доли времени простоя от времени производства");

            gr.ShowDialog();
        }

        private void averageDelay_button_Click(object sender, EventArgs e)
        {
            var x = new int[1][];
            var y = new Dictionary<int, double[]>();
            var days = new List<int>();

            for (int i = 0; i < imitator.GetDemandAverageDelayPerDayStatistic().Length; i++)
            {
                days.Add(i + 1);
            }

            x[0] = days.ToArray();
            y[0] = imitator.GetDemandAverageDelayPerDayStatistic();

            var lines = new[] { "Среднее время задержки заказов ('-1' - нет выполненных заказов)" };

            GraphForm gr = new GraphForm(y, x, lines, "Дни", "Среднее время (дни)", "Изменение среднего времени задержки заказов");

            gr.ShowDialog();
        }

        private void finish_button_Click(object sender, EventArgs e)
        {
            var x = new int[1][];
            var y = new Dictionary<int, double[]>();
            var days = new List<int>();
            var finishedDemandsPerDayStatistic = imitator.GetFinishedDemandsPerDayStatistic();

            for (int i = 0; i < finishedDemandsPerDayStatistic.Length; i++)
            {
                days.Add(i + 1);
            }

            x[0] = days.ToArray();
            y[0] = finishedDemandsPerDayStatistic;

            var lines = new[] { "Доля выполненных заказов" };

            GraphForm gr = new GraphForm(y, x, lines, "Дни", "Доля", "Изменение доли выполненных заказов");

            gr.ShowDialog();
        }

        private void front_office_button_Click(object sender, EventArgs e)
        {
            if (frontOfficeFlag == true)
            {
                this.frontOfficeFlag = false;
                frontOffice.ClearOrders();
            }
            frontOffice.SetDate(this.imitator.GetModelingTime());
            Process.Start("IExplore.exe", frontOfficeUrl);
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            var x = new int[1][];
            var y = new Dictionary<int, double[]>();
            var days = new List<int>();
            var canceledDemandsPerDayStatistic = imitator.GetCanceledDemandsPerDayStatistic();

            for (int i = 0; i < canceledDemandsPerDayStatistic.Length; i++)
            {
                days.Add(i + 1);
            }

            x[0] = days.ToArray();
            y[0] = canceledDemandsPerDayStatistic;

            string[] lines = new string[1] { "Доля отменённых заказов" };

            GraphForm gr = new GraphForm(y, x, lines, "Дни", "Доля", "Изменение доли отменённых заказов");
            gr.ShowDialog();
        }
    }
}