using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.IO;


namespace LibAssist
{
	public class AddinUtility
	{
		public AddinUtility()
		{
		}

		public static bool sAddToMenu(DTE2 objApplication, AddIn objAddin, string strParentName, string strMenuName, string strButtonText, string strTooltipText)
		{
			try {
				object[] objContextGUIDs = new object[] { };
				Commands2 objCmds = (Commands2)objApplication.Commands;

				Microsoft.VisualStudio.CommandBars.CommandBar objMenuBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)objApplication.CommandBars)["MenuBar"];

				CommandBarControl objCmdControl = objMenuBar.Controls[strParentName];
				CommandBarPopup objPopup = (CommandBarPopup)objCmdControl;

				if( null == objPopup )
					return false;

				Command objCmd = objCmds.AddNamedCommand2(objAddin,
									   strMenuName,
									   strButtonText,
									   strTooltipText,
									   true,
									   59,
									   ref objContextGUIDs,
									   (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled,
									   (int)vsCommandStyle.vsCommandStylePictAndText,
									   vsCommandControlType.vsCommandControlTypeButton);

				if (null == objCmd )
					return false;

				objCmd.AddControl(objPopup.CommandBar, 1);

				return true;
			} catch (Exception objExcept) {
				Trace.WriteLine(objExcept.Message);
				return false;
			}
		}

		public static bool sDeleteFromMenu(DTE2 objApplication, string strParentName, string strMenuName)
		{
			try {
				object[] objContextGUIDs = new object[] { };
				Commands2 objCmds = (Commands2)objApplication.Commands;

				Microsoft.VisualStudio.CommandBars.CommandBar objMenuBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)objApplication.CommandBars)["MenuBar"];

				CommandBarControl objCmdControl = objMenuBar.Controls[strParentName];
				CommandBarPopup objPopup = (CommandBarPopup)objCmdControl;

				objPopup.Controls[strMenuName].Delete(null);

				return true;
			} catch (Exception objExcept) {
				Trace.WriteLine(objExcept.Message);
				return false;
			}
		}

		public static bool sAddCommand(DTE2 objApplication, AddIn objAddin, string strParent, string strProgID, string strButtonText, string strTooltipText, int nCmdDisableFlag)
		{
			try {
				Commands objCmds = objApplication.Commands;
				if(null == objCmds)
					return false;

				object[] objContextGUIDs = new object[] { };
				Command objCreateCommand = objCmds.AddNamedCommand(objAddin,
										    strProgID,
										    strButtonText,
										    strTooltipText,
										    true,
										    0,
										    ref objContextGUIDs,
										    nCmdDisableFlag);

				if (null == objCreateCommand)
					return false;

				_CommandBars objCmdBars = (_CommandBars)objApplication.CommandBars;
				CommandBar objMenuCmdBar = objCmdBars[strParent];

				objCreateCommand.AddControl(objMenuCmdBar, 1);
			} catch (Exception objException) {
				Trace.WriteLine(objException.Message);
				return false;
			}

			return true;
		}

		public static bool sDeleteCommand(DTE2 objApplication, string strCommand)
		{
			try {
				objApplication.Commands.Item(strCommand, 0).Delete();
			} catch (Exception objExcept) {
				Trace.WriteLine(objExcept.Message);
				return false;
			}

			return true;
		}

		public static OutputWindowPane sGetOutputWindow(DTE2 objApplication, string strItemName)
		{
            bool                bNeedToAddNewWindow = true;
            EnvDTE.OutputWindow OutputWindow = null;
            OutputWindowPane    OutputPane = null;
			try {
				Window objWindow = objApplication.Windows.Item(Constants.vsWindowKindOutput);

                objWindow.Activate();

                OutputWindow = ((EnvDTE.OutputWindow)objWindow.Object);

                if(OutputWindow != null && OutputWindow.OutputWindowPanes.Count <= 0 )
                {
                    throw new Exception( "Not enough Output window" );
                }

                OutputPane = OutputWindow.OutputWindowPanes.Item(strItemName);
                if(OutputPane != null)
                {
                    bNeedToAddNewWindow = false;
                }
			} catch (Exception objExcept) {
				Trace.WriteLine(objExcept.Message);
			}

            if(bNeedToAddNewWindow)
            {
                OutputPane = OutputWindow.OutputWindowPanes.Add(strItemName);
            }

            OutputPane.Activate();

            return OutputPane;
		}

		public static ArrayList sGetSelectedProjectList(DTE2 objApplication)
		{
			ArrayList objProjectList = new ArrayList();

			foreach(SelectedItem objItem in objApplication.SelectedItems) {
				string strProjectPath = Path.GetDirectoryName(objItem.Project.FullName);

				bool bIsExistProject = false;
				foreach(string strProject in objProjectList.ToArray()) {
					if (strProject == strProjectPath) {
						bIsExistProject = true;
						break;
					}
				}

				if (!bIsExistProject)
					objProjectList.Add(strProjectPath);
			}

			return objProjectList;
		}
	}
}