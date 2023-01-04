using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace shapezAnalyzer
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _databasePath = "未载入数据库";

        public string DatabasePath
        {
            get => _databasePath;
            set
            {
                _databasePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DatabasePath"));
            }
        }

        private string _expression = "";

        public string Expression
        {
            get => _expression;
            set
            {
                _expression = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Expression"));
                RefreshUi();
            }
        }

        private string _formatCheckResultColor = "Black";

        public string FormatCheckResultColor
        {
            get => _formatCheckResultColor;
            set
            {
                _formatCheckResultColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FormatCheckResultColor"));
            }
        }

        private string _formatCheckResult = "↑快输入图形表达式试试看吧w↑";

        public string FormatCheckResult
        {
            get => _formatCheckResult;
            set
            {
                _formatCheckResult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FormatCheckResult"));
            }
        }

        private string _shapeInfo = "图形ID：\n\n多层表达式：";

        public string ShapeInfo
        {
            get => _shapeInfo;
            set
            {
                _shapeInfo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShapeInfo"));
            }
        }

        private string _compositionPathColor = "Black";

        public string CompositionPathColor
        {
            get => _compositionPathColor;
            set
            {
                _compositionPathColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CompositionPathColor"));
            }
        }

        private string _compositionPath = "输入正确的表达式后\n此处将显示合成路径";

        public string CompositionPath
        {
            get => _compositionPath;
            set
            {
                _compositionPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CompositionPath"));
            }
        }

        private readonly ShapeAnalyzer _analyzer = new ShapeAnalyzer();

        /// <summary>
        /// 自动载入与exe相同目录下的csv数据库文件。
        /// 直至使用某个文件成功载入数据库，或所有文件都读取失败，读取才会停止。
        /// </summary>
        public void AutoLoadDb()
        {
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
            bool loadSuccess = false;
            foreach (string file in files)
            {
                string extension = System.IO.Path.GetExtension(file); // 后缀名".csv"
                if (extension == ".csv")
                {
                    if (_analyzer.ChangeDb(file))
                    {
                        DatabasePath = System.IO.Path.GetFileNameWithoutExtension(file);
                        loadSuccess = true;
                        break;
                    }
                }
            }
            if (!loadSuccess)
            {
                DatabasePath = "自动读取数据库失败，请手动选择数据库文件！";
            }
        }

        /// <summary>
        /// 切换数据库文件。
        /// </summary>
        /// <param name="file">要载入的数据库文件</param>
        public void ChangeDb(string file)
        {
            if (_analyzer.ChangeDb(file))
            {
                DatabasePath = System.IO.Path.GetFileNameWithoutExtension(file);
                MessageBox.Show("成功载入数据库！", "成功啦");
                RefreshUi();
            }
            else
            {
                MessageBox.Show("载入数据库失败，请检查数据库文件！", "失败了");
            }
        }

        /// <summary>
        /// 更改主界面所有需要更改的内容。
        /// </summary>
        private void RefreshUi()
        {
            if (!IsExpressionFormatReasonable())
            {
                ShapeInfo = "图形ID：\n\n多层表达式：";
                CompositionPath = "输入正确的表达式后\n此处将显示合成路径";
                return;
            }
            Shape shape = new Shape(Expression);
            int id = shape.GetId();
            ShapeInfo = "图形ID：" + id + "\n\n" +
                        "多层表达式：\n" + shape.MultilineExpression();
            if (!_analyzer.IsExpressionExists(id))
            {
                CompositionPathColor = "Red";
                CompositionPath = "该图形无法合成！";
                return;
            }
            CompositionPathColor = "Black";
            if (Expression.Length == 8)
            {
                CompositionPath = "该表达式为基类图形\n本软件将其视为无需合成的最小模块";
                return;
            }
            CompositionPath = _analyzer.GetCompositionInfo(id) +
                              "\n合成路径如下（X表示任意形状，x表示任意颜色）：\n" +
                              _analyzer.GetPath(shape);
        }

        private const string LayerReg = @"([CRWS][rgbypcuw]|--){4}";

        /// <summary>
        /// 判断表达式是否合理，并将结果显示至FormatCheckResult
        /// </summary>
        /// <returns>表达式可用返回true，否则返回false</returns>
        private bool IsExpressionFormatReasonable()
        {
            var s = _expression;
            var layers = _expression.Split(':');
            if (layers.Length > 4)
            {
                FormatCheckResultColor = "Red";
                FormatCheckResult = "不能超过4层，当前为" + layers.Length + "层";
                return false;
            }
            for (var i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                if (layer.Length != 8)
                {
                    FormatCheckResultColor = "Red";
                    FormatCheckResult = "第" + (i + 1) + "层“" + layers[i] + "”必须为8字符";
                    return false;
                }
                if (layers[i] == "--------")
                {
                    FormatCheckResultColor = "Red";
                    FormatCheckResult = "第" + (i + 1) + "层“" + layers[i] + "”不能全空";
                    return false;
                }
                if (!Regex.IsMatch(layers[i], LayerReg))
                {
                    FormatCheckResultColor = "Red";
                    FormatCheckResult = "第" + (i + 1) + "层“" + layers[i] + "”内容有误";
                    return false;
                }
            }
            FormatCheckResultColor = "Green";
            FormatCheckResult = "表达式格式正确";
            return true;
        }
    }
}