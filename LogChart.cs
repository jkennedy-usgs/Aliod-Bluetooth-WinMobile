//------------------------------------------------------------------------------
/// <copyright from='1997' to='2005' company='Microsoft Corporation'>
///		Copyright (c) Microsoft Corporation. All Rights Reserved.
///
///   This source code is intended only as a supplement to Microsoft
///   Development Tools and/or on-line documentation.  See these other
///   materials for detailed information regarding Microsoft code samples.
/// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections;

namespace Gravity
{
	/// <summary>
	/// This is the LogChart class.
	/// </summary>
	public class LogChart : IDisposable
	{
    #region Private declarations
    private Bitmap bitmap;
    private Graphics graphics;
    private Pen forePen;
    private Pen valuesPen;
    private Pen valuesPen2;
    private SolidBrush foreBrush;
    private ArrayList values = new ArrayList();
    private DateTime lastValueTime;
    //private double yMin;
    //private double yMax;
    private double yTransform = 0;
    private float xFactor = 1;
    private double yFactor = 1;

    public class Point
    {
      public int X = 0;
      public double Y = 0;
      public double Y2 = 0;
      public Point(int X, double Y, double Y2)
      {
        this.X = X;
        this.Y = Y;
        this.Y2 = Y2;
      }
    }
    #endregion

    #region Properties
    private int width;
    public int Width
    {
      get { return width; }
      set { width = value; }
    }

    private double yMin;
    public double YMin
    {
        get { return yMin; }
        set { yMin = value; }
    }

    private double yMax;
    public double YMax
    {
        get { return yMax; }
        set { yMax = value; }
    }

    private int height;
    public int Height
    {
      get { return height; }
      set { height = value; }
    }

    private Color backColor = SystemColors.Window;
    public Color BackColor
    {
      get { return backColor; }
      set { backColor = value; }
    }

    private Color foreColor = SystemColors.WindowText;
    public Color ForeColor
    {
      get { return foreColor; }
      set { foreColor = value; }
    }

    private Color gridColor = SystemColors.Control;
    public Color GridColor
    {
      get { return gridColor; }
      set { gridColor = value; }
    }

    private Color valuesColor = SystemColors.Highlight;
    public Color ValuesColor
    {
      get { return valuesColor; }
      set { valuesColor = value; }
    }

    private Color valuesColor2 = SystemColors.HotTrack;
    public Color ValuesColor2
    {
        get { return valuesColor2; }
        set { valuesColor2 = value; }
    }

    private Font font = new Font("Tahoma", 8.25F, FontStyle.Regular);
    public Font Font
    {
      get { return font; }
      set { font = value; }
    }

    private Rectangle chartRectangle;
    public Rectangle ChartRectangle
    {
      get { return chartRectangle; }
      set { chartRectangle = value; }
    }

    private double step;
    public double Step
    {
      get { return step; }
      set { step = value; }
    }

    private int secondsWidth;
    public int SecondsWidth
    {
      get { return secondsWidth; }
      set { secondsWidth = value; }
    }

    private int stepWidth;
    public int StepWidth
    {
      get { return stepWidth; }
      set { stepWidth = value; }
    }
    #endregion

    #region Constructor
		public LogChart(int width, int height, double yMin, double yMax, double yStep, int xMax, int xStep)
		{
      this.width = width;
      this.height = height;
      this.yMin = yMin;
      this.yMax = yMax;
      this.step = yStep;
      this.secondsWidth = xMax;
      this.stepWidth = xStep;
      yTransform = -yMin;
      chartRectangle = new Rectangle(24, 6, width - 25, height - 12);
      xFactor = (float)chartRectangle.Width / secondsWidth;
      yFactor = chartRectangle.Height / (yMax - yMin);
      forePen = new Pen(foreColor);
      foreBrush = new SolidBrush(foreColor);
      bitmap = new Bitmap(width, height);
      graphics = Graphics.FromImage(bitmap);
    }
    #endregion

    #region Add values
    public void AddValue(double val1, double val2)
    {
      if(values.Count > 0)
      {
        //int secondsPassed = ((TimeSpan)DateTime.Now.Subtract(lastValueTime)).Seconds;
        TimeSpan ts = DateTime.Now - lastValueTime;
        int secondsPassed = ts.Seconds;
        if(secondsPassed < 0)
          secondsPassed += 60;
        for(int i = 0; i < values.Count; i++)
        {
          Point p = (Point)values[i];
          
          p.X += secondsPassed;
          if(p.X > secondsWidth)
          {
            if(((Point)values[i-1]).X < secondsWidth)
            {
              p.X = secondsWidth;
              p.Y = ((Point)values[i-1]).Y + ((int)((int)((float)(((Point)values[i-1]).Y - p.Y)) *
                ((float)(secondsWidth - ((Point)values[i-1]).X) / (p.X - ((Point)values[i-1]).X))));
              p.Y2 = ((Point)values[i - 1]).Y2 + ((int)((int)((float)(((Point)values[i - 1]).Y2 - p.Y2)) *
                ((float)(secondsWidth - ((Point)values[i - 1]).X) / (p.X - ((Point)values[i - 1]).X))));
            }
            else
              values.Remove(p);
          }
        }
        values.Insert(0, new Point(0, val1, val2));
      }
      else
        values.Add(new Point(0, val1, val2));
      lastValueTime = DateTime.Now;
    }
    #endregion

    #region Paint
    public Bitmap Paint()
    {
      // Draw chart
      graphics.Clear(backColor);
      //for(decimal i = yMin; i <= yMax; i += step / 2)
      //{
      //  graphics.DrawLine(new Pen(gridColor), chartRectangle.X - 3, Convert.ToInt32(chartRectangle.Y + chartRectangle.Height - (i + yTransform) * yFactor),
      //    chartRectangle.X + chartRectangle.Width, Convert.ToInt32(chartRectangle.Y + chartRectangle.Height - (i + yTransform) * yFactor));
      //  if(((i - yMin + step / 2) / step) != (((float)i - yMin + step / 2) / step))
      //  {
      //    string s = Math.Abs(i).ToString();
      //    graphics.DrawString(s, font, foreBrush, 20 - graphics.MeasureString(s, font).Width,
      //      chartRectangle.Y + chartRectangle.Height - 7 - (i + yTransform) * yFactor);
      //  }
      //}
      for(int i = 0; i <= secondsWidth; i += stepWidth)
      {
        graphics.DrawLine(new Pen(gridColor), Convert.ToInt32(chartRectangle.X + chartRectangle.Width - i * xFactor),
          chartRectangle.Y,
          Convert.ToInt32(chartRectangle.X + chartRectangle.Width - i * xFactor),
          chartRectangle.Y + chartRectangle.Height + 3);
      }
      graphics.DrawRectangle(forePen, chartRectangle);
      yTransform = -yMin;
        if(yMax == yMin)
        {
            yMax += 0.01;
        }
      yFactor = chartRectangle.Height / (yMax - yMin);

      // Draw values
      valuesPen = new Pen(valuesColor);
      valuesPen2 = new Pen(valuesColor2);
      for(int i = 0; i < values.Count - 1; i++)
      {
        Point p1 = (Point)values[i];
        Point p2 = (Point)values[i+1];
        graphics.DrawLine(valuesPen, Convert.ToInt32(chartRectangle.X + chartRectangle.Width - p1.X * xFactor),
          Convert.ToInt32(chartRectangle.Y + chartRectangle.Height - (p1.Y + yTransform) * yFactor),
          Convert.ToInt32(chartRectangle.X + chartRectangle.Width - p2.X * xFactor),
          Convert.ToInt32(chartRectangle.Y + chartRectangle.Height - (p2.Y + yTransform) * yFactor));
        graphics.DrawLine(valuesPen2, Convert.ToInt32(chartRectangle.X + chartRectangle.Width - p1.X * xFactor),
          Convert.ToInt32(chartRectangle.Y + chartRectangle.Height - (p1.Y2 + yTransform) * yFactor),
          Convert.ToInt32(chartRectangle.X + chartRectangle.Width - p2.X * xFactor),
          Convert.ToInt32(chartRectangle.Y + chartRectangle.Height - (p2.Y2 + yTransform) * yFactor));
      }
      return bitmap;
    }
    #endregion

    #region IDisposable Members
    protected void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Code to clean up managed resources
        graphics.Dispose();
        bitmap.Dispose();
      }
      // Code to clean up unmanaged resources
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    #endregion

    #region Destructor
    ~LogChart()
    {
      Dispose(false);
    }
    #endregion
  }
}
