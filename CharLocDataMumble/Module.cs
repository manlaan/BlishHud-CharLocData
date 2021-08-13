using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Entities;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using MumbleLink_CSharp_GW2;
using MumbleLink_CSharp;

namespace CharLocDataMumble
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();
        private List<Locs> LocData = new List<Locs>();
        private GW2Link link = new GW2Link();

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }


        Stopwatch stopWatch = new Stopwatch();

        protected override void DefineSettings(SettingCollection settings) {

        }

        protected override void Initialize() {
            stopWatch.Start();
            link = new GW2Link();
        }

        protected override async Task LoadAsync() {

        }

        protected override void OnModuleLoaded(EventArgs e) {
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        protected override void Update(GameTime gameTime) {
            try {
                MumbleLinkedMemory data = link.Read();//We got the default MumbleLink Struct, useful for position/rotation
                GW2Context context = link.GetContext();//We're extracting the Context field from the default MumbleLink Struct for easy usage (contains : MapId, MapType, ShardId, Instance and Build)
                GW2Link.Coordinates coord = link.GetCoordinates();//Get Coordinates converted to Meters, GW2 writes them in inches into the Mumble Link API

                LocData.Add(
                    new Locs() {
                        Loc = new Vector3((float)coord.X, (float)coord.Y, (float)coord.Z),
                        Time = stopWatch.ElapsedMilliseconds
                    }
                    );
            }
            catch { }
        }

        /// <inheritdoc />
        protected override void Unload() {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt"))) {
                foreach (Locs l in LocData)
                    outputFile.WriteLine(l.Log);
            }
            link.Dispose(); //To free the resources
        }
    }
    public class Locs
    {
        public Vector3 Loc { get; set; }
        public long Time { get; set; }
        public string Log {
            get { return this.Time.ToString() + "," + this.Loc.X + "," + this.Loc.Y + "," + this.Loc.Z; }
        }
    }
}
