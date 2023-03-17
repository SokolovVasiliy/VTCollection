using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace VT.Collections.Specialized
{
    public static class MathStreamExtension
    {
        /// <summary>
        /// Добавляет значения в индикатор с заданной периодичностью. Например для Sma(30) с периодом 1000 мсек будет
        /// расчитано среднее значение за последние 30 секунд с частотой раз в секунду.
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="stream">Любой математический потоковый преобразователь, например Sma или Ema</param>
        /// <param name="milliseconds">Периодичность в милисекундах, с которой будет обновляться индикатор stram</param>
        /// <returns>Модуль таймера, связанный с базовым индикатором. Добавление значения нужно призводить через него</returns>
        public static MathTimer<TIn, TOut> ByTimer<TIn, TOut>(this MathStream<TIn, TOut> stream, int milliseconds)
        {
            MathTimer<TIn, TOut> t = new MathTimer<TIn, TOut>(stream, milliseconds);
            return t;
        }
    }
    /// <summary>
    /// Добавляет в переданный индикатор последнее добавленное значение с заданной периодичностью, например раз в секунду.
    /// </summary>
    public class MathTimer<TIn, TOut> : MathStream<TIn, TOut>
    {
        /// <summary>
        /// Math stream
        /// </summary>
        private MathStream<TIn, TOut> Stream { get; }
        /// <summary>
        /// Timer
        /// </summary>
        private Timer m_timer;
        /// <summary>
        /// Last added value
        /// </summary>
        private TIn m_last_added_value;
        /// <summary>
        /// True if first element was added
        /// </summary>
        private bool m_was_first_added;
        public MathTimer(MathStream<TIn, TOut> mathStream, int milliseconds)
        {
            Stream = mathStream;
            m_timer = new Timer(OnTimer, null, milliseconds, milliseconds);
        }
        private void OnTimer(object sender)
        {
            if (!m_was_first_added)
                return;
            Stream.Add(m_last_added_value);
        }

        protected override bool OnAdd(TIn value)
        {
            bool res = Stream.CheckInValue(value);
            if (res == false)
                return false;
            m_last_added_value = value;
            m_was_first_added = true;
            return true;
        }
        
        public override TOut Value => Stream.Value;
    }
}
