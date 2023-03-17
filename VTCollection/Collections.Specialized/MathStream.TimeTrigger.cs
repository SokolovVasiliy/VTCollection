// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Вызывает событие Out через задданый промежуток времени
    /// </summary>
    public class TimeTrigger : MathStream<DateTime, bool>
    {
        /// <summary>
        /// Последнее время срабатывания тригера
        /// </summary>
        private DateTime m_last_time_start;
        /// <summary>
        /// Истина, если триггер сработал, ложь в противном случае
        /// </summary>
        private bool m_is_started = false;
        /// <summary>
        /// Необходимо указать интервал срабатывания триггера
        /// </summary>
        /// <param name="time_period"></param>
        public TimeTrigger(TimeSpan time_period)
        {
            TimePeriod = time_period;
        }
        /// <summary>
        /// Устанавливает или возвращает временной тригер
        /// </summary>
        public TimeSpan TimePeriod
        {
            get; set;
        }
        /// <summary>
        /// Время последнего старта тригера
        /// </summary>
        public DateTime LastTimeStart
        {
            get { return m_last_time_start; }
        }
        /// <summary>
        /// Истина, если временной триггер сработал
        /// </summary>
        public override bool Value
        {
            get { return m_is_started; }
        }
        /// <summary>
        /// Добавляем новое значение
        /// </summary>
        /// <param name="item"></param>
        protected override bool OnAdd(DateTime time)
        {
            //-- init
            if (m_last_time_start == DateTime.MinValue)
            {
                m_last_time_start = time;
                return false;
            }
            m_is_started = time - m_last_time_start >= TimePeriod;
            if (m_is_started)
                m_last_time_start = time;
            return m_is_started;
        }
    }
}
