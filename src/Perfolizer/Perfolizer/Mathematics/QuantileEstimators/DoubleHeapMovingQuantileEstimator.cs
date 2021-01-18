using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;
using Perfolizer.Common;
using Perfolizer.Exceptions;
using Perfolizer.Mathematics.Common;

namespace Perfolizer.Mathematics.QuantileEstimators
{
    /// <summary>
    /// A moving selector based on a double heap data structure.
    /// Memory: O(windowSize).
    /// Add complexity: O(log(windowSize)).
    /// GetValue complexity: O(1).
    /// 
    /// <remarks>
    /// Based on the following paper:
    /// Hardle, W., and William Steiger. "Algorithm AS 296: Optimal median smoothing." Journal of the Royal Statistical Society.
    /// Series C (Applied Statistics) 44, no. 2 (1995): 258-264.
    /// </remarks>
    /// </summary>
    public class DoubleHeapMovingQuantileEstimator : ISequentialQuantileEstimator
    {
        private readonly int windowSize, k;
        private readonly Probability probability;
        private readonly double[] h;
        private readonly int[] heapToElementIndex;
        private readonly int[] elementToHeapIndex;
        private readonly int rootHeapIndex, lowerHeapMaxSize;
        private readonly MovingQuantileEstimatorInitStrategy initStrategy;
        private readonly HyndmanYanType? hyndmanYanType = null;
        private int upperHeapSize, lowerHeapSize, totalElementCount;

        public DoubleHeapMovingQuantileEstimator(int windowSize, int k,
            MovingQuantileEstimatorInitStrategy initStrategy = MovingQuantileEstimatorInitStrategy.QuantileApproximation)
        {
            Assertion.Positive(nameof(windowSize), windowSize);
            Assertion.InRangeInclusive(nameof(k), k, 0, windowSize - 1);

            this.windowSize = windowSize;
            this.k = k;
            probability = Probability.NaN;
            h = new double[windowSize];
            heapToElementIndex = new int[windowSize];
            elementToHeapIndex = new int[windowSize];

            lowerHeapMaxSize = k;
            this.initStrategy = initStrategy;
            rootHeapIndex = k;
        }

        public DoubleHeapMovingQuantileEstimator(int windowSize, Probability p) 
            : this(windowSize, (int) Math.Round((windowSize - 1) * p))
        {
            probability = p;
        }

        public DoubleHeapMovingQuantileEstimator(int windowSize, Probability p, HyndmanYanType hyndmanYanType)
            : this(windowSize, ((int) HyndmanYanEquations.GetH(hyndmanYanType, windowSize, p) - 1).Clamp(0, windowSize - 1))
        {
            this.hyndmanYanType = hyndmanYanType;
            probability = p;
        }

        private void Swap(int heapIndex1, int heapIndex2)
        {
            int elementIndex1 = heapToElementIndex[heapIndex1];
            int elementIndex2 = heapToElementIndex[heapIndex2];
            double value1 = h[heapIndex1];
            double value2 = h[heapIndex2];

            h[heapIndex1] = value2;
            h[heapIndex2] = value1;
            heapToElementIndex[heapIndex1] = elementIndex2;
            heapToElementIndex[heapIndex2] = elementIndex1;
            elementToHeapIndex[elementIndex1] = heapIndex2;
            elementToHeapIndex[elementIndex2] = heapIndex1;
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
        [SuppressMessage("ReSharper", "RedundantIfElseBlock")]
        private void Sift(int heapIndex)
        {
            int SwapWithChildren(int heapCurrentIndex, int heapChildIndex1, int heapChildIndex2, bool isUpperHeap)
            {
                bool hasChild1 = rootHeapIndex - lowerHeapSize <= heapChildIndex1 && heapChildIndex1 <= rootHeapIndex + upperHeapSize;
                bool hasChild2 = rootHeapIndex - lowerHeapSize <= heapChildIndex2 && heapChildIndex2 <= rootHeapIndex + upperHeapSize;

                if (!hasChild1 && !hasChild2)
                    return heapCurrentIndex;

                if (hasChild1 && !hasChild2)
                {
                    if (h[heapIndex] < h[heapChildIndex1] && !isUpperHeap || h[heapIndex] > h[heapChildIndex1] && isUpperHeap)
                    {
                        Swap(heapIndex, heapChildIndex1);
                        return heapChildIndex1;
                    }
                    return heapCurrentIndex;
                }

                if (hasChild1 && hasChild2)
                {
                    if ((h[heapIndex] < h[heapChildIndex1] || h[heapIndex] < h[heapChildIndex2]) && !isUpperHeap ||
                        (h[heapIndex] > h[heapChildIndex1] || h[heapIndex] > h[heapChildIndex2]) && isUpperHeap)
                    {
                        int heapChildIndex0 =
                            h[heapChildIndex1] > h[heapChildIndex2] && !isUpperHeap ||
                            h[heapChildIndex1] < h[heapChildIndex2] && isUpperHeap
                                ? heapChildIndex1
                                : heapChildIndex2;
                        Swap(heapIndex, heapChildIndex0);
                        return heapChildIndex0;
                    }
                    return heapCurrentIndex;
                }

                throw new InvalidOperationException();
            }

            while (true)
            {
                if (heapIndex != rootHeapIndex)
                {
                    bool isUpHeap = heapIndex > rootHeapIndex;
                    int heapParentIndex = rootHeapIndex + (heapIndex - rootHeapIndex) / 2;
                    if (h[heapParentIndex] < h[heapIndex] && !isUpHeap || h[heapParentIndex] > h[heapIndex] && isUpHeap)
                    {
                        Swap(heapIndex, heapParentIndex);
                        heapIndex = heapParentIndex;
                        continue;
                    }
                    else
                    {
                        int heapChildIndex1 = rootHeapIndex + (heapIndex - rootHeapIndex) * 2;
                        int heapChildIndex2 = rootHeapIndex + (heapIndex - rootHeapIndex) * 2 + Math.Sign(heapIndex - rootHeapIndex);
                        int newHeapIndex = SwapWithChildren(heapIndex, heapChildIndex1, heapChildIndex2, isUpHeap);
                        if (newHeapIndex != heapIndex)
                        {
                            heapIndex = newHeapIndex;
                            continue;
                        }
                    }
                }
                else // heapIndex == rootHeapIndex
                {
                    if (lowerHeapSize > 0)
                    {
                        int newHeapIndex = SwapWithChildren(heapIndex, heapIndex - 1, -1, false);
                        if (newHeapIndex != heapIndex)
                        {
                            heapIndex = newHeapIndex;
                            continue;
                        }
                    }

                    if (upperHeapSize > 0)
                    {
                        int newHeapIndex = SwapWithChildren(heapIndex, heapIndex + 1, -1, true);
                        if (newHeapIndex != heapIndex)
                        {
                            heapIndex = newHeapIndex;
                            continue;
                        }
                    }
                }

                break;
            }
        }

        public void Add(double value)
        {
            int elementIndex = totalElementCount % windowSize;

            int Insert(int heapIndex)
            {
                h[heapIndex] = value;
                heapToElementIndex[heapIndex] = elementIndex;
                elementToHeapIndex[elementIndex] = heapIndex;
                return heapIndex;
            }

            if (totalElementCount++ < windowSize) // Heap is not full
            {
                if (totalElementCount == 1) // First element
                {
                    Insert(rootHeapIndex);
                }
                else
                {
                    bool quantileApproximationCondition =
                        initStrategy == MovingQuantileEstimatorInitStrategy.QuantileApproximation &&
                        lowerHeapSize < k * totalElementCount / windowSize ||
                        initStrategy == MovingQuantileEstimatorInitStrategy.OrderStatistics;
                    if (lowerHeapSize < lowerHeapMaxSize && quantileApproximationCondition)
                    {
                        lowerHeapSize++;
                        int heapIndex = Insert(rootHeapIndex - lowerHeapSize);
                        Sift(heapIndex);
                    }
                    else
                    {
                        upperHeapSize++;
                        int heapIndex = Insert(rootHeapIndex + upperHeapSize);
                        Sift(heapIndex);
                    }
                }
            }
            else
            {
                // Replace old element
                int heapIndex = elementToHeapIndex[elementIndex];
                Insert(heapIndex);
                Sift(heapIndex);
            }
        }

        public double GetQuantile()
        {
            if (totalElementCount == 0)
                throw new EmptySequenceException();
            if (hyndmanYanType != null && !double.IsNaN(probability))
            {
                if (totalElementCount < windowSize)
                    throw new InvalidOperationException($"Sequence should contain at least {windowSize} elements");
                
                double GetValue(int index)
                {
                    index = (index - 1).Clamp(0, windowSize - 1); // Adapt one-based formula to the zero-based list
                    if (k - 1 <= index && index <= k + 1)
                        return h[rootHeapIndex + index - k];
                    throw new InvalidOperationException();
                }

                return HyndmanYanEquations.Evaluate(hyndmanYanType.Value, windowSize, probability, GetValue);
            }
            
            if (initStrategy == MovingQuantileEstimatorInitStrategy.OrderStatistics && k >= totalElementCount)
                throw new IndexOutOfRangeException($"Not enough values (n = {totalElementCount}, k = {k})");
            return h[rootHeapIndex];
        }

        [NotNull]
        internal string Dump()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < windowSize; i++)
            {
                if (i != 0)
                    builder.Append(", ");
                if (rootHeapIndex - lowerHeapSize <= i && i <= rootHeapIndex + upperHeapSize)
                    builder.Append(h[i].ToString(DefaultCultureInfo.Instance));
                else
                    builder.Append("*");
            }
            return builder.ToString();
        }
    }
}