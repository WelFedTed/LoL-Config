using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoL_Config
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public string configFile; // File path to game.cfg
		public string[] configContents; // Hold file contents		
		public string[] newSettings; // Hold new settings

		public MainWindow()
		{
			configFile = @"C:\Riot Games\League of Legends\Config\game.cfg";
			bool backup = false;

			if (File.Exists(configFile))
			{
				// game.cfg exists
				try
				{
					// Backup config file
					File.Copy(configFile, configFile + ".bak", false);
					Debug.WriteLine("game.cfg backup created");
					backup = true;
				}
				catch
				{
					// Backup already exists
					Debug.WriteLine("game.cfg backup already exists, did not overwrite");
					backup = false;
				}
				// Read file contents
				configContents = File.ReadAllLines(configFile);
				newSettings = configContents;
			}
			else
			{
				// game.cfg does not exist
				MessageBox.Show("This program only supports League of Legends being installed in the following directory:" + "\n" + @"C:\Riot Games\League of Legends\", "game.cfg not found");
				Environment.Exit(0);
			}

			InitializeComponent(); // Moved to after loading configContents to avoid null references from ui elements
			configContents = File.ReadAllLines(configFile); // re-read file contents to fix settings being overwritten by component initialization
			getResolutions();

			if (backup)
			{
				txt_dump.Text = "game.cfg backup created";
			}
			else
			{
				txt_dump.Text = "game.cfg backup already exists\ndid not overwrite";
			}

			loadSettings();

			/*	Settings Legend
			 *	----------------------------------------
			 *	WindowMode=0			Full Screen
			 *	WindowMode=1			Windowed
			 *	WindowMode=2			Borderless
			 *	CharacterQuality=0		Very Low
			 *	CharacterQuality=1		Low
			 *	CharacterQuality=2		Medium
			 *	CharacterQuality=3		High
			 *	CharacterQuality=4		Very High
			 *	ShadowQuality=0			Off
			 *	ShadowQuality=1			Low
			 *	ShadowQuality=2			Medium
			 *	ShadowQuality=3			High
			 *	ShadowQuality=4			Very High
			 *	FrameCapType=3			25 FPS
			 *	FrameCapType=4			30 FPS
			 *	FrameCapType=5			60 FPS
			 *	FrameCapType=6			80 FPS
			 *	FrameCapType=7			120 FPS
			 *	FrameCapType=8			144 FPS
			 *	FrameCapType=9			200 FPS
			 *	FrameCapType=2			240 FPS
			 *	FrameCapType=10			Uncapped
			 *	----------------------------------------
			*/
		}

		private void getResolutions()                                                                           // Create list of resolutions
		{
			int[,] resolutions = new int[,] // array of resolutions
			{
				{ 7680, 4320 }, // 16:9		4320p Wide
				{ 7680, 2160 }, // 32:9		2160p Super Ultra Wide
				{ 5120, 2880 }, // 16:9
				{ 5120, 2160 }, // 21:9		2160p Ultra Wide
				{ 5120, 1440 }, // 32:9		1440p Super Ultra Wide
				{ 3840, 2160 }, // 16:9		2160p Wide
				{ 3840, 1080 }, // 32:9		1080p Super Ultra Wide
				{ 3440, 1440 }, // 21:9		1440p Ultra Wide
				{ 2560, 1700 }, //  3:2
				{ 2560, 1600 }, // 16:10
				{ 2560, 1440 }, // 16:9		1440p Wide
				{ 2560, 1080 }, // 21:9		1080p Ultra Wide
				{ 2160, 1440 }, //  3:2
				{ 2048, 1536 }, //  4:3
				{ 1920, 1440 }, //  4:3		1440p Standard
				{ 1920, 1200 }, // 16:10
				{ 1920, 1080 }, // 16:9		1080p Wide
				{ 1856, 1392 }, //  4:3
				{ 1680, 1050 }, // 16:10
				{ 1600, 1200 }, //  4:3
				{ 1600, 1024 },
				{ 1600,  900 }, // 16:9
				{ 1440, 1080 }, //  4:3		1080p Standard
				{ 1440,  900 }, // 16:10
				{ 1400, 1050 }, //  4:3
				{ 1366,  768 }, // 16:9
				{ 1360,  768 },
				{ 1280, 1024 }, //  5:4
				{ 1280,  960 }, //  4:3
				{ 1280,  800 }, // 16:10
				{ 1280,  768 },
				{ 1280,  720 }, // 16:9		720p Wide
				{ 1152,  864 }, //  4:3
				{ 1152,  648 }, // 16:9
				{ 1024,  768 }, //  4:3
				{ 1024,  576 }, // 16:9
				{  960,  720 }, //  4:3		720p Standard
				{  854,  480 }, // 16:9		480p Wide
				{  800,  600 }, //  4:3
				{  720,  576 }, //  4:3
				{  640,  480 }  //  4:3		480p Standard
			};

			// assume current resolution is native resultion of display
			double currentWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
			double currentHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

			// create resolution combo box items
			Debug.WriteLine("----------------------");
			Debug.WriteLine("Supported Resolutions:");
			Debug.WriteLine("----------------------");
			bool inRange = false;
			for (int x = 0; x <= resolutions.GetUpperBound(0); x++)
			{
				ComboBoxItem item = new ComboBoxItem();
				if (resolutions[x, 0] == currentWidth && resolutions[x, 1] == currentHeight) // mark native resolution with an *
				{					
					item.Content = resolutions[x, 0] + " x " + resolutions[x, 1] + " *";
					inRange = true;
				}
				else
				{
					item.Content = resolutions[x, 0] + " x " + resolutions[x, 1];
				}
				if (!inRange) // make resolutions above native resolution light pink
				{
					item.Background = Brushes.LightPink;
				}
				cbo_resolution.Items.Add(item);
				Debug.WriteLine(item.Content);
			}
			Debug.WriteLine("----------------------");
		}

		private void loadSettings()																				// Update ui with current settings
		{
			// load settings
			int height = 0;
			int width = 0;
			foreach (string s in configContents)
			{
				string[] setting = s.Split('=');
				switch (setting[0])
				{
					case "Height":
						height = Convert.ToInt32(setting[1]); // store height
						break;
					case "Width":
						width = Convert.ToInt32(setting[1]); // store width
						int r = 0;
						foreach (ComboBoxItem item in cbo_resolution.Items)
						{
							Debug.WriteLine(width + " x " + height);
							if (item.Content.ToString().StartsWith(width + " x " + height))
							{
								Debug.WriteLine("---- found current res @ index " + r + "----");
								break;
							}
							r++;
						}
						cbo_resolution.SelectedIndex = r;
						break;
					case "ColorPalette":
						chk_colorblindMode.IsChecked = Convert.ToBoolean(Convert.ToInt32(setting[1]));
						break;
					case "HideEyeCandy":
						chk_hideEyeCandy.IsChecked = Convert.ToBoolean(Convert.ToInt32(setting[1]));
						break;
					case "WindowMode":
						cbo_windowedMode.SelectedIndex = Convert.ToInt32(setting[1]);
						break;
					case "RelativeTeamColors":
						chk_useRelativeTeamColors.IsChecked = Convert.ToBoolean(Convert.ToInt32(setting[1]));
						break;
					case "EnableScreenShake":
						chk_enableScreenShake.IsChecked = Convert.ToBoolean(Convert.ToInt32(setting[1]));
						break;
					case "CharacterQuality":
						cbo_characterQuality.SelectedIndex = Convert.ToInt32(setting[1]);
						break;
					case "EffectsQuality":
						cbo_effectsQuality.SelectedIndex = Convert.ToInt32(setting[1]);
						break;
					case "CharacterInking":
						chk_characterInking.IsChecked = Convert.ToBoolean(Convert.ToInt32(setting[1]));
						break;
					case "EnvironmentQuality":
						cbo_envrionmentQuality.SelectedIndex = Convert.ToInt32(setting[1]);
						break;
					case "ShadowQuality":
						cbo_shadows.SelectedIndex = Convert.ToInt32(setting[1]);
						break;
					case "FrameCapType":
						int i = Convert.ToInt32(setting[1]);
						switch (i)
						{
							case 3: // 25 FPS
								cbo_frameRateCap.SelectedIndex = 0;
								break;
							case 4: // 30 FPS
								cbo_frameRateCap.SelectedIndex = 1;
								break;
							case 5: // 60 FPS
								cbo_frameRateCap.SelectedIndex = 2;
								break;
							case 6: // 80 FPS
								cbo_frameRateCap.SelectedIndex = 3;
								break;
							case 7: // 120 FPS
								cbo_frameRateCap.SelectedIndex = 4;
								break;
							case 8: // 144 FPS
								cbo_frameRateCap.SelectedIndex = 5;
								break;
							case 9: // 200 FPS
								cbo_frameRateCap.SelectedIndex = 6;
								break;
							case 2: // 240 FPS
								cbo_frameRateCap.SelectedIndex = 7;
								break;
							case 10: // Uncapped
								cbo_frameRateCap.SelectedIndex = 8;
								break;
							default:
								break;
						}
						break;
					case "EnableFXAA":
						chk_antiAliasing.IsChecked = Convert.ToBoolean(Convert.ToInt32(setting[1]));
						break;
					case "WaitForVerticalSync":
						chk_waitForVerticalSync.IsChecked = Convert.ToBoolean(Convert.ToInt32(setting[1]));
						break;
					case "ColorLevel":
						sli_colorLevel.Value = Convert.ToInt32(Convert.ToSingle(setting[1]) * 100);
						break;
					case "ColorGamma":
						sli_colorGamma.Value = Convert.ToInt32(Convert.ToSingle(setting[1]) * 100);
						break;
					case "ColorBrightness":
						sli_colorBrightness.Value = Convert.ToInt32(Convert.ToSingle(setting[1]) * 100);
						break;
					case "ColorContrast":
						sli_colorContrast.Value = Convert.ToInt32(Convert.ToSingle(setting[1]) * 100);
						break;
					default:
						break;
				}
			}
		}

		private void updateDumpBox(string heading, string[] readText)											// Open League of Legends game.cfg file
		{
			// Dump output
			txt_dump.Text = heading + "\n" + "--------------------------------------------------";
			foreach (string s in readText)
			{
				txt_dump.AppendText(s + "\n");
			}
		}

		private void updateSettings(string s, int x)															// Update settings store
		{
			// Edit settings
			int i = 0;
			foreach (string l in newSettings)
			{
				if (l.StartsWith(s))
				{
					if (l.StartsWith("Color"))  // convert accessibility options to formatted float
					{
						float f = x;
						newSettings[i] = s + "=" + (f / 100).ToString("0.0000");
						Debug.WriteLine(s + "=" + (f / 100).ToString("0.0000"));
					}
					else // treat other settings as int
					{
						newSettings[i] = s + "=" + x;
						Debug.WriteLine(s + "=" + x);
					}
				}
				i++;
			}
		}

		private void btn_restoreBackup_Click(object sender, RoutedEventArgs e)									// Restore settings from backup file
		{
			// Restore backup config file
			File.Copy(configFile + ".bak", configFile, true);

			// Read file contents
			configContents = File.ReadAllLines(configFile);

			// Show updated config file contents
			updateDumpBox("game.cfg restored from backup", configContents);
		}

		private void btn_save_Click(object sender, RoutedEventArgs e)											// Save changes to config file
		{
			// Save settings
			try
			{
				// Save successfully
				File.WriteAllLines(configFile, newSettings);
				Debug.WriteLine("save successful");
			}
			catch
			{
				// Failed to save file
				Debug.WriteLine("save failed");
				MessageBox.Show("Failed to save changes to config file:" + "\n" + "Try disabling 'Read-only' on game.cfg and try again.");
			}

			// Read file contents
			configContents = File.ReadAllLines(configFile);

			// Show updated config file contents
			updateDumpBox("Updated game.cfg", configContents);
		}

		private void cbo_resolution_SelectionChanged(object sender, SelectionChangedEventArgs e)				// 'Resolution' changed
		{
			updateSettings("UserSetResolution", 1);
			string[] res = ((ComboBoxItem)cbo_resolution.SelectedItem).Content.ToString().Trim(' ', '*').Split('x');
			updateSettings("Width", Convert.ToInt32(res[0]));
			updateSettings("Height", Convert.ToInt32(res[1]));
		}

		private void chk_colorblindMode_Click(object sender, RoutedEventArgs e)									// 'Colorblind Mode' changed
		{
			updateSettings("ColorPalette", Convert.ToInt32(chk_colorblindMode.IsChecked));
		}

		private void chk_hideEyeCandy_Click(object sender, RoutedEventArgs e)									// 'Hide eye candy' changed
		{
			updateSettings("HideEyeCandy", Convert.ToInt32(chk_hideEyeCandy.IsChecked));
		}

		private void cbo_windowedMode_SelectionChanged(object sender, SelectionChangedEventArgs e)				// 'Window Mode' changed
		{
			updateSettings("WindowMode", cbo_windowedMode.SelectedIndex);
		}

		private void chk_useRelativeTeamColors_Click(object sender, RoutedEventArgs e)							// 'Use relative team colors' changed
		{
			updateSettings("RelativeTeamColors", Convert.ToInt32(chk_useRelativeTeamColors.IsChecked));
		}

		private void chk_enableScreenShake_Click(object sender, RoutedEventArgs e)								// 'Enable screen shake' changed
		{
			updateSettings("EnableScreenShake", Convert.ToInt32(chk_enableScreenShake.IsChecked));
		}

		private void cbo_characterQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)			// 'Character Quality' changed
		{
			updateSettings("CharacterQuality", cbo_characterQuality.SelectedIndex);
		}

		private void cbo_effectsQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)			// 'Effect Quality' change
		{
			updateSettings("EffectsQuality", cbo_effectsQuality.SelectedIndex);
		}

		private void chk_characterInking_Click(object sender, RoutedEventArgs e)								// 'Character Inking' changed
		{
			updateSettings("CharacterInking", Convert.ToInt32(chk_characterInking.IsChecked));
		}

		private void cbo_envrionmentQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)		// 'Environment Quality' change
		{
			updateSettings("EnvironmentQuality", cbo_envrionmentQuality.SelectedIndex);
		}

		private void cbo_shadows_SelectionChanged(object sender, SelectionChangedEventArgs e)					// 'Shadows' changed
		{
			updateSettings("ShadowsEnabled", 1);
			updateSettings("ShadowQuality", cbo_shadows.SelectedIndex);
		}

		private void cbo_frameRateCap_SelectionChanged(object sender, SelectionChangedEventArgs e)				// 'Frame Rate Cap' changed
		{
			switch (cbo_frameRateCap.SelectedIndex)
			{
				case 0: // 25 FPS
					updateSettings("FrameCapType", 3);
					break;
				case 1: // 30 FPS
					updateSettings("FrameCapType", 4);
					break;
				case 2: // 60 FPS
					updateSettings("FrameCapType", 5);
					break;
				case 3: // 80 FPS
					updateSettings("FrameCapType", 6);
					break;
				case 4: // 120 FPS
					updateSettings("FrameCapType", 7);
					break;
				case 5: // 144 FPS
					updateSettings("FrameCapType", 8);
					break;
				case 6: // 200 FPS
					updateSettings("FrameCapType", 9);
					break;
				case 7: // 240 FPS
					updateSettings("FrameCapType", 2);
					break;
				case 8: // Uncapped
					updateSettings("FrameCapType", 10);
					break;
				default:
					break;
			}
		}

		private void chk_antiAliasing_Click(object sender, RoutedEventArgs e)									// 'Anti-Aliasing' changed
		{
			updateSettings("EnableFXAA", Convert.ToInt32(chk_antiAliasing.IsChecked));
		}

		private void chk_waitForVerticalSync_Click(object sender, RoutedEventArgs e)							// 'Wait for Vertical Sync' changed
		{
			updateSettings("WaitForVerticalSync", Convert.ToInt32(chk_waitForVerticalSync.IsChecked));
		}

		private void sli_colorLevel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)		// 'Color Level' changed
		{
			updateSettings("ColorLevel", Convert.ToInt32(sli_colorLevel.Value));
		}

		private void sli_colorGamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)		// 'Color Gamma' changed
		{
			updateSettings("ColorGamma", Convert.ToInt32(sli_colorGamma.Value));
		}

		private void sli_colorBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)	// 'Color Brightness' changed
		{
			updateSettings("ColorBrightness", Convert.ToInt32(sli_colorBrightness.Value));
		}

		private void sli_colorContrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)	// 'Color Contrast' changed
		{
			updateSettings("ColorContrast", Convert.ToInt32(sli_colorContrast.Value));
		}
	}
}
