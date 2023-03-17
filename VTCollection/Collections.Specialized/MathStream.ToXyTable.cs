// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace VT.Collections.Specialized
{
    /// <summary>
    /// <para>ToXyTable добавляет в двухмерную таблицу новую строку line в виде одномерного массива T[].</para> 
    /// <para>Свойство Value возвращает двухмерную таблицу M*N в виде двухмерного массива
    /// T[,] где первое измерение - k-последних добавленных строк, а N количество колонок
    /// этой таблицы.</para>
    /// <para>Величина k задается свойством Lines. Количество колонок (факторов) должно
    /// быть заранее предопредлено и указыватся в конструкторе</para>
    /// </summary>
    /// <typeparam name="InTuple"></typeparam>
    public class ToXyTable<T> : MathStream<T[], T[,]>
    {
        /// <summary>
        /// Кольцевые буфера таблицы
        /// </summary>
        private RingBuffer<T>[] m_values = null;
        /// <summary>
        /// Создает преобразователь в двухмерную таблицу
        /// </summary>
        /// <param name="factors">Количество факторов (столбцов)</param>
        public ToXyTable(int factors)
        {
            if (factors <= 0)
                throw new ArgumentException("Argument 'factors' must be more 0", "factors");
            m_values = new RingBuffer<T>[factors];
            for (int i = 0; i < m_values.Length; i++)
                m_values[i] = new RingBuffer<T>();
        }
        /// <summary>
        /// Создает преобразователь в двухмерную таблицу
        /// </summary>
        /// <param name="lines">Количество линий таблицы</param>
        /// <param name="factors">Количество факторов (столбцов)</param>
        public ToXyTable(int lines, int factors) : this(factors)
        {
            Lines = lines;
        }
        /// <summary>
        /// Возвращает двухмерную таблицу M*N
        /// </summary>
        public override T[,] Value
        {
            get
            {
                T[,] table = new T[Lines, Factors];
                for (int line = 0; line < Lines; line++)
                {
                    for (int factor = 0; factor < Factors; factor++)
                        table[line, factor] = m_values[factor][line];
                }
                return table;
            }
        }
        /// <summary>
        /// Количество факторов (количество столбцов) формируемой таблицы
        /// </summary>
        public int Factors
        {
            get { return m_values.Length; }
        }
        /// <summary>
        /// Устанавливает предельное количество строк в таблице, либо возвращает текущее количество строк
        /// </summary>
        public int Lines
        {
            get { return m_values[0].Count; }
            set
            {
                for (int i = 0; i < m_values.Length; i++)
                    m_values[i].Capacity = value;
            }
        }
        /// <summary>
        /// Возвращает строку таблицы под номером index в виде массива T[]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T[] LineAt(int index)
        {
            T[] line = new T[Factors];
            for (int i = 0; i < m_values.Length; i++)
                line[i] = m_values[i][index];
            return line;
        }
        /// <summary>
        /// Возвращает истину, если размерность переданной строки соответствует размерности таблицы
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CheckInValue(T[] value)
        {
            //System.Type type = typeof(T).Equals(Double);
            return value.Length == Factors;
        }
        /// <summary>
        /// Добавляет строку в таблицу
        /// </summary>
        /// <param name="line"></param>
        protected override bool OnAdd(T[] line)
        {
            for (int i = 0; i < line.Length; i++)
                m_values[i].Add(line[i]);
            return true;
        }
    }
}
