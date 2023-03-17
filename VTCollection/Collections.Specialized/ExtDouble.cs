// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace System
{
    /// <summary>
    /// Добавляет расширяющий метод
    /// </summary>
    public static class ExtDouble
    {
        /// <summary>
        /// Истина, если значение является действительным вещественным числом.
        /// Если число является NaN, +/-Inf или Epsilon возвращается ложь.
        /// </summary>
        /// <param name="value">Вещественное значение</param>
        /// <returns>Истина, если число является действительным и ложь в противном случае</returns>
        public static bool Valid(this double value)
        {
            if (double.IsNaN(value))
                return false;
            if (value == double.NegativeInfinity)
                return false;
            if (value == double.PositiveInfinity)
                return false;
            if (Equal(value, double.Epsilon))
                return false;
            if (value < double.Epsilon)
                return false;
            return true;
        }
        /// <summary>
        /// Истина, если значение является действительным вещественным числом и не равно нулю.
        /// Если число является нулем, NaN, +/-Inf или Epsilon возвращается ложь.
        /// </summary>
        /// <param name="value">Вещественное значение</param>
        /// <returns>Истина, если число является действительным и ложь в противном случае</returns>
        public static bool IsValidAndNotZerro(this double value)
        {
            if (!Valid(value))
                return false;
            
            bool notZerro = Math.Abs(value) > double.Epsilon;
            return notZerro;
        }
        public static bool IsZerro(this double value)
        {
            double d = Math.Round(value, 13);
            if (value == 0.0)
                return true;
            return false;
        }
        /// <summary>
        /// Return true if value <c>v1</c> equal <c>v2</c>. Return false otherwise
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Equal(double v1, double v2) => Math.Abs(v1 - v2) < double.Epsilon;
        public static int Digits(this double v)
        {
            const int maxDigits = 14;
            var vr = Math.Round(v, maxDigits - 1);
            for(int d = 0; d < maxDigits; d++)
            {
                var res =  Math.Round(v, d);
                if (Equal(res, vr))
                    return d;
            }
            return maxDigits - 1;
        }
        public static int Digits(this decimal v)
        {
            const int maxDigits = 26;
            decimal vr = Math.Round(v, maxDigits);
            for (int d = 0; d < maxDigits; d++)
            {
                decimal res = Math.Round(v, d);
                if (res.Equals(vr))
                    return d;
            }
            return maxDigits;
        }

        public static double Normalize(this double v)
        {
            return Math.Round(v, v.Digits());
        }
        public static double Normalize(this double v, double priceStep)
        {
            int steps = (int)Math.Round(v / priceStep);
            double rezult = steps * priceStep;
            return Math.Round(rezult, rezult.Digits());
        }
        public static decimal Normalize(this decimal v)
        {
            return Math.Round(v, v.Digits());
        }
        public static decimal Normalize(this decimal v, decimal priceStep)
        {
            int steps = (int)Math.Round(v / priceStep);
            decimal rezult = steps * priceStep;
            return Math.Round(rezult, rezult.Digits());
        }

        public static double ToDouble(this decimal v)
        {
            return Convert.ToDouble(v);
        }

        public static Nullable<decimal> Normalize(this Nullable<decimal> v)
        {
            if (!v.HasValue)
                return v;
            decimal? vn = v.Value.Normalize();
            return vn;
        }
        public static Nullable<decimal> Normalize(this Nullable<decimal> v, decimal priceStep)
        {
            if (!v.HasValue)
                return v;
            decimal? vn = v.Value.Normalize(priceStep);
            return vn;
        }
        public static Nullable<double> Normalize(this Nullable<double> v)
        {
            if (!v.HasValue)
                return v;
            double? vn = v.Value.Normalize();
            return vn;
        }
        public static Nullable<double> Normalize(this Nullable<double> v, double priceStep)
        {
            if (!v.HasValue)
                return v;
            double? vn = v.Value.Normalize(priceStep);
            return vn;
        }
        //// [2023-01-12] Передвинул этот метод в API.Shared в файл Double.Extensions.cs
        //public static decimal ToDecimal(this double v)
        //{
        //    if (double.IsNaN(v))
        //        return 0.0m;
        //    if (double.IsInfinity(v))
        //        return 0.0m;
        //    return Convert.ToDecimal(v);
        //}

        /// <summary>
        /// If value has digits more 'maxDigits' value will be round to maxDigits, otherwise the number will be normalize and return
        /// </summary>
        /// <param name="v"></param>
        /// <param name="maxDigits"></param>
        /// <returns></returns>
        public static decimal RoundIfMore(this decimal v, int maxDigits)
        {
            if (v.Digits() > maxDigits)
                return Math.Round(v, maxDigits);
            return v.Normalize();
        }
    }
}
