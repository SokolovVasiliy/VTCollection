// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Возвращает разницу между переданным значением и значением N лагов назад
    /// </summary>
    public class Lag : MathStream<double, double>
    {
        /// <summary>
        /// Кольцевой буфер значений
        /// </summary>
        private RingBuffer<double> m_values = new RingBuffer<double>();
        /// <summary>
        /// Расчет разницы
        /// </summary>
        private Diff m_diff = new Diff();
        /// <summary>
        /// Type diff
        /// </summary>
        public Diff.DiffType Type
        {
            get { return m_diff.Type; }
            set { m_diff.Type = value; }
        }
        /// <summary>
        /// Последнее значение разницы
        /// </summary>
        public override double Value
        {
            get
            {
                return m_diff.Value;
            }
        }
        /// <summary>
        /// По-умолчанию, лаг равен 1
        /// </summary>
        public Lag() : this(1)
        {

        }
        /// <summary>
        /// Устанавливает количество лагов
        /// </summary>
        /// <param name="n">Количество лагов</param>
        public Lag(int n)
        {
            m_values = new RingBuffer<double>(n + 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CheckInValue(double value)
        {
            return value.Valid();
        }
        /// <summary>
        /// Вычисляет разницу между добавленным значением и значением N лагов назад
        /// </summary>
        /// <param name="item">добавленное значение</param>
        /// <returns></returns>
        protected override bool OnAdd(double item)
        {
            m_values.Add(item);
            if (m_values.Count != m_values.Capacity)
                return false;
            Tuple<double, double> tuple = new Tuple<double, double>(m_values.First, m_values.Last);
            m_diff.Add(tuple);
            return m_diff.IsValidAdd;
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
