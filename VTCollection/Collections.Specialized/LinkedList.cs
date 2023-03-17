using System;
using System.Collections.Generic;
using System.Text;

namespace MEX
{
    public class MainListNode<T>
    {
        public MainListNode<T> Next { get; set; }
        public MainListNode<T> Preview { get; set; }
        public T Value { get; set; }
        public MainListNode(T value)
        {
            Value = value;
        }
        public override string ToString()
        {
            return $"Node<{Value}>";
        }
    }
	
	public class MainLinkedList<T>
    {
        MainListNode<T> m_first;
        MainListNode<T> m_last;
        public int Count { get; private set; } = 0;
        public MainListNode<T> First
        {
            get => m_first;
            set => m_first = value;
        }
        public MainListNode<T> Last
        {
            get => m_last;
            set => m_last = value;
        }
        public void AddFirst(T value)
        {
            MainListNode<T> main = new MainListNode<T>(value);
            AddFirst(main);
        }
        public void AddLast(T value)
        {
            MainListNode<T> main = new MainListNode<T>(value);
            AddLast(main);
        }
        public void AddLast(MainListNode<T> value)
        {
            if (m_first == null)
            {
                m_first = value;
                m_last = value;
            }
            else
            {
                if (m_first.Next == null)
                    m_first.Next = m_last;
                var t = m_last;
                m_last = value;
                t.Next = m_last;
                m_last.Preview = t;
            }
            Count++;
        }
        public void AddFirst(MainListNode<T> value)
        {
            if (m_first == null)
            {
                m_first = value;
                m_last = value;
            }
            else
            {
                var t = m_first;
                m_first = value;
                m_first.Next = t;
                t.Preview = m_first;
            }
            Count++;
        }
    }
}
