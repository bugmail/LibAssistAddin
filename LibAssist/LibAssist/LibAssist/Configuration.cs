using System;
using System.Collections;
using Microsoft.Win32;

namespace LibAssist
{
	public class Configuration
	{
		public Configuration()
		{
			m_objUserDefinedDatas = new ArrayList();
		}

		public void Initialize()
		{
			RegistryKey objPathKey = Registry.CurrentUser.OpenSubKey( REGISTRY_PATH, false );
			
			if( null == objPathKey )
				return;

			string[] arrSavedValues = objPathKey.GetValueNames();

			foreach( string strKey in arrSavedValues ) {
				UserDefinedData objNewData = new UserDefinedData();
				objNewData.strKey = strKey;
				objNewData.strValue = GetInfoFromRegistry( strKey );
				m_objUserDefinedDatas.Add( objNewData );
			}

			objPathKey.Close();
		}

		public void Save()
		{
			foreach( UserDefinedData objUserData in m_objUserDefinedDatas )
				SetInfoToRegistry( objUserData.strKey, objUserData.strValue );
		}

		public void Clear()
		{
			m_objUserDefinedDatas.Clear();
		}

		public string GetInfoFromRegistry( string strKey )
		{
			RegistryKey objPathKey = Registry.CurrentUser.OpenSubKey( REGISTRY_PATH, false );

			if (objPathKey == null)
				return "";

			string strValue = (string)objPathKey.GetValue( strKey, "" );
			objPathKey.Close();

			return strValue;
		}

		public bool SetInfoToRegistry( string strKey, string strValue )
		{
			RegistryKey objPathKey = null;
			objPathKey = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true);

			if (objPathKey == null)
				return false;

			objPathKey.SetValue(strKey, strValue);

			int nIndex = FindFromList( strKey );

			if( -1 == nIndex ) {
				UserDefinedData objData	= new UserDefinedData();
				objData.strKey		= strKey;
				objData.strValue	= strValue;

				m_objUserDefinedDatas.Add( objData );
			} else
				((UserDefinedData)(m_objUserDefinedDatas[nIndex])).strValue = strValue;

			objPathKey.Close();

			return true;
		}

		private int FindFromList( string strKey )
		{
			int nIndex = 0;

			foreach( UserDefinedData objData in m_objUserDefinedDatas ) {
				if( objData.strKey == strKey )
					return nIndex;
				++nIndex;
			}

			return -1;
		}

		public class UserDefinedData : Object
		{
			public string strKey;
			public string strValue;
		}

		public ArrayList UserDefineDatas
		{
			get { return m_objUserDefinedDatas;	}
		}

		private const string REGISTRY_PATH = "SOFTWARE\\LibAssist";
		private ArrayList m_objUserDefinedDatas = null;
	}
}