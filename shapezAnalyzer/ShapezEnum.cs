using System;

namespace shapezAnalyzer
{
    public class ShapezEnum
    {
        public enum BaseShape
        {
            //圆
            CIRCLE,
            //方
            RECTANGLE,
            //风车
            WINDMILL,
            //星
            STAR,
            //空
            NONE,
            //任意非空
            NOT_NONE
        }

        public static string BsToStr(BaseShape bs)
        {
            switch (bs)
            {
                case BaseShape.CIRCLE:
                    return "C";
                case BaseShape.RECTANGLE:
                    return "R";
                case BaseShape.WINDMILL:
                    return "W";
                case BaseShape.STAR:
                    return "S";
                case BaseShape.NONE:
                    return "-";
                case BaseShape.NOT_NONE:
                    return "X";
                default:
                    throw new Exception("错误的形状：" + bs);
            }
        }

        public static BaseShape StrToBs(string s)
        {
            switch (s)
            {
                case "C":
                    return BaseShape.CIRCLE;
                case "R":
                    return BaseShape.RECTANGLE;
                case "W":
                    return BaseShape.WINDMILL;
                case "S":
                    return BaseShape.STAR;
                case "-":
                    return BaseShape.NONE;
                case "X":
                    return BaseShape.NOT_NONE;
                default:
                    throw new Exception("错误的形状：" + s);
            }
        }

        public enum BaseColor
        {
            //颜色
            RED,
            GREEN,
            BLUE,
            YELLOW,
            PURPLE,
            CYAN,
            UNCOLORED,
            WHITE,
            //不存在角，所以不考虑颜色
            NONE,
            //任意颜色
            NOT_NONE
        }

        public static string BcToStr(BaseColor bc)
        {
            switch (bc)
            {
                case BaseColor.RED:
                    return "r";
                case BaseColor.GREEN:
                    return "g";
                case BaseColor.BLUE:
                    return "b";
                case BaseColor.YELLOW:
                    return "y";
                case BaseColor.PURPLE:
                    return "p";
                case BaseColor.CYAN:
                    return "c";
                case BaseColor.UNCOLORED:
                    return "u";
                case BaseColor.WHITE:
                    return "w";
                case BaseColor.NONE:
                    return "-";
                case BaseColor.NOT_NONE:
                    return "x";
                default:
                    throw new Exception("错误的颜色：" + bc);
            }
        }

        public static BaseColor StrToBc(string s)
        {
            switch (s)
            {
                case "r":
                    return BaseColor.RED;
                case "g":
                    return BaseColor.GREEN;
                case "b":
                    return BaseColor.BLUE;
                case "y":
                    return BaseColor.YELLOW;
                case "p":
                    return BaseColor.PURPLE;
                case "c":
                    return BaseColor.CYAN;
                case "u":
                    return BaseColor.UNCOLORED;
                case "w":
                    return BaseColor.WHITE;
                case "-":
                    return BaseColor.NONE;
                case "x":
                    return BaseColor.NOT_NONE;
                default:
                    throw new Exception("错误的颜色：" + s);
            }
        }

        public enum BaseOperate
        {
            // 基类
            BASE,
            // 切割后的左边
            LEFT,
            // 切割后的右边
            RIGHT,
            // 顺时针90度
            R90,
            // 不需要的操作
            //R180,
            // 不需要的操作
            //R270,
            // 堆叠
            ADD
        }
    }
}
