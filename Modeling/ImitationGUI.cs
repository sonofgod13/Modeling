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
    public partial class ImitationGUI : Form
    {
        private bool modelFlag;
        private bool pauseModelFlag;
        private bool frontOfficeFlag;
        private CImitation imitator;
        private FrontOffice.FrontOfficeImitationHelper frontOffice;
        private string frontOfficeUrl;

        private delegate void B1Delegate(Button button, string text);
        private void setStringButton(Button button, string text)
        {
            if (button.InvokeRequired)
            {
                B1Delegate deleg = new B1Delegate(setStringButton);
                button.Invoke(deleg, new object[] { button, text });
            }
            else
            {
                button.Text = text;
            }

        }

        private delegate void B2Delegate(Button button, bool enabled);
        private void setEnableButton(Button button, bool enabled)
        {
            if (button.InvokeRequired)
            {
                B2Delegate deleg = new B2Delegate(setEnableButton);
                button.Invoke(deleg, new object[] { button, enabled });
            }
            else
            {
                button.Enabled = enabled;
            }

        }

        private delegate void lDelegate(Label label, string text);
        private void setLabelText(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                lDelegate deleg = new lDelegate(setLabelText);
                label.Invoke(deleg, new object[] { label, text });
            }
            else
            {
                label.Text = text;
            }

        }


        public ImitationGUI()  //при загрузке формы основных параметров
        {
            CParams.Initialization();   //инициализация начальных параметров
            InitializeComponent();

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

            dUrg_BASE = CParams.m_fUrgencyPropabilityDemand;    //вероятность срочности заявки
            dRef_BASE = CParams.m_fRefusePropabilityDemand;     //вероятность отказа от заявки
            this.textBox_UrgencyPropabilityDemand.Text = dUrg_BASE.ToString();          
            //вероятность срочности заявки (поле ввода)
            this.textBox_RefusePropabilityDemand.Text = dRef_BASE.ToString();          
            //вероятность отказа от заявки (поле ввода)

            ModelingDayToWork_BASE = CParams.m_iModelingDayToWork;  //время моделирования в днях
            this.textBox_ModelingDayCount.Text = ModelingDayToWork_BASE.ToString();
            //время моделирования в днях (поле ввода)
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            if (modelFlag == false)
            {
                if (!(CheckUrgPropabilities())) return;
                button_start.Text = "Пауза";
                modelFlag = true;
                frontOfficeFlag = true;
                setLabelText(this.activityFactorLabel, "NaN");
                setLabelText(this.modellingTimeLabel, "NaN");
                setLabelText(this.demandAverageDelayLabel, "NaN");
                setLabelText(this.retargetTimePercentLabel, "NaN");
                setLabelText(this.refuseNumLabel, "NaN");
                setLabelText(this.acceptedNumLabel, "NaN");
                setLabelText(this.finishedNumLabel, "NaN");
                //setLabelText(this.canceledNumLabel, "NaN");
                imitator = new CImitation();
                button_stop.Enabled = true;
                front_office_button.Enabled = false;
                Thread t = new Thread(delegate() 
                    {
                        bool end = false;
                        end = imitator.Start(this.modellingTimeLabel);
                        if (end == true)
                        {
                            setStringButton(this.button_start, "Старт");
                            modelFlag = false;
                            setEnableButton(this.button_stop, false);
                            double demandAverageDelay = this.imitator.getDemandAverageDelay();
                            if (demandAverageDelay == -1) setLabelText(this.demandAverageDelayLabel, "нет выполненных заказов");
                            else setLabelText(this.demandAverageDelayLabel, Math.Round(demandAverageDelay, 3).ToString() + " дней");
                            setLabelText(this.activityFactorLabel, Math.Round(imitator.getActivityFactor(), 5).ToString());
                            setLabelText(this.retargetTimePercentLabel, Math.Round(imitator.getRetargetTimePercent(), 5).ToString());
                            setLabelText(this.refuseNumLabel, imitator.getRefusesNum().ToString());
                            setLabelText(this.acceptedNumLabel, imitator.getAcceptedDemandsNum().ToString());
                            setLabelText(this.finishedNumLabel, imitator.getFinishedDemandsNum().ToString());
                            //setLabelText(this.canceledNumLabel, imitator.getCanceledDemandsNum().ToString());
                            setEnableButton(this.materials_button, true);
                            setEnableButton(this.idle_button, true);
                            setEnableButton(this.averageDelay_button, true);
                            setEnableButton(this.finish_button, true);
                            //setEnableButton(this.cancel_button, true);
                            setEnableButton(this.front_office_button, true);
                        }
                    });
                t.IsBackground = true;
                t.Priority = ThreadPriority.Highest;
                t.Start();
                
                
            }
            else
            {
                if (pauseModelFlag == false)
                {
                    setEnableButton(this.button_start, false);
                    setEnableButton(this.button_stop, false);
                    button_start.Text = "Продолжить";
                    pauseModelFlag = true;
                    Thread t = new Thread(delegate() 
                    {
                        bool pauseDone = false;
                        pauseDone = imitator.Stop();
                        setEnableButton(this.button_start, true);
                        setEnableButton(this.button_stop, true);
                        if (pauseDone == true)
                        {                            
                            double demandAverageDelay = this.imitator.getDemandAverageDelay();
                            if (demandAverageDelay == -1) setLabelText(this.demandAverageDelayLabel, "нет выполненных заказов");
                            else setLabelText(this.demandAverageDelayLabel, Math.Round(demandAverageDelay, 3).ToString() + " дней");
                            setLabelText(this.activityFactorLabel, Math.Round(imitator.getActivityFactor(), 5).ToString());
                            setLabelText(this.retargetTimePercentLabel, Math.Round(imitator.getRetargetTimePercent(), 5).ToString());
                            setLabelText(this.refuseNumLabel, imitator.getRefusesNum().ToString());
                            setLabelText(this.acceptedNumLabel, imitator.getAcceptedDemandsNum().ToString());
                            setLabelText(this.finishedNumLabel, imitator.getFinishedDemandsNum().ToString());
                            //setLabelText(this.canceledNumLabel, imitator.getCanceledDemandsNum().ToString());
                            setEnableButton(this.materials_button, true);
                            setEnableButton(this.idle_button, true);
                            setEnableButton(this.averageDelay_button, true);
                            setEnableButton(this.finish_button, true);
                            //setEnableButton(this.cancel_button, true);
                            setEnableButton(this.front_office_button, true);
                        }
                    });
                    t.IsBackground = true;
                    t.Priority = ThreadPriority.Normal;
                    t.Start();
                }
                else
                {
                    button_start.Text = "Пауза";
                    materials_button.Enabled = false;
                    idle_button.Enabled = false;
                    averageDelay_button.Enabled = false;
                    finish_button.Enabled = false;
                    //cancel_button.Enabled = false;
                    pauseModelFlag = false;
                    front_office_button.Enabled = false;
                    this.imitator.saveNewFrontDemands(frontOffice.GetNewAtDate(this.imitator.getModelingTime()));                    
                    this.imitator.saveChangedFrontDemands(frontOffice.GetChangedAtDate(this.imitator.getModelingTime()));
                    
                    Thread t = new Thread(delegate()
                    {
                        bool end = false;
                        end = imitator.Continue(this.modellingTimeLabel);
                        if (end == true)
                        {
                            setStringButton(this.button_start, "Старт");
                            modelFlag = false;
                            setEnableButton(this.button_stop,false);
                            double demandAverageDelay = this.imitator.getDemandAverageDelay();
                            if (demandAverageDelay == -1) setLabelText(this.demandAverageDelayLabel, "нет выполненных заказов");
                            else setLabelText(this.demandAverageDelayLabel, Math.Round(demandAverageDelay, 3).ToString() + " дней");
                            setLabelText(this.activityFactorLabel, Math.Round(imitator.getActivityFactor(), 5).ToString());
                            setLabelText(this.retargetTimePercentLabel, Math.Round(imitator.getRetargetTimePercent(), 5).ToString());
                            setLabelText(this.refuseNumLabel, imitator.getRefusesNum().ToString());
                            setLabelText(this.acceptedNumLabel, imitator.getAcceptedDemandsNum().ToString());
                            setLabelText(this.finishedNumLabel, imitator.getFinishedDemandsNum().ToString());
                            //setLabelText(this.canceledNumLabel, imitator.getCanceledDemandsNum().ToString());
                            setEnableButton(this.materials_button, true);
                            setEnableButton(this.idle_button, true);
                            setEnableButton(this.averageDelay_button, true);
                            setEnableButton(this.finish_button, true);
                            //setEnableButton(this.cancel_button, true);
                            setEnableButton(this.front_office_button, true);
                        }
                    });
                    t.IsBackground = true;
                    t.Priority = ThreadPriority.Highest;                    
                    t.Start(); 
                    
                }
            }
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            modelFlag = false;
            pauseModelFlag = false;
            setEnableButton(this.button_start, false);
            setEnableButton(this.button_stop, false);
            Thread t = new Thread(delegate() 
                    {
                        bool stopDone = false;
                        stopDone = imitator.Stop();
                        setEnableButton(this.button_start, true);
                        if (stopDone == true)
                        {
                            double demandAverageDelay = this.imitator.getDemandAverageDelay();
                            if (demandAverageDelay == -1) setLabelText(this.demandAverageDelayLabel, "нет выполненных заказов");
                            else setLabelText(this.demandAverageDelayLabel, Math.Round(demandAverageDelay, 3).ToString() + " дней");
                            setLabelText(this.activityFactorLabel, Math.Round(imitator.getActivityFactor(), 5).ToString());
                            setLabelText(this.retargetTimePercentLabel, Math.Round(imitator.getRetargetTimePercent(), 5).ToString());
                            setLabelText(this.refuseNumLabel, imitator.getRefusesNum().ToString());
                            setLabelText(this.acceptedNumLabel, imitator.getAcceptedDemandsNum().ToString());
                            setLabelText(this.finishedNumLabel, imitator.getFinishedDemandsNum().ToString());
                            //setLabelText(this.canceledNumLabel, imitator.getCanceledDemandsNum().ToString());
                            setEnableButton(materials_button,true);
                            setEnableButton(idle_button, true);
                            setEnableButton(averageDelay_button, true);
                            setEnableButton(finish_button,true);
                            //cancel_button.Enabled = true;
                            setEnableButton(front_office_button, true);
                        }
                    });
                    t.IsBackground = true;
                    t.Priority = ThreadPriority.Normal;
                    t.Start();
                    button_start.Text = "Старт";
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private bool SetGeneratorParameters(ref GeneratorType iGeneratorType, ref double fGeneratorParamFirst, ref double fGeneratorParamSecond) 
            //функция установки параметров генератора
        {
            bool bSetResult = false;
            //iGeneratorType = 0;
            //fGeneratorParamFirst = 0.0;
            //fGeneratorParamSecond = 0.0;

            using (generator_params generatorForm = new generator_params(iGeneratorType, fGeneratorParamFirst, fGeneratorParamSecond))
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


        private void Z_g_time_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_generatorDemandsTime.m_iGeneratorType,
                ref CParams.m_generatorDemandsTime.m_fA,
                ref CParams.m_generatorDemandsTime.m_fB
                );
        }

        private void Z_g_p1_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_products[1].m_iGeneratorType,
                ref CParams.m_products[1].m_fA,
                ref CParams.m_products[1].m_fB
                );
        }

        private void Z_g_p2_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_products[2].m_iGeneratorType,
                ref CParams.m_products[2].m_fA,
                ref CParams.m_products[2].m_fB
                );
        }

        private void Z_g_p3_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_products[3].m_iGeneratorType,
                ref CParams.m_products[3].m_fA,
                ref CParams.m_products[3].m_fB
                );
        }

        private void M_g_m1_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[1].m_iGeneratorType,
                ref CParams.m_materials[1].m_fA,
                ref CParams.m_materials[1].m_fB
                );
        }

        private void M_g_m2_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[2].m_iGeneratorType,
                ref CParams.m_materials[2].m_fA,
                ref CParams.m_materials[2].m_fB
                );
        }

        private void M_g_m3_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[3].m_iGeneratorType,
                ref CParams.m_materials[3].m_fA,
                ref CParams.m_materials[3].m_fB
                );
        }

        private void M_g_m4_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[4].m_iGeneratorType,
                ref CParams.m_materials[4].m_fA,
                ref CParams.m_materials[4].m_fB
                );
        }

        private void M_g_m5_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[5].m_iGeneratorType,
                ref CParams.m_materials[5].m_fA,
                ref CParams.m_materials[5].m_fB
                );
        }

        private void M_g_m6_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[6].m_iGeneratorType,
                ref CParams.m_materials[6].m_fA,
                ref CParams.m_materials[6].m_fB
                );
        }

        private void M_g_m7_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[7].m_iGeneratorType,
                ref CParams.m_materials[7].m_fA,
                ref CParams.m_materials[7].m_fB
                );
        }

        private void M_g_m8_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[8].m_iGeneratorType,
                ref CParams.m_materials[8].m_fA,
                ref CParams.m_materials[8].m_fB
                );
        }

        private void M_g_m9_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[9].m_iGeneratorType,
                ref CParams.m_materials[9].m_fA,
                ref CParams.m_materials[9].m_fB
                );
        }

        private void M_g_m10_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[10].m_iGeneratorType,
                ref CParams.m_materials[10].m_fA,
                ref CParams.m_materials[10].m_fB
                );
        }

        private void M_g_m11_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[11].m_iGeneratorType,
                ref CParams.m_materials[11].m_fA,
                ref CParams.m_materials[11].m_fB
                );
        }

        private void M_g_m12_button_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_materials[12].m_iGeneratorType,
                ref CParams.m_materials[12].m_fA,
                ref CParams.m_materials[12].m_fB
                );
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_deliveryDelayGenerator.m_iGeneratorType,
                ref CParams.m_deliveryDelayGenerator.m_fA,
                ref CParams.m_deliveryDelayGenerator.m_fB
                );
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_demandModifyTime.m_iGeneratorType,
                ref CParams.m_demandModifyTime.m_fA,
                ref CParams.m_demandModifyTime.m_fB
                );
        }

        private void button8_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_articlesModify.m_iGeneratorType,
                ref CParams.m_articlesModify.m_fA,
                ref CParams.m_articlesModify.m_fB
                );
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_products[1].m_modify.m_iGeneratorType,
                ref CParams.m_products[1].m_modify.m_fA,
                ref CParams.m_products[1].m_modify.m_fB
                );
        }

        private void button6_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_products[2].m_modify.m_iGeneratorType,
                ref CParams.m_products[2].m_modify.m_fA,
                ref CParams.m_products[2].m_modify.m_fB
                );
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_products[3].m_modify.m_iGeneratorType,
                ref CParams.m_products[3].m_modify.m_fA,
                ref CParams.m_products[3].m_modify.m_fB
                );
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_ugrToStandModify.m_iGeneratorType,
                ref CParams.m_ugrToStandModify.m_fA,
                ref CParams.m_ugrToStandModify.m_fB
                );
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool bSetOK = false;

            bSetOK = SetGeneratorParameters(
                ref CParams.m_standToUrgModify.m_iGeneratorType,
                ref CParams.m_standToUrgModify.m_fA,
                ref CParams.m_standToUrgModify.m_fB
                );
        }

        private void materials_button_Click(object sender, EventArgs e)
        {
            int[][] x = new int[CParams.MATERIALS_NUMBER][];
            List<int> days = new List<int>();
            for (int i = 0; i < imitator.getMaterialsPerDayStatistic()[0].Length; i++)
            {
                days.Add(i+1);
            }
            for (int i=0;i<CParams.MATERIALS_NUMBER;i++)
            {                       
                x[i] = days.ToArray();
            }
            string[] lines = new string[12] {
                "Материал 1","Материал 2","Материал 3","Материал 4",
                "Материал 5","Материал 6","Материал 7","Материал 8",
                "Материал 9","Материал 10","Материал 11","Материал 12" };
            Graph gr = new Graph(imitator.getMaterialsPerDayStatistic(),x,lines,"Дни","Количество","Изменение количества материалов на складе");
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

            if ((dUrg<0) || (dRef<0))
            {
                string str = "Параметр должен быть больше или равен нулю:";
                if (dUrg<0)
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
            CParams.m_fRefusePropabilityDemand = dRef;
            CParams.m_fUrgencyPropabilityDemand = dUrg;
            return true;
        }

        public static bool ConvertStrToDouble(string sIn, out double dOut)
            //преобразует строку в double иначе возвращает false
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
                        CParams.m_iModelingDayToWork = iModelDays;
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
            int[][] x = new int[1][];
            Dictionary<int, double[]> y = new Dictionary<int, double[]>();
            List<int> days = new List<int>();
            for (int i = 0; i < imitator.getIdlePerDayStatistic().Length; i++)
            {
                days.Add(i + 1);
            }
            x[0] = days.ToArray();
            y[0] = imitator.getIdlePerDayStatistic();
            string[] lines = new string[1] {"Доля времени простоя"};
            Graph gr = new Graph(y, x, lines, "Дни", "Доля", "Изменение доли времени простоя от времени производства");
            gr.ShowDialog();
        }

        private void averageDelay_button_Click(object sender, EventArgs e)
        {
            int[][] x = new int[1][];
            Dictionary<int, double[]> y = new Dictionary<int, double[]>();
            List<int> days = new List<int>();
            for (int i = 0; i < imitator.getDemandAverageDelayPerDayStatistic().Length; i++)
            {
                days.Add(i + 1);
            }
            x[0] = days.ToArray();
            y[0] = imitator.getDemandAverageDelayPerDayStatistic();
            string[] lines = new string[1] { "Среднее время задержки заказов ('-1' - нет выполненных заказов)" };
            Graph gr = new Graph(y, x, lines, "Дни", "Среднее время (дни)", "Изменение среднего времени задержки заказов");
            gr.ShowDialog();
        }

        private void finish_button_Click(object sender, EventArgs e)
        {
            int[][] x = new int[1][];
            Dictionary<int, double[]> y = new Dictionary<int, double[]>();
            List<int> days = new List<int>();
            double[] finishedDemandsPerDayStatistic = imitator.getFinishedDemandsPerDayStatistic();
            for (int i = 0; i < finishedDemandsPerDayStatistic.Length; i++)
            {
                days.Add(i + 1);
            }
            x[0] = days.ToArray();
            y[0] = finishedDemandsPerDayStatistic;
            string[] lines = new string[1] { "Доля выполненных заказов" };
            Graph gr = new Graph(y, x, lines, "Дни", "Доля", "Изменение доли выполненных заказов");
            gr.ShowDialog();
        }

        private void front_office_button_Click(object sender, EventArgs e)
        {
            if (frontOfficeFlag == true)  
            {
                this.frontOfficeFlag = false;
                frontOffice.ClearOrders();                              
            }
            frontOffice.SetDate(this.imitator.getModelingTime());                                
            Process.Start("IExplore.exe", frontOfficeUrl);                        
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            int[][] x = new int[1][];
            Dictionary<int, double[]> y = new Dictionary<int, double[]>();
            List<int> days = new List<int>();
            double[] canceledDemandsPerDayStatistic = imitator.getCanceledDemandsPerDayStatistic();
            for (int i = 0; i < canceledDemandsPerDayStatistic.Length; i++)
            {
                days.Add(i + 1);
            }
            x[0] = days.ToArray();
            y[0] = canceledDemandsPerDayStatistic;
            string[] lines = new string[1] { "Доля отменённых заказов" };
            Graph gr = new Graph(y, x, lines, "Дни", "Доля", "Изменение доли отменённых заказов");
            gr.ShowDialog();
        }

    }
}