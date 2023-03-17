// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Calculate sum of values for period 'Period'
    /// </summary>
    public class Sum : MathStream<double, double>
    {
        /// <summary>
        /// Inner ring buffer of double values
        /// </summary>
        private RingBuffer<double> m_values = null;
        /// <summary>
        /// Sum of elements
        /// </summary>
        private double m_sum = 0.0;
        /// <summary>
        /// Set unlimeted period
        /// </summary>
        public Sum()
        {
            m_values = new RingBuffer<double>();
            m_values.EventRemove += MinusValue;
        }
        /// <summary>
        /// Set limited period
        /// </summary>
        /// <param name="capacity"></param>
        public Sum(int capacity)
        {
            m_values = new RingBuffer<double>(capacity);
            m_values.EventRemove += MinusValue;
        }
        /// <summary>
        /// Get/Set period of sum
        /// </summary>
        public int Period
        {
            get
            {
                return m_values.Capacity;
            }
            set
            {
                m_values.Capacity = value;
            }
        }
        /// <summary>
        /// Фактическое количество размещенных элементов
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return m_values.Count; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override double Value
        {
            get { return m_sum; }
        }
        /// <summary>
        /// sum -= out value 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void MinusValue(int index, double value)
        {
            m_sum -= value;
        }
        /// <summary>
        /// Add new value
        /// </summary>
        /// <param name="value">New value</param>
        protected override bool OnAdd(double value)
        {
            m_sum += value;
            m_values.Add(value);
            return true;
        }
    }
}
