// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace VT.Collections.Specialized
{

    /// <summary>
    /// Содержит две новые переменные для рассчета одномерной линейной регрессии
    /// </summary>
    public struct lrv
    {
        /// <summary>
        /// Зависимое значение 'y' в линейной регрессии вида y = ax + b 
        /// </summary>
        public double DependentY { get; }
        /// <summary>
        /// Предсказывающее значение 'x' в линейной регрессии вида y = ax + b 
        /// </summary>
        public double PredictorX { get; }
        /// <summary>
        /// Создает два новых значения
        /// </summary>
        /// <param name="x">Предсказывающее значение 'x' в линейной регрессии вида y = ax + b </param>
        /// <param name="y">Зависимое значение 'y' в линейной регрессии вида y = ax + b </param>
        public lrv(double x, double y)
        {
            DependentY = y;
            PredictorX = x;
        }
    }
    /// <summary>
    /// Модуль расчета линейной одномерной регрессии вида y = ax + b
    /// Расчет основных коэффициентов осуществляется за линейное время O(1).
    /// </summary>
    public class LineReg : MathStream<lrv, LineReg>
    {
        #region
        /// <summary>
        /// Добавляемые значения предиктора
        /// </summary>
        private RingBuffer<double> m_x;
        /// <summary>
        /// Добавляемые значения зависимой переменной
        /// </summary>
        private RingBuffer<double> m_y;
        /// <summary>
        /// Добавляемые квадраты значения предиктора
        /// </summary>
        private RingBuffer<double> m_x2;
        /// <summary>
        /// Добавляемые квадраты значения зависимой переменной
        /// </summary>
        private RingBuffer<double> m_y2;
        /// <summary>
        /// Сумма предиктора
        /// </summary>
        private Sum m_sum_x;
        /// <summary>
        /// Сумма зависимой переменной 
        /// </summary>
        private Sum m_sum_y;
        /// <summary>
        /// Сумма предиктора
        /// </summary>
        private Sum m_sum_x2;
        /// <summary>
        /// Сумма зависимой переменной 
        /// </summary>
        private Sum m_sum_y2;
        /// <summary>
        /// Сумма перемноженных значений xy
        /// </summary>
        private Sum m_sum_xy;
        /// <summary>
        /// Модуль корреляции для рассчета корреляции между двумя рядами и R^2
        /// </summary>
        private PearsonCorr m_corr;
        #endregion
        #region public interface
        /// <summary>
        /// Создание скользящей линейной модели с предустановленным периодом
        /// </summary>
        public LineReg()
        {
            m_x = new RingBuffer<double>();
            m_y = new RingBuffer<double>();
            m_x2 = new RingBuffer<double>();
            m_y2 = new RingBuffer<double>();
            m_sum_x = new Sum();
            m_sum_y = new Sum();
            m_sum_x2 = new Sum();
            m_sum_y2 = new Sum();
            m_sum_xy = new Sum();
            m_corr = new PearsonCorr();
        }
        /// <summary>
        /// Создание скользящей линейной модели с предустановленным периодом
        /// </summary>
        /// <param name="period">Предельный период рассчета модели</param>
        public LineReg(int period)
        {
            m_x = new RingBuffer<double>(period);
            m_y = new RingBuffer<double>(period);
            m_x2 = new RingBuffer<double>(period);
            m_y2 = new RingBuffer<double>(period);
            m_sum_x = new Sum(period);
            m_sum_y = new Sum(period);
            m_sum_x2 = new Sum(period);
            m_sum_y2 = new Sum(period);
            m_sum_xy = new Sum(period);
            m_corr = new PearsonCorr(period);
        }
        /// <summary>
        /// Период скользящей линейной модели
        /// </summary>
        public int Period { get { return m_x.Count; } }
        /// <summary>
        /// Добавляет новые значения в рассчетную модель
        /// </summary>
        /// <param name="last_value">Кортеж состоящий из двух последних значений.
        /// last_value.Item1 содержит значение предиктора. last_value.Item2 - 
        /// содержит значение объясняющий переменной
        /// </param>
        /// <returns></returns>
        protected override bool OnAdd(lrv last_value)
        {
            m_x.Add(last_value.PredictorX);
            m_x2.Add(Math.Pow(last_value.PredictorX, 2.0));
            m_y.Add(last_value.DependentY);
            m_y2.Add(Math.Pow(last_value.DependentY, 2.0));
            //--
            m_sum_x.Add(last_value.PredictorX);
            m_sum_x2.Add(Math.Pow(last_value.PredictorX, 2.0));
            //--
            m_sum_y.Add(last_value.DependentY);
            m_sum_y2.Add(Math.Pow(last_value.DependentY, 2.0));
            //--
            m_sum_xy.Add(last_value.PredictorX * last_value.DependentY);

            m_corr.Add(Tuple.Create(last_value.PredictorX, last_value.DependentY));
            return true;
        }
        /// <summary>
        /// Коэффициент 'a'
        /// </summary>
        public double KoeffA
        {
            get
            {
                double a1 = (Period * m_sum_xy.Value - m_sum_x.Value * m_sum_y.Value);
                double a2 = (Period * m_sum_x2.Value - m_sum_x.Value * m_sum_x.Value);
                double a = !ExtDouble.Equal(a2, 0.0) ? a1 / a2 : 0.0;
                return a;
            }
        }
        /// <summary>
        /// Коэффициент b
        /// </summary>
        public double KoeffB
        {
            get
            {
                double b = Period != 0 ? (m_sum_y.Value - KoeffA * m_sum_x.Value) / Period : 0;
                return b;
            }
        }
        /// <summary>
        /// Возвращает коэффициент корряляции Пирсона между рядами x и y
        /// </summary>
        public double PearsonCorr
        {
            get { return m_corr.Value; }
        }
        /// <summary>
        /// Возвращает оценку достоверности R^2 текущего регрессионного уравнения
        /// </summary>
        public double R2
        {
            get
            {
                double corr = PearsonCorr;
                return corr * corr;
            }
        }
        /// <summary>
        /// Возвращает рассчитанную линейную модель. Используется модель линивых вычислений по
        /// требованию. Фактический рассчет коэффициентов происходит при запросе данного свойства.
        /// </summary>
        public override LineReg Value
        {
            get
            {
                return this;
            }
        }
        /// <summary>
        /// Фактическое количество размещенных элементов
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return m_x.Count; }
        }
        #endregion
    }
}
