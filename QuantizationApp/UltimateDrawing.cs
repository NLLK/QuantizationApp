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
    class UltimateDrawing
    {
        public MainWindow window;

        Point from;
        Point to;
        public int levelOfQuantization = 6;
        public int numberOfSamples = 100;
        List<Point> functionPointsList = new List<Point>();

        public UltimateDrawing(MainWindow _window)
        {
            window = _window;

        }

        public void DrawChart()
        {
            to = new Point(window.MainCanvas.ActualWidth, window.MainCanvas.ActualHeight / 4);
            from = new Point(0, window.MainCanvas.ActualHeight / 2);

            int widthAccuracy = 1;

            DrawAxis();

            int stepXNumber = (int)((float)(to.X - from.X) / widthAccuracy);

            int stepX = (int)((to.X - from.X) / stepXNumber);

            List<int> xCoordsList = new List<int>();
            for (int i = 0; i <= to.X; i += stepX)
            {
                xCoordsList.Add(i);
            }

            List<Point> pointsList = new List<Point>();

            for (int i = (int)from.X; i <= to.X; i += stepX)
            {
                Point p = new Point();
                p.X = i;
                p.Y = (from.Y - to.Y * Function((float)i));
                pointsList.Add(p);
            }

            functionPointsList = pointsList;

            for (int i = 0; i < pointsList.Count - 1; i++)
            {
                Line line = new Line();
                line.X1 = pointsList[i].X;
                line.Y1 = pointsList[i].Y;
                line.X2 = pointsList[i + 1].X;
                line.Y2 = pointsList[i + 1].Y;
                line.Stroke = Brushes.Black;
                window.FunctionCanvas.Children.Add(line);
            }





        }
        private float Function(float x)
        {
            x *= (float)Math.PI / 180;
            return (float)Math.Sin(x) + (float)Math.Sin(1.5 * x);
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
            lineY.X1 = from.X + 100;
            lineY.Y1 = window.MainCanvas.ActualHeight;
            lineY.X2 = from.X + 100;
            lineY.Y2 = 0;
            lineY.Stroke = Brushes.Black;
            lineY.StrokeDashArray = lineStyle;


            window.AxisCanvas.Children.Add(lineX);
            window.AxisCanvas.Children.Add(lineY);
        }
        public void DrawLevels(int number)
        {
            window.LevelsCanvas.Children.Clear();

            int sizeY = (int)(window.MainCanvas.ActualHeight / number);

            for (int i = 1; i <= number / 2; i++)
            {
                Line line = new Line();
                line.X1 = 0;
                line.Y1 = window.MainCanvas.ActualHeight / 2 + sizeY * i;
                line.X2 = window.MainCanvas.ActualWidth;
                line.Y2 = window.MainCanvas.ActualHeight / 2 + sizeY * i;
                line.Stroke = Brushes.Blue;
                line.StrokeDashArray = new DoubleCollection() { 5, 10 };
                window.LevelsCanvas.Children.Add(line);
            }
            for (int i = 1; i <= number / 2; i++)
            {
                Line line = new Line();
                line.X1 = 0;
                line.Y1 = window.MainCanvas.ActualHeight / 2 - sizeY * i;
                line.X2 = window.MainCanvas.ActualWidth;
                line.Y2 = window.MainCanvas.ActualHeight / 2 - sizeY * i;
                line.Stroke = Brushes.Blue;
                line.StrokeDashArray = new DoubleCollection() { 5, 10 };
                window.LevelsCanvas.Children.Add(line);
            }

        }
        public void SomeQuantizationProcess()
        {
            float max = 0;
            foreach (Point p in functionPointsList)
            {
                if (p.Y > max) max = (float)p.Y;
            }
            float maxY = (max - (float)from.Y) / (float)to.Y;


            int samplingStep = (int)(window.MainCanvas.ActualWidth / numberOfSamples);//шаг дискритизации
            int levelSize = (int)((window.MainCanvas.ActualHeight / 2) / (levelOfQuantization / 2));//размер в пикселях одного уровня

            float levelSizeY = (float)(maxY / (levelOfQuantization / 2));

            List<Line> linesList = new List<Line>();

            for (int s = (int)from.X; s < window.MainCanvas.ActualWidth; s += samplingStep)
            {
                float y = Function(s);
                int level = (int)(y / levelSizeY);



                Console.WriteLine($"{s:f2},\t{y:f2},\t{level:f2} ");

                Line line = new Line();
                line.X1 = s;
                line.X2 = s;
                line.Stroke = Brushes.Red;
                line.Y1 = window.MainCanvas.ActualHeight / 2;
                if (y > 0)
                    line.Y2 = window.MainCanvas.ActualHeight / 2 - (level + 1) * levelSize;
                else if (y < 0)
                    line.Y2 = window.MainCanvas.ActualHeight / 2 - (level - 1) * levelSize;

                window.QuantizationCanvas.Children.Add(line);
                linesList.Add(line);

                if (linesList.Count != 1)
                {
                    Line prevLine = linesList[linesList.Count - 2];
                    Line newLine = new Line();
                    newLine.Stroke = Brushes.Red;
                    newLine.X1 = prevLine.X1;
                    newLine.X2 = line.X2;

                    if ((window.MainCanvas.ActualHeight / 2) - prevLine.Y2 > 100 && (window.MainCanvas.ActualHeight / 2) - line.Y2 >100) //над уровнем нуля
                    {
                        if (prevLine.Y2 >= line.Y2)
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

                    else if (((window.MainCanvas.ActualHeight / 2) - prevLine.Y2 == 0 && (window.MainCanvas.ActualHeight / 2) - line.Y2 == 0))
                    {
                        
                    }

                    else
                    {
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


                }


            }
        }
    }
}
