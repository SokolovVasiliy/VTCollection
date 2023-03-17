// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Объединяет результаты двух событий в один кортеж
    /// </summary>
    /// <typeparam name="T1">Тип результата в первом событии</typeparam>
    /// <typeparam name="T2">Тип результата во вотором событии</typeparam>
    class Bind<T1, T2>
    {
        /// <summary>
        /// Значение первого события
        /// </summary>
        private T1 m_item1;
        /// <summary>
        /// Значение второго события
        /// </summary>
        private T2 m_item2;
        /// <summary>
        /// Истина, если данные первого события доступны
        /// </summary>
        private bool m_avaliable_1 = false;
        /// <summary>
        /// Истина, если данные второго события доступны
        /// </summary>
        private bool m_avaliable_2 = false;
        /// <summary>
        /// Возвращает истину, если оба события доступны для чтения
        /// </summary>
        public bool IsAvaliable
        {
            get { return m_avaliable_1 && m_avaliable_2; }
        }
        /// <summary>
        /// Возвращает текущий кортеж двух данных
        /// </summary>
        public Tuple<T1, T2> Value
        {
            get
            {
                return Tuple.Create(m_item1, m_item2);
            }
        }
        /// <summary>
        /// Это событие происходит, когда оба результата доступны
        /// </summary>
        /// <param name="item">Кортеж, содержащий результаты обоих событий</param>
        public delegate void OutHendler(Tuple<T1, T2> item);
        /// <summary>
        /// Это событие происходит, когда оба результата доступны
        /// </summary>
        public event OutHendler Out;
        /// <summary>
        /// Получатель первого события
        /// </summary>
        /// <param name="item"></param>
        public void V1(T1 item)
        {
            m_item1 = item;
            if (m_avaliable_2)
            {
                Out?.Invoke(Tuple.Create(m_item1, m_item2));
                m_avaliable_1 = false;
                m_avaliable_2 = false;
            }
            else
                m_avaliable_1 = true;
        }
        /// <summary>
        /// Получатель второго события
        /// </summary>
        /// <param name="item"></param>
        public void V2(T2 item)
        {
            m_item2 = item;
            if (m_avaliable_1)
            {
                Out?.Invoke(Tuple.Create(m_item1, m_item2));
                m_avaliable_1 = false;
                m_avaliable_2 = false;
            }
            else
                m_avaliable_2 = true;
        }
    }
}
