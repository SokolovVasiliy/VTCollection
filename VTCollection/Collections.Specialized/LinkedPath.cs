using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MV;

namespace VT.Collections.Specialized
{
    /// <summary>
    /// Contein full symbol name include its relative path.
    /// </summary>
    [Serializable]
    public class LinkedPath : IEnumerable<string>, IReadOnlyCollection<string>
    {
        LinkedList<string> m_path = new LinkedList<string>();
        public char Separator { get; set; } = '\\';
        public bool ShowRootSeparator { get; set; }
        public bool IsSubPath { get; private set; }
        /*public bool IsSubPath
        {
            get
            {
                int index = m_path.Last.Value.Length - 1;
                char last = m_path.Last.Value[index];
                bool isSub = last == Separator;
                return isSub;
            } 
            private set
            {
                int index = m_path.Last.Value.Length - 1;
                char last = m_path.Last.Value[index];
                if(last != Separator)
                    m_path.Last.Value += Separator;
            }
        }*/
        public LinkedPath(string stringPath)
        {
            string[] elements = stringPath.Split(Separator);
            InitLinkedPath(elements);
        }
        private void InitLinkedPath(IEnumerable<string> elements)
        {
            elements.Map(s => m_path.AddLast(s));
            CheckSubPath();
            RemoveEmptyRoot();
        }
        public LinkedPath Clone()
        {
            LinkedPath npath = new LinkedPath(m_path, this);
            return npath;
        }
        private void CheckSubPath()
        {
            if(m_path.Count == 0)
            {
                IsSubPath = true;
                return;
            }
            if (!string.IsNullOrWhiteSpace(m_path.Last.Value))
            {
                IsSubPath = false;
                return;
            }
            while (m_path.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(m_path.Last.Value))
                {
                    m_path.RemoveLast();
                    IsSubPath = true;
                }
                else
                    break;
            }
        }
        private void RemoveEmptyRoot()
        {
            while(m_path.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(m_path.First.Value))
                    break;
                m_path.RemoveFirst();
            }
        }
        public LinkedPath()
        {
        }
        /// <summary>
        /// Create LinkedPath from any IEnumerable collection
        /// </summary>
        /// <param name="path">Eny enumerable collection conteins path elements</param>
        public LinkedPath(IEnumerable<string> path) => InitLinkedPath(path);
        private LinkedPath(LinkedList<string> list, LinkedPath path)
        {
            Separator = path.Separator;
            ShowRootSeparator = path.ShowRootSeparator;
            InitLinkedPath(list);
        }
        /// <inheritdoc/>
        public override string ToString()
        {
            if (m_path.Count == 0)
            {
                if(ShowRootSeparator)
                    return Separator.ToString();
                return "";
            }
            StringBuilder str = new StringBuilder();
            if (ShowRootSeparator)
                str.Append(Separator);
            m_path.Map(s => str.Append(s).Append(Separator));
            if(!IsSubPath)
                str.Remove(str.Length - 1, 1);
            return str.ToString();
        }
        /// <summary>
        /// Return true, if subpath begin root contein in current path. Otherwise return false
        /// </summary>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public bool Contein(LinkedPath subpath)
        {
            if (subpath == null)
                return false;
            if (subpath.Count == 0)
                return false;
            if (m_path == null || m_path.Count == 0)
                return false;
            if (subpath.Count > m_path.Count)
                return false;
            var sublist = subpath.ToLinkedList();
            LinkedListNode<string> current = m_path.First;
            for(LinkedListNode<string> node = sublist.First; 
                node != null; 
                node = node.Next, current = current.Next)
            {
                if (current.Value != node.Value)
                    return false;
            }
            return true;
        }
        public override bool Equals(object obj)
        {
            LinkedPath path = obj as LinkedPath;
            if (path == null)
                return false;
            //-- if pointer are equal - then two objects are equal
            if (path == this)
                return true;
            //-- if content are equal - then tow objects are equal
            if (path.ToString() == ToString())
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        /// <summary>
        /// Return linked list of elements conteins into summary path. For example, 
        /// will be return list '{Deribit} -> {USD/BTC}' for path 'Deribit\BTC-PERPETUAL' 
        /// </summary>
        /// <returns>List of elements of path</returns>
        public LinkedList<string> ToLinkedList()
        {
            LinkedList<string> list = new LinkedList<string>();
            m_path.Map(s => list.AddLast(s));
            return list;
        }
        /// <summary>
        /// Trim name of full path to name (last element) and return path only
        /// </summary>
        /// <returns></returns>
        public LinkedPath TrimLast()
        {
            LinkedList<string> list = ToLinkedList();
            list.RemoveLast();
            LinkedPath path = new LinkedPath(list, this);
            //-- after trim, result path always is subpath
            path.IsSubPath = true;
            return path;
        }
        /// <summary>
        /// Trim first root element and return right path as new LinkedPath
        /// </summary>
        /// <returns></returns>
        public LinkedPath TrimRoot()
        {
            LinkedList<string> list = ToLinkedList();
            list.RemoveFirst();
            LinkedPath path = new LinkedPath(list, this);
            return path;
        }
        /// <summary>
        /// Get last element of path
        /// </summary>
        public LinkedPath GetLast()
        {
            if (m_path.Count == 0)
                return Clone();
            if (IsSubPath)
                return m_path.Last.Value + Separator;
            return m_path.Last.Value;
        }
        /// <summary>
        /// Get first element of path
        /// </summary>
        public string GetFirst()
        {
            if (m_path.Count == 0)
                return Clone();
            return m_path.First.Value;
        }
        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator() => m_path.GetEnumerator();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => m_path.GetEnumerator();
        /// <inheritdoc/>
        public int Count => m_path.Count;
        /// <summary>
        /// Convert string path to LinkedPath
        /// </summary>
        /// <param name="stringPath"></param>
        public static implicit operator LinkedPath(string stringPath) => new LinkedPath(stringPath);
        /// <summary>
        /// Convert string path to LinkedPath
        /// </summary>
        /// <param name="path"></param>
        public static implicit operator string(LinkedPath path) => path.ToString();
        /// <summary>
        /// Concatinate path1 with path2. For example: path1 = Deribit; path2 = BTCUSD; path3 = path1 + path2 = Deribit\BTCUSD.
        /// </summary>
        /// <param name="path1">Path #1</param>
        /// <param name="path2">Path #2</param>
        /// <returns>Concatinate Path #1 + Path #2</returns>
        public static LinkedPath operator+(LinkedPath path1, LinkedPath path2)
        {
            LinkedList<string> sum = new LinkedList<string>();
            path1.Map((s) => sum.AddLast(s));
            path2.Map((s) => sum.AddLast(s));
            LinkedPath pathSum = new LinkedPath(sum);
            return pathSum;
        }
        /// <summary>
        /// Concatinate base name with path2. For example: base name = TSLabQuotes; path2 = Derebit\BTCUSD;
        /// path3 = base conector + path2 = TSLabQuotes\Deribit\BTCUSD.
        /// </summary>
        /// <param name="name">Base name</param>
        /// <param name="path2">Path #2</param>
        /// <returns>Concatinate Path #1 + Path #2</returns>
        public static LinkedPath operator+(string name, LinkedPath path2)
        {
            LinkedList<string> sum = new LinkedList<string>();
            LinkedPath path1 = new LinkedPath(name);
            path1.Map((s) => sum.AddLast(s));
            path2.Map((s) => sum.AddLast(s));
            LinkedPath pathSum = new LinkedPath(sum, path2);
            return pathSum;
        }
        /// <summary>
        /// Concatinate base name with path2. For example: base name = TSLabQuotes; path2 = Derebit\BTCUSD;
        /// path3 = base conector + path2 = TSLabQuotes\Deribit\BTCUSD.
        /// </summary>
        /// <param name="name">Base name</param>
        /// <param name="path2">Path #2</param>
        /// <returns>Concatinate Path #1 + Path #2</returns>
        public static LinkedPath operator +(LinkedPath path1, string name)
        {
            LinkedList<string> sum = new LinkedList<string>();
            LinkedPath path2 = new LinkedPath(name);
            path1.Map((s) => sum.AddLast(s));
            path2.Map((s) => sum.AddLast(s));
            LinkedPath pathSum = new LinkedPath(sum, path1);
            return pathSum;
        }
    }
}
