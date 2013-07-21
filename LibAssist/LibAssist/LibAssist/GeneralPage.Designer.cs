using System.Windows.Forms;
using System;
using System.Drawing;
using Artist.Picasso;

namespace LibAssist
{
	partial class GeneralPage
	{
		const int MARGIN = 5;
		const int BUTTON_WIDTH = 75;
		const int LABEL_WIDTH = 100;
		const int BUTTON_HEIGHT = 20;
		const int CONTROL_PADDING = 5;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components		= new System.ComponentModel.Container();
			this.AutoScaleMode	= System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll		= true;

			if( null == Connect.Configuration.UserDefineDatas || 0 == Connect.Configuration.UserDefineDatas.Count ) {
				Label objLabel = new Label();
				objLabel.Text = "Empty";
				objLabel.TextAlign = ContentAlignment.MiddleCenter;
				objLabel.Location = new Point( 0, 0 );
				//objLabel.Size = new Size( Parent.Width, Parent.Height );
				objLabel.Dock = DockStyle.Fill;

				Controls.Add( objLabel );
			} else {
				this.SizeChanged += new EventHandler(GeneralPage_SizeChanged);
				ListViewEx objListView	= new ListViewEx();
				objListView.Name		= "RegistryList";
				objListView.View		= View.Details;
				objListView.MultiSelect		= false;
				//objListView.CheckBoxes		= true;
				//objListView.Size		= new Size( this.Width - MARGIN * 2, 200 );
				objListView.LabelEdit		= true;
				objListView.Visible		= true;
				objListView.FullRowSelect	= true;
				objListView.Anchor		= AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
				objListView.SubItemClicked	+= new SubItemEventHandler( objListView_SubItemClicked );
				objListView.SubItemEndEditing	+= new SubItemEndEditingEventHandler( objListView_SubItemEndEditing );
				objListView.DoubleClickActivation = true;
				objListView.Columns.Add( "Key", 100 );
				objListView.Columns.Add( "Value", 250 );

				Controls.Add( objListView );

				m_objListViewEx = objListView;

				foreach( Configuration.UserDefinedData objUserData in Connect.Configuration.UserDefineDatas ) {
					ListViewItem objListItem = new ListViewItem();
					objListItem.Name = objUserData.strKey;
					objListItem.Text = objUserData.strKey;
					objListItem.SubItems.Add( objUserData.strValue );

					objListView.Items.Add( objListItem );
				}

				// Editors
				TextBox objEDTKey	= new TextBox();
				objEDTKey.BorderStyle	= BorderStyle.FixedSingle;
				objEDTKey.Location	= new Point( 0, 0 );
				objEDTKey.Name		= "KeyEditor";
				objEDTKey.Multiline	= true;
				objEDTKey.Size		= new Size( 80, 16 );
				objEDTKey.Text		= "";
				objEDTKey.Visible	= false;
				m_objEDTKey		= objEDTKey;

				Controls.Add( objEDTKey );

				TextBox objEDTValue	= new TextBox();
				objEDTValue.BorderStyle	= BorderStyle.FixedSingle;
				objEDTValue.Location	= new Point( 0, 0 );
				objEDTValue.Name	= "ValueEditor";
				objEDTValue.Multiline	= true;
				objEDTValue.Size	= new Size( 80, 16 );
				objEDTValue.Text	= "";
				objEDTValue.Visible	= false;
				m_objEDTValue		= objEDTValue;

				Controls.Add( objEDTValue );

				Button objOk		= new Button();
				Button objCancel 	= new Button();
				Button objAssign 	= new Button();
				
				objAssign.Name		= "AssignButton";
				objAssign.Text		= "&Assign";
				objAssign.Anchor	= AnchorStyles.Right | AnchorStyles.Bottom;
				objAssign.Click		+= new EventHandler(OnAssign);
				objAssign.Visible	= true;

				objCancel.Name		= "CancelButton";
				objCancel.Text		= "&Cancel";
				objCancel.Anchor	= AnchorStyles.Right | AnchorStyles.Bottom;
				objCancel.Click		+= new EventHandler(OnCancel);
				objCancel.Visible	= true;

				objOk.Name		= "OkButton";
				objOk.Text		= "&Ok";
				objOk.Anchor		= AnchorStyles.Right | AnchorStyles.Bottom;
				objOk.Click		+= new EventHandler(OnOK);
				objOk.Visible		= true;

				Controls.Add( objAssign );
				Controls.Add( objCancel );
				Controls.Add( objOk );

				m_objAssignButton	= objAssign;
				m_objCancelButton	= objCancel;
				m_objOKButton		= objOk;

				UpdateLayout();

				Editors			= new Control[] { objEDTKey, objEDTValue	};
			}
		}

		private void UpdateLayout()
		{
			m_objListViewEx.Location	= new Point( MARGIN, MARGIN );
			m_objListViewEx.Size		= new Size( this.Width - MARGIN * 2, this.Height - MARGIN * 2 - CONTROL_PADDING - BUTTON_HEIGHT );
			
			m_objAssignButton.Location	= new Point( this.ClientRectangle.Right - MARGIN - BUTTON_WIDTH, m_objListViewEx.Bottom + CONTROL_PADDING );
			m_objAssignButton.Size		= new Size( BUTTON_WIDTH, BUTTON_HEIGHT );
			
			m_objCancelButton.Location	= new Point( m_objAssignButton.Left - CONTROL_PADDING - BUTTON_WIDTH, m_objAssignButton.Top );
			m_objCancelButton.Size		= new Size( BUTTON_WIDTH, BUTTON_HEIGHT );
			
			m_objOKButton.Location		= new Point( m_objCancelButton.Left - CONTROL_PADDING - BUTTON_WIDTH, m_objAssignButton.Top );
			m_objOKButton.Size		= new Size( BUTTON_WIDTH, BUTTON_HEIGHT );
		}

		private void GeneralPage_SizeChanged(object sender, EventArgs e)
		{
			Point ptNewLocation = Location;
			Size newSize = Size;
			
			Point ptNewListView = m_objListViewEx.Location;
			Size sizeNewList = m_objListViewEx.Size;

			Point ptNewAssignButton = m_objAssignButton.Location;
			Size sizeNewAssignButton = m_objAssignButton.Size;

			Point ptNewCancelButton = m_objCancelButton.Location;
			Size sizeNewCancelButton = m_objCancelButton.Size;

			Point ptNewOkButton = m_objOKButton.Location;
			Size sizeNewOkButton = m_objOKButton.Size;
		}

		private Button		m_objAssignButton 	= null;
		private Button		m_objCancelButton 	= null;
		private Button		m_objOKButton		= null;
		private	Control		m_objEDTValue		= null;
		private Control		m_objEDTKey		= null;
		private Control[]	Editors			= null;
		private ListViewEx	m_objListViewEx 	= null;

		private void OnOK( object objSender, EventArgs objEventArgs )
		{
			((Preference)Parent.Parent.Parent).OnOk(objSender, objEventArgs);
		}

		private void OnCancel( object objSender, EventArgs objEventArgs )
		{
			((Preference)Parent.Parent.Parent).OnCancel(objSender, objEventArgs);
		}

		private void OnAssign( object objSender, EventArgs objEventArgs )
		{
			((Preference)Parent.Parent.Parent).OnAssign(objSender, objEventArgs);
		}

		private void objListView_SubItemClicked(object objSender, SubItemEventArgs objEventArgs)
		{
			((ListViewEx)objSender).StartEditing( Editors[objEventArgs.SubItem], objEventArgs.Item, objEventArgs.SubItem );
		}

		private void objListView_SubItemEndEditing(object objSender, SubItemEndEditingEventArgs objEventArgs)
		{
			
		}

		private void OnClickChangeButton( object objSender, EventArgs eArgs )
		{
			DataSet objDataSet = (DataSet)((Button)objSender).Tag;

			if( Connect.Configuration.SetInfoToRegistry( objDataSet.Key.Text, objDataSet.Value.Text ) )
				MessageBox.Show( "Change success" );
			else
				MessageBox.Show( "Error: Can't change a registry value." );
		}

		class DataSet : Object
		{
			public Label	Key	= null;
			public TextBox	Value	= null;
		};

		#endregion
	}
}
