using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QuantizationApp
{
    class UltimateDrawing2
    {
        public MainWindow window;

        Point from;
        Point to;
        public int levelOfQuantization = 6;
        public int numberOfSamples = 100;
        List<Point> functionPointsList = new List<Point>();
        float maxY = 2f;
        float localMax = 10f;
        private SolidColorBrush quantizationLineColor = Brushes.DarkBlue;
        private SolidColorBrush accuracyLineColor = Brushes.Red;
        public UltimateDrawing2(MainWindow _window)
        {
            window = _window;
        }

        public void DrawChart()
        {
            to = new Point(window.MainCanvas.ActualWidth, window.MainCanvas.ActualHeight);
            from = new Point(0, 0);

            int widthAccuracy = 1;

            DrawAxis();

            int stepXNumber = (int)((float)(to.X - from.X) / widthAccuracy);

            int stepX = (int)((to.X - from.X) / stepXNumber);

            List<Point> pointsList = new List<Point>();

            for (int i = (int)from.X; i <= to.X; i += stepX)
            {
                float y = Function(i);

                Point p = new Point();
                p.X = i;
                if (Math.Abs(y) != 42069f)
                    p.Y = F_toCoordY(y);
                else
                    p.Y = 42069f;
                pointsList.Add(p);
            }

            for (int i = 0; i < pointsList.Count - 1; i++)
            {
                Line line = new Line();
                line.X1 = pointsList[i].X;
                if (pointsList[i].Y == 42069f)
                    continue;
                if (pointsList[i].Y <= from.Y || pointsList[i].Y >= to.Y)
                    continue;
                line.Y1 = pointsList[i].Y;

                line.X2 = pointsList[i + 1].X;
                if (pointsList[i + 1].Y == 42069f)
                    continue;
                line.Y2 = pointsList[i + 1].Y;
                line.Stroke = Brushes.Black;
                window.FunctionCanvas.Children.Add(line);
            }

        }
        private int F_toCoordY(float f)
        {
            float multiplier = (float)window.ScaleYSlider.Value * 5;
            int ret = (int)((to.Y / 2) + f * multiplier);
            return ret;
        }

        private float Function(float x)
        {
            return window.GetFunctionValue(x);
        }

        private void DrawAxis()
        {
            DoubleCollection lineStyle = new DoubleCollection() { 5, 2 };

            Line lineX = new Line();
            lineX.X1 = 0;
            lineX.Y1 = from.Y;
            lineX.X2 = window.MainCanvas.ActualWidth;
            lineX.Y2 = from.Y;
            lineX.Stroke = Brushes.Black;
            lineX.StrokeDashArray = lineStyle;

            Line lineY = new Line();
            lineY.X1 = from.X + 20;
            lineY.Y1 = window.MainCanvas.ActualHeight;
            lineY.X2 = from.X + 20;
            lineY.Y2 = 0;
            lineY.Stroke = Brushes.Black;
            lineY.StrokeDashArray = lineStyle;


            window.AxisCanvas.Children.Add(lineX);
            window.AxisCanvas.Children.Add(lineY);
        }
        public void DrawLevels(int number)
        {
            window.LevelsCanvas.Children.Clear();

            int sizeY = (int)((to.Y - from.Y) / number);

            for (int i = (int)from.Y + sizeY; i < to.Y; i += sizeY)
            {
                Line line = new Line();
                line.X1 = 0;
                line.Y1 = i;
                line.X2 = window.MainCanvas.ActualWidth;
                line.Y2 = i;
                line.Stroke = Brushes.CadetBlue;
                line.StrokeDashArray = new DoubleCollection() { 5, 10 };
                window.LevelsCanvas.Children.Add(line);
            }
        }
        public void DrawMiddleLevels(int number)
        {
            int sizeY = (int)((to.Y) / number);

            for (int i = (int)from.Y + (sizeY / 2); i < to.Y; i += sizeY)
            {
                Line line = new Line();
                line.X1 = 0;
                line.Y1 = i;
                line.X2 = window.MainCanvas.ActualWidth;
                line.Y2 = i;
                line.Stroke = Brushes.LightBlue;
                line.StrokeDashArray = new DoubleCollection() { 5, 10 };
                window.LevelsCanvas.Children.Add(line);
            }

        }
        public void DrawMarks()
        {
            int samplingStep = (int)(window.MainCanvas.ActualWidth / numberOfSamples);//шаг дискритизации

            int markHeight = 6;

            for (int s = (int)from.X; s < window.MainCanvas.ActualWidth; s += samplingStep)
            {
                Line line = new Line();
                line.Stroke = Brushes.Gray;
                line.Y1 = from.Y - markHeight;
                line.Y2 = from.Y + markHeight;
                line.X1 = s;
                line.X2 = s;
                window.AxisCanvas.Children.Add(line);
            }
        }
        public void DrawUpAndDownQuantizationProcess()
        {
            int samplingStep = (int)(window.MainCanvas.ActualWidth / numberOfSamples);//шаг дискритизации
            int levelSize = (int)((to.Y - from.Y) / (levelOfQuantization));//размер в пикселях одного уровня 

            List<Line> linesList = new List<Line>();

            for (int s = (int)from.X; s < window.MainCanvas.ActualWidth; s += samplingStep)
            {
                Line line = new Line();
                line.StrokeThickness = 1;
                line.X1 = s;
                line.X2 = s;
                line.Stroke = quantizationLineColor;

                float y = Function(s);

                int yCoords = F_toCoordY(y);

                int level = (int)((float)yCoords / levelSize);
                if (window.QuantizationTypeComboBox.SelectedIndex == 1)
                {
                    level = LevelRounding(F_toCoordY(y), levelSize);
                }
                line.Y1 = from.Y;
                line.Y2 = from.Y+ (level * levelSize);

                window.QuantizationCanvas.Children.Add(line);

                linesList.Add(line);

                if (linesList.Count != 1)
                {
                    Line prevLine = linesList[linesList.Count - 2];
                    Line newLine = new Line();
                    newLine.Stroke = quantizationLineColor;
                    newLine.X1 = prevLine.X1;
                    newLine.X2 = line.X2;

                    if (prevLine.Y2 <= line.Y2)
                    {
                        newLine.Y1 = prevLine.Y2;
                        newLine.Y2 = prevLine.Y2;
                    }
                    else
                    {
                        newLine.Y1 = line.Y2;
                        newLine.Y2 = line.Y2;
                    }
                    window.QuantizationCanvas.Children.Add(newLine);
                }

                Line accuracyLine = new Line();
                accuracyLine.Stroke = accuracyLineColor;
                accuracyLine.StrokeThickness = 2 ;
                accuracyLine.X1 = s;
                accuracyLine.X2 = s;
                accuracyLine.Y1 = from.Y + (level * levelSize);
                accuracyLine.Y2 = yCoords;

                window.AccuracyCanvas.Children.Add(accuracyLine);
            }
        }
        public void DrawMiddleTypeQuantizationProcess()
        {
            int samplingStep = 1;//(int)(window.MainCanvas.ActualWidth / numberOfSamples);//шаг дискритизации
            int levelSize = (int)((to.Y - from.Y) / (levelOfQuantization));//размер в пикселях одного уровня
            int subLevelSize = levelSize / 2;

            int accuracy = 2; //погрешность

            List<Line> linesList = new List<Line>();
            List<Line> horizontalLinesList = new List<Line>();

            float prevY = 0f;

            for (int s = (int)from.X; s < window.MainCanvas.ActualWidth; s += samplingStep)
            {
                float y = Function(s);
                int yC = F_toCoordY(y);

                int positionInLevel = yC % levelSize;

                int levelNumber = LevelRounding(yC, levelSize);

                if (Math.Abs(subLevelSize - positionInLevel) <= accuracy)
                {
                    Line line = new Line();
                    line.Stroke = quantizationLineColor;
                    line.X1 = s;
                    line.X2 = s;

                    if (prevY < y)
                    {
                        line.Y1 = (levelNumber - 1) * levelSize + from.Y;
                        line.Y2 = levelNumber * levelSize + from.Y;
                    }
                    else
                    {
                        line.Y1 = levelNumber * levelSize + from.Y;
                        line.Y2 = (levelNumber - 1) * levelSize + from.Y;
                    }

                    linesList.Add(line);

                    window.QuantizationCanvas.Children.Add(line);

                    Line addLine = new Line();
                    addLine.Stroke = quantizationLineColor;
                    addLine.X1 = line.X1;
                    addLine.X2 = line.X2;

                    if (line.Y2 > line.Y1)
                        addLine.Y2 = line.Y1;
                    else addLine.Y2 = line.Y2;

                    addLine.Y1 = from.Y;

                    window.QuantizationCanvas.Children.Add(addLine);

                    s += 15;//пропускаем несколько позиций
                }
                prevY = y;
            }

            for (int i = 0; i < linesList.Count; i++)
            {
                Line line = linesList[i];
                Line newLine = new Line();
                newLine.Stroke = quantizationLineColor;
                if (i == 0) newLine.X1 = 0;
                else newLine.X1 = linesList[i - 1].X1;

                newLine.X2 = line.X2;

                double y = line.Y1;
                if (i != 0)
                {
                    y = linesList[i-1].Y2;
                }
                
                newLine.Y1 = y;
                newLine.Y2 = y;

                horizontalLinesList.Add(newLine);

                window.QuantizationCanvas.Children.Add(newLine);
            }

            if (linesList.Count != 0)
            {
                Line line = linesList[linesList.Count - 1];
                Line lastLine = new Line();
                lastLine.Stroke = quantizationLineColor;
                lastLine.X1 = line.X1;
                lastLine.X2 = window.MainCanvas.ActualWidth;

                lastLine.Y1 = line.Y2;
                lastLine.Y2 = line.Y2;

                horizontalLinesList.Add(lastLine);
                window.QuantizationCanvas.Children.Add(lastLine);
            }

            int pos = 0;

            foreach(Line line in horizontalLinesList)
            {
                int step = samplingStep; 
                //for ()
            }

        }
        public int LevelRounding(float initValue, int levelSize)
        {
            float level = (int)(initValue / levelSize);
            if (level < (initValue / levelSize))
            {
                level++;
            }
            return (int)level;
        }
    }
}
