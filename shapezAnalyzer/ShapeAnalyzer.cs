using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using static shapezAnalyzer.Shape;
using static shapezAnalyzer.ShapezEnum;

namespace shapezAnalyzer
{
    public class ShapeAnalyzer
    {
        private bool[] _ids = new bool[65536];
        private int[] _materialNum = new int[65536];
        private int[] _operateNum = new int[65536];
        private BaseOperate[] _operates = new BaseOperate[65536];
        private int[] _id1s = new int[65536];
        private int[] _id2s = new int[65536];
        private const string IdReg = @"[0-9]+";

        public bool ChangeDb(string dbPath)
        {
            bool[] ids = new bool[65536];
            int[] materialNum = new int[65536];
            int[] operateNum = new int[65536];
            BaseOperate[] operates = new BaseOperate[65536];
            int[] id1s = new int[65536];
            int[] id2s = new int[65536];
            int num = 0;
            using (StreamReader sr = new StreamReader(dbPath))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    try
                    {
                        string[] data = s.Split(',');
                        if (data.Length != 6 || !Regex.IsMatch(data[0], IdReg))
                        {
                            continue;
                        }
                        int id = int.Parse(data[0]);
                        if (id <= 0 || id >= 65536)
                        {
                            continue;
                        }
                        if (ids[id])
                        {
                            // 同一id出现两次，直接返回false
                            return false;
                        }
                        ids[id] = true;
                        materialNum[id] = int.Parse(data[1]);
                        operateNum[id] = int.Parse(data[2]);
                        operates[id] = (BaseOperate)System.Enum.Parse(typeof(BaseOperate), data[3]);
                        id1s[id] = int.Parse(data[4]);
                        id2s[id] = int.Parse(data[5]);
                        num++;
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }
            }
            if (num != 48432)
            {
                // 48432是所有可合成的图形数目，不等于该数目说明数据库有误
                return false;
            }
            this._ids = ids;
            this._materialNum = materialNum;
            this._operateNum = operateNum;
            this._operates = operates;
            this._id1s = id1s;
            this._id2s = id2s;
            return true;
        }

        public bool IsExpressionExists(int id)
        {
            return _ids[id];
        }

        public string GetCompositionInfo(int id)
        {
            return "需要基类图形" + _materialNum[id] + "个，操作" + _operateNum[id] + "步";
        }

        public string GetPath(Shape shape)
        {
            List<string> list = new List<string>();
            GetPath(shape, 0, list);
            StringBuilder sb = new StringBuilder();
            foreach (var s in list)
            {
                sb.Append(s).Append("\n");
            }
            return sb.ToString().Substring(0, sb.Length - 1);
        }

        public void GetPath(Shape shape, int placeholderNum, List<string> list)
        {
            int id = shape.GetId();
            StringBuilder sb = new StringBuilder();
            if (id < 16)
            {
                //基类显示shape的表达式
                for (int i = 0; i < placeholderNum; i++)
                {
                    sb.Append("| ");
                }
                sb.Append(shape.SingleLineExpression());
                list.Add(sb.ToString());
            }
            else
            {
                //非基类显示操作，然后调用自身，显示被操作shape合成路线
                switch (_operates[id])
                {
                    case BaseOperate.LEFT:
                    case BaseOperate.RIGHT:
                    case BaseOperate.R90:
                        Shape shape0 = GetShape(_id1s[id], _operates[id], shape);
                        GetPath(shape0, placeholderNum + 1, list);
                        break;
                    case BaseOperate.ADD:
                        Shape[] shapes = GetShape(_id1s[id], _id2s[id], shape);
                        GetPath(shapes[0], placeholderNum + 1, list);
                        GetPath(shapes[1], placeholderNum + 1, list);
                        break;
                    default:
                        throw new Exception();
                }
                for (int i = 0; i < placeholderNum; i++)
                {
                    sb.Append("| ");
                }
                sb.Append(_operates[id]);
                list.Add(sb.ToString());
            }
        }
    }
}
