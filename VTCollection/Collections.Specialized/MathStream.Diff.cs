// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace VT.Collections.Specialized
{

    /// <summary>
    /// Возвращает разницу между двумя переданными значениями
    /// </summary>
    public class Diff : MathStream<Tuple<double, double>, double>
    {
        /// <summary>
        /// Последняя рассчитанная разница
        /// </summary>
        private double m_diff = 0.0;
        /// <summary>
        /// Тип разницы
        /// </summary>
        public enum DiffType
        {
            /// <summary>
            /// Простая разница вида Value1 - Value2
            /// </summary>
            Simple,
            /// <summary>
            /// Процент: (Value1-Value2)/Value1*100.0
            /// </summary>
            Percent,
            /// <summary>
            /// Отношение логарифмов Ln(Value1)/Ln(Value2)
            /// </summary>
            Ln
        }
        /// <summary>
        /// Возвращает истину, если два переданных числа являются корректными для вычисления разницы,
        /// возвращает ложь в противном случае
        /// </summary>
        /// <param name="values">Кортеж двух вещественных чисел</param>
        /// <returns></returns>
        public override bool CheckInValue(Tuple<double, double> values)
        {
            if (values.Item1 <= 0.0 || values.Item2 <= 0.0)
                return false;
            if (!values.Item1.Valid() || !values.Item2.Valid())
                return false;
            return true;
        }
        /// <summary>
        /// Тип разницы
        /// </summary>
        public DiffType Type = DiffType.Simple;
        /// <summary>
        /// Содержит последнее известное значение
        /// </summary>
        public override double Value
        {
            get { return m_diff; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values">Два значения, между которыми следует рассчитать разницу</param>
        protected override bool OnAdd(Tuple<double, double> values)
        {
            switch (Type)
            {
                case DiffType.Simple:
                    m_diff = values.Item2 - values.Item1;
                    break;
                case DiffType.Percent:
                    m_diff = (values.Item2 - values.Item1) / values.Item1 * 100.0;
                    break;
                case DiffType.Ln:
                    m_diff = Math.Log(values.Item2) / Math.Log(values.Item1);
                    break;
                default:
                    throw new ArgumentException("Caption 'Type' of Diff is undefined", "Diff.Type");
            }
            return true;
        }

        public static Diff operator <<(Diff item1, int n)
        {
            throw new NotImplementedException();
        }

        public static Diff operator +(Diff item1, int n)
        {
            throw new NotImplementedException();
        }
    }
}
