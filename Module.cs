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

namespace CharLocData
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();
        private List<Locs> LocData = new List<Locs>();

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
        }

        protected override async Task LoadAsync() {

        }

        protected override void OnModuleLoaded(EventArgs e) {
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        protected override void Update(GameTime gameTime) {
            try {
                LocData.Add(
                    new Locs() {
                        Loc = GameService.Gw2Mumble.PlayerCharacter.Position,
                        Cam = GameService.Gw2Mumble.PlayerCamera.Position,
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
        }
    }
    public class Locs
    {
        public Vector3 Loc { get; set; }
        public Vector3 Cam { get; set; }
        public long Time { get; set; }
        public string Log {
            get { return this.Time.ToString() + "," + this.Loc.X + "," + this.Loc.Y + "," + this.Loc.Z + "," + this.Cam.X + "," + this.Cam.Y + "," + this.Cam.Z; }
        }
    }
}
