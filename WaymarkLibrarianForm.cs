﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WaymarkLibrarian
{
	public partial class WaymarkLibrarianForm : Form
	{
		public WaymarkLibrarianForm()
		{
			//	***** TODO LIST: *****
			//		Make import button bring up a context menu with both import and empty new options (work in pp format import too).
			//		Make copy to game bring up a context menu of which slot to use.
			//		Restrict date dropdown to not allow before 5.2 patch.  Have date in zone dictionary with release date for each zone?
			//		Add a button for default character setting?
			//		Add expected file size to game data settings and check that before doing anything.  Refuse to load if it doesn't match the expected size.

			//	WinForms Stuff
			InitializeComponent();

			//	Get config settings.
			mSettings = new Config();

			//	Set up the object that handles interfacing with the game save files.
			mGameDataHandler = new GameDataHandler( mSettings.GameDataSettings );

			//	Load zone ID dictionary data.

			//	Load waymark library.

			//	Initialize the game settings folder, character list, and game preset list.
			if( Directory.Exists( mSettings.ProgramSettings.CharacterDataFolderPath ) )
			{
				CharacterDataFolderTextBox.Text = mSettings.ProgramSettings.CharacterDataFolderPath;
				PopulateCharacterListDropdown();
				if( ( mSettings.ProgramSettings.DefaultCharacterID.Length > 0 ) && File.Exists( mSettings.ProgramSettings.CharacterDataFolderPath + '\\' + mSettings.ProgramSettings.DefaultCharacterID + '\\' + mSettings.GameDataSettings.WaymarkDataFileName ) )
				{
					for( int i = 0; i < CharacterListDropdown.Items.Count; ++i )
					{
						if( mCharacterFolderList[i].Split( '\\' ).Last() == mSettings.ProgramSettings.DefaultCharacterID )
						{
							CharacterListDropdown.SelectedIndex = i;
							PopulateGamePresetListBox();
							break;
						}
					}
				}
			}
		}

		private void GamePresetListBox_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( mGamePresetContainer != null )
			{
				if( GamePresetListBox.SelectedIndex < 0 )
				{
					SelectedPresetInfoBox.Text = "";
				}
				else
				{
					SelectedPresetInfoBox.Text = mGamePresetContainer[(uint)GamePresetListBox.SelectedIndex].GetPresetDataString();
				}
			}
		}

		private WaymarkPresets mGamePresetContainer;
		private GameDataHandler mGameDataHandler;
		private string[] mCharacterFolderList;
		private Config mSettings;

		private void PopulateCharacterListDropdown()
		{
			CharacterListDropdown.Items.Clear();
			PopulateGamePresetListBox( true );
			mCharacterFolderList = Directory.GetDirectories( mSettings.ProgramSettings.CharacterDataFolderPath, "FFXIV_CHR*", SearchOption.TopDirectoryOnly );
			foreach( string dir in mCharacterFolderList )
			{
				CharacterListDropdown.Items.Add( mSettings.CharacterAliasSettings.GetAlias( dir.Split('\\').Last() ) );
			}
		}
		private void PopulateGamePresetListBox( bool clear = false )
		{
			GamePresetListBox.SelectedIndex = -1;
			GamePresetListBox.Items.Clear();

			if( !clear )
			{
				mGamePresetContainer = mGameDataHandler.ReadGameData( mCharacterFolderList[CharacterListDropdown.SelectedIndex] + '\\' + mSettings.GameDataSettings.WaymarkDataFileName );
				if( mGamePresetContainer != null )
				{
					for( uint i = 0u; i < mGamePresetContainer.Presets.Length; ++i )
					{
						GamePresetListBox.Items.Add( "Slot " + ( i + 1 ).ToString() + ": " + ( mGamePresetContainer[i].ZoneID == 0u ? "Empty" : ( "Zone " + mGamePresetContainer[i].ZoneID.ToString() ) ) );
					}
				}
			}
		}

		private void CharacterFolderBrowseButton_Click( object sender, EventArgs e )
		{
			CharacterDataFolderDialog.ShowDialog();
			if( ( CharacterDataFolderDialog.SelectedPath != null ) && Directory.Exists( CharacterDataFolderDialog.SelectedPath ) )
			{
				mSettings.ProgramSettings.CharacterDataFolderPath = CharacterDataFolderDialog.SelectedPath;
				CharacterDataFolderTextBox.Text = mSettings.ProgramSettings.CharacterDataFolderPath;
				PopulateCharacterListDropdown();
			}
		}

		private void CharacterListDropdown_SelectionChangeCommitted( object sender, EventArgs e )
		{
			PopulateGamePresetListBox( CharacterListDropdown.SelectedIndex < 0 );
		}
	}
}