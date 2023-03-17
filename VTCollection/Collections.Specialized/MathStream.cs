// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Преобразует входящие данные InT в данные типа OutT. Выходящие данные можно получить подписавшись на событие Out,
    /// либо прочитав свойство Value 
    /// </summary>
    /// <typeparam name="InT">Данные подаваемые на вход</typeparam>
    /// <typeparam name="OutT">Данные выдаваемые на выходе</typeparam>
    public abstract class MathStream<InT, OutT>
    {
        /// <summary>
        /// Делегат, указывающий на метод обработки выходящих данных
        /// </summary>
        /// <param name="out_item"></param>
        public delegate void OutHandler(OutT out_item);
        /// <summary>
        /// Событие, подписавшись на которое, можно получать данные, выдаваемые преобразователем
        /// </summary>
        public event OutHandler Out;
        /// <summary>
        /// Установить в истину, если требутеся выбросить исключение, в случае, если принимаемые данные
        /// содержат ошибку и не могут быть обработаны потоком. Использовать, когда требуется 'отловить'
        /// источник 'плохих' данных
        /// </summary>
        public bool StreamException
        {
            get; set;
        }
        /// <summary>
        /// Содержит последнее известное значение выданных данных
        /// </summary>
        abstract public OutT Value
        {
            get;
        }
        /// <summary>
        /// Истина, если последняя операция добавления была выполнина корректно.
        /// Ложь в противном случае.
        /// </summary>
        public bool IsValidAdd
        {
            get; private set;
        }
        /// <summary>
        /// Возвращает истину, если входные данные корректны, в противном случае возвращает ложь.
        /// Так как тип получаемых данных в обобщенном классе неизвестен, а их корректность определяется контекстом вычисления,
        /// этот метод необходимо переопределить в конкретном математическом примитиве. Например, если математический примитив возвращает
        /// отношение двух передаваемых значений (Value1/Value2), он должен выполнить проверку в данном методе значения Value2
        /// на НЕравенство его нулю, для избежания операции деления на ноль. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool CheckInValue(InT value)
        {
            return true;
        }
        /// <summary>
        /// Добавляет новое значение
        /// </summary>
        /// <param name="value">New value</param>
        public void Add(InT value)
        {
            //-- Данные могут быть обработаны, только в том случае
            //-- если они корректны
            IsValidAdd = CheckInValue(value);
            if (IsValidAdd)
            {
                //-- Невсегда возможно выполнить корректный расчет на поступивших данных,
                //-- например данных еще недостаточно (незаполнены буфера).
                //-- В этом случае выходные данные не передаются по конвееру дальше
                IsValidAdd = OnAdd(value);
                if (IsValidAdd)
                    Out?.Invoke(Value);
            }
            else if (!StreamException)
                return;
            else
                throw new ArgumentException("", "");
        }
        /// <summary>
        /// This methods need override in child class
        /// </summary>
        /// <param name="item"></param>
        abstract protected bool OnAdd(InT item);


    }
}
