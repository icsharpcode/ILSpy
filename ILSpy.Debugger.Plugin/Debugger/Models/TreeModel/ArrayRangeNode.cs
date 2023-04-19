using Debugger;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal class ArrayRangeNode : TreeNode
    {
        public ArrayRangeNode(Expression arrayTarget, ArrayDimensions bounds, ArrayDimensions originalBounds)
        {
            this.arrayTarget = arrayTarget;
            this.bounds = bounds;
            this.originalBounds = originalBounds;
            base.Name = this.GetName();
            this.ChildNodes = this.LazyGetChildren();
        }

        private string GetName()
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = true;
            stringBuilder.Append("[");
            for (int i = 0; i < this.bounds.Count; i++)
            {
                if (!flag)
                {
                    stringBuilder.Append(", ");
                }
                flag = false;
                ArrayDimension arrayDimension = this.bounds[i];
                ArrayDimension obj = this.originalBounds[i];
                if (arrayDimension.Count == 0)
                {
                    throw new DebuggerException("Empty dimension");
                }
                if (arrayDimension.Count == 1)
                {
                    stringBuilder.Append(arrayDimension.LowerBound.ToString());
                }
                else if (arrayDimension.Equals(obj))
                {
                    stringBuilder.Append("*");
                }
                else
                {
                    stringBuilder.Append(arrayDimension.LowerBound);
                    stringBuilder.Append("..");
                    stringBuilder.Append(arrayDimension.UpperBound);
                }
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        private static string GetName(int[] indices)
        {
            StringBuilder stringBuilder = new StringBuilder(indices.Length * 4);
            stringBuilder.Append("[");
            bool flag = true;
            foreach (int num in indices)
            {
                if (!flag)
                {
                    stringBuilder.Append(", ");
                }
                stringBuilder.Append(num.ToString());
                flag = false;
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        private IEnumerable<TreeNode> LazyGetChildren()
        {
            if (this.bounds.TotalElementCount <= 100)
            {
                foreach (int[] indices in this.bounds.Indices)
                {
                    string imageName;
                    ImageSource image = ExpressionNode.GetImageForArrayIndexer(out imageName);
                    yield return new ExpressionNode(image, ArrayRangeNode.GetName(indices), this.arrayTarget.AppendIndexer(indices))
                    {
                        ImageName = imageName
                    };
                }
            }
            else
            {
                int splitDimensionIndex = this.bounds.Count - 1;
                for (int j = 0; j < this.bounds.Count; j++)
                {
                    if (this.bounds[j].Count > 1)
                    {
                        splitDimensionIndex = j;
                        break;
                    }
                }
                ArrayDimension splitDim = this.bounds[splitDimensionIndex];
                int elementsPerSegment = 1;
                while (splitDim.Count > elementsPerSegment * 100)
                {
                    elementsPerSegment *= 100;
                }
                for (int i = splitDim.LowerBound; i <= splitDim.UpperBound; i += elementsPerSegment)
                {
                    List<ArrayDimension> newDims = new List<ArrayDimension>(this.bounds);
                    newDims[splitDimensionIndex] = new ArrayDimension(i, Math.Min(i + elementsPerSegment - 1, splitDim.UpperBound));
                    yield return new ArrayRangeNode(this.arrayTarget, new ArrayDimensions(newDims), this.originalBounds);
                }
            }
            yield break;
        }

        private const int MaxElementCount = 100;

        private Expression arrayTarget;

        private ArrayDimensions bounds;

        private ArrayDimensions originalBounds;
    }
}
