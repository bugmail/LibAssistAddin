using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LibAssist
{
	public class LibAssistDetailPageForm : Form
	{
		public virtual void Save()		{}
	}

	public partial class Preference : Form
	{
		public Preference()
		{
			InitializeComponent();
		}

		public void OnOk( object objSender, EventArgs eEventArgs )
		{
			OnAssign( objSender, eEventArgs );
			DialogResult = DialogResult.OK;
		}

		public void OnCancel( object objSender, EventArgs eEventArgs )
		{
			DialogResult = DialogResult.Cancel;
		}

		public void OnAssign( object objSender, EventArgs eEventArgs )
		{
			foreach( LibAssistDetailPageForm objForm in m_objDetailPages )
				objForm.Save();
		}
	}
}