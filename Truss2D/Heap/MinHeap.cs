using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truss2D.Heap
{
    /// <summary>
    /// 
    /// </summary>
    public class MinHeap<T> where T:class
    {
        private Comparison<T> comparison;
        private T[] arr;
        private int capacity;
        private int size;

        private const int DefaultCapacity = 10;

        public MinHeap(Comparison<T> comparison, int capacity)
        {
            size = 0;
            this.capacity = capacity;
            arr = new T[capacity+1];
            this.comparison = comparison;
        }

        public MinHeap(Comparison<T> comparison) : this(comparison,DefaultCapacity)
        {
        }

        public void PercolateDown(int pos)
        {
            T temp = arr[pos];

            while (pos < size / 2)
            {
                int child = pos * 2;
                if (comparison.Invoke(arr[child], arr[child + 1]) < 0)
                    ++child;

                if (comparison.Invoke(temp, arr[child])>0)
                    arr[pos] = arr[child];
                else
                    break;
                pos = child;
            }

            arr[pos] = temp;
        }

        public void PercolateUp(int pos)
        {
            T temp = arr[pos];

            while (pos > 1)
            {
                int parent = pos / 2;
                if (comparison.Invoke(arr[parent], temp) > 0)
                    arr[pos] = arr[parent];
                else
                {
                    arr[pos] = temp;
                    return;
                }
                pos = parent;
            }
            
        }

        /// <summary>
        /// Returns the position of the obj, '-1' indicates not found
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Search(T obj)
        {
            for (int i=0; i<size;)
            {
                if (obj == arr[++i])
                    return i;
            }
            return 0;
        }

        public void Expand()
        {
            T[] temp = new T[capacity *= 2];
            for (int i=0; i<size;)
            {
                temp[++i] = arr[i];
            }
            arr = temp;
        }

        public void TrimToSize()
        {
            if (capacity>size+1)
            {
                T[] temp = new T[capacity = size + 1];
                
                for (int i=0; i<size;)
                {
                    temp[++i] = arr[i];
                }
                arr = temp;
            }
        }

        public void Push(T obj)
        {
            if (capacity == size + 1)
                Expand();
            arr[++size] = obj;
            PercolateUp(size);
        }

        public T Pop()
        {
            T temp = arr[1];
            arr[1] = arr[size];
            arr[size--]=null;

            PercolateDown(1);
            return temp;
        }
    }
}
