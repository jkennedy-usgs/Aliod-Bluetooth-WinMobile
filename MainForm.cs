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
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading;
using System.Text;
using System.IO.Ports;

namespace Gravity
{
	/// <summary>
	/// This is the main form.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
    #region Private declarations
    private NmeaHelper nmea;
    private int currentStep = 1;
    public LogChart gravChart;
    public string gravity;
    public decimal Gravity;
    public string avgGrav;
    public decimal AvgGrav;
    public decimal[] GravData = new decimal[121];
    public int avgCount = 1;
    static SerialPort _serialPort;
    private bool logging = false;

    // Simulation
    private bool connected = false;
    private bool serialPort_open = false;
    private string testFileName = "d127.txt";
    private string logFileName;
    private bool simulationThreadActive = false;

    // Modes
    private int gravType = 0;
    #endregion

    #region Windows Form Designer generated declarations 
    private System.Windows.Forms.Label gravLabel;
    private System.Windows.Forms.Panel secondPanel;
    private System.Windows.Forms.Panel firstPanel;
    private System.Windows.Forms.PictureBox gravPictureBox;
    public System.Windows.Forms.Timer timer1;
    private MenuItem menuItem1;
    private RadioButton radOpen;
    private Label label2;
    private RadioButton radClosed;
    private Label avgLabel;
    private Label label6;
    private Label label5;
    private Label label4;
    private Label label3;
    private TextBox textBox1;
    private Button btnReset;
    private ContextMenu avgContextMenu1;
    private MenuItem menuItem2;
    private TextBox yMin;
    private TextBox yMax;
    private OpenFileDialog openFileDialog1;
    private TextBox txtLogFile;
    private RadioButton radLogOff;
    private RadioButton radLogOn;
    private Label label1;
    private Panel panel1;
    private Panel panel2;
    private Button btnOverwrite;
    private Panel msgFileexists;
    private Microsoft.WindowsCE.Forms.Notification notification1;
    private Button btnCancel;
    private Button btnAppend;
    private Label label7;
    private TextBox textBox2;
    private System.Windows.Forms.MainMenu mainMenu;
    #endregion

    #region Constructor
		public MainForm()
		{
			InitializeComponent();
    }
    #endregion

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.mainMenu = new System.Windows.Forms.MainMenu();
        this.menuItem1 = new System.Windows.Forms.MenuItem();
        this.gravLabel = new System.Windows.Forms.Label();
        this.secondPanel = new System.Windows.Forms.Panel();
        this.btnReset = new System.Windows.Forms.Button();
        this.label6 = new System.Windows.Forms.Label();
        this.label5 = new System.Windows.Forms.Label();
        this.avgLabel = new System.Windows.Forms.Label();
        this.avgContextMenu1 = new System.Windows.Forms.ContextMenu();
        this.menuItem2 = new System.Windows.Forms.MenuItem();
        this.yMax = new System.Windows.Forms.TextBox();
        this.yMin = new System.Windows.Forms.TextBox();
        this.gravPictureBox = new System.Windows.Forms.PictureBox();
        this.msgFileexists = new System.Windows.Forms.Panel();
        this.label7 = new System.Windows.Forms.Label();
        this.btnCancel = new System.Windows.Forms.Button();
        this.btnAppend = new System.Windows.Forms.Button();
        this.btnOverwrite = new System.Windows.Forms.Button();
        this.textBox2 = new System.Windows.Forms.TextBox();
        this.firstPanel = new System.Windows.Forms.Panel();
        this.txtLogFile = new System.Windows.Forms.TextBox();
        this.label4 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.panel1 = new System.Windows.Forms.Panel();
        this.radClosed = new System.Windows.Forms.RadioButton();
        this.label2 = new System.Windows.Forms.Label();
        this.radOpen = new System.Windows.Forms.RadioButton();
        this.panel2 = new System.Windows.Forms.Panel();
        this.radLogOn = new System.Windows.Forms.RadioButton();
        this.radLogOff = new System.Windows.Forms.RadioButton();
        this.label1 = new System.Windows.Forms.Label();
        this.timer1 = new System.Windows.Forms.Timer();
        this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
        this.notification1 = new Microsoft.WindowsCE.Forms.Notification();
        this.secondPanel.SuspendLayout();
        this.msgFileexists.SuspendLayout();
        this.firstPanel.SuspendLayout();
        this.panel1.SuspendLayout();
        this.panel2.SuspendLayout();
        this.SuspendLayout();
        // 
        // mainMenu
        // 
        this.mainMenu.MenuItems.Add(this.menuItem1);
        // 
        // menuItem1
        // 
        this.menuItem1.Text = "Plot";
        this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
        // 
        // gravLabel
        // 
        this.gravLabel.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
        this.gravLabel.Location = new System.Drawing.Point(97, 140);
        this.gravLabel.Name = "gravLabel";
        this.gravLabel.Size = new System.Drawing.Size(108, 24);
        this.gravLabel.Text = "<Gravity>";
        // 
        // secondPanel
        // 
        this.secondPanel.Controls.Add(this.btnReset);
        this.secondPanel.Controls.Add(this.label6);
        this.secondPanel.Controls.Add(this.label5);
        this.secondPanel.Controls.Add(this.avgLabel);
        this.secondPanel.Controls.Add(this.gravLabel);
        this.secondPanel.Controls.Add(this.yMax);
        this.secondPanel.Controls.Add(this.yMin);
        this.secondPanel.Controls.Add(this.gravPictureBox);
        this.secondPanel.Location = new System.Drawing.Point(248, 0);
        this.secondPanel.Name = "secondPanel";
        this.secondPanel.Size = new System.Drawing.Size(240, 269);
        this.secondPanel.Visible = false;
        // 
        // btnReset
        // 
        this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.btnReset.Location = new System.Drawing.Point(197, 176);
        this.btnReset.Name = "btnReset";
        this.btnReset.Size = new System.Drawing.Size(40, 20);
        this.btnReset.TabIndex = 5;
        this.btnReset.Text = " rst";
        this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
        // 
        // label6
        // 
        this.label6.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
        this.label6.Location = new System.Drawing.Point(-2, 173);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(93, 33);
        this.label6.Text = "Average";
        // 
        // label5
        // 
        this.label5.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
        this.label5.Location = new System.Drawing.Point(-2, 140);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(93, 30);
        this.label5.Text = "Current";
        // 
        // avgLabel
        // 
        this.avgLabel.ContextMenu = this.avgContextMenu1;
        this.avgLabel.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
        this.avgLabel.Location = new System.Drawing.Point(97, 173);
        this.avgLabel.Name = "avgLabel";
        this.avgLabel.Size = new System.Drawing.Size(108, 24);
        this.avgLabel.Text = "<Avg>";
        // 
        // avgContextMenu1
        // 
        this.avgContextMenu1.MenuItems.Add(this.menuItem2);
        // 
        // menuItem2
        // 
        this.menuItem2.Text = "Copy";
        this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
        // 
        // yMax
        // 
        this.yMax.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
        this.yMax.Location = new System.Drawing.Point(5, 9);
        this.yMax.Name = "yMax";
        this.yMax.Size = new System.Drawing.Size(24, 25);
        this.yMax.TabIndex = 10;
        this.yMax.Text = "2";
        this.yMax.TextChanged += new System.EventHandler(this.yMax_TextChanged);
        // 
        // yMin
        // 
        this.yMin.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
        this.yMin.Location = new System.Drawing.Point(6, 117);
        this.yMin.Name = "yMin";
        this.yMin.Size = new System.Drawing.Size(24, 25);
        this.yMin.TabIndex = 11;
        this.yMin.Text = "1";
        this.yMin.TextChanged += new System.EventHandler(this.yMin_TextChanged);
        // 
        // gravPictureBox
        // 
        this.gravPictureBox.Location = new System.Drawing.Point(3, 3);
        this.gravPictureBox.Name = "gravPictureBox";
        this.gravPictureBox.Size = new System.Drawing.Size(237, 134);
        // 
        // msgFileexists
        // 
        this.msgFileexists.Controls.Add(this.label7);
        this.msgFileexists.Controls.Add(this.btnCancel);
        this.msgFileexists.Controls.Add(this.btnAppend);
        this.msgFileexists.Controls.Add(this.btnOverwrite);
        this.msgFileexists.Controls.Add(this.textBox2);
        this.msgFileexists.Location = new System.Drawing.Point(7, 156);
        this.msgFileexists.Name = "msgFileexists";
        this.msgFileexists.Size = new System.Drawing.Size(230, 91);
        this.msgFileexists.Visible = false;
        // 
        // label7
        // 
        this.label7.Location = new System.Drawing.Point(10, 14);
        this.label7.Name = "label7";
        this.label7.Size = new System.Drawing.Size(116, 20);
        this.label7.Text = "Log file exists.";
        // 
        // btnCancel
        // 
        this.btnCancel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
        this.btnCancel.Location = new System.Drawing.Point(158, 47);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(65, 20);
        this.btnCancel.TabIndex = 19;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
        // 
        // btnAppend
        // 
        this.btnAppend.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
        this.btnAppend.Location = new System.Drawing.Point(10, 47);
        this.btnAppend.Name = "btnAppend";
        this.btnAppend.Size = new System.Drawing.Size(65, 20);
        this.btnAppend.TabIndex = 18;
        this.btnAppend.Text = "Append";
        this.btnAppend.Click += new System.EventHandler(this.btnAppend_Click);
        // 
        // btnOverwrite
        // 
        this.btnOverwrite.Font = new System.Drawing.Font("Tahoma", 7.5F, System.Drawing.FontStyle.Bold);
        this.btnOverwrite.Location = new System.Drawing.Point(81, 47);
        this.btnOverwrite.Name = "btnOverwrite";
        this.btnOverwrite.Size = new System.Drawing.Size(71, 20);
        this.btnOverwrite.TabIndex = 17;
        this.btnOverwrite.Text = "Overwrite";
        this.btnOverwrite.Click += new System.EventHandler(this.btnOverwrite_Click);
        // 
        // textBox2
        // 
        this.textBox2.Location = new System.Drawing.Point(0, 0);
        this.textBox2.Multiline = true;
        this.textBox2.Name = "textBox2";
        this.textBox2.Size = new System.Drawing.Size(227, 88);
        this.textBox2.TabIndex = 20;
        // 
        // firstPanel
        // 
        this.firstPanel.Controls.Add(this.msgFileexists);
        this.firstPanel.Controls.Add(this.txtLogFile);
        this.firstPanel.Controls.Add(this.label4);
        this.firstPanel.Controls.Add(this.label3);
        this.firstPanel.Controls.Add(this.textBox1);
        this.firstPanel.Controls.Add(this.panel1);
        this.firstPanel.Controls.Add(this.panel2);
        this.firstPanel.Location = new System.Drawing.Point(0, 0);
        this.firstPanel.Name = "firstPanel";
        this.firstPanel.Size = new System.Drawing.Size(240, 266);
        // 
        // txtLogFile
        // 
        this.txtLogFile.Location = new System.Drawing.Point(32, 180);
        this.txtLogFile.Name = "txtLogFile";
        this.txtLogFile.Size = new System.Drawing.Size(179, 25);
        this.txtLogFile.TabIndex = 10;
        // 
        // label4
        // 
        this.label4.Location = new System.Drawing.Point(20, 157);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(100, 20);
        this.label4.Text = "Log File";
        // 
        // label3
        // 
        this.label3.Location = new System.Drawing.Point(20, 50);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(178, 20);
        this.label3.Text = "Averaging Interval (sec.)";
        // 
        // textBox1
        // 
        this.textBox1.Location = new System.Drawing.Point(32, 78);
        this.textBox1.Name = "textBox1";
        this.textBox1.Size = new System.Drawing.Size(100, 25);
        this.textBox1.TabIndex = 6;
        this.textBox1.Text = "10";
        this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
        // 
        // panel1
        // 
        this.panel1.Controls.Add(this.radClosed);
        this.panel1.Controls.Add(this.label2);
        this.panel1.Controls.Add(this.radOpen);
        this.panel1.Location = new System.Drawing.Point(0, 6);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(237, 47);
        // 
        // radClosed
        // 
        this.radClosed.Checked = true;
        this.radClosed.Location = new System.Drawing.Point(90, 23);
        this.radClosed.Name = "radClosed";
        this.radClosed.Size = new System.Drawing.Size(100, 20);
        this.radClosed.TabIndex = 3;
        this.radClosed.Text = "Closed";
        this.radClosed.CheckedChanged += new System.EventHandler(this.radClosed_CheckedChanged);
        // 
        // label2
        // 
        this.label2.Location = new System.Drawing.Point(20, 0);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(100, 20);
        this.label2.Text = "Serial Port";
        // 
        // radOpen
        // 
        this.radOpen.Location = new System.Drawing.Point(14, 23);
        this.radOpen.Name = "radOpen";
        this.radOpen.Size = new System.Drawing.Size(100, 20);
        this.radOpen.TabIndex = 2;
        this.radOpen.TabStop = false;
        this.radOpen.Text = "Open";
        this.radOpen.CheckedChanged += new System.EventHandler(this.radOpen_CheckedChanged);
        // 
        // panel2
        // 
        this.panel2.Controls.Add(this.radLogOn);
        this.panel2.Controls.Add(this.radLogOff);
        this.panel2.Controls.Add(this.label1);
        this.panel2.Location = new System.Drawing.Point(0, 97);
        this.panel2.Name = "panel2";
        this.panel2.Size = new System.Drawing.Size(240, 57);
        // 
        // radLogOn
        // 
        this.radLogOn.Location = new System.Drawing.Point(11, 33);
        this.radLogOn.Name = "radLogOn";
        this.radLogOn.Size = new System.Drawing.Size(53, 20);
        this.radLogOn.TabIndex = 18;
        this.radLogOn.TabStop = false;
        this.radLogOn.Text = "On";
        this.radLogOn.CheckedChanged += new System.EventHandler(this.radLogOn_CheckedChanged);
        // 
        // radLogOff
        // 
        this.radLogOff.Checked = true;
        this.radLogOff.Location = new System.Drawing.Point(113, 33);
        this.radLogOff.Name = "radLogOff";
        this.radLogOff.Size = new System.Drawing.Size(100, 20);
        this.radLogOff.TabIndex = 19;
        this.radLogOff.Text = "Off";
        this.radLogOff.CheckedChanged += new System.EventHandler(this.radLogOff_CheckedChanged);
        // 
        // label1
        // 
        this.label1.Location = new System.Drawing.Point(20, 10);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(100, 20);
        this.label1.Text = "Logging";
        // 
        // openFileDialog1
        // 
        this.openFileDialog1.FileName = "openFileDialog1";
        // 
        // notification1
        // 
        this.notification1.Text = "notification1";
        // 
        // MainForm
        // 
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.ClientSize = new System.Drawing.Size(503, 269);
        this.Controls.Add(this.secondPanel);
        this.Controls.Add(this.firstPanel);
        this.KeyPreview = true;
        this.Menu = this.mainMenu;
        this.MinimizeBox = false;
        this.Name = "MainForm";
        this.Text = "USGS Gravity";
        this.Load += new System.EventHandler(this.MainForm_Load);
        this.secondPanel.ResumeLayout(false);
        this.msgFileexists.ResumeLayout(false);
        this.firstPanel.ResumeLayout(false);
        this.panel1.ResumeLayout(false);
        this.panel2.ResumeLayout(false);
        this.ResumeLayout(false);

    }
    #endregion

    #region Main
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main() 
		{
			Application.Run(new MainForm());
    }
    #endregion

    #region Load
    private void MainForm_Load(object sender, System.EventArgs e)
    {
        PowerNotifications pn = new PowerNotifications();
        pn.Start();
        pn.PowerStatusChanged += new PowerNotifications.PowerStatusChangeEventHandler(this.PowerNotifications_PowerStatusChanged);

        this.secondPanel.Left = 0;
        this.secondPanel.Top = 0;

      gravChart = new LogChart(gravPictureBox.Width, gravPictureBox.Height, 1, 2, 0.2, 90, 60);
      _serialPort = new SerialPort("COM8", 9600);
      if(connected)
      {
        _serialPort = new SerialPort("COM8", 9600);
        _serialPort.Handshake = Handshake.RequestToSend;
        _serialPort.ReceivedBytesThreshold = 1;
        _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
        //_serialPort.Open();
        //serialPort_open = true;
      }
      else
      {
        testFileName = 
          Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
          Path.DirectorySeparatorChar + testFileName;
        Thread simulationThread = new Thread(new ThreadStart(simulation));
        simulationThreadActive = true;
        simulationThread.Start();
      }

      // Set up the NMEA helper
      nmea = new NmeaHelper();
      nmea.Grav += new NmeaHelper.GravEventHandler(nmea_Grav);
      this.radLogOff.Checked = true;
        
    }
        

    #endregion

    #region Wizard buttons

    //#region Options
    //private void optionButton_Click(object sender, System.EventArgs e)
    //{
    //  switch (currentStep)
    //  {
    //    case 0: // Controls
    //      break;

    //    case 1: // Depth
    //      gravType++;
    //      if(gravType > 4)
    //        gravType = 0;
    //      gravChart.Dispose();
    //      gravChart = null;
    //      break;

    //    case 2: // Water Temperature
    //      fahrenheit = !fahrenheit;
    //      tempChart.Dispose();
    //      tempChart = null;
    //      break;

    //    case 3: // Speed
    //      knots = !knots;
    //      speedChart.Dispose();
    //      speedChart = null;
    //      break;

    //    case 4: // Wind
    //      relative = !relative;
    //      break;

    //    case 5: // Heading
    //      magnetic = !magnetic;
    //      break;
    //  }
    //}
    //#endregion

    //#region Back
    //private void backButton_Click(object sender, System.EventArgs e)
    //{
    //  switch (currentStep)
    //  {

    //    case 2:
    //      // Go to second step
    //      headingLabel.Visible = false;
    //      headingLabel.Text = "Gravity";
    //      firstPanel.Visible = true;
    //      secondPanel.Visible = false;
    //      currentStep = 1;
    //      break;

    //    case 1:
    //      // Go to first step
    //      headingLabel.Visible = true;
    //      headingLabel.Text = "Control";
    //      secondPanel.Visible = true;
    //      firstPanel.Visible = false;
    //      optionButton.Text = "Map";
    //      currentStep = 2;
    //      break;

    //  }
    //}
    //#endregion

    //#region Next
    //private void nextButton_Click(object sender, System.EventArgs e)
    //{
    //  switch (currentStep)       
    //  {         
    //    case 0:   
    //      // Go to second step
    //      headingLabel.Text = "Gravity";
    //      secondPanel.Visible = true;
    //      firstPanel.Visible = false;
    //      optionButton.Text = "Toggle";
    //      currentStep = 1;
    //      break;                  

    //    case 1:   
    //      // Go to third step
    //      headingLabel.Text = "Water Temperature";
    //      thirdPanel.Visible = true;
    //      secondPanel.Visible = false;
    //      currentStep = 2;
    //      break;                  

    //    case 2:   
    //      // Go to fourth step
    //      headingLabel.Text = "Speed";
    //      fourthPanel.Visible = true;
    //      thirdPanel.Visible = false;
    //      currentStep = 3;
    //      break;                  

    //    case 3:   
    //      // Go to fifth step
    //      headingLabel.Text = "Wind";
    //      fifthPanel.Visible = true;
    //      fourthPanel.Visible = false;
    //      currentStep = 4;
    //      break;                  

    //    case 4:   
    //      // Go to sixth step
    //      headingLabel.Text = "Heading";
    //      sixthPanel.Visible = true;
    //      fifthPanel.Visible = false;
    //      currentStep = 5;
    //      break;                  

    //    case 5:   
    //      // Go to first step
    //      headingLabel.Text = "Position";
    //      firstPanel.Visible = true;
    //      sixthPanel.Visible = false;
    //      optionButton.Text = "Map";
    //      currentStep = 0;
    //      break;                  
    //  }
    //}
    #endregion

    //#endregion

    #region Serial
    void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        string s = String.Empty;
        try
        {
            s = _serialPort.ReadLine();
            if(logging) log(s);
            nmea.ParseSentence(s);
        }
        catch
        {
            _serialPort.DiscardInBuffer();
        }
    }
    #endregion

    #region Private methods
    private string nmeaSentence;
    private void parseNmeaSentence(string s)
    {
      nmeaSentence = s; 
      this.Invoke(new EventHandler(parseNmeaSentenceEventHandler));
    }
    private void parseNmeaSentenceEventHandler(object sender, System.EventArgs e)
    {
      nmea.ParseSentence(nmeaSentence);
      nmeaSentence = string.Empty;
    }
    #endregion

    #region Simulation
    private void simulation()
    {
      StreamReader logFile = new StreamReader(testFileName);
      string s = logFile.ReadLine();
      while(simulationThreadActive)
      {
        //parseNmeaSentence(s);
          if(logging) log(s);
          nmea.ParseSentence(s);
        Thread.Sleep(1000);
        s = logFile.ReadLine();
        if(s == null)
        {
          logFile.Close();
          logFile = new StreamReader(testFileName);
          s = logFile.ReadLine();
        }
      }
      logFile.Close();
    }
    #endregion

    public void log(string datatowrite)
    {
        string Path = @logFileName;
        try
        {
            StreamWriter sw = File.AppendText(Path);
            string formattedData = datatowrite.Substring(2);
            sw.WriteLine(string.Concat(System.DateTime.Now.ToShortDateString(), ", ", System.DateTime.Now.ToLongTimeString(), ",", formattedData));
            sw.Close();
        }
        catch
        {
            MessageBox.Show("Unable to write data. Is log file open?");
            this.Invoke(new EventHandler(cancelLogging));            
        }
    }

    #region Nmea Events

    public void WorkerUpdate(object sender, EventArgs e)
    {
        this.gravLabel.Text = this.gravity;
        this.avgLabel.Text = this.avgGrav;
        this.gravChart.AddValue((double)this.Gravity, (double)this.AvgGrav);
        this.Update();
    }

    public void cancelLogging(object sender, EventArgs e)
    {
        radLogOn.Checked = false;
        radLogOff.Checked = true;
        logging = false;
    }

    public void msgDisplay(object sender, EventArgs e)
    {
        msgFileexists.Visible = true;
    }

    public void ChartUpdate(object sender, EventArgs e)
    {
        gravPictureBox.Image = this.gravChart.Paint();
        this.gravPictureBox.Update();
    }


    #region Grav - Depth below transducer
    private void nmea_Grav(object sender, GravEventArgs e)
    {
      switch(gravType)
      {
        case 0: // Feey
          this.gravity = string.Format("{0:0.000}", e.Gravity);
          this.Gravity = e.Gravity;
          this.AvgGrav = e.AvgGrav;
          this.avgGrav = string.Format("{0:0.000}", e.AvgGrav);
          this.gravLabel.Invoke(new EventHandler(WorkerUpdate));
          break;

        case 1: // Meters
          gravLabel.Text = string.Format("{0:0.0} m", e.CrossLevel);
          if(gravChart == null)
            gravChart = new LogChart(gravPictureBox.Width, gravPictureBox.Height, -50, 0, 10, 180, 60);
          //gravChart.AddValue(-(int)e.CrossLevel);
          break;

        case 2: // Fathoms
          gravLabel.Text = string.Format("{0:0.0} fm", e.LongLevel);
          if(gravChart == null)
            gravChart = new LogChart(gravPictureBox.Width, gravPictureBox.Height, -30, 0, 10, 180, 60);
          //gravChart.AddValue(-(int)e.LongLevel);
          break;
      }
      this.gravPictureBox.Invoke(new EventHandler(ChartUpdate));

      //gravPictureBox.Image = gravChart.Paint();
    }
    #endregion
    #endregion

    #region Closing
    private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if(connected)
      {
        if(_serialPort.IsOpen)
          _serialPort.Close();
      }
      else
        simulationThreadActive = false;
    }
    #endregion

    private void menuItem1_Click(object sender, EventArgs e)
    {
        switch (currentStep)
        {
            case 2:
                // Go to control page
                firstPanel.Visible = true;
                secondPanel.Visible = false;
                currentStep = 1;
                menuItem1.Text = "Plot";
                break;

            case 1:
                // Go to plot page
                secondPanel.Visible = true;
                firstPanel.Visible = false;
                currentStep = 2;
                menuItem1.Text = "Control";
                break;
        }
    }

    private void radioButton1_CheckedChanged(object sender, EventArgs e)
    {
        if (!serialPort_open)
        {
            _serialPort.Open();
            _serialPort.DiscardInBuffer();
            serialPort_open = true;
        }
    }

    private void radioButton2_CheckedChanged(object sender, EventArgs e)
    {
        if (_serialPort != null)
        {
            _serialPort.Close();
            serialPort_open = false;
        }
    }

    private void PowerNotifications_PowerStatusChanged(object sender, System.EventArgs e, string strPowerMessage)
    {
        if (strPowerMessage == "Power Off")
            _serialPort.Close();
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
        nmea.avgCount = 0;
        nmea.ShowAvg = false;
    }


    private void timer1_Tick(object sender, EventArgs e)
    {
    }

    private void avgContextMenu1_Popup(object sender, EventArgs e)
    {
    }

    private void yMin_GotFocus(object sender, EventArgs e)
    {
        yMin.SelectAll();
    }

    private void yMin_TextChanged(object sender, EventArgs e)
    {
        if (yMin.Text != "" && yMin.Text != "-" && yMin.Text != "-." && yMin.Text != "-0.")
        {
            try
            {
                gravChart.YMin = Convert.ToDouble(yMin.Text);
            }
            catch
            {
                MessageBox.Show("Invalid!");
            }
        }
    }

    private void yMax_TextChanged(object sender, EventArgs e)
    {
        if (yMax.Text != "" && yMax.Text != "-" && yMax.Text != "-." && yMax.Text != "-0.")
        {
            try
            {
                gravChart.YMax = Convert.ToDouble(yMax.Text);
            }
            catch
            {
                MessageBox.Show("Invalid!");
            }
        }
    }


    private void textBox1_TextChanged(object sender, EventArgs e)
    {
        if (textBox1.Text != "")
        {
            nmea.avgTime = Convert.ToInt16(textBox1.Text);
            nmea.ShowAvg = false;
            nmea.avgCount = 0;
        }
    }

    private void radLogOn_CheckedChanged(object sender, EventArgs e)
    {

        if (radLogOn.Checked == true)
        {
            if (txtLogFile.Text == "")
            {
                MessageBox.Show("Please enter a log file.");
                radLogOff.Checked = true;
                radLogOn.Checked = false;
                logging = false;
            }
            if (txtLogFile.Text.Length < 4)
            {
                MessageBox.Show("Filename must be four or more characters.");
                radLogOff.Checked = true;
                radLogOn.Checked = false;
                logging = false;
            }

            else
            {

                int stringlength = txtLogFile.Text.Length;
                if ((string)(txtLogFile.Text).Substring(stringlength - 4) != ".txt")
                {
                    logFileName = System.String.Concat(txtLogFile.Text, ".txt");
                }
                else
                {
                    logFileName = txtLogFile.Text;
                }
                string Path = @logFileName;
                if (File.Exists(Path))
                {
                    this.msgFileexists.Invoke(new EventHandler(msgDisplay));
                }
                else
                {
                    logging = true;
                }
            }
        }
        else
        {
            logging = false;
        }
            
    }
    private void radLogOff_CheckedChanged(object sender, EventArgs e)
    {
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        radLogOn.Checked = false;
        radLogOff.Checked = true;
        msgFileexists.Visible = false;
    }

    private void btnOverwrite_Click(object sender, EventArgs e)
    {
        string Path = @logFileName;
        if (File.Exists(Path))
        {
            File.Delete(Path);
        }
        else{
            MessageBox.Show("Error! File doesn't exist");
        }
        logging = true;
        msgFileexists.Visible = false;
    }

    private void btnAppend_Click(object sender, EventArgs e)
    {
        logging = true;
        msgFileexists.Visible = false;
    }


    private void radOpen_CheckedChanged(object sender, EventArgs e)
    {

    }

    private void radClosed_CheckedChanged(object sender, EventArgs e)
    {

    }

    private void menuItem2_Click(object sender, EventArgs e)
    {
        Clipboard.SetDataObject(avgLabel.Text);
    }

 }


}
