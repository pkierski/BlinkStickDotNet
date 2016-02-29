using System;
using System.Collections;
using System.Collections.Generic;

namespace BlinkStickDotNet.Filters
{
    public abstract class AbstractFilter : IFilter
    {
        protected AbstractFilter(IFilter underlyingFilter)
        {
            m_underlyingFilter = underlyingFilter;
            m_colors = underlyingFilter.GetAll();
        }

        protected AbstractFilter()
        {

        }

        protected RgbColorF[] m_colors = null;
        protected IFilter m_underlyingFilter = null;

        public virtual void SetAll(RgbColorF[] all)
        {
            if (all == null)
                throw new ArgumentNullException();

            if (all.Length != Count)
                throw new ArgumentException(string.Format("Array with lenght {0} expected, {1} used", all.Length, Count));

            RgbColorF[] temp = new RgbColorF[all.Length];
            for (int i = 0; i < m_colors.Length; ++i)
                temp[i] = ConvertOnSet(all[i]);

            m_underlyingFilter.SetAll(temp);
        }

        public virtual RgbColorF[] GetAll()
        {
            RgbColorF[] underlyingValues = m_underlyingFilter.GetAll();
            RgbColorF[] values = new RgbColorF[Count];
            for (int i = 0; i < values.Length; ++i)
            {
                values[i] = ConvertOnGet(underlyingValues[i]);
                m_colors[i] = values[i];
            }
            return values;
        }

        public virtual RgbColorF this[int index]
        {
            get
            {
                RgbColorF underlyingValue = m_underlyingFilter[index];
                RgbColorF value = ConvertOnGet(underlyingValue);
                m_colors[index] = value;
                return value;
            }

            set
            {
                m_colors[index] = value;
                m_underlyingFilter[index] = ConvertOnSet(value);
            }
        }

        protected virtual RgbColorF ConvertOnGet(RgbColorF underlyingValue) { return underlyingValue; }
        protected virtual RgbColorF ConvertOnSet(RgbColorF value) { return value; }

        public int Count
        {
            get
            {
                return m_colors.Length;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public void Add(RgbColorF item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(RgbColorF item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(RgbColorF[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<RgbColorF> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(RgbColorF item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, RgbColorF item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(RgbColorF item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }



}