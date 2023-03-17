// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Расчет экспоненциальной средней в скользящем окне
    /// </summary>
    public class Ema : MathStream<double, double>
    {
        /// <summary>
        /// Предыдущее и текущее значения
        /// </summary>
        private RingBuffer<double> m_values = new RingBuffer<double>(2);
        /// <summary>
        /// Период EMA, участвует в расчете зглаживающего фактора
        /// </summary>
        private int m_period;
        /// <summary>
        /// Текущее значение EMA
        /// </summary>
        private double m_ema = 0.0;
        /// <summary>
        /// Сглаживающий фактор
        /// </summary>
        private double SmoothFactor
        {
            get { return 2.0 / (1.0 + m_period); }
        }
        /// <summary>
        /// Период необходимо задать явно
        /// </summary>
        /// <param name="period"></param>
        public Ema(int period)
        {
            Period = period;
            m_values.EventInsert += CalculateEma;
        }
        /// <summary>
        /// Период скользящей средней
        /// </summary>
        public int Period
        {
            get { return m_period; }
            set { m_period = value; }
        }
        /// <summary>
        /// Возвращает текущее значение EMA
        /// </summary>
        public override double Value
        {
            get { return m_ema; }
        }
        /// <summary>
        /// Автоматический рассчет EMA при добавлении нового значения
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void CalculateEma(int index, double value)
        {
            /*  double value = m_last_value * m_smooth_factor + m_prev_ema * (1.0 - m_smooth_factor);  */
            //-- Вызывается только один раз
            if (m_values.Count != m_values.Capacity)
                m_ema = value;
            else
                m_ema = m_values[1] * SmoothFactor + m_ema * (1.0 - SmoothFactor);
        }
        /// <summary>
        /// Добавляет новое значение 
        /// </summary>
        /// <param name="item"></param>
        protected override bool OnAdd(double item)
        {
            m_values.Add(item);
            return true;
        }
        /// <summary>
        /// Фактическое количество размещенных элементов
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return m_values.Count; }
        }
    }
}
