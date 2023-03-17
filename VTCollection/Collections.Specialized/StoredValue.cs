// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Diagnostics;

namespace VT.Collections.Specialized
{
    public class QualifiedNameAttribute : Attribute
    {
        private string m_qualified_name;
        public QualifiedNameAttribute()
        {
        }
        public QualifiedNameAttribute(string qualifiedName)
        {
            m_qualified_name = qualifiedName;
        }
        public QualifiedNameAttribute(Func<string> getqname)
        {
            m_qualified_name = getqname();
        }
    }
    public class StoredValue<T>
    {
        private T m_value;
        public T Value
        {
            get => m_value;
            set => m_value = value;
        }
        public StoredValue(T value)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = new StackFrame();
            //System.Diagnostics.
            Value = value;
            //int k = 0;
        }
        /// <summary>
        /// Get equalization operator. value = StoredValueT.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator T(StoredValue<T> value)
        {

            return value.m_value;
        }
        /// <summary>
        /// Set equalization operator. StoredValueT = value.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator StoredValue<T>(T value)
        {
            return new StoredValue<T>(value);
        }

    }
}
