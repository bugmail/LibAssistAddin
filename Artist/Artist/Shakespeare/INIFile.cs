using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;

namespace Artist
{
	namespace Shakespeare
	{
		public class INIFile
		{
			#region Kernel's functions
			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern long WritePrivateProfileSection(string strSection, string strValue, string strFilePath);
			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern long WritePrivateProfileString(string strSection, string strKey, string strValue, string strFilePath);
			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern long WritePrivateProfileStruct(string strSection, string strKey, ref object objStruct, uint uSizeSturct, string strFilePath);


			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern int GetPrivateProfileString(string strSection, string strKey, string strDefault, StringBuilder strRet, int nSize, string strFilePath);
			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern uint GetPrivateProfileInt(string strSection, string strKey, int nDefault, string strFilePath);
			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern UInt32 GetPrivateProfileSection(string strSection, StringBuilder strRet, UInt32 uiSize, string strFilePath);
			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern UInt32 GetPrivateProfileSection(string strSection, IntPtr pRet, UInt32 uiSize, string strFilePath);
			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern UInt32 GetPrivateProfileSectionNames(IntPtr pszReturnBuffer, UInt32 uiSize, string strFilePath);
			[DllImport("Kernel32.dll", SetLastError = true)]
			public static extern long GetPrivateProfileStruct(string strSection, string strKey, ref object objStruct, UInt32 uiSizeStruct, string strFilePath);
			#endregion

			protected string m_strPath = "";
			public string Path
			{
				get { return m_strPath; }
				set { m_strPath = value; }
			}

			public INIFile()
			{
			}

			public INIFile(string strINIFilePath)
			{
				Path = strINIFilePath;
			}

			public bool WriteValue(string strSection, string strKey, string strValue)
			{
				if( Path == "" )
					return false;

				if( 0 == WritePrivateProfileString(strSection, strKey, strValue, this.Path) )
					return false;

				return true;
			}

			public bool WriteValue(string strSection, string strKey, ref object objStruct, uint uSizeStruct )
			{
				if( Path == "" )
					return false;

				if( 0 == WritePrivateProfileStruct(strSection, strKey, ref objStruct, uSizeStruct, this.Path) )
					return false;

				return true;
			}

			public bool Write(string strSetion, string strValue)
			{
				if( Path == "" )
					return false;

				if( 0 == WritePrivateProfileSection(strSetion, strValue, this.Path) )
					return false;

				return true;
			}

			public object[] Sections
			{
				get
				{
					IntPtr pszReturnBuffer = Marshal.StringToHGlobalAnsi(new string('\0', 1024));
					uint uCopiedBuffer = GetPrivateProfileSectionNames((IntPtr)pszReturnBuffer, 1024, Path);
					string strBuffer = Marshal.PtrToStringAnsi(pszReturnBuffer, (int)uCopiedBuffer);
					Marshal.FreeHGlobal(pszReturnBuffer);

					if( uCopiedBuffer == 1026 )
						return null;

					ArrayList arrSections = new ArrayList();
					StringBuilder strBuilder = new StringBuilder(1024);
					for( int nIndex = 0; nIndex < uCopiedBuffer; ++nIndex ) {
						if( strBuffer[nIndex] == '\0' ) {
							arrSections.Add(strBuilder.ToString());
							strBuilder.Length = 0;
						}  else
							strBuilder.Append(strBuffer[nIndex]);
					}

					return arrSections.ToArray();
				}
			}

			public string[] ReadSection(string strSection)
			{
				if( Path == "" )
					return null;

				StringBuilder objTemp = new StringBuilder();
				IntPtr pReturned = Marshal.AllocCoTaskMem(short.MaxValue);
				try {
					int nRet = (int)GetPrivateProfileSection(strSection, pReturned, (uint)short.MaxValue, this.Path);

					if( nRet == 0 )
						return null;

					for( int nIndex = 0; nIndex < nRet - 1; ++nIndex )
						objTemp.Append((char)Marshal.ReadByte(new IntPtr((uint)pReturned + (uint)nIndex)));
				} finally {
					Marshal.FreeCoTaskMem(pReturned);
				}

				string strReturn = objTemp.ToString();
				return strReturn.Split( '\0' );
			}

			public string ReadValue(string strSection, string strKey, string strDefault)
			{
				if( Path == "" )
					return "";

				StringBuilder objTemp = new StringBuilder(short.MaxValue);
				int nRet = GetPrivateProfileString(strSection, strKey, strDefault, objTemp, short.MaxValue, this.Path);
				string strValue = objTemp.ToString();
				if( strValue.IndexOf('#') != -1 )
					strValue = strValue.Substring(0, strValue.IndexOf('#'));

				return strValue.TrimEnd();
			}

			public int ReadValueInt(string strSection, string strKey)
			{
				if( Path == "" )
					return -1;

				return (int)GetPrivateProfileInt(strSection, strKey, -1, this.Path);
			}

			public bool ReadValue(string strSection, string strKey, ref object objStruct, uint uSizeStruct)
			{
				if( Path == "" )
					return false;

				if( 0 == GetPrivateProfileStruct(strSection, strKey, ref objStruct, uSizeStruct, this.Path) )
					return false;

				return true;
			}
		}
	}

}