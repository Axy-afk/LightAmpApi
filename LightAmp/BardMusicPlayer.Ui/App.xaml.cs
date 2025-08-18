/*
 * Copyright(c) 2025 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using BardMusicPlayer.Coffer;
using BardMusicPlayer.Jamboree;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Script;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Siren;
using BardMusicPlayer.XIVMIDI;
using Microsoft.Owin.Hosting;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace BardMusicPlayer.Ui
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public sealed partial class App : Application
    {
        public static string TempPath { get; } = System.IO.Path.GetTempPath() + "LightAmp\\";
        string baseAddress = "http://localhost:9001/";
        IDisposable webApp;
        protected override void OnStartup(StartupEventArgs e)
        {
            webApp = WebApp.Start<Startup>(baseAddress);
            SplashScreen splashScreen = new SplashScreen("/Resources/Images/splash.jpg");
            splashScreen.Show(true);

            Globals.Globals.DataPath = @"data\";

            //init pigeon at first
            BmpPigeonhole.Initialize(Globals.Globals.DataPath + @"\Configuration.json");

            // LogManager.Initialize(new(view.Log));

            //Load the last used catalog
            string CatalogFile = BmpPigeonhole.Instance.LastLoadedCatalog;
            if (System.IO.File.Exists(CatalogFile))
                BmpCoffer.Initialize(CatalogFile);
            else
                BmpCoffer.Initialize(Globals.Globals.DataPath + @"\MusicCatalog.db");

            //Setup seer
            BmpSeer.Instance.SetupFirewall("BardMusicPlayer");
            //Start meastro before seer, else we'll not get all the players
            BmpMaestro.Instance.Start();
            //Start seer
            BmpSeer.Instance.Start();

            DalamudBridge.DalamudBridge.Instance.Start();

            //Start the scripting
            BmpScript.Instance.Start();

            BmpSiren.Instance.Setup();
            XIVMIDI.XIVMIDI.Instance.Start();
            //BmpJamboree.Instance.Start();
            ConfigureLanguage(System.Threading.Thread.CurrentThread.CurrentUICulture.ToString());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            XIVMIDI.XIVMIDI.Instance.Stop();
            //LogManager.Shutdown();
            BmpJamboree.Instance.Stop();
            if (BmpSiren.Instance.IsReadyForPlayback)
                BmpSiren.Instance.Stop();
            BmpSiren.Instance.ShutDown();
            BmpMaestro.Instance.Stop();

            BmpScript.Instance.Stop();

            DalamudBridge.DalamudBridge.Instance.Stop();
            BmpSeer.Instance.Stop();
            BmpSeer.Instance.DestroyFirewall("BardMusicPlayer");
            BmpCoffer.Instance.Dispose();
            BmpPigeonhole.Instance.Dispose();

            //Wasabi hangs kill it with fire
            Process.GetCurrentProcess().Kill();
        }
        internal static void ConfigureLanguage(string langCode = null)
        {
            try
            {
                Locales.Language.Culture = new CultureInfo(langCode);
            }
            catch (Exception)
            {
                Locales.Language.Culture = CultureInfo.DefaultThreadCurrentUICulture;
            }
        }
    }
}
