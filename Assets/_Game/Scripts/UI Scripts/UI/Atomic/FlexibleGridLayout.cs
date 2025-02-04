using UnityEngine;
using UnityEngine.UI;

namespace UserInterfaceAmsterdam
{
    public class FlexibleGridLayout : LayoutGroup
    {
        private enum FitType
        {
            Uniform,
            FixedRows,
            FixedColumns
        }

        [SerializeField] private FitType fitType;
        [SerializeField] private int rows;
        [SerializeField] private int columns;
        [SerializeField] private Vector2 spacing;
        private Vector2 _cellSize;


        public override void CalculateLayoutInputVertical()
        {
            base.CalculateLayoutInputHorizontal();
            var childCount = rectTransform.childCount;

            if (fitType == FitType.Uniform)
            {
                float squareRoot = Mathf.Sqrt(rectTransform.childCount);
                rows = Mathf.CeilToInt(squareRoot);
                columns = Mathf.CeilToInt(squareRoot);
            }

            if (fitType == FitType.FixedColumns)
            {
                columns = Mathf.Clamp(columns, 1, childCount);
                rows = Mathf.CeilToInt(childCount / (float)columns);
            }

            if (fitType == FitType.FixedRows)
            {
                rows = Mathf.Clamp(rows, 1, childCount);
                columns = Mathf.CeilToInt(childCount / (float)rows);
            }

            var parentRect = rectTransform.rect;
            float parentWidth = parentRect.width;
            float parentHeight = parentRect.height;

            float cellWidth = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * (columns - 1)) - ((padding.left / (float)columns) + (padding.right / (float)columns));
            float cellHeight = (parentHeight / (float)rows) - ((spacing.y / (float)rows) * (rows - 1)) - ((padding.top / (float)rows) + (padding.bottom / (float)rows));

            _cellSize.x = cellWidth;
            _cellSize.y = cellHeight;

            int columnCount = 0;
            int rowCount = 0;

            for (int i = 0; i < rectTransform.childCount; i++)
            {
                columnCount = i % columns;
                rowCount = i / columns;
                var item = rectChildren[i];
                var xPos = (_cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                var yPos = (_cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;
                
                SetChildAlongAxis(item,0,xPos,_cellSize.x);
                SetChildAlongAxis(item,1,yPos,_cellSize.y);
            }
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
    }
}

