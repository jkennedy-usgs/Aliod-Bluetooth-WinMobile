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
using System.Globalization;

namespace Gravity
{
	/// <summary>
	/// This is the helper class to parse NMEA sentences.
	/// </summary>

	public class NmeaHelper
	{
        public int avgCount = 0;
        public int avgTime = 10;
        public bool ShowAvg = false;
        decimal[] GravData = new decimal[121];
        decimal avggrav = 0;

		public NmeaHelper()
		{
		}

    #region Event delegates and handlers

    #region Grav
    public delegate void GravEventHandler(object sender, GravEventArgs e);
    public event GravEventHandler Grav;
    protected virtual void OnGrav(GravEventArgs e) 
    {
      if (Grav != null) Grav(this, e);
    }
    #endregion


    #endregion

    #region Parse
    public void ParseSentence(string nmeaSentence)
    {
      // NMEA sentence must include talker and identifier, first must be '$', and max 82 chars
       
      if(nmeaSentence.Length <22) return;

      if(nmeaSentence.Length > 60) return;
      if (nmeaSentence[0] == '$')//String from D127
      {
          // Remove leading '$'
          string sentence = nmeaSentence.Substring(1);
          string identifier = sentence.Substring(0, 4);
          string[] fields = sentence.Substring(5).Split(',');
          grav(fields);

      }
      else                      //String from D209
      {
          string sentence = nmeaSentence;
          string[] fields = sentence.Substring(2).Split(',');
          grav(fields);
      }
    }
    #endregion

    #region GRAV - Depth below transducer
  
    private void grav(string[] fields)
    {
      // Get field values    

      decimal gravity = Convert.ToDecimal(fields[0]);
      decimal crossLevel = Convert.ToDecimal(fields[1]);
      decimal longLevel = Convert.ToDecimal(fields[2]);
      decimal volts = Convert.ToDecimal(fields[3]);
      decimal temp = Convert.ToDecimal(fields[4]);

      GravData[avgCount] = gravity;
      avgCount++;
        if(avgCount > avgTime - 1)
        {
            ShowAvg = true;
            avgCount = 0;
        }
        if (ShowAvg == true)
        {
            decimal GravSum = 0;

            foreach (decimal i in GravData)
                GravSum += i;
            avggrav = GravSum / avgTime;
        }
        else
            avggrav = 0;
    
      // Raise event
      OnGrav(new GravEventArgs(gravity,crossLevel,longLevel,volts,temp,avggrav));
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Checksum control.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private bool checksum(string s, string checksum)
    {
      int sum = 0;
      for(int i = 0; i < s.Length; i++)
        sum = sum ^ (int)(s[i]);
      return (checksum == string.Format("{0:X2}", sum));
    }

    ///summary>
    /// Converts fractional degrees to decimal degrees.
    /// </summary>
    /// <param name="factionalDegrees">Fractional degrees.</param>
    /// <returns>Decimal degrees.</returns>
    public static decimal fractionalToDecimalDegrees(decimal factionalDegrees) 
    {
      bool positve = factionalDegrees > 0;
      string factionalDegreesString = Math.Abs(factionalDegrees).ToString("00000.0000");
      int delimeterPosition = factionalDegreesString.IndexOf(
        CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

      // Get the fractional part of minutes
      decimal fractionalMinutes = Convert.ToDecimal("0" + factionalDegreesString.Substring(delimeterPosition));

      // Get the minutes
      decimal minutes = Convert.ToDecimal(factionalDegreesString.Substring(delimeterPosition - 2, 2));

      // Degrees
      decimal degrees= Convert.ToDecimal(factionalDegreesString.Substring(0, delimeterPosition - 2));

      decimal result = degrees + (minutes + fractionalMinutes) / 60;

      if(positve)
        return result;
      else
        return -result;
    }

    public static string formatFractionalDegrees(string factionalDegrees, string direction) 
    {
      string factionalDegreesString = Math.Abs(Convert.ToDecimal(factionalDegrees)).ToString("000.000");

      int delimeterPosition = factionalDegreesString.IndexOf(
        CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

      // Get minutes with fractions
      string minutes = factionalDegreesString.Substring(delimeterPosition - 2);

      // Degrees
      string degrees = factionalDegreesString.Substring(0, delimeterPosition - 2).Trim();

      return degrees + "° " + minutes + "' " + direction;
    }

    /// <summary>
    /// Convert from Celcius to Fahrenheit.
    /// </summary>
    /// <param name="celcius">Degress Celcius</param>
    /// <returns>Degress Fahrenheit</returns>
    public decimal celciusToFahrenheit(decimal celcius)
    {
      // °F = (°C * 9/5) + 32
      return (celcius * 9 / 5) + 32;
    }
    #endregion
	}

  #region Event argument classes

  #region GravEventArgs
  /// <summary>
  /// Event arguments for DBT - Depth below transducer
  /// </summary>
  public class GravEventArgs : EventArgs
  {
    /// <summary>
    /// Event arguments for DBT.
    /// </summary>
    /// <param name="depthFeet">Depth in feet.</param>
    /// <param name="depthMeters">Depth in meters.</param>
    /// <param name="depthFathoms">Depth in Fathoms.</param>
    public GravEventArgs(decimal gravity, decimal crossLevel, decimal longLevel, decimal volts, decimal temp, decimal avggrav)
    {
      this.gravity = gravity;
      this.crossLevel = crossLevel;
      this.longLevel = longLevel;
      this.volts = volts;
      this.temp = temp;
      this.avggrav = avggrav;
    } 
    private decimal gravity;
    public decimal Gravity
    {
      get { return gravity; }
    }
    private decimal crossLevel;
    public decimal CrossLevel
    {
      get { return crossLevel; }
    }
    private decimal longLevel;
    public decimal LongLevel
    {
      get { return longLevel; }
    }
    private decimal volts;
    public decimal Volts
    {
      get { return volts;}
    }
    private decimal temp;
    public decimal Temp
    {
    get {return temp;}
    }
    private decimal avggrav;
    public decimal AvgGrav
    {
        get { return avggrav; }
    }
  }
  #endregion

  #endregion
}
