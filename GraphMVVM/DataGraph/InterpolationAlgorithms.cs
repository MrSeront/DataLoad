using GraphMVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMVVM.DataGraph
{
    class InterpolationAlgorithms // Алгоритмы интерполяции
    {
        public static int Linear(int[] xarray, int[] yarray, int x)
        {
            int y = 0;
            for (int i = 0; i<xarray.Length - 1; i++)
            {
                if ((x >= xarray[i]) && (x<xarray[i + 1]))
                {
                    y = yarray[i] + (x - xarray[i]) * (yarray[i + 1] - yarray[i]) /
                    (xarray[i + 1] - xarray[i]);
                }
}
            return y;
        }
        public static int[] Linear(int[] xarray, int[] yarray, int[] x)
{
    int[] y = new int[x.Length];
    for (int i = 0; i < x.Length; i++)
        y[i] = Linear(xarray, yarray, x[i]);
    return y;
}

        public static int Sinus(int[] xarray, int[] yarray, int x)
        {
            int y = 0;
            for (int i = 0; i < xarray.Length - 1; i++)
            {
                if ( yarray[i]>yarray[i + 1])
                {
                    //y = Convert.ToInt32((float)(yarray[i + 1] - yarray[i]) * Math.Sin(-x * 3.14 / (float)(2 * (xarray[i + 1] - (xarray[i]))) + 3.14 / 2) + yarray[i + 1]);
                    y = Convert.ToInt32((float)(yarray[i]- yarray[i+1]) * Math.Sin(x*3.14/(float)(2*(xarray[i + 1]-(xarray[i])))+3.14/2)+ yarray[i+1]);
                }
                else
                {
                    y = Convert.ToInt32((float)(yarray[i + 1] - yarray[i]) * Math.Sin(x * 3.14 / (float)(2 * (xarray[i + 1] - (xarray[i])))) + yarray[i]);
                    
                }
            }
            return y;
        }
        public static int[] Sinus(int[] xarray, int[] yarray, int[] x)
        {
            int[] y = new int[x.Length];
            for (int i = 0; i < x.Length; i++)
                y[i] = Sinus(xarray, yarray, x[i]);
            return y;
        }

        public static int Sinus1(int[] xarray, int[] yarray, int x)
        {
            int y = 0;
            for (int i = 0; i < xarray.Length - 1; i++)
            {
                if (yarray[i] > yarray[i + 1])
                {
                    y = Convert.ToInt32((float)(yarray[i+1] - yarray[i]) * Math.Sin(x * 3.14 / (float)(2 * (xarray[i + 1] - (xarray[i])))) + yarray[i]);
                }
                else
                {
                    y = Convert.ToInt32((float)(yarray[i] - yarray[i + 1]) * Math.Sin(x * 3.14 / (float)(2 * (xarray[i + 1] - (xarray[i]))) + 3.14 / 2) + yarray[i + 1]);
                }
            }
            return y;
        }
        public static int[] Sinus1(int[] xarray, int[] yarray, int[] x)
        {
            int[] y = new int[x.Length];
            for (int i = 0; i < x.Length; i++)
                y[i] = Sinus1(xarray, yarray, x[i]);
            return y;
        }



        //public static int Linear(PointModel firstPoint, PointModel secondPoint, int x)
        //{
        //    int y = 0;

        //    if ((x >= firstPoint.X) && (x < secondPoint.X))
        //    {
        //        y = firstPoint.Y + (secondPoint.Y - firstPoint.Y) * (x - firstPoint.X) /
        //         (secondPoint.X - firstPoint.X);
        //    }

        //    return y;
        //}
        //public static int[] Linear(PointModel firstPoint, PointModel secondPoint, int[] x)
        //{
        //    int[] y = new int[x.Length];
        //    for (int i = 0; i < x.Length; i++)
        //        y[i] = Linear(firstPoint, secondPoint, x[i]);
        //    return y;
        //}

        //public static int Sinus(PointModel firstPoint, PointModel secondPoint, int x)
        //{
        //    int y = 0;
            
        //        if (firstPoint.Y > secondPoint.Y)
        //        {
        //            //y = Convert.ToInt32((float)(yarray[i + 1] - yarray[i]) * Math.Sin(-x * 3.14 / (float)(2 * (xarray[i + 1] - (xarray[i]))) + 3.14 / 2) + yarray[i + 1]);
        //            y = Convert.ToInt32((float)(firstPoint.Y - secondPoint.Y) * Math.Sin(x * 3.14 / (float)(2 * (secondPoint.X - (firstPoint.X))) + 3.14 / 2) + secondPoint.Y);
        //        }
        //        else
        //        {
        //            y = Convert.ToInt32((float)(secondPoint.Y - firstPoint.Y) * Math.Sin(x * 3.14 / (float)(2 * (secondPoint.X - (firstPoint.X)))) + firstPoint.Y);

        //        }
           
        //    return y;
        //}
        //public static int[] Sinus(PointModel firstPoint, PointModel secondPoint, int[] x)
        //{
        //    int[] y = new int[x.Length];
        //    for (int i = 0; i < x.Length; i++)
        //        y[i] = Sinus(firstPoint, secondPoint, x[i]);
        //    return y;
        //}

        //public static int Sinus1(PointModel firstPoint, PointModel secondPoint, int x)
        //{
        //    int y = 0;
            
        //        if (firstPoint.Y > secondPoint.Y)
        //    {
        //            y = Convert.ToInt32((float)(secondPoint.Y - firstPoint.Y) * Math.Sin(x * 3.14 / (float)(2 * (secondPoint.X - (firstPoint.X)))) + firstPoint.Y);
        //        }
        //        else
        //        {
        //            y = Convert.ToInt32((float)(firstPoint.Y - secondPoint.Y) * Math.Sin(x * 3.14 / (float)(2 * (secondPoint.X - (firstPoint.X))) + 3.14 / 2) + secondPoint.Y);
        //        }
         
        //    return y;
        //}
        //public static int[] Sinus1(PointModel firstPoint, PointModel secondPoint, int[] x)
        //{
        //    int[] y = new int[x.Length];
        //    for (int i = 0; i < x.Length; i++)
        //        y[i] = Sinus1(firstPoint, secondPoint, x[i]);
        //    return y;
        //}

        public static int Linear(int x1, int y1, int x2, int y2, int x)
        {
            int y = 0;

            if ((x >= x1) && (x < x2))
            {
                y = y1 + (y2 - y1) * (x - x1) / (x2 - x1);
            }

            return y;
        }
        public static int[] Linear(int x1, int y1, int x2, int y2, int[] x)
        {
            int[] y = new int[x.Length];
            for (int i = 0; i < x.Length; i++)
                y[i] = Linear(x1,y1,x2,y2, x[i]);
            return y;
        }

        public static int Sinus(int x1, int y1, int x2, int y2, int x)
        {
            int y = 0;

            if (y1 > y2)
            {
                y = Convert.ToInt32((float)(y1 - y2) * Math.Sin(x * 3.14 / (float)(2 * (x2 - (x1))) + 3.14 / 2) + y2);
            }
            else
            {
                y = Convert.ToInt32((float)(y2 - y1) * Math.Sin(x * 3.14 / (float)(2 * (x2 - (x1)))) + y1);
            }

            return y;
        }
        public static int[] Sinus(int x1, int y1, int x2, int y2, int[] x)
        {
            int[] y = new int[x.Length];
            for (int i = 0; i < x.Length; i++)
                y[i] = Sinus(x1,y1,x2,y2, x[i]);
            return y;
        }

        public static int Sinus1(int x1, int y1, int x2, int y2, int x)
        {
            int y = 0;

            if (y1 > y2)
            {
                y = Convert.ToInt32((float)(y2 - y1) * Math.Sin(x * 3.14 / (float)(2 * (x2 - (x1)))) + y1);
            }
            else
            {
                y = Convert.ToInt32((float)(y1 - y2) * Math.Sin(x * 3.14 / (float)(2 * (x2 - (x1))) + 3.14 / 2) + y2);
            }

            return y;
        }
        public static int[] Sinus1(int x1, int y1, int x2, int y2, int[] x)
        {
            int[] y = new int[x.Length];
            for (int i = 0; i < x.Length; i++)
                y[i] = Sinus1(x1, y1, x2, y2, x[i]);
            return y;
        }

    }
}
