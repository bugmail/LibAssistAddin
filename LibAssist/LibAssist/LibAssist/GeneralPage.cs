using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LibAssist
{
	public partial class GeneralPage : LibAssistDetailPageForm
	{
		public GeneralPage()
		{
			InitializeComponent();
		}

		public override void Save()
		{
			foreach( ListViewItem objItem in m_objListViewEx.Items )
				Connect.Configuration.SetInfoToRegistry( objItem.Text, objItem.SubItems[0].Text );
		}

		public new Control Parent
		{
			get { return base.Parent;	}
			set {
				base.Parent = value;

				this.Size = new Size( Parent.Width - 2 * MARGIN, Parent.Height - 2 * MARGIN );

				UpdateLayout();
			}
		}
	}
}
