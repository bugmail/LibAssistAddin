using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using LibAssist;
using EnvDTE;

namespace LibAssistConsole
{
	class Program
	{
		public class CStandradOutput : LibAssist.OutputWindow
		{
			public void Write(string str)			{ Console.Write(str);		}
			public void WriteLine(string str)		{ Console.WriteLine(str);	}
		}

		static void PrintUsage()
		{
			Console.WriteLine("Console of LibAssist");
			Console.WriteLine("Usage : ");
			Console.WriteLine("LibAssistConsole [/u] [/i] [ProjectPath 1] [ProjectPath 2] ... [ProjectPath N]");
			Console.WriteLine("" );
			Console.WriteLine("/u          - Uninstall project(default)");
			Console.WriteLine("/i          - Install project" );
			Console.WriteLine("ProjectPath - Path of project for install/uninstall");
		}

		static void Main(string[] args)
		{
			if( args.Length < 2 ) {
				Console.WriteLine("Invalid Argument");
				PrintUsage();
				return;
			}

			foreach (string strArg in args)
				Console.WriteLine(strArg);
			bool bInstall = false;

			if( args[0] == "/i" )
				bInstall = true;
			else if( args[0] == "/u" )
				bInstall = false;
			else {
				Console.WriteLine("Invalid Argument");
				PrintUsage();
				return;
			}
			
			ArrayList objProjectPathList = new ArrayList();
			for( int nIndex = 1; nIndex <  args.Length; ++nIndex )
				objProjectPathList.Add( args[nIndex] );

			CStandradOutput	objOutput = new CStandradOutput();
			if (bInstall)
			{
				if (!ReleaseFiles.Install(objProjectPathList, objOutput))
					Console.WriteLine("Fail to project installation");
			}
			else
			{
				if (!ReleaseFiles.Uninstall(objProjectPathList, objOutput))
					Console.WriteLine("Fail to project uninstallation");
			}
		}
	}
}
