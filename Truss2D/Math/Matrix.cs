using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truss2D.Math
{
    public class Matrix
    {
        /// <summary>
        /// # of rows
        /// </summary>
        public int M { get; private set; }

        /// <summary>
        /// # of cols
        /// </summary>
        public int N { get; private set; }

        private decimal[,] matrix;

        public Matrix(int m, int n, decimal[,] matrix)
        {
            M = m;
            N = n;
            this.matrix = matrix;
        }

        /// <summary>
        /// Each vector is a column
        /// </summary>
        /// <param name="vectors"></param>
        public Matrix(List<Vector> vectors)
        {
            M = 2;
            N = vectors.Count;
            matrix = new decimal[M, N];

            for (int i=0; i<vectors.Count; ++i)
            {
                matrix[0, i] = vectors[i].X;
                matrix[1, i] = vectors[i].Y;
            }
        }

        /// <summary>
        /// For the creation of augmented matrix
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="known"></param>
        public Matrix(List<Vector> vectors, Vector known)
        {
            M = 2;
            N = vectors.Count+1;
            matrix = new decimal[M, N];

            for (int i = 0; i < vectors.Count; ++i)
            {
                matrix[0, i] = vectors[i].X;
                matrix[1, i] = vectors[i].Y;
            }

            matrix[0, N - 1] = known.X;
            matrix[1, N - 1] = known.Y;
        }
        
        /// <summary>
        /// Get and set the value of matrix[i,j]
        /// </summary>
        /// <param name="i">row #</param>
        /// <param name="j">col #</param>
        /// <returns></returns>
        public decimal this[int i, int j]
        {
            get
            {
                return matrix[i, j];
            }
            set
            {
                matrix[i, j] = value;
            }
        }

        /// <summary>
        /// returns rank.
        /// </summary>
        /// <returns></returns>
        public int ReduceToRREF()
        {

            int i = 0, j = 0;
            int headRow;

            int rank = 0;

            decimal leadingTerm;

            while (j < N && i < M)
            {
                headRow = i;
                while (i < M && matrix[i,j] == 0)
                {
                    i++;
                }
                if (i != M)
                {
                    rank++; //no pivot values needed because it is already leading one
                    Scale(i, ((decimal)1.0 / matrix[i, j]));
                    if (i != headRow)
                    {
                        Swap(i, headRow);
                    }

                    i++;

                    //first loop clear all non-zero terms till end of column
                    while (i < M)
                    {
                        leadingTerm = matrix[i, j];
                        if (leadingTerm != 0)
                        {
                            AddToRow(i, headRow, (-leadingTerm));
                        }
                        i++;
                    }

                    i = headRow;
                    i--;

                    //second loop clear all non-zero terms till begin of 
                    while (i >= 0)
                    {
                        leadingTerm = matrix[i, j];
                        if (leadingTerm != 0)
                        {
                            AddToRow(i, headRow, (-leadingTerm));
                        }
                        i--;
                    }
                    i = headRow + 1; //reset for next iteration
                }
                else
                {
                    i = headRow;
                }
                j++;
            }
            return rank;
        }

        protected void Swap(int row1, int row2)
        {
            for (int i=0; i<N; ++i)
            {
                var temp = matrix[row1, i];
                matrix[row1, i] = matrix[row2, i];
                matrix[row2, i] = temp;
            }
        }

        protected void Scale(int row, decimal constant)
        { 
            for (int j = 0; j < N; j++)
            {
                matrix[row,j] *= constant;
            }
        }

        protected void AddToRow(int row1, int row2, decimal multipleOfRow2)
        {
            for (int j = 0; j < N; j++)
            {
                matrix[row1,j] += multipleOfRow2 * matrix[row2,j];
            }
        }

    }
}
