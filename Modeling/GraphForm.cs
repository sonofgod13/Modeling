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
    public partial class GraphForm : Form
    {
        static Color[] colors = new Color[12] {
            Color.Blue, Color.Red, Color.Green, Color.Yellow,
            Color.Black, Color.Brown, Color.Khaki, Color.Gray,
            Color.Orange, Color.YellowGreen, Color.Teal, Color.Pink };


        private GraphForm()
        {
            InitializeComponent();
        }

        public GraphForm(double[][] YAxis, int[][] XAxis, string[] linesLegend, string XAxisTitle, string YAxisTitle, string graphTitle)
            : this()
        {
            this.InitGraph(YAxis, XAxis, linesLegend, XAxisTitle, YAxisTitle, graphTitle);
        }

        public GraphForm(int[][] YAxis, int[][] XAxis, string[] linesLegend, string XAxisTitle, string YAxisTitle, string graphTitle)
            : this()
        {
            // sould be rewritten
            var YAxisConverted = YAxis.Select(
                                    values => values.Select(
                                        value => (double)value
                                    ).ToArray()
                                ).ToArray();

            this.InitGraph(YAxisConverted, XAxis, linesLegend, XAxisTitle, YAxisTitle, graphTitle);
        }

        private void InitGraph(double[][] YAxis, int[][] XAxis, string[] linesLegend, string XAxisTitle, string YAxisTitle, string graphTitle)
        {
            var graphPane = this.zedGraphControl1.GraphPane;

            var xMax = int.MinValue;

            PointPairList pairList;

            for (int lineIndex = 0; lineIndex < YAxis.Length; lineIndex++)
            {
                pairList = new PointPairList();
                for (int pointIndex = 0; pointIndex < YAxis[lineIndex].Length; pointIndex++)
                {
                    xMax = Math.Max(xMax, XAxis[lineIndex][pointIndex]);

                    pairList.Add(
                        XAxis[lineIndex][pointIndex], 
                        YAxis[lineIndex][pointIndex]
                    );
                }

                var curve = graphPane.AddCurve(linesLegend[lineIndex], pairList, colors[lineIndex], SymbolType.None);

                curve.Line.Width = 2.0F;
                curve.Line.IsAntiAlias = true;
            }

            graphPane.XAxis.Title.Text = XAxisTitle;
            graphPane.XAxis.Scale.Min = 1;
            graphPane.XAxis.Scale.Max = xMax + 1;
            graphPane.YAxis.Title.Text = YAxisTitle;
            graphPane.Title.Text = graphTitle;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            // В противном случае на рисунке будет показана только часть графика, 
            // которая умещается в интервалы по осям, установленные по умолчанию

            graphPane.AxisChange();

            // Обновляем график
            this.zedGraphControl1.Invalidate();
        }
    }
}
