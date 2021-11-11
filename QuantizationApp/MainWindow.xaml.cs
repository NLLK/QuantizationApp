
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuantizationApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private UltimateDrawing ultimateDrawing;
        private UltimateDrawing2 ultimateDrawing;
        private bool WindowLoaded = false;

        private List<Function> FunctionsList = new List<Function>();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            FunctionsList.Add(new Function());
            FunctionsList.Add(new Function());
            FunctionsList.Add(new Function());
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowLoaded = true;
            ultimateDrawing = new UltimateDrawing2(this);//new UltimateDrawing(this);
            IntegerUpDown.Value = 6;
            SampleIntegerUpDown.Value = 100;
            DrawButton_Click(null, null);
        }
        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowLoaded)
            {
                FunctionCanvas.Children.Clear();
                AxisCanvas.Children.Clear();
                QuantizationCanvas.Children.Clear();
                LevelsCanvas.Children.Clear();
                AccuracyCanvas.Children.Clear();
                ultimateDrawing.DrawChart();
                ultimateDrawing.DrawMarks();
                if (IntegerUpDown.Value != null)
                    ultimateDrawing.DrawLevels((int)IntegerUpDown.Value);
                
                switch (QuantizationTypeComboBox.SelectedIndex)
                {
                    case 0: case 1: ultimateDrawing.DrawUpAndDownQuantizationProcess(); break;
                    case 2:
                        {
                            ultimateDrawing.DrawMiddleTypeQuantizationProcess();
                            ultimateDrawing.DrawMiddleLevels((int)IntegerUpDown.Value);
                            break;
                        }
                }
            }
        }

        private void Polygon_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            /*            var mousePoint = Mouse.GetPosition(MainCanvas);
                        MainCanvasScaleTransform.CenterX = MainCanvas.ActualWidth - mousePoint.X;
                        MainCanvasScaleTransform.CenterY = MainCanvas.ActualHeight - mousePoint.Y;*/
            return;

            if (e.Delta > 0)
            {
                MainCanvasScaleTransform.ScaleX += 0.1;
            }
            else
            {
                if (MainCanvasScaleTransform.ScaleX>1)
                    MainCanvasScaleTransform.ScaleX -= 0.1;
            }
        }

        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                ultimateDrawing.levelOfQuantization = (int)e.NewValue;
                ultimateDrawing.DrawLevels((int)e.NewValue);
            }
            DrawButton_Click(null, null);
        }


        private void Polygon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePoint = Mouse.GetPosition(MainCanvas);
            //MainCanvasScaleTransform.CenterX = mousePoint.X- MainCanvas.ActualWidth;

            if (e.ChangedButton == MouseButton.Middle)
            {
                MainCanvasScaleTransform.CenterX = 0;
                MainCanvasScaleTransform.CenterY = 0;
                MainCanvasScaleTransform.ScaleX = 1;
            }

        }

        private void SampleIntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                ultimateDrawing.numberOfSamples = (int)e.NewValue;
            }
            DrawButton_Click(null, null);
        }

        private void ScaleYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ScaleYTextBlock.Text = $"{((Slider)sender).Value:f1}";
            DrawButton_Click(null, null);
        }

        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawButton_Click(null, null);
        }

        private void ScaleYSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                ScaleYSlider.Value += ScaleYSlider.SmallChange;
            else
                ScaleYSlider.Value -= ScaleYSlider.SmallChange;
        }

        private void QuantizationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawButton_Click(null, null);
        }
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewTab.IsSelected)
                DrawButton_Click(null, null);
        }

        //
        //Настройки функции
        //

        private void FunctionSetupAddFunction_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int buttonIndex = Convert.ToInt32(button.Tag) - 1;
            int nextFunctionGridIndex = buttonIndex + 1;
            Grid nextGrid = (Grid)FunctionSetupGrid.Children[nextFunctionGridIndex];
            nextGrid.IsEnabled = true;
            FunctionsList[nextFunctionGridIndex] = new Function();
            UpdateFunctionPreview(nextFunctionGridIndex);

        }
        private void UpdateFunctionPreview(int senderIndex)
        {
            if (WindowLoaded)
            {
                string functionString = "y = ";

                for (int i = 0; i < 3; i++)
                {
                    if (FunctionSetupGrid.Children[i].IsEnabled)
                    {
                        functionString += GetFunctionPreviewByIndex(i);
                    }
                }

                FunctionSetupFunctionPreview.Text = functionString;
            }
        }

        private void UpdateFunctionPreviewTextChanged(object sender, TextChangedEventArgs e)
        {
            if (WindowLoaded)
            {
                int functionIndex = Convert.ToInt32(((TextBox)sender).Tag) - 1;
                Function newData = new Function((Grid)FunctionSetupGrid.Children[functionIndex]);
                FunctionsList[functionIndex] = newData;

                UpdateFunctionPreview(functionIndex);
            }
        }

        private void UpdateFunctionPreviewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WindowLoaded)
            {
                int functionIndex = Convert.ToInt32(((ComboBox)sender).Tag) - 1;
                Function newData = new Function((Grid)FunctionSetupGrid.Children[functionIndex]);
                FunctionsList[functionIndex] = newData;

                UpdateFunctionPreview(functionIndex);
            }
        }

        private string GetFunctionPreviewByIndex(int index)
        {
            Function function = FunctionsList[index];
            float constA = function.constA;
            string signA = function.signAString;
            string funcName = function.funcNameString;
            float constB = function.constB;
            string signB = function.signBString;

            string functionPart = "";

            if (function.funcName != FunctionType.X)
            {
                functionPart += "(";
                if (constA != 0f)
                {
                    functionPart += $"{constA:f2} {signA} ";
                }
                functionPart += $"{funcName}(";
                if (constB != 0f)
                {
                    functionPart += $"{constB:f2} {signB} ";
                }
                functionPart += "X))";
                //functionPart = $"({constA:f4} {signA} {funcName}({constB:f4} {signB} X))";
            }
            else
            {
                functionPart = $"({constA:f2} {signA} {funcName})";
            }

            if (index != 0)
            {
                string signString = FunctionsList[index - 1].signAfterString;

                functionPart = $" {signString} {functionPart}";
            }

            return functionPart;
        }

        public float GetFunctionValue(float x)
        {
            x *= (float)Math.PI / 180;
            float answer = 0;
            for (int i = 0; i < FunctionsList.Count; i++)
            {
                if (FunctionSetupGrid.Children[i].IsEnabled)
                {
                    Function f = FunctionsList[i];
                    float functionArg = DoMathmeticOperation(f.signB, f.constB, x);
                    float functionResult = DoMathmeticFunction(f.funcName, functionArg);
                    float preResult = DoMathmeticOperation(f.signA, f.constA, functionResult);
                    if (i != 0)
                        answer = DoMathmeticOperation(FunctionsList[i - 1].signAfter, answer, preResult);
                    else
                        answer = preResult;
                }
            }
            return answer;
        }
        public float DoMathmeticOperation(Sign sign, float op1, float op2)
        {
            switch (sign)
            {
                case Sign.Plus:
                    return op1 + op2;
                case Sign.Minus:
                    return op1 - op2;
                case Sign.Mulpiply:
                    return op1 * op2;
                case Sign.Devide:
                    {
                        if (Math.Abs(op2) < 0.0001)
                        {
                            if (op1>0)return 42069f;
                            else return -42069f;
                        }
                        else return op1 / op2;
                    }
            }
            return 0;
        }
        public float DoMathmeticFunction(FunctionType function, float op1)
        {

            switch (function)
            {
                case FunctionType.Sin:
                    return (float)Math.Sin(op1);
                case FunctionType.Cos:
                    return (float)Math.Cos(op1);
                case FunctionType.X:
                    return op1;
            }
            return 0;
        }


    }
    public enum Sign
    {
        Plus, Minus, Mulpiply, Devide
    }
    public enum FunctionType
    {
        Sin,
        Cos,
        X
    }
    public class Function
    {
        public float constA = 0;
        public string signAString = "+";
        public Sign signA = Sign.Plus;
        public string funcNameString = "Sin";
        public FunctionType funcName = FunctionType.Sin;
        public float constB = 0;
        public string signBString = "+";
        public Sign signB = Sign.Plus;
        public string signAfterString = "+";
        public Sign signAfter = Sign.Plus;

        public Function() { }
        public Function(Grid grid)
        {
            for (int i = 1; i < grid.Children.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        {
                            float a = 0f;
                            string text = ((TextBox)grid.Children[i]).Text.Replace('.', ',');
                            try
                            {
                                a = (float)Convert.ToDouble(text);
                            }
                            catch
                            {
                                a = 0;
                            }
                            finally
                            {
                                constA = a;
                            }
                            break;
                        }
                    case 2:
                        {
                            string text = ((TextBox)grid.Children[i]).Text;
                            signA = getSignByString(text);
                            signAString = getStringBySing(signA);
                            break;
                        }
                    case 3:
                        {
                            string text = ((TextBlock)(((ComboBox)grid.Children[i]).SelectedItem)).Text;
                            funcName = getFunctionTypeByString(text);
                            funcNameString = text;
                            break;
                        }
                    case 5:
                        {
                            float a = 0f;
                            string text = ((TextBox)grid.Children[i]).Text.Replace('.', ',');
                            try
                            {
                                a = (float)Convert.ToDouble(text);
                            }
                            catch
                            {
                                a = 0;
                            }
                            finally
                            {
                                constB = a;
                            }
                            break;
                        }
                    case 6:
                        {
                            string text = ((TextBox)grid.Children[i]).Text;
                            signB = getSignByString(text);
                            signBString = getStringBySing(signB);
                            break;
                        }
                    case 9:
                        {
                            Grid subGrid = (Grid)grid.Children[i];
                            string text = ((TextBox)subGrid.Children[0]).Text;
                            signAfter = getSignByString(text);
                            signAfterString = getStringBySing(signAfter);
                            break;
                        }
                    default: break;
                }
            }
        }
        static Sign getSignByString(string input)
        {
            switch (input)
            {
                case "+": return Sign.Plus;
                case "-": return Sign.Minus;
                case "*": return Sign.Mulpiply;
                case "/": return Sign.Devide;
                default: return Sign.Plus;
            }
        }
        static string getStringBySing(Sign input)
        {
            switch (input)
            {
                case Sign.Plus: return "+";
                case Sign.Minus: return "-";
                case Sign.Mulpiply: return "*";
                case Sign.Devide: return "/";
                default: return "+";
            }
        }
        static FunctionType getFunctionTypeByString(string input)
        {
            switch (input)
            {
                case "Sin": return FunctionType.Sin;
                case "Cos": return FunctionType.Cos;
                case "X": return FunctionType.X;
                default: return FunctionType.Sin;
            }
        }
    }
}
