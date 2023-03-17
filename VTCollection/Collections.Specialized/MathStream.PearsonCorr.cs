// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Корреляция пирсона для кольцевых вычислений за константное время
    /// </summary>
    public class PearsonCorr : MathStream<Tuple<double, double>, double>
    {
        /// <summary>
        /// Сумма произведений x*y
        /// </summary>
        private Sum m_sum_xy;
        /// <summary>
        /// Сумма x
        /// </summary>
        private Sum m_sum_x;
        /// <summary>
        /// Сумма y
        /// </summary>
        private Sum m_sum_y;
        /// <summary>
        /// Сумма x^2
        /// </summary>
        private Sum m_sum_x2;
        /// <summary>
        /// Сумма y^2
        /// </summary>
        private Sum m_sum_y2;
        /// <summary>
        /// Инициализация безразмерной величины
        /// </summary>
        public PearsonCorr()
        {
            m_sum_x = new Sum();
            m_sum_x2 = new Sum();
            m_sum_xy = new Sum();
            m_sum_y = new Sum();
            m_sum_y2 = new Sum();
        }
        /// <summary>
        /// Инициализация корреляции с периодом
        /// </summary>
        /// <param name="period"></param>
        public PearsonCorr(int period)
        {
            m_sum_x = new Sum(period);
            m_sum_x2 = new Sum(period);
            m_sum_xy = new Sum(period);
            m_sum_y = new Sum(period);
            m_sum_y2 = new Sum(period);
        }
        /// <summary>
        /// Количество фактических элементов
        /// </summary>
        public int Count
        {
            get
            {
                return m_sum_x.Count;
            }
        }
        /// <summary>
        /// Добавляет новые два значения
        /// </summary>
        /// <param name="item">Кортеж, item.Item1 = x, item.Item2 = y</param>
        /// <returns></returns>
        protected override bool OnAdd(Tuple<double, double> item)
        {
            m_sum_xy.Add(item.Item1 * item.Item2);
            m_sum_x.Add(item.Item1);
            m_sum_y.Add(item.Item2);
            m_sum_x2.Add(item.Item1 * item.Item1);
            m_sum_y2.Add(item.Item2 * item.Item2);
            return true;
        }
        /// <summary>
        /// Возвращает коэффициент корреляции
        /// </summary>
        public override double Value
        {
            get
            {
                //-- https://studfiles.net/preview/2966946/page:34/
                double cov = m_sum_xy.Value - ((m_sum_x.Value * m_sum_y.Value) / Count);
                double s1 = m_sum_x2.Value - Math.Pow(m_sum_x.Value, 2.0) / Count;
                double s2 = m_sum_y2.Value - Math.Pow(m_sum_y.Value, 2.0) / Count;
                double corr = cov / Math.Sqrt(s1 * s2);
                return corr;
            }
        }
    }
}
