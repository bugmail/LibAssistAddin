/**
 * LibAssist Connect class
 * TODO : Insert file header utility
 */
using System;
using System.Collections;
using System.IO;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.CommandBars;
using Artist.Shakespeare;
using System.Text;
using System.Windows.Forms;

namespace LibAssist
{
	public enum ECommands
	{
		eInstallProject = 0,
		eUninstallProject,
		eSwapFiles,
		eOpenFolder,
		eInsertClassName//,
		//eInsertFileHeader
	};

	public enum ETypeOfCustom
	{
		eColor = 0,
		ePath,
		eString
	};

	public class COutputWindowToDTEWindow : OutputWindow
	{
		public COutputWindowToDTEWindow(OutputWindowPane objOutputPane)
		{
			m_objOutput = objOutputPane;
		}

		public void Write(string str)		{ m_objOutput.OutputString(str); }
		public void WriteLine(string str)	{ m_objOutput.OutputString(str + "\r\n"); }

		private OutputWindowPane	m_objOutput;
	};

	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		private DTE2		m_objApplication;	// Instance of Visual studio
		private AddIn		m_objAddIn;		// Instance of Add-in(me)
		System.Threading.Thread m_objThread = null;
		private object		m_objLock = new object();
		static public Configuration Configuration;

		private static string[] StrProgIDs = new string[] {
            "Undeploy",
			"Deploy",
			"SwapFiles",
			"OpenExplorer",
			"InsertClassName"
		};

        private static string [] StrParentWindow = new string[] {
            "Project",
            "Project",
            "Code Window",
            "Code Window",
            "Code Window"
        };

		private const string StrSwapFiles = ".h;.cpp;.inl;.inc";

		private const string StrThreadName = "LibAssist thread";

		private const string StrCommandPrefix = "LibAssist.Connect.";

		private string GetTooltipText(string strCommand)
		{
			return "Executes the " + strCommand + " command for Library Assistant";
		}

		private string GetCommand(ECommands eCommand)
		{
			return StrCommandPrefix + StrProgIDs[(int)eCommand];
		}

		public Connect()
		{
			Configuration = new Configuration();
			Configuration.Initialize();
		}

		// IDTExtensibility2
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			m_objApplication	= (DTE2)application;
			m_objAddIn		= (AddIn)addInInst;

			Debug.WriteLine("LIBASSIST : Init OnConnection");

			if (connectMode == ext_ConnectMode.ext_cm_Startup || connectMode == ext_ConnectMode.ext_cm_AfterStartup)
			{
                
                LibAssist.Culture = new System.Globalization.CultureInfo( m_objApplication.LocaleID );

                for(int nProg = 0; nProg < StrProgIDs.Length; ++nProg)
                {
                    string ProgID = StrProgIDs[nProg];
                    AddinUtility.sAddCommand(m_objApplication, m_objAddIn, StrParentWindow[nProg], ProgID, LibAssist.ResourceManager.GetString(ProgID, LibAssist.Culture), GetTooltipText(ProgID), (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled);
                }

				//AddinUtility.sAddToMenu(m_objApplication, m_objAddIn, "Tools", "LibAssist", "LibAssist", "Library Assistant");
			}
		}

		
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
			Debug.WriteLine("LIBASSIST : Init OnDisconnection");
			if (disconnectMode == ext_DisconnectMode.ext_dm_HostShutdown)
			{
				AddinUtility.sDeleteCommand(m_objApplication, GetCommand(ECommands.eUninstallProject));
				AddinUtility.sDeleteCommand(m_objApplication, GetCommand(ECommands.eInstallProject));
				AddinUtility.sDeleteCommand(m_objApplication, GetCommand(ECommands.eOpenFolder));
				AddinUtility.sDeleteCommand(m_objApplication, GetCommand(ECommands.eSwapFiles));
				AddinUtility.sDeleteCommand(m_objApplication, GetCommand(ECommands.eInsertClassName));
			}

			Configuration.Clear();
		}

		public void OnAddInsUpdate(ref Array custom)
		{
		}


		public void OnStartupComplete(ref Array custom)
		{
		}


		public void OnBeginShutdown(ref Array custom)
		{
		}


		// IDTCommandTarget
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone) {
				if( "LibAssist.Connect.LibAssist" == commandName ) {
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					return;
				}

				foreach (string strProgID in StrProgIDs) {
					if (commandName == StrCommandPrefix + strProgID) {
						status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
						return;
					}
				}
			}
		}

		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if (m_objThread != null) {
				if (m_objThread.IsAlive)
					return;
			}

			ThreadStart objEntryPoint = null;
			if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault) {
				if (commandName == "LibAssist.Connect.LibAssist") {
					handled = true;
					return;
					//objEntryPoint = new ThreadStart(RunLibAssist);
				} else if (commandName == GetCommand(ECommands.eInstallProject)) {
					handled = true;
					objEntryPoint = new ThreadStart(InstallProject);
				} else if (commandName == GetCommand(ECommands.eUninstallProject)) {
					handled = true;
					objEntryPoint = new ThreadStart(UninstallProject);
				} else if (commandName == GetCommand(ECommands.eOpenFolder)) {
					handled = true;
					objEntryPoint = new ThreadStart(OpenExplorer);
				} else if (commandName == GetCommand(ECommands.eSwapFiles)) {
					handled = true;
					objEntryPoint = new ThreadStart(SwapFiles);
				} else if (commandName == GetCommand(ECommands.eInsertClassName)) {
					handled = true;
					objEntryPoint = new ThreadStart(InsertClassName);
					//} else if (commandName == GetCommand(ECommands.eInsertFileHeader)) {
					//        handled = true;
					//        objEntryPoint = new ThreadStart(InsertFileHeader);
				}
			}

			if (objEntryPoint != null) {
				m_objThread = new System.Threading.Thread(objEntryPoint);

				if(commandName == GetCommand(ECommands.eInstallProject) || commandName == GetCommand(ECommands.eUninstallProject))
					m_objThread.SetApartmentState(ApartmentState.STA);

				m_objThread.Name = StrThreadName;
				m_objThread.Start();
			}
		}

		private OutputWindowPane StartOutputWindow()
		{
			OutputWindowPane objPane = AddinUtility.sGetOutputWindow( m_objApplication, "LibAssist" );
			if( null == objPane )
				Trace.WriteLine( "LibAssist.Connect.StartOutputWindow : Can not get a source control window" );
			else
				objPane.OutputString("------ LibAssist Processing...------\r\n");
		        return objPane;
		}

		//private void RunLibAssist()
		//{
		//        lock( m_objLock ) {
		//                Configuration.Save();

		//                Preference	objPreferenceDlg = new Preference();
		//                if( DialogResult.OK == objPreferenceDlg.ShowDialog() ) {
		//                        Configuration.Save();
		//                }
		//        }
		//}

		private void InstallProject()
		{
			lock( m_objLock ) {
				ArrayList strProjectArray = AddinUtility.sGetSelectedProjectList(m_objApplication);

				if (strProjectArray.Count != 0)
				{
					COutputWindowToDTEWindow objOutput = new COutputWindowToDTEWindow( StartOutputWindow() );
					ReleaseFiles.Install(strProjectArray, objOutput);
				}
			}
		}

		private void UninstallProject()
		{
			lock( m_objLock ) {
				ArrayList strProjectArray = AddinUtility.sGetSelectedProjectList(m_objApplication);

				if (strProjectArray.Count != 0)
				{
					COutputWindowToDTEWindow objOutput = new COutputWindowToDTEWindow( StartOutputWindow() );
					ReleaseFiles.Uninstall(strProjectArray, objOutput);
				}
			}
		}

		private void SwapFiles()
		{
			lock( m_objLock ) {
				string strCurrentFileName = m_objApplication.ActiveDocument.FullName;
				if (strCurrentFileName != "")
				{
					string strFolder = Path.GetDirectoryName(strCurrentFileName);
					string strFileName = Path.GetFileNameWithoutExtension(strCurrentFileName);
					string strNewFileName = null;

					string[] arrFileExtensions = StrSwapFiles.Split(';');

					foreach (string strExtension in arrFileExtensions)
					{
						if (strExtension != Path.GetExtension(strCurrentFileName))
						{
							strNewFileName = strFileName + strExtension;
							strNewFileName = Path.Combine(strFolder, strNewFileName);

							if (File.Exists(strNewFileName))
								break;
							else
								strNewFileName = null;
						}
					}

					if (strNewFileName == null)
					{
						string strExtension = Path.GetExtension(strCurrentFileName);
						if (strExtension == ".cs")
						{
							if (PathUtility.MatchFileName("*.Designer.cs", strCurrentFileName, false))
								strNewFileName = Path.GetFileNameWithoutExtension(strFileName) + ".cs";
							else
								strNewFileName = strFileName + ".Designer.cs";

							strNewFileName = Path.Combine(strFolder, strNewFileName);
						}
					}

					if (strNewFileName != null)
					{
						if (File.Exists(strNewFileName))
						{
							try
							{
								m_objApplication.ItemOperations.OpenFile(strNewFileName, Constants.vsViewKindTextView);
								return;
							}
							catch (Exception objExcept)
							{
								Trace.WriteLine(objExcept.Message);
							}
						}
					}
				}
			}
		}

		private void OpenExplorer()
		{
			lock( m_objLock ) {
				string strCurrentFileName = m_objApplication.ActiveDocument.FullName;
				if (strCurrentFileName != "")
				{
					//string strFolder = Path.GetDirectoryName(strCurrentFileName);
					StringBuilder objString = new StringBuilder();
					objString.Append( "/e, /select," );
					objString.Append( strCurrentFileName );
					System.Diagnostics.Process.Start("explorer.exe", objString.ToString());
				}
			}
		}

		private void InsertClassName()
		{
			lock( m_objLock ) {
				string strCurrentFileName = m_objApplication.ActiveDocument.FullName;
				if (strCurrentFileName != "")
				{
					string strFileName = Path.GetFileNameWithoutExtension(strCurrentFileName);
					try
					{
						((TextSelection)m_objApplication.ActiveDocument.Selection).Text = strFileName;
					}
					catch (Exception objExcept)
					{
						Trace.WriteLine(objExcept.Message);
					}
				}
			}
		}
	}
}