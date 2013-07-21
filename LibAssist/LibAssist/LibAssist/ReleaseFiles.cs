using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using Artist.Shakespeare;
using EnvDTE;

namespace LibAssist
{
	public interface OutputWindow
	{
		void Write(string str);
		void WriteLine(string str);
	};


	public class ReleaseFiles
	{
		// Key - Macro Name, Value - Value of replacement

		public ReleaseFiles()
		{
		}

		public static bool Install(ArrayList objProjectList, OutputWindow objOutput)
		{
			string strData = "";
			INIFile cINIFile = new INIFile();
			Hashtable objMacroTable = new Hashtable();
			Hashtable objCustomMacroType = new Hashtable();

			ArrayList objIncludeMatchList = new ArrayList();
			ArrayList objExcludeMatchList = new ArrayList();

			objOutput.Write("------ Install start ------\r\n");

			string strOldCurrentDirectory = Directory.GetCurrentDirectory();
			int nNumberOfCopied = 0;
			foreach (string strProjectPath in objProjectList)
			{
				Directory.SetCurrentDirectory(strProjectPath);

				cINIFile.Path = Path.Combine(strProjectPath, PROJECT_CONFIG_FILE);

				if (!File.Exists(cINIFile.Path)) {
					objOutput.Write( "Error : Can't not find \"" + cINIFile.Path + "\".\r\nPlease your project config file.\r\n" );
					continue;
				}

				ReadGlobalMacro(cINIFile, objMacroTable);
				ReadGlobalTypeInfo(cINIFile, objCustomMacroType);

				foreach (string strSection in cINIFile.Sections)
				{
					if (0 == StringComparer.CurrentCultureIgnoreCase.Compare(strSection, "GLOBAL"))
						continue;
					if (0 == StringComparer.CurrentCultureIgnoreCase.Compare(strSection, "GLOBALTYPE"))
						continue;

					objOutput.Write("------ Section \"" + strSection + "\" begin\r\n");

					bool bSourceRecursive = false;
					bool bDestinationRecursive = false;

					strData = cINIFile.ReadValue(strSection, "SourceDirectory", strProjectPath);
					string strSourceDirectory = ReplaceMacro(strData, objMacroTable, objCustomMacroType);

					strData = cINIFile.ReadValue(strSection, "DestinationDirectory", strProjectPath);
					string strDestinationDirectory = ReplaceMacro(strData, objMacroTable, objCustomMacroType);
					strData = cINIFile.ReadValue(strSection, "SourceRecursive", "no");
					if (0 == StringComparer.CurrentCultureIgnoreCase.Compare(strData, "yes"))
						bSourceRecursive = true;
					strData = cINIFile.ReadValue(strSection, "DestinationRecursive", "no");
					if (0 == StringComparer.CurrentCultureIgnoreCase.Compare(strData, "yes"))
						bDestinationRecursive = true;

					for (int nIndex = 1; ; ++nIndex)
					{
						strData = cINIFile.ReadValue(strSection, "FilesMatch" + nIndex.ToString(), "");
						if (strData == "")
							break;

						if( !SplitFileMatches( objIncludeMatchList, objExcludeMatchList, ReplaceMacro(strData, objMacroTable, objCustomMacroType) ) )
							objOutput.Write( "WARNING : Invalid match string \"" + strData + "\"\r\n" );
					}

					if( objIncludeMatchList.Count == 0 ) {
						objOutput.Write( "WARNING : Include match list is empty! Terminate this section.\r\n" );
					} else {
						nNumberOfCopied += CopyFiles(strSourceDirectory, strDestinationDirectory, bSourceRecursive, bDestinationRecursive, objIncludeMatchList, objExcludeMatchList, objOutput);
					}

					objIncludeMatchList.Clear();
					objExcludeMatchList.Clear();

					objOutput.Write( "------ End of \"" + strSection + "\" Section\n" );
				}

				objMacroTable.Clear();
			}

			Directory.SetCurrentDirectory(strOldCurrentDirectory);

			objOutput.Write("------ Install complete. " + nNumberOfCopied.ToString() + " files copied ------\r\n");

			return true;
		}

		protected static bool SplitFileMatches( ArrayList objIncludeList, ArrayList objExcludeList, string strMatchString )
		{
			string[] arrPatternToken = strMatchString.Split(':');
			ArrayList objListForAdd = null;

			if( arrPatternToken[0] == "E" || arrPatternToken[0] == "e" )
				objListForAdd = objExcludeList;
			else if( arrPatternToken[0] == "I" || arrPatternToken[0] == "i" )
				objListForAdd = objIncludeList;
			else
				return false;

			if( arrPatternToken.Length <= 1 )
				return true;

			for( int nPatternIndex = 1; nPatternIndex < arrPatternToken.Length; ++nPatternIndex )
				objListForAdd.Add( arrPatternToken[nPatternIndex] );

			return true;
		}

		public static bool Uninstall(ArrayList objProjectList, OutputWindow objOutput)
		{
			string strData = "";
			INIFile cINIFile = new INIFile();
			Hashtable objMacroTable = new Hashtable();
			Hashtable objCustomMacroType = new Hashtable();
			ArrayList objIncludeMatchList = new ArrayList();
			ArrayList objExcludeMatchList = new ArrayList();

			objOutput.Write("------ Uninstall start ------\r\n");

			int nNumberOfDeletedFiles = 0;

			foreach (string strProjectPath in objProjectList)
			{
				cINIFile.Path = Path.Combine(strProjectPath, PROJECT_CONFIG_FILE);

				if (!File.Exists(cINIFile.Path))
					continue;

				ReadGlobalMacro(cINIFile, objMacroTable);
				ReadGlobalTypeInfo(cINIFile, objCustomMacroType);

				foreach (string strSection in cINIFile.Sections)
				{
					if (0 == StringComparer.CurrentCultureIgnoreCase.Compare(strSection, "GLOBAL"))
						continue;
					if (0 == StringComparer.CurrentCultureIgnoreCase.Compare(strSection, "GLOBALTYPE"))
						continue;

					objOutput.Write("------ Section Start: " + strSection + "\r\n");

					bool bDeleteDestinationDirectory = false;

					strData = cINIFile.ReadValue(strSection, "DestinationDirectory", strProjectPath);
					string strDestinationDirectory = ReplaceMacro(strData, objMacroTable, objCustomMacroType);

					strData = cINIFile.ReadValue(strSection, "UninstallDestinationDirectory", "no");
					if (0 == StringComparer.CurrentCultureIgnoreCase.Compare(strData, "yes"))
						bDeleteDestinationDirectory = true;

					for (int nIndex = 1; ; ++nIndex)
					{
						strData = cINIFile.ReadValue(strSection, "UninstallFilesMatch" + nIndex.ToString(), "");
						if (strData == "")
							break;

						if( !SplitFileMatches( objIncludeMatchList, objExcludeMatchList, ReplaceMacro(strData, objMacroTable, objCustomMacroType) ) )
							objOutput.Write( "WARNING : Invalid match string \"" + strData + "\"\r\n" );
					}

					nNumberOfDeletedFiles += DeleteFiles(strDestinationDirectory, bDeleteDestinationDirectory, objIncludeMatchList, objExcludeMatchList, objOutput);

					objIncludeMatchList.Clear();
					objExcludeMatchList.Clear();
				}

				objMacroTable.Clear();
			}

			objOutput.Write("------ Uninstall complete. " + nNumberOfDeletedFiles.ToString() + " files deleted ------\r\n");

			return true;
		}

		private static int DeleteFiles(string strDestinationFolder, bool bDeleteDestinationDirectory, ArrayList objIncludeList, ArrayList objExcludeList, OutputWindow objOutput)
		{
			int nCount = 0;
			if (bDeleteDestinationDirectory)
			{
				if (Directory.Exists(strDestinationFolder))
				{
					try
					{
						nCount = Directory.GetFiles(strDestinationFolder, "*.*").Length;
						Directory.Delete(strDestinationFolder, true);
						objOutput.Write("Delete Directory : " + strDestinationFolder + "\r\n");
					}
					catch (Exception objExcept)
					{
						Trace.WriteLine(objExcept.Message);
					}
				}
			}
			else
			{
				if (Directory.Exists(strDestinationFolder))
				{
					foreach (string strFileName in Directory.GetFiles(strDestinationFolder, "*.*"))
					{
						if (strFileName == "." || strFileName == "..")
							continue;
						if (IsMatchedFile(strFileName, objIncludeList, objExcludeList, false))
						{
							try
							{
								File.Delete(strFileName);
								objOutput.Write(Path.GetFileName(strFileName) + "\r\n");
							}
							catch (Exception objExcept)
							{
								Trace.WriteLine(objExcept.Message);
							}
							++nCount;
						}
					}
				}
			}
			return nCount;
		}

		private static int CopyFiles(string strSourceFolder, string strDestinationFolder, bool bSourceRecursive, bool bDestinationRecursive, ArrayList objIncludeList, ArrayList objExcludeList, OutputWindow objOutput)
		{
			int nCount = 0;
			if( !Directory.Exists( strSourceFolder ) ) {
				objOutput.Write( "## Could not find a part of the path \"" + strSourceFolder + "\"\r\n" );
				objOutput.Write( "## Please check your configuration file.\r\n" );
				return 0;
			}

			foreach (string strFileName in Directory.GetFiles(strSourceFolder, "*.*", bSourceRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
			{
				if (strFileName == "." || strFileName == "..")
					continue;
				if (IsMatchedFile(strFileName, objIncludeList, objExcludeList, false))
				{
					string strDestPath = strDestinationFolder;
					if (bDestinationRecursive)
					{
						strDestPath += strFileName.Substring(strSourceFolder.Length, strFileName.Length - strSourceFolder.Length);
						string strDestDirectory = Path.GetDirectoryName(strDestPath);
						if (!Directory.Exists(strDestDirectory))
							Directory.CreateDirectory(strDestDirectory);
					}
					else
					{
						if (!Directory.Exists(strDestPath))
						{
							Directory.CreateDirectory(strDestPath);
						}

						strDestPath = Path.Combine(strDestPath, Path.GetFileName(strFileName));
					}

					try
					{
						File.Copy(strFileName, strDestPath, true);
						File.SetAttributes(strDestPath, File.GetAttributes(strDestPath) & ~(FileAttributes.ReadOnly));
						objOutput.Write(Path.GetFileName(strFileName) + "\r\n");
					}
					catch (Exception objExcept)
					{
						Trace.WriteLine(objExcept.Message);
					}

					nCount++;
				}
			}

			return nCount;
		}

		private static bool IsMatchedFile(string strFileName, ArrayList objIncludeList, ArrayList objExcludeList, bool bDefault)
		{
			foreach( string strExclude in objExcludeList ) {
				if( !Path.HasExtension( strExclude ) ) {
					string strSplitedFileName = strFileName;
					for( ; ; ) {
						string strPath = Path.GetDirectoryName( strSplitedFileName );
						int nIndex = strPath.IndexOf( '\\' );
						string strFolder;
						if( -1 != nIndex )
							strFolder = strPath.Substring( nIndex + 1 );
						else
							strFolder = strPath;

						if( strFolder == "" )
							break;

						if( 0 == StringComparer.CurrentCultureIgnoreCase.Compare( strFolder, strExclude ) )
							return false;

						strSplitedFileName = strPath;
					}
				} else if( PathUtility.MatchFileName( strExclude, strFileName, false ) )
					return false;
			}

			foreach( string strInclude in objIncludeList ) {
				if( PathUtility.MatchFileName( strInclude, strFileName, false ) )
					return true;
			}

			return bDefault;
		}

		private static string ReplaceMacro(string strFolder, Hashtable objMacroTable, Hashtable objMacroTypeTable)
		{
			StringBuilder objStringBuilder = new StringBuilder(strFolder, 1024);
			char[] arrTempMacroName = new char[256];
			int nStringLength = 0;

			for (int nIndex = 0; nIndex < objStringBuilder.Length; ++nIndex)
			{
				if (objStringBuilder[nIndex] == '$')
				{
					if (objStringBuilder[nIndex + 1] == '(')
					{
						Array.Clear(arrTempMacroName, 0, 256);

						for (nStringLength = nIndex + 1; nStringLength < objStringBuilder.Length && objStringBuilder[nStringLength] != ')'; ++nStringLength) ;
						if (nStringLength >= objStringBuilder.Length)
							break;

						nStringLength -= nIndex + 2; // exclude $, (
						objStringBuilder.CopyTo(nIndex + 2, arrTempMacroName, 0, nStringLength);
						string strName = new string(arrTempMacroName, 0, nStringLength);
						strName = strName.ToUpper();
						string strNewValue;
						if (objMacroTable[strName] != null)
						{
							strNewValue = (string)objMacroTable[strName];
							nStringLength += 3; // include $, (, )
							objStringBuilder.CopyTo(nIndex, arrTempMacroName, 0, nStringLength);
							strName = new string(arrTempMacroName, 0, nStringLength);
						}
						else if (objMacroTypeTable[strName] != null)
						{
							RegistryKey objDAKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\LibAssist", true);

							if (objDAKey == null)
								objDAKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\LibAssist");

							strNewValue = (string)objDAKey.GetValue(strName);

							if (strNewValue == null || strNewValue == "")
							{
								if ((string)objMacroTypeTable[strName] == "DIRECTORY")
								{
									FolderBrowserDialog objOpenFolderDlg = new FolderBrowserDialog();
									objOpenFolderDlg.ShowNewFolderButton = true;
									objOpenFolderDlg.Description = "Please select a library folder";
									objOpenFolderDlg.RootFolder = Environment.SpecialFolder.Desktop;

									if (objOpenFolderDlg.ShowDialog() == DialogResult.OK)
									{
										objDAKey.SetValue(strName, objOpenFolderDlg.SelectedPath);
										strNewValue = objOpenFolderDlg.SelectedPath;
									}
									else
									{
										MessageBox.Show("Can't continue process");
										break;
									}
								}
								else if ((string)objMacroTypeTable[strName] == "COLOR")
								{
									ColorDialog objColorDialog = new ColorDialog();
									objColorDialog.FullOpen = true;
									if (objColorDialog.ShowDialog() == DialogResult.OK)
									{
									}
									else
									{
										MessageBox.Show("Can't continue process");
										break;
									}
								}
							}

							nStringLength += 3; // include $, (, )
							objStringBuilder.CopyTo(nIndex, arrTempMacroName, 0, nStringLength);
							strName = new string(arrTempMacroName, 0, nStringLength);
						}
						else
							continue;

						objStringBuilder.Replace(strName, strNewValue, nIndex, nStringLength);
					}
				}
			}

			return objStringBuilder.ToString();
		}

		private static void ReadGlobalMacro(INIFile objINIFile, Hashtable objMacroMap)
		{
			string[] objGLOBALSections = objINIFile.ReadSection("GLOBAL");
			if (objGLOBALSections == null)
				return;

			foreach (string strGlobalMacro in objGLOBALSections)
			{
				if (strGlobalMacro == "")
					continue;
				string strKey, strValue;
				int nPad = 0;
				if (-1 != (nPad = strGlobalMacro.IndexOf('=')))
				{
					strKey = strGlobalMacro.Substring(0, nPad);
					strValue = strGlobalMacro.Substring(nPad + 1);
					objMacroMap[strKey.ToUpper()] = strValue;
				}
			}
		}

		private static void ReadGlobalTypeInfo(INIFile objINIFile, Hashtable objMacroMap)
		{
			string[] objGlobalTypeInfo = objINIFile.ReadSection("GLOBALTYPE");
			if (objGlobalTypeInfo == null)
				return;

			foreach (string strGlobalType in objGlobalTypeInfo)
			{
				if (strGlobalType == "")
					continue;
				string strKey, strValue;
				int nPad = 0;
				if (-1 != (nPad = strGlobalType.IndexOf('=')))
				{
					strKey = strGlobalType.Substring(0, nPad);
					strValue = strGlobalType.Substring(nPad + 1);
					objMacroMap[strKey.ToUpper()] = strValue;
				}
			}
		}

		const string PROJECT_CONFIG_FILE = "LibAssistConfig.INI";
	}
}