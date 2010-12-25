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
        private ModelingState modelingState;

        private bool frontOfficeFlag;
        private Imitation imitator;

        private FrontOffice.FrontOfficeImitationHelper frontOffice;

        private string frontOfficeUrl;

        private Dictionary<Button, Func<GeneratedElement>> BindedButtons = new Dictionary<Button, Func<GeneratedElement>>();

        private void SetButtonText(Button button, string text)
        {
            this.InvokeOnFormThread(() => button.Text = text);
        }

        private void SetEnableButton(Button button, bool enabled)
        {
            this.InvokeOnFormThread(() => button.Enabled = enabled);
        }

        private void SetLabelText(Label label, string text)
        {
            this.InvokeOnFormThread(() => label.Text = text);
        }

        public ImitationGUIForm()  //при загрузке формы основных параметров
        {
            Params.Initialization();   //инициализация начальных параметров
            this.InitializeComponent();
            this.BindButtons();

            /////////  Это чтоб нельзя было задать параметры изменений срочности заявок
            label4.Visible = false;
            label3.Visible = false;
            button4.Visible = false;
            button3.Visible = false;
            ///////////////////////////////////////

            modelingState = ModelingState.Stopped;
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

        private void InvokeOnFormThread(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            switch (this.modelingState)
            {
                case ModelingState.Stopped:
                    if (!CheckUrgPropabilities())
                        return;

                    this.imitator = new Imitation();

                    this.modelingState = ModelingState.Running;

                    this.button_start.Text = "Пауза";

                    this.activityFactorLabel.Text = "NaN";
                    this.modellingTimeLabel.Text = "NaN";
                    this.demandAverageDelayLabel.Text = "NaN";
                    this.retargetTimePercentLabel.Text = "NaN";
                    this.refuseNumLabel.Text = "NaN";
                    this.acceptedNumLabel.Text = "NaN";
                    this.finishedNumLabel.Text = "NaN";

                    this.button_stop.Enabled = true;
                    this.front_office_button.Enabled = false;

                    this.StartImitation();
                    break;

                case ModelingState.Running:
                    this.button_start.Enabled = false;
                    this.button_stop.Enabled = false;
                    this.button_start.Text = "Продолжить";

                    this.modelingState = ModelingState.Paused;

                    this.PauseImitation();

                    break;

                case ModelingState.Paused:
                    this.button_start.Text = "Пауза";
                    this.materials_button.Enabled = false;
                    this.idle_button.Enabled = false;
                    this.averageDelay_button.Enabled = false;
                    this.finish_button.Enabled = false;

                    this.modelingState = ModelingState.Running;

                    this.front_office_button.Enabled = false;
                    this.imitator.SaveNewFrontDemands(frontOffice.GetNewAtDate(this.imitator.GetModelingTime()));
                    this.imitator.SaveChangedFrontDemands(frontOffice.GetChangedAtDate(this.imitator.GetModelingTime()));

                    this.ContinueImitation();

                    break;
            }
        }

        private void ContinueImitation()
        {
            ThreadStart worker = () =>
            {
                if (!imitator.Continue(this.modellingTimeLabel))
                    return;

                this.InvokeOnFormThread(() =>
                {
                    this.button_start.Text = "Старт";
                    this.button_stop.Enabled = false;
                });

                this.modelingState = ModelingState.Stopped;

                this.UpdateUI();
            };

            this.StartThread(worker, ThreadPriority.Highest);
        }

        private void PauseImitation()
        {
            ThreadStart worker = () =>
            {
                bool pauseDone = imitator.Stop();

                this.InvokeOnFormThread(() =>
                {
                    this.button_start.Enabled = true;
                    this.button_stop.Enabled = true;
                });

                if (pauseDone)
                {
                    this.UpdateUI();
                }
            };

            this.StartThread(worker, ThreadPriority.Normal);
        }

        private void StartImitation()
        {
            ThreadStart worker = () =>
            {
                bool end = imitator.Start(this.modellingTimeLabel);
                if (end)
                {
                    this.InvokeOnFormThread(() =>
                    {
                        this.button_start.Text = "Старт";
                        this.button_stop.Enabled = false;
                    });

                    this.modelingState = ModelingState.Stopped;

                    this.UpdateUI();
                }
            };

            this.StartThread(worker, ThreadPriority.Highest);
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            this.modelingState = ModelingState.Stopped;

            this.button_start.Enabled = false;
            this.button_stop.Enabled = false;

            this.StopImitation();

            button_start.Text = "Старт";
        }

        private void StopImitation()
        {
            ThreadStart worker = () =>
            {
                bool stopDone = imitator.Stop();

                this.InvokeOnFormThread(() => this.button_start.Enabled = true);

                if (stopDone)
                {
                    this.UpdateUI();
                }
            };

            this.StartThread(worker, ThreadPriority.Normal);
        }

        private void StartThread(ThreadStart threadWorker, ThreadPriority priority)
        {
            var thread = new Thread(threadWorker)
            {
                IsBackground = true,
                Priority = priority
            };

            thread.Start();
        }

        private void UpdateUI()
        {
            double demandAverageDelay = this.imitator.GetDemandAverageDelay();

            var demandAverageDelayLabelText = (demandAverageDelay == -1)
                ? "нет выполненных заказов"
                : String.Format("{0} дней", Math.Round(demandAverageDelay, 3));

            this.InvokeOnFormThread(() =>
            {
                this.demandAverageDelayLabel.Text = demandAverageDelayLabelText;

                this.activityFactorLabel.Text = Math.Round(imitator.GetActivityFactor(), 5).ToString();
                this.retargetTimePercentLabel.Text = Math.Round(imitator.GetRetargetTimePercent(), 5).ToString();

                this.refuseNumLabel.Text = imitator.GetRefusesNum().ToString();
                this.acceptedNumLabel.Text = imitator.GetAcceptedDemandsNum().ToString();
                this.finishedNumLabel.Text = imitator.GetFinishedDemandsNum().ToString();

                this.materials_button.Enabled = true;
                this.idle_button.Enabled = true;
                this.averageDelay_button.Enabled = true;
                this.finish_button.Enabled = true;

                this.front_office_button.Enabled = true;
            });
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

            var lines = new string[Params.MATERIALS_NUMBER];
            for (var i = 0; i < lines.Length; i++)
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

            bool bResultUrg = double.TryParse(textBox_UrgencyPropabilityDemand.Text, out dUrg);
            bool bResultRef = double.TryParse(textBox_RefusePropabilityDemand.Text, out dRef);

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

                MessageBox.Show("Сумма вероятностей срочности заявки и отаза от заявки не должна быть больше единицы!");
                return false;
            }

            Params.fRefusePropabilityDemand = dRef;
            Params.fUrgencyPropabilityDemand = dUrg;

            return true;
        }

        private void textBox_ModelingDayCount_Leave(object sender, EventArgs e)
        {
            double dModelDays = 0.0;
            int iModelDays = 0;

            bool bResultModelDays = double.TryParse(textBox_ModelingDayCount.Text, out dModelDays);

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
                    else
                        bResultModelDays = false;
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
            var y = new double[1][];
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
            var y = new double[1][];
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
            var y = new double[1][];
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
            var y = new double[1][];
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