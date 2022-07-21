using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Perfolizer.Mathematics.Selectors
{
    /// <summary>
    /// <remarks>
    /// Based on http://erdani.com/research/sea2017.pdf
    /// </remarks>
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static class QuickSelectAdaptiveAlgorithms
    {
        public delegate int Partition(Span<double> A);

        public static int HoarePartition(Span<double> A, int p)
        {
            Swap(A, 0, p);
            int a = 1;
            int b = A.Length - 1;
            while (true)
            {
                while (a <= b && A[a] < A[0])
                    a++;
                while (A[0] < A[b])
                    b--;
                if (a >= b)
                    break;
                Swap(A, a, b);
                a++;
                b--;
            }
            Swap(A, 0, a - 1);
            return a - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(Span<double> A, int i, int j)
        {
            double t = A[i];
            A[i] = A[j];
            A[j] = t;
        }

        /// <summary>
        /// Swaps the median of A[a], A[b], A[c] into A[b]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Median3(Span<double> A, int a, int b, int c)
        {
            if (A[b] < A[a])
            {
                if (A[b] < A[c])
                {
                    if (A[c] < A[a])
                        Swap(A, b, c);
                    else
                        Swap(A, a, b);
                }
            }
            else if (A[c] < A[b])
            {
                if (A[c] < A[a])
                    Swap(A, a, b);
                else
                    Swap(A, b, c);
            }
        }

        /// <summary>
        /// Swaps the median of A[a], A[b], A[c], A[d], A[e] into A[c]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Median5(Span<double> A, int a, int b, int c, int d, int e)
        {
            Sort(A, a, b); // a <= b
            Sort(A, c, d); // c <= d

            // 1. Select minimum from (a, b, c, d) and swap it with e.
            //    The selected value is one of the two smallest values from A (has index 0 or 1 in sorted(A))
            if (A[a] < A[c]) // a <= b && a < c <= d
            {
                Swap(A, a, e); // a = e
                Sort(A, a, b); // restore a <= b
            }
            else // c <= a <= b && c <= d
            {
                Swap(A, c, e); // c = e
                Sort(A, c, d); // restore c <= d
            }

            // 2. Select two central elements from (a, b, c, d)
            if (A[a] > A[c]) // c <= d && c < a <= b => median in (a, d). Else a <= b && a <= c <= d => median in (b, c)
            {
                Swap(A, b, a); // b = a
                Swap(A, c, d); // c = d
            }

            // 3. select minimum from previous selected elements
            if (A[b] < A[c])
                Swap(A, c, b); // c = b
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Sort(Span<double> A, int a, int b)
        {
            if (A[a] > A[b])
                Swap(A, a, b);
        }

        [SuppressMessage("ReSharper", "RedundantIfElseBlock")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MedianIndex(Span<double> A, int a, int b, int c)
        {
            if (A[a] < A[b]) // a < b
            {
                if (A[b] < A[c]) // a < b < c
                    return b;
                else // a < b, c < b
                    return A[a] < A[b] ? b : a;
            }
            else // b < a
            {
                if (A[a] < A[c]) // b < a < c
                    return a;
                else // b < a, c < a
                    return A[b] < A[c] ? c : b;
            }
        }

        /// <summary>
        /// Tukey's Ninther
        /// </summary>
        public static void Ninther(Span<double> A, int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8, int i9)
        {
            int j1 = MedianIndex(A, i1, i2, i3);
            int j2 = MedianIndex(A, i4, i5, i6);
            int j3 = MedianIndex(A, i7, i8, i9);
            int j = MedianIndex(A, j1, j2, j3);
            Swap(A, j, i5);
        }

        [PublicAPI]
        public static int BfprtBaseline(Span<double> A)
        {
            if (A.Length < 5)
                return HoarePartition(A, A.Length / 2);
            int i = 0, j = 0;
            while (i + 4 < A.Length)
            {
                Median5(A, i, i + 1, i + 2, i + 3, i + 4);
                Swap(A, i + 2, j);
                i += 5;
                j += 1;
            }
            QuickSelect(BfprtBaseline, A.Slice(0, j), j / 2);
            return HoarePartition(A, j / 2);
        }

        /// <summary>
        /// Puts k-th smallest element of a in a[k] and partitions a around it.
        /// </summary>
        public static void QuickSelect(Partition partition, Span<double> A, int k)
        {
            while (true)
            {
                int p = partition(A);
                if (p == k)
                    return;
                if (p > k)
                    A = A.Slice(0, p);
                else
                {
                    k = k - p - 1;
                    int i = p + 1;
                    A = A.Slice(i, A.Length - i);
                }
            }
        }

        public static int RepeatedStep(Span<double> A)
        {
            if (A.Length < 9)
                return HoarePartition(A, A.Length / 2);
            int i = 0, j = 0;
            while (i + 2 < A.Length)
            {
                Median3(A, i, i + 1, i + 2);
                Swap(A, i + 1, j);
                i += 3;
                j += 1;
            }
            i = 0;
            int m = 0;
            while (i + 2 < j)
            {
                Median3(A, i, i + 1, i + 2);
                Swap(A, i + 1, m);
                i += 3;
                m += 1;
            }
            QuickSelect(RepeatedStep, A.Slice(0, m), m / 2);
            return HoarePartition(A, m / 2);
        }

        [PublicAPI]
        public static int MedianOfNinthersBasic(Span<double> A)
        {
            if (A.Length < 9)
                return HoarePartition(A, A.Length / 2);
            int f = A.Length / 9;
            for (int i = 4 * f; i < 5 * f; i++)
            {
                int l = i - 4 * f;
                int r = i + 5 * f;
                Ninther(A, l, l + 1, l + 2, l + 4, i, r, r + 1, r + 2, r + 3);
            }

            int i1 = 4 * f;
            int j = 5 * f;
            QuickSelect(RepeatedStep, A.Slice(i1, j - i1), f / 2);
            return ExpandPartition(A, 4 * f, 4 * f + f / 2, 5 * f - 1);
        }

        private static int ExpandPositionRight(Span<double> A, int hi, int right)
        {
            int pivot = 0;
            const int oldPivot = 0;
            while (pivot < hi)
            {
                if (right == hi)
                {
                    Swap(A, 0, pivot);
                    return pivot;
                }
                if (A[right] > A[oldPivot])
                {
                    right--;
                    continue;
                }

                pivot++;
                Swap(A, right, pivot);
                right--;
            }

            while (pivot < right)
            {
                if (A[right] >= A[oldPivot])
                {
                    right--;
                    continue;
                }
                while (pivot < right)
                {
                    pivot++;
                    if (A[oldPivot] < A[pivot])
                    {
                        Swap(A, right, pivot);
                        break;
                    }
                }
            }

            Swap(A, oldPivot, pivot);
            return pivot;
        }

        private static int ExpandPositionLeft(Span<double> A, int lo, int pivot)
        {
            int left = 0;
            int oldPivot = pivot;
            while (lo < pivot)
            {
                if (left == lo)
                {
                    Swap(A, oldPivot, pivot);
                    return pivot;
                }
                if (A[oldPivot] >= A[left])
                {
                    left++;
                    continue;
                }
                pivot--;
                Swap(A, left, pivot);
            }
            while (left < pivot)
            {
                if (A[left] <= A[oldPivot])
                {
                    left++;
                    continue;
                }
                while (left < pivot)
                {
                    pivot--;
                    if (A[pivot] < A[oldPivot])
                    {
                        Swap(A, left, pivot);
                        break;
                    }
                }
            }

            Swap(A, oldPivot, pivot);
            return pivot;
        }

        private static int ExpandPartition(Span<double> A, int lo, int pivot, int hi)
        {
            int left = 0;
            int right = A.Length - 1;
            while (true)
            {
                while (left < lo)
                {
                    if (A[left] > A[pivot])
                        break;
                    left++;
                }
                if (left == lo)
                    return pivot + ExpandPositionRight(A.Slice(pivot, A.Length - pivot), hi - pivot, right - pivot);

                while (right > hi)
                {
                    if (A[right] < A[pivot])
                        break;
                    right--;
                }
                if (right == hi)
                    return left + ExpandPositionLeft(A.Slice(left, A.Length - left), lo - left, pivot - left);

                Swap(A, left, right);
            }
        }

        public static int MedianOfNinthers(Span<double> A)
        {
            int n = A.Length;
            double fi;
            if (n <= 1024)
                fi = 1.0 / 12;
            else if (n <= 128 * 1024)
                fi = 1.0 / 64;
            else
                fi = 1.0 / 1024;
            int n0 = (int) Math.Floor(fi * n / 3.0);
            if (n0 < 3)
                return HoarePartition(A, A.Length / 2);
            int g = (n - 3 * n0) / 4;
            int i1 = 2 * g + n0;
            int j = 2 * g + 2 * n0;
            var Am = A.Slice(i1, j - i1);
            int l = g;
            int m = 2 * g + n0;
            int r = 3 * g + 2 * n0;
            for (int i = 0; i < n0 / 3; i++)
            {
                Ninther(A, l, m, r, l + 1, m + n0 / 3, r + 1, l + 2, m + 2 * n0 / 3, r + 2);
                m += 1;
                l += 3;
                r += 3;
            }
            QuickSelectAdaptive(Am, n0 / 2);
            return ExpandPartition(A, 2 * g + n0, 2 * g + n0 + n0 / 2, 2 * g + 2 * n0 - 1);
        }

        public static int MedianOfMinima(Span<double> A, int k)
        {
            if (A.Length == 1)
                return 0;
            int groupCount = 2 * k;
            int groupSize = A.Length / groupCount;
            for (int i = 0; i < groupCount; i++)
            {
                int groupStart = groupCount + i * (groupSize - 1);
                int groupEnd = groupCount + (i + 1) * (groupSize - 1);
                int m = groupStart;
                for (int j = groupStart + 1; j < groupEnd; j++)
                {
                    if (A[j] < A[m])
                        m = j;
                }
                if (A[m] < A[i])
                    Swap(A, i, m);
            }
            QuickSelectAdaptive(A.Slice(0, groupCount), k);
            return ExpandPartition(A, 0, k, groupCount - 1);
        }

        public static int MedianOfMaxima(Span<double> A, int k)
        {
            if (A.Length == 1)
                return 0;
            int groupCount = 2 * (A.Length - 1 - k);
            int groupSize = A.Length / groupCount;
            for (int i = A.Length - 1; i >= A.Length - groupCount; i--)
            {
                int groupStart = A.Length - groupCount - 1 - (A.Length - 1 - (i - 1)) * (groupSize - 1) + 1;
                int groupEnd = A.Length - groupCount - 1 - (A.Length - 1 - i) * (groupSize - 1) + 1;
                int m = groupStart;
                for (int j = groupStart + 1; j < groupEnd; j++)
                {
                    if (A[j] > A[m])
                        m = j;
                }
                if (A[m] > A[i])
                    Swap(A, i, m);
            }

            int i1 = A.Length - groupCount;
            QuickSelectAdaptive(A.Slice(i1, A.Length - i1), k - (A.Length - groupCount));
            return ExpandPartition(A, A.Length - groupCount, k, A.Length - 1);
        }

        private static void Minimum(Span<double> A)
        {
            int pivot = 0;
            for (int i = 1; i < A.Length; i++)
                if (A[i] < A[pivot])
                    pivot = i;
            Swap(A, 0, pivot);
        }

        private static void Maximum(Span<double> A)
        {
            int pivot = 0;
            for (int i = 1; i < A.Length; i++)
                if (A[i] > A[pivot])
                    pivot = i;
            Swap(A, A.Length - 1, pivot);
        }

        public static void QuickSelectAdaptive(Span<double> A, int k)
        {
            while (true)
            {
                if (k == 0)
                {
                    Minimum(A);
                    return;
                }
                if (k == A.Length - 1)
                {
                    Maximum(A);
                    return;
                }

                int p;
                if (A.Length < 16)
                    p = HoarePartition(A, k);
                else if (6 * k < A.Length)
                    p = MedianOfMinima(A, k);
                else if (6 * k > 5 * A.Length)
                    p = MedianOfMaxima(A, k);
                else
                    p = MedianOfNinthers(A);

                if (p == k)
                    return;
                if (p > k)
                    A = A.Slice(0, p);
                else
                {
                    k = k - p - 1;
                    int i = p + 1;
                    A = A.Slice(i, A.Length - i);
                }
            }
        }

        [PublicAPI]
        public static void QuickSelectAdaptive(double[] values, int k)
            => QuickSelectAdaptive(new Span<double>(values), k);
    }
}