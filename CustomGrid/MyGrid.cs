using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
namespace CustomGrid
{
    class MyGrid:Panel
    {
        #region Properties


        public static int GetRow(DependencyObject obj)
        {
            return (int)obj.GetValue(RowProperty);
        }

        public static void SetRow(DependencyObject obj, int value)
        {
            obj.SetValue(RowProperty, value);
        }

        // Using a DependencyProperty as the backing store for Row.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached("Row", typeof(int), typeof(MyGrid), new UIPropertyMetadata(0));



        public static int GetColumn(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnProperty);
        }

        public static void SetColumn(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnProperty, value);
        }

        // Using a DependencyProperty as the backing store for Column.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached("Column", typeof(int), typeof(MyGrid), new UIPropertyMetadata(0));



        public static int GetRowSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(RowSpanProperty);
        }

        public static void SetRowSpan(DependencyObject obj, int value)
        {
            obj.SetValue(RowSpanProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(MyGrid), new UIPropertyMetadata(1));



        public static int GetColumnSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnSpanProperty);
        }

        public static void SetColumnSpan(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnSpanProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnSpanProperty =
            DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(MyGrid), new UIPropertyMetadata(1));



        public ObservableCollection<RowDefinition> RowDefinitions
        {
            get { return (ObservableCollection<RowDefinition>)GetValue(RowDefinitionsProperty); }
            set { SetValue(RowDefinitionsProperty, value); }
        }
  
        // Using a DependencyProperty as the backing store for RowDefinitions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowDefinitionsProperty =
            DependencyProperty.Register("RowDefinitions", typeof(ObservableCollection<RowDefinition>), typeof(MyGrid), new UIPropertyMetadata(new ObservableCollection<RowDefinition> { new RowDefinition(){ Height=GridLength.Auto}}));





        public ObservableCollection<ColumnDefinition> ColumnDefinitions
        {
            get { return (ObservableCollection<ColumnDefinition>)GetValue(ColumnDefinitionsProperty); }
            set { SetValue(ColumnDefinitionsProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ColumnDefinitions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.Register("ColumnDefinitions", typeof(ObservableCollection<ColumnDefinition>), typeof(MyGrid), new UIPropertyMetadata(new ObservableCollection<ColumnDefinition> { new ColumnDefinition(){Width=GridLength.Auto}}));

        
        #endregion

        #region Overrides
        int rowCount;
        int columnCount;
        Size[] columnSize;
        Size[] rowSize;
        double[] columnIncrement;
        double[] rowIncrement;
        
        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = new Size(0,0);
            Size myAvailableSize=new Size();
            rowCount = this.RowDefinitions.Count;
            columnCount = this.ColumnDefinitions.Count;
            columnSize = new Size[columnCount];
            rowSize = new Size[rowCount];
            Size[] myRowChildSize = new Size[rowCount];
            Size[] myColumnChildSize = new Size[columnCount];
            columnIncrement = new double[columnCount];
            rowIncrement = new double[rowCount];
            int rowNumber;
            int columnNumber;
            double rowStar=0;
            double columnStar=0;
            double[] infinityHeight = new double[rowCount];
            double[] infinityWidth = new double[columnCount];
            bool isInfinityHeightStar=false;
            bool isInfinityWidthStar = false; ;
            if (!Double.IsPositiveInfinity(availableSize.Height) && !Double.IsPositiveInfinity(availableSize.Width))
                myAvailableSize = availableSize;
            foreach (UIElement child in Children)
            {
                rowNumber = MyGrid.GetRow(child);
                columnNumber = MyGrid.GetColumn(child);
                child.Measure(availableSize);
                if (Double.IsPositiveInfinity(availableSize.Height))
                {
                    infinityHeight[rowNumber + 1] = Math.Max(child.DesiredSize.Height, infinityHeight[rowNumber + 1]);
                    isInfinityHeightStar = true;
                }
                else
                    myAvailableSize.Height = availableSize.Height;
                if (Double.IsPositiveInfinity(availableSize.Width))
                {
                    infinityWidth[columnNumber + 1] = Math.Max(child.DesiredSize.Width, infinityWidth[columnNumber + 1]);
                    isInfinityWidthStar = true;
                }
                else
                    myAvailableSize.Width = availableSize.Width;
                if (columnNumber >= columnCount-1)
                {
                    columnNumber = columnCount - 2;
                }
                if (rowNumber >= rowCount-1)
                {
                    rowNumber = rowCount - 2;
                }
                //if (RowDefinitions[rowNumber + 1].Height.IsAuto)
                //{
                    myRowChildSize[rowNumber+1].Height = Math.Max(myRowChildSize[rowNumber+1].Height, child.DesiredSize.Height);
                //}
                //if (ColumnDefinitions[columnNumber + 1].Width.IsAuto)
                //{
                    myColumnChildSize[columnNumber + 1].Width = Math.Max(myColumnChildSize[columnNumber + 1].Width, child.DesiredSize.Width);
                //}
            }
            if (Double.IsPositiveInfinity(availableSize.Width))
            {
                for (int i = 0; i < columnCount; i++)
                {
                    myAvailableSize.Width += infinityWidth[i];
                }
            }
            else if (Double.IsPositiveInfinity(availableSize.Height))
            {
                for (int j = 0; j < rowCount; j++)
                {
                    myAvailableSize.Height+=infinityHeight[j];
                }
            }
            for (int k = 1; k < rowCount; k++)
            {
                if (RowDefinitions[k].Height.IsStar)
                {
                    rowStar += RowDefinitions[k].Height.Value;
                }
                else if (RowDefinitions[k].Height.IsAuto)
                {
                    if(myAvailableSize.Height - myRowChildSize[k].Height>=0)
                    myAvailableSize.Height -= myRowChildSize[k].Height;
                }
                else if(RowDefinitions[k].Height.IsAbsolute)
                {
                    if(myAvailableSize.Height - RowDefinitions[k].Height.Value>=0)
                    myAvailableSize.Height -= RowDefinitions[k].Height.Value;
                }
            }
            for (int l = 1; l < columnCount; l++)
            {
                if (ColumnDefinitions[l].Width.IsStar)
                {
                    columnStar += ColumnDefinitions[l].Width.Value;
                }
                else if (ColumnDefinitions[l].Width.IsAuto)
                {
                    if(myAvailableSize.Width - myColumnChildSize[l].Width>=0)
                    myAvailableSize.Width -= myColumnChildSize[l].Width;
                }
                else if (ColumnDefinitions[l].Width.IsAbsolute)
                {
                    if(myAvailableSize.Width -ColumnDefinitions[l].Width.Value>=0)
                    myAvailableSize.Width -= ColumnDefinitions[l].Width.Value;
                }
            }
            for (int j = 0; j < rowCount; j++)
            {
                if (j > 0)
                    rowIncrement[j] = rowIncrement[j - 1];
                else
                    columnIncrement[j] = 0;
                if (rowCount > 1 && j>0)
                {
                    if (RowDefinitions[j].Height.IsAuto)
                    {
                            rowSize[j].Height = myRowChildSize[j].Height ;
                    }
                    else if (RowDefinitions[j].Height.IsStar)
                    {
                        if (!isInfinityHeightStar)
                        {
                            Double rowStarRatio = RowDefinitions[j].Height.Value / rowStar;
                            rowSize[j].Height = rowStarRatio * (myAvailableSize.Height);
                        }
                        else
                        {
                            rowSize[j].Height = myRowChildSize[j].Height;
                        }
                    }
                    else
                    {
                        rowSize[j].Height = RowDefinitions[j].Height.Value;
                    }
                }
                else if(rowCount==1)
                {
                    rowSize[0].Height = myAvailableSize.Height;
                    desiredSize.Height = rowSize[0].Height;
                }
                rowIncrement[j] += rowSize[j].Height;
                desiredSize.Height += rowSize[j].Height;
            }
            for (int i = 0; i < columnCount; i++)
            {
                if (i > 0)
                    columnIncrement[i] = columnIncrement[i - 1];
                else
                    columnIncrement[i] = 0;
                if (columnCount > 1 && i>0)
                {
                    if (ColumnDefinitions[i].Width.IsAuto)
                    {
                            columnSize[i].Width = myColumnChildSize[i].Width;
                    }
                    else if (ColumnDefinitions[i].Width.IsStar)
                    {
                        if (!isInfinityWidthStar)
                        {
                            Double columnStarRatio = ColumnDefinitions[i].Width.Value / columnStar;
                            columnSize[i].Width = columnStarRatio * (myAvailableSize.Width);
                        }
                        else
                        {
                            columnSize[i].Width = myColumnChildSize[i].Width;
                        }
                    }
                    else
                    {
                        columnSize[i].Width = ColumnDefinitions[i].Width.Value;
                    }
                    columnIncrement[i] += columnSize[i].Width;
                    desiredSize.Width += columnSize[i].Width;
                }
                else if(columnCount==1)
                {
                    columnSize[0].Width = myAvailableSize.Width;
                    desiredSize.Width = columnSize[0].Width;
                }
            }
            return desiredSize;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            int rowNumber;
            int columnNumber;
            int rowSpan;
            int columnSpan;
            foreach (UIElement child in Children)
            {
                rowNumber = MyGrid.GetRow(child);
                columnNumber = MyGrid.GetColumn(child);
                rowSpan = MyGrid.GetRowSpan(child);
                columnSpan = MyGrid.GetColumnSpan(child);
                if (((columnSpan + columnNumber)) >= columnCount)
                    columnSpan = 1;
                if (((rowSpan + rowNumber)) >= rowCount)
                    rowSpan = 1;
                if (columnCount != 1)
                {
                    if (columnNumber >= columnCount - 1)
                    {
                        columnNumber = columnCount - 2;
                    }
                }
                if (rowCount != 1)
                {
                    if (rowNumber >= rowCount - 1)
                    {
                        rowNumber = rowCount - 2;
                    }
                }
                if (rowSpan > 1 && columnSpan > 1)
                {
                    if (columnCount > 1 && rowCount > 1)
                        child.Arrange(new Rect(columnIncrement[columnNumber], rowIncrement[rowNumber], columnIncrement[columnSpan + columnNumber], rowIncrement[rowSpan + rowNumber]));
                    else if (rowCount == 1 && columnCount == 1)
                        child.Arrange(new Rect(columnNumber * columnSize[0].Width, rowNumber * rowSize[0].Height, columnSize[0].Width, rowSize[0].Height));
                    else if (rowCount == 1)
                        child.Arrange(new Rect(columnIncrement[columnNumber], rowNumber * rowSize[0].Height, columnIncrement[columnSpan + columnNumber], rowSize[0].Height));
                    else if (columnCount == 1)
                        child.Arrange(new Rect(columnNumber * columnSize[0].Width, rowIncrement[rowNumber], columnSize[0].Width, rowIncrement[rowSpan + rowNumber]));
                }
                else if (columnSpan > 1)
                {
                    if (columnCount > 1 && rowCount > 1)
                        child.Arrange(new Rect(columnIncrement[columnNumber], rowIncrement[rowNumber], columnIncrement[columnSpan+ columnNumber], rowSize[rowNumber + 1].Height));
                    else if (rowCount == 1 && columnCount == 1)
                        child.Arrange(new Rect(columnNumber * columnSize[0].Width, rowNumber * rowSize[0].Height, columnSize[0].Width, rowSize[0].Height));
                    else if (rowCount == 1)
                        child.Arrange(new Rect(columnIncrement[columnNumber], rowNumber * rowSize[0].Height, columnIncrement[columnSpan + columnNumber], rowSize[0].Height));
                    else if (columnCount == 1)
                        child.Arrange(new Rect(columnNumber * columnSize[0].Width, rowIncrement[rowNumber], columnSize[0].Width, rowSize[rowNumber + 1].Height));
                }
                else if(rowSpan>1)
                {
                    if (columnCount > 1 && rowCount > 1)
                        child.Arrange(new Rect(columnIncrement[columnNumber], rowIncrement[rowNumber], columnSize[columnNumber + 1].Width,rowIncrement[rowSpan+rowNumber]));
                    else if (rowCount == 1 && columnCount == 1)
                        child.Arrange(new Rect(columnNumber * columnSize[0].Width, rowNumber * rowSize[0].Height, columnSize[0].Width, rowSize[0].Height));
                    else if (rowCount == 1)
                        child.Arrange(new Rect(columnIncrement[columnNumber], rowNumber * rowSize[0].Height, columnSize[columnNumber + 1].Width, rowSize[0].Height));
                    else if (columnCount == 1)
                        child.Arrange(new Rect(columnNumber * columnSize[0].Width, rowIncrement[rowNumber], columnSize[0].Width, rowIncrement[rowSpan  + rowNumber]));
                }
                else
                {
                    if (columnCount > 1 && rowCount > 1)
                        child.Arrange(new Rect(columnIncrement[columnNumber], rowIncrement[rowNumber], columnSize[columnNumber + 1].Width, rowSize[rowNumber + 1].Height));
                    else if (rowCount == 1 && columnCount == 1)
                        child.Arrange(new Rect(columnNumber * columnSize[0].Width, rowNumber * rowSize[0].Height, columnSize[0].Width, rowSize[0].Height));
                    else if (rowCount == 1)
                        child.Arrange(new Rect(columnIncrement[columnNumber], rowNumber * rowSize[0].Height, columnSize[columnNumber + 1].Width, rowSize[0].Height));
                    else if (columnCount == 1)
                        child.Arrange(new Rect(columnNumber * columnSize[0].Width, rowIncrement[rowNumber], columnSize[0].Width, rowSize[rowNumber + 1].Height));
                }
                
            }
 
            return finalSize;
        }
        #endregion
    }
}
