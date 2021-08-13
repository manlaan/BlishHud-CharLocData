using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MumbleLink_CSharp_GW2;
using MumbleLink_CSharp;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace CharLocDataStandalone
{
    public partial class Form1 : Form
    {
        bool running = false;
        private List<Locs> LocData = new List<Locs>();
        private GW2Link link = new GW2Link();
        Stopwatch stopWatch = new Stopwatch();
        Thread th;

        public Form1() {
            InitializeComponent();
            link = new GW2Link();
        }

        private void button1_Click(object sender, EventArgs e) {
            running = !running;
            button1.Text = (running ? "running" : "stopped");
            if (running) {
                th = new Thread(RunThread);
                th.Start();
            } else {
                th.Abort();
            }
        }
        public void RunThread(object parameter) {
            stopWatch.Start();
            while (true) {
                if (!running)
                    return;
                try {
                    MumbleLinkedMemory data = link.Read();//We got the default MumbleLink Struct, useful for position/rotation
                    GW2Context context = link.GetContext();//We're extracting the Context field from the default MumbleLink Struct for easy usage (contains : MapId, MapType, ShardId, Instance and Build)
                    GW2Link.Coordinates coord = link.GetCoordinates();//Get Coordinates converted to Meters, GW2 writes them in inches into the Mumble Link API

                    LocData.Add(
                        new Locs() {
                            LocX = (float)coord.X,
                            LocY = (float)coord.Y,
                            LocZ = (float)coord.Z,
                            Time = stopWatch.ElapsedMilliseconds
                        }
                        );
                    Thread.Sleep(10);
                }
                catch { }
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt"))) {
                foreach (Locs l in LocData)
                    outputFile.WriteLine(l.Log);
            }
            link.Dispose();
        }
    }
    public class Locs
    {
        public float LocX { get; set; }
        public float LocY { get; set; }
        public float LocZ { get; set; }
        public long Time { get; set; }
        public string Log {
            get { return this.Time.ToString() + "," + this.LocX + "," + this.LocY + "," + this.LocZ; }
        }
    }
}
