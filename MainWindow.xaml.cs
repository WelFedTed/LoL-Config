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

		public MainWindow()
		{
			InitializeComponent();

			configFile = @"C:\Riot Games\League of Legends\Config\game.cfg";

			if (File.Exists(configFile))
			{
				// game.cfg exists
				try
				{
					// Backup config file
					File.Copy(configFile, configFile + ".bak", false);
					Debug.WriteLine("game.cfg backup created");
				}
				catch
				{
					// Backup already exists
					Debug.WriteLine("game.cfg backup already exists, did not overwrite");
				}
				// Read file contents
				configContents = File.ReadAllLines(configFile);
			}
			else
			{
				// game.cfg does not exist
				MessageBox.Show("This program only supports League of Legends being installed in the following directory:" + "\n" + @"C:\Riot Games\League of Legends\", "game.cfg not found");
				Environment.Exit(0);
			}




			/*	Settings Legend
			 *	----------------------------------------
			 *	WindowMode=0	Full Screen
			 *	WindowMode=1	Windowed
			 *	WindowMode=2	Borderless
			 *	----------------------------------------
			*/
		}

		private void updateDumpBox(string[] readText)	// Open League of Legends game.cfg file (C:\Riot Games\League of Legends\Config\game.cfg)
		{
			txt_dump.Text = "Updated game.cfg" +"\n" + "------------------------------------------------------------";
			foreach (string s in readText)
			{
				txt_dump.AppendText(s + "\n");
			}
		}


		private void updateComboBox(string s)	// Set combo box values to current settings if they exist
		{
			String[] setting = s.Split('=');
			//if (setting.Length > 1) Debug.WriteLine(setting[0] + "=" + setting[1]);

			if ("cbo_"+s == null)
			{
				Debug.WriteLine("cbo_" + s + " does not exist");
			}
		}

		private void btn_save_Click(object sender, RoutedEventArgs e)	// Save changes to config file
		{
			// Edit settings
			int i = 0;
			foreach (string s in configContents)
			{
				if (s.StartsWith("WindowMode"))
				{
					configContents[i] = "WindowMode=" + cbo_windowedMode.SelectedIndex;
				}
				i++;
			}

			// Save settings
			try
			{
				// Save successfully
				File.WriteAllLines(configFile, configContents);
				Debug.WriteLine("save successful");
			}
			catch
			{
				// Failed to save file
				Debug.WriteLine("save failed");
				MessageBox.Show("Failed to save changes to config file:" + "\n" + "Try disabling 'Read-only' on game.cfg and try again.");
			}

			// Show updated config file contents
			updateDumpBox(configContents);
		}

		private void cbo_windowedMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("WindowedMode=" + cbo_windowedMode.SelectedIndex);
		}
	}
}
