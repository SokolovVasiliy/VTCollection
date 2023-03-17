// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Рассчет дисперсии за линейной время
    /// </summary>
    public class Dispersion : MathStream<double, double>
    {
        /// <summary>
        /// Сумма квадратов посупивших значений
        /// </summary>
        private Sum m_sum2;
        /// <summary>
        /// Среднее поступивших значений
        /// </summary>
        private Sma m_sma;
        /// <summary>
        /// Инициализирует безразмерную кольцевую дисперсию (Новые данные добавляются, но не удаляются)
        /// </summary>
        public Dispersion()
        {
            m_sma = new Sma();
            m_sum2 = new Sum();
        }
        /// <summary>
        /// Инициализирует кольцевую дисперсию с периодом period
        /// </summary>
        /// <param name="period"></param>
        public Dispersion(int period)
        {
            m_sma = new Sma(period);
            m_sum2 = new Sum(period);
        }

        /// <summary>
        /// Добавляет новое значение
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool OnAdd(double item)
        {
            m_sma.Add(item);
            m_sum2.Add(item * item);
            return true;
        }
        /// <summary>
        /// Дисперсия ряда
        /// </summary>
        public override double Value
        {
            get
            {
                double avrg = m_sum2.Value / m_sma.Count;
                double disp = avrg - m_sma.Value * m_sma.Value;
                return disp;
            }
        }
        /// <summary>
        /// Среднее квадратичное отклонение
        /// </summary>
        public double StdDev
        {
            get
            {
                return Math.Sqrt(Value);
            }
        }
        /// <summary>
        /// Фактическое количество размещенных элементов
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return m_sum2.Count; }
        }
    }
}
