using System;
using System.Text;
using System.Text.RegularExpressions;
using static shapezAnalyzer.ShapezEnum;

namespace shapezAnalyzer
{
    public class Piece
    {
        public BaseShape BS { get; set; }

        public BaseColor BC { get; set; }

        public Piece()
        {
            BS = BaseShape.NONE;
            BC = BaseColor.NONE;
        }

        public Piece(BaseShape bs, BaseColor bc)
        {
            BS = bs;
            BC = bc;
        }

        public Piece(string piece)
        {
            if (piece.Length != 2)
            {
                throw new Exception();
            }
            BS = StrToBs(piece.Substring(0, 1));
            BC = StrToBc(piece.Substring(1, 1));
        }
    }

    public class Shape
    {
        /// <summary>
        /// 4*4的原料。
        /// </summary>
        private readonly Piece[][] pieces;

        private readonly int layerNum;

        private Shape()
        {
            pieces = new Piece[4][];
            for (var i = 0; i < 4; i++)
            {
                pieces[i] = new Piece[4];
                for (var j = 0; j < 4; j++)
                {
                    pieces[i][j] = new Piece();
                }
            }
            layerNum = 0;
        }

        private const string ShapeReg =
            @"((?!--------)([CRWS][rgbypcuw]|--){4}:){0,3}(?!--------)([CRWS][rgbypcuw]|--){4}";

        public static bool IsReasonable(string expression)
        {
            return Regex.IsMatch(expression, ShapeReg);
        }

        public Shape(string expression) : this()
        {
            if (!IsReasonable(expression))
            {
                throw new Exception();
            }
            var layers = expression.Split(':');
            for (var i = 0; i < layers.Length; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    pieces[i][j] = new Piece(layers[i].Substring(j * 2, 2));
                }
            }
            layerNum = layers.Length;
        }

        public Shape(int id) : this()
        {
            if (id <= 0 || id >= 65536)
            {
                throw new Exception();
            }
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    if ((id & (1 << (i * 4 + j))) != 0)
                    {
                        pieces[i][j] = new Piece(BaseShape.NOT_NONE, BaseColor.NOT_NONE);
                        layerNum = i + 1;
                    }
                }
            }
        }

        public int GetId()
        {
            var id = 0;
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    if (pieces[i][j].BS != BaseShape.NONE)
                    {
                        id |= 1 << (i * 4 + j);
                    }
                }
            }
            return id;
        }

        public string SingleLineExpression()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < layerNum; i++)
            {
                if (i != 0)
                {
                    sb.Append(":");
                }
                for (int j = 0; j < 4; j++)
                {
                    sb.Append(BsToStr(pieces[i][j].BS)).Append(BcToStr(pieces[i][j].BC));
                }
            }
            return sb.ToString();
        }

        public string MultilineExpression()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = layerNum - 1; i >= 0; i--)
            {
                if (i != layerNum - 1)
                {
                    sb.Append("\n");
                }
                for (int j = 0; j < 4; j++)
                {
                    sb.Append(BsToStr(pieces[i][j].BS)).Append(BcToStr(pieces[i][j].BC));
                }
            }
            return sb.ToString();
        }

        public Shape R270()
        {
            Shape ret = new Shape();
            for (var i = 0; i < layerNum; i++)
            {
                Piece piece = pieces[i][0];
                ret.pieces[i][0] = pieces[i][1];
                ret.pieces[i][1] = pieces[i][2];
                ret.pieces[i][2] = pieces[i][3];
                ret.pieces[i][3] = piece;
            }
            return ret;
        }

        /// <summary>
        /// 通过具体的目标图形以及合成方式，得到被操作图形的具体情况并返回。
        /// </summary>
        /// <param name="operateId">被操作图形的id</param>
        /// <param name="operate">操作类型，不是add</param>
        /// <param name="shape">目标图形</param>
        /// <returns>被操作图形</returns>
        public static Shape GetShape(int operateId, BaseOperate operate, Shape shape)
        {
            Shape ret = new Shape(operateId);
            switch (operate)
            {
                case BaseOperate.LEFT:
                case BaseOperate.RIGHT:
                    // 这里默认切割后的产物无全空层
                    for (var i = 0; i < 4; i++)
                    {
                        for (var j = 0; j < 4; j++)
                        {
                            if (shape.pieces[i][j].BS != BaseShape.NONE)
                            {
                                ret.pieces[i][j] = shape.pieces[i][j];
                            }
                        }
                    }
                    break;
                case BaseOperate.R90:
                    ret = shape.R270();
                    break;
                default:
                    throw new Exception();
            }
            return ret;
        }

        /// <summary>
        /// 通过具体的目标图形以及合成方式，得到被操作图形的具体情况并返回。
        /// </summary>
        /// <param name="operateId1">堆叠的上方图形的id</param>
        /// <param name="operateId2">堆叠的下方图形的id</param>
        /// <param name="shape">目标图形</param>
        /// <returns>两个被操作图形</returns>
        public static Shape[] GetShape(int operateId1, int operateId2, Shape shape)
        {
            // ret1堆叠到ret2上面，则得到shape
            Shape ret1 = new Shape(operateId1);
            Shape ret2 = new Shape(operateId2);
            // 计算ret1下落距离
            int distance = int.MaxValue;
            for (var col = 0; col < 4; col++)
            {
                int aboveDistance = 4;
                for (var i = 0; i < 4; i++)
                {
                    if (ret1.pieces[i][col].BS != BaseShape.NONE)
                    {
                        aboveDistance = i;
                        break;
                    }
                }
                int belowDistance = 4;
                for (var i = 3; i >= 0; i--)
                {
                    if (ret2.pieces[i][col].BS != BaseShape.NONE)
                    {
                        belowDistance = 3 - i;
                        break;
                    }
                }
                distance = Math.Min(distance, aboveDistance + belowDistance);
            }
            // 构造ret1
            for (var i = 4 - distance; i < 4; i++)
            {
                int kk = i - (4 - distance);
                for (var j = 0; j < 4; j++)
                {
                    if (shape.pieces[i][j].BS != BaseShape.NONE && ret1.pieces[kk][j].BS != BaseShape.NONE)
                    {
                        ret1.pieces[kk][j] = shape.pieces[i][j];
                    }
                }
            }
            // 构造ret2
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    if (shape.pieces[i][j].BS != BaseShape.NONE && ret2.pieces[i][j].BS != BaseShape.NONE)
                    {
                        ret2.pieces[i][j] = shape.pieces[i][j];
                    }
                }
            }
            return new[] { ret1, ret2 };
        }
    }
}