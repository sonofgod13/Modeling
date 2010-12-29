using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace Modeling
{
    public partial class Graph : Form
    {
        static Color[] colors = new Color[12] {
            Color.Blue, Color.Red, Color.Green, Color.Yellow,
            Color.Black, Color.Brown, Color.Khaki, Color.Gray,
            Color.Orange, Color.YellowGreen, Color.Teal, Color.Pink };

        public Graph(int[][] Y, int[][] X, string[] lines, string XAxis, string YAxis, string title)
        {
            InitializeComponent();
            /*
            this.zedGraphControl1.GraphPane.Chart.Fill = new ZedGraph.Fill(Color.Cornsilk);
            
            PointPairList list = new PointPairList();
            const int count = 105;
            for (int i = 0; i < count; i++)
            {
                double x = i + 1;

                double y = 21.1 * (1.0 + Math.Sin((double)i * 0.15));

                list.Add(x, y);
            }
            
            // Hide the legend
            this.zedGraphControl1.GraphPane.Legend.IsVisible = true;

            // Add a curve
            LineItem curve = this.zedGraphControl1.GraphPane.AddCurve("line", list, Color.Red, SymbolType.None);
            curve.Line.Width = 2.0F;
            curve.Line.IsAntiAlias = true;
            curve.Symbol.Fill = new Fill(Color.White);
            curve.Symbol.Size = 7;
           */
            
            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            this.zedGraphControl1.GraphPane.CurveList.Clear();

            // Создадим список точек для кривой 
            List<PointPairList> fList = new List<PointPairList>();
            int? xMax=null;
            // !!!
            // Заполним массив точек для кривой f1(x)
            for (int i = 0; i < Y.Length; i ++)
            {
                PointPairList pL= new PointPairList();
                for (int j = 0; j < Y[i].Length; j++)
                {
                    if (xMax.HasValue == false) xMax = X[i][j];
                    else if (xMax.Value < X[i][j]) xMax = X[i][j];
                    pL.Add(X[i][j], Y[i][j]);
                }
                fList.Add(pL);
            }
                        

            // !!!
            // Создадим кривую с названием "Sinc", 
            // которая будет рисоваться голубым цветом (Color.Blue),
            // Опорные точки выделяться не будут (SymbolType.None)
            for (int i = 0; i < fList.Count; i++)
            {
                LineItem curve = this.zedGraphControl1.GraphPane.AddCurve(lines[i], fList[i], colors[i], SymbolType.None);
                curve.Line.Width = 2.0F;
                curve.Line.IsAntiAlias = true;
            }

            this.zedGraphControl1.GraphPane.XAxis.Title.Text = XAxis;
            this.zedGraphControl1.GraphPane.XAxis.Scale.Min = 1;
            this.zedGraphControl1.GraphPane.XAxis.Scale.Max = xMax.Value+1;
            this.zedGraphControl1.GraphPane.YAxis.Title.Text = YAxis;
            this.zedGraphControl1.GraphPane.Title.Text = title;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            // В противном случае на рисунке будет показана только часть графика, 
            // которая умещается в интервалы по осям, установленные по умолчанию
            
            this.zedGraphControl1.GraphPane.AxisChange();

            // Обновляем график
            this.zedGraphControl1.Invalidate();

        }

        public Graph(Dictionary<int,double[]> Y, int[][] X, string[] lines, string XAxis, string YAxis, string title)
        {
            InitializeComponent();
            
            this.zedGraphControl1.GraphPane.CurveList.Clear();
            List<PointPairList> fList = new List<PointPairList>();
            int? xMax = null;
            for (int i = 0; i < Y.Count; i++)
            {
                PointPairList pL = new PointPairList();
                for (int j = 0; j < Y[i].Length; j++)
                {
                    if (xMax.HasValue == false) xMax = X[i][j];
                    else if (xMax.Value < X[i][j]) xMax = X[i][j];
                    pL.Add(X[i][j], Y[i][j]);
                }
                fList.Add(pL);
            }

            for (int i = 0; i < fList.Count; i++)
            {
                LineItem curve = this.zedGraphControl1.GraphPane.AddCurve(lines[i], fList[i], colors[i], SymbolType.None);
                curve.Line.Width = 2.0F;
                curve.Line.IsAntiAlias = true;
            }
            
            this.zedGraphControl1.GraphPane.XAxis.Title.Text = XAxis;
            this.zedGraphControl1.GraphPane.XAxis.Scale.Min = 1;
            if (xMax.HasValue ==true) this.zedGraphControl1.GraphPane.XAxis.Scale.Max = xMax.Value + 1;
            this.zedGraphControl1.GraphPane.YAxis.Title.Text = YAxis;
            this.zedGraphControl1.GraphPane.Title.Text = title;

            this.zedGraphControl1.GraphPane.AxisChange();
            this.zedGraphControl1.Invalidate();
        }

        public Graph(Dictionary<int, double?[]> Y, int[][] X, string[] lines, string XAxis, string YAxis, string title)
        {
            InitializeComponent();

            this.zedGraphControl1.GraphPane.CurveList.Clear();
            List<PointPairList> fList = new List<PointPairList>();
            int? xMax = null;
            for (int i = 0; i < Y.Count; i++)
            {
                PointPairList pL = new PointPairList();
                for (int j = 0; j < Y[i].Length; j++)
                {
                    if (xMax.HasValue == false) xMax = X[i][j];
                    else if (xMax.Value < X[i][j]) xMax = X[i][j];
                    if (Y[i][j].HasValue == true) pL.Add(X[i][j], Y[i][j].Value);
                }
                fList.Add(pL);
            }

            for (int i = 0; i < fList.Count; i++)
            {
                LineItem curve = this.zedGraphControl1.GraphPane.AddCurve(lines[i], fList[i], colors[i], SymbolType.None);
                curve.Line.Width = 2.0F;
                curve.Line.IsAntiAlias = true;
            }

            this.zedGraphControl1.GraphPane.XAxis.Title.Text = XAxis;
            this.zedGraphControl1.GraphPane.XAxis.Scale.Min = 1;
            if (xMax.HasValue == true) this.zedGraphControl1.GraphPane.XAxis.Scale.Max = xMax.Value + 1;
            this.zedGraphControl1.GraphPane.YAxis.Title.Text = YAxis;
            this.zedGraphControl1.GraphPane.Title.Text = title;

            this.zedGraphControl1.GraphPane.AxisChange();
            this.zedGraphControl1.Invalidate();
        }
    }
}
