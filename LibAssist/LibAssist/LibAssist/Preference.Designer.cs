using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LibAssist
{
	partial class Preference
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private OutlookBar	m_objSection = null;
		private ArrayList	m_objDetailPages = null;

		private Size DEFAULT_WINDOWSIZE = new Size(550, 300);
		private const int DEFAULT_OUTLOOKBARWIDTH = 200;
		private const int DEFAULT_BUTTONHEIGHT = 25;

		//private SplitContainer	m_objSplitContainer	= new SplitContainer();

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components		= new System.ComponentModel.Container();
			this.AutoScaleMode	= System.Windows.Forms.AutoScaleMode.Font;
			this.Size		= DEFAULT_WINDOWSIZE;
			this.Text		= "Preference";
			this.m_objDetailPages	= new ArrayList();

			m_objSplitContainer		= new SplitContainer();
			m_objSplitContainer.Parent	= this;
			m_objSplitContainer.Location	= new Point( this.ClientRectangle.Left, this.ClientRectangle.Top );
			m_objSplitContainer.Size	= new Size( this.ClientSize.Width, this.ClientSize.Height );
			m_objSplitContainer.Anchor	= AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
			m_objSplitContainer.Panel2.AutoScroll	= true;

			m_objSection			= new OutlookBar();
			m_objSection.Parent		= m_objSplitContainer.Panel1;
			m_objSection.Location		= new Point(0, 0);
			m_objSection.Size		= new Size(DEFAULT_OUTLOOKBARWIDTH, this.ClientSize.Height - 25 );
			m_objSection.BorderStyle	= BorderStyle.FixedSingle;
			m_objSection.Dock		= DockStyle.Fill;
			m_objSection.Initialize( DEFAULT_BUTTONHEIGHT, 0 );

			TextPanel objPanel = new TextPanel();
			m_objSection.AddBand( "Environment", objPanel );

			GeneralPage objDetailForm = new GeneralPage();
			objDetailForm.Name		= "GeneralPage";
			objDetailForm.FormBorderStyle	= FormBorderStyle.None;
			objDetailForm.Size		= new Size( this.Width - DEFAULT_BUTTONHEIGHT, this.ClientSize.Height - 25 );
			objPanel.AddItem( "Registry", new EventHandler( OnClickSection ), objDetailForm );
			m_objDetailPages.Add( objDetailForm );
			//objPanel.AddItem( "Registry", new EventHandler( OnClickSection ), null );
		}

		public void OnClickSection( object objSender, EventArgs eArgs )
		{
			GeneralPage objContent	= (GeneralPage)( ((Control)objSender).Tag );
			if( null != objContent ) {
				objContent.TopLevel	= false;
				objContent.Parent	= m_objSplitContainer.Panel2;
				objContent.Dock		= DockStyle.Fill;
				objContent.Visible	= true;
			}
		}

		private SplitContainer	m_objSplitContainer;

		#endregion
	}
}