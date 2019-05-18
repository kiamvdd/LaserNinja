using System;
using System.Collections;
using System.Collections.Generic;

public class RingBuffer<T>
{
    private T[] m_array;
    private int m_tailPosition = 0;
    private int m_length = 0;
    public int Length { get { return m_length; } }

    public RingBuffer(int size)
    {
        m_array = new T[size];
    }

    public int FindIndexIf(Func<T, bool> testCondition)
    {
        for (int i = 0; i < m_length; i++) {
            int index = IndexLinearToCircular(i);
            if (testCondition(m_array[index]))
                return i;
        }

        return -1;
    }

    public int IndexOf(T obj)
    {
        for (int i = 0; i < m_length; i++) {
            int index = IndexLinearToCircular(i);
            if (EqualityComparer<T>.Default.Equals(m_array[index], obj))
                return i;
        }

        return -1;
    }

    public void Add(T obj)
    {
        if (m_length < m_array.Length)
            m_length++;
        else
            m_tailPosition = (m_tailPosition + 1) % m_array.Length;

        int index = (m_tailPosition + (m_length - 1)) % m_array.Length;
        m_array[index] = obj;
    }

    private int IndexLinearToCircular(int index)
    {
        return (m_tailPosition + index) % m_length;
    }

    public T this[int i]
    {
        get
        {
            if (i < 0 || i >= m_length)
                throw new IndexOutOfRangeException();

            int index = IndexLinearToCircular(i);
            return m_array[index];
        }

        set
        {
            if (i < 0 || i >= m_length)
                throw new IndexOutOfRangeException();

            int index = IndexLinearToCircular(i);
            m_array[index] = value;
        }
    }
}
