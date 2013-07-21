using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibAssist
{
	internal class BandTagInfo
	{
		public BandTagInfo()
		{
		}

		public BandTagInfo( OutlookBar objOutlookBar, int nIndex )
		{
			ParentOutlookBar = objOutlookBar;
			Index = nIndex;
		}

		public OutlookBar ParentOutlookBar
		{
			set { m_objParentOutlookBar = value;	}
			get { return m_objParentOutlookBar;	}
		}

		public int Index
		{
			set { m_nIndex = value; 		}
			get { return m_nIndex;			}
		}

		protected OutlookBar m_objParentOutlookBar;
		protected int m_nIndex;
	}

	internal class BandPanel : Panel
	{
		public BandPanel( string strCaption, ContentPanel objContent, BandTagInfo objTagInfo )
		{
			BandButton objBandButton = new BandButton(strCaption, objTagInfo);
			Controls.Add(objBandButton);
			Controls.Add(objContent);
		}
	}

	internal class BandButton : Button
	{
		private BandTagInfo m_objTagInfo;

		public BandTagInfo TagInfo
		{
			set { m_objTagInfo = value;		}
			get { return m_objTagInfo;		}
		}

		public BandButton(string strCaption, BandTagInfo objTagInfo)
		{
			this.BackColor	= Color.Transparent;
			this.Text	= strCaption;
			this.Visible	= true;
			this.TagInfo	= objTagInfo;
			this.FlatStyle	= FlatStyle.Flat;
			this.TextAlign	= ContentAlignment.BottomLeft;
			this.Margin	= new Padding( 3 );
			this.Font	= new Font( "Verdana", 10, FontStyle.Bold );
			Focus();
			this.Click += new EventHandler(SelectBand);
		}

		private void SelectBand(object objSender, EventArgs eEventArgs)
		{
			TagInfo.ParentOutlookBar.SelectBand(TagInfo.Index);
		}
	}


	public class OutlookBar : Panel
	{
		public int ButtonHeight
		{
			set { m_nButtonHeight = value;		}
			get { return m_nButtonHeight;		}
		}

		public int SelectedBand
		{
			set { m_nSelectedBand = value;		}
			get { return m_nSelectedBand;		}
		}

		public int SelectedBandHeight
		{
			set { m_nSelectedBandHeight = value;	}
			get { return m_nSelectedBandHeight;	}
		}

		public Size PanelMargin
		{
			set { m_objPanelMargin = value;		}
			get { return m_objPanelMargin;		}
		}

		public OutlookBar()
		{
			ButtonHeight 		= DEFAULT_BUTTONHEIGHT;
			SelectedBand 		= 0;
			SelectedBandHeight	= 0;
			m_objPanelMargin.Width	= DEFAULT_PANELMARGIN_WIDTH;
			m_objPanelMargin.Height	= DEFAULT_PANELMARGIN_HEIGHT;
		}

		public void Initialize(int nButtonHeight, int nSelectedBand)
		{
			ButtonHeight = nButtonHeight;
			SelectedBand = nSelectedBand;

			Parent.SizeChanged += new EventHandler(SizeChangedEvent);
		}

		public int AddBand(string strCaption, ContentPanel objChildPanel)
		{
			objChildPanel.ParentOutlookBar = this;

			int nIndex = Controls.Count;
			BandTagInfo objBandtagInfo = new BandTagInfo(this, nIndex);
			BandPanel objBandPanel = new BandPanel(strCaption, objChildPanel, objBandtagInfo);
			Controls.Add(objBandPanel);
			UpdateBarInfo();
			UpdateLayout(objBandPanel, nIndex);

			return nIndex;
		}

		private void SizeChangedEvent(object objSender, EventArgs eEventArgs)
		{
			Size = new Size(Size.Width, ((Control)objSender).ClientRectangle.Size.Height);

			UpdateBarInfo();
			RedrawBands();
		}

		public void SelectBand(int nIndex)
		{
			m_nSelectedBand = nIndex;
			RedrawBands();
		}

		private void RedrawBands()
		{
			int nIndex = 0;
			foreach (BandPanel objBandPanel in Controls)
				UpdateLayout(objBandPanel, nIndex++);
		}

		private void UpdateBarInfo()
		{
			m_nSelectedBandHeight = ClientRectangle.Height - (Controls.Count * m_nButtonHeight);
		}

		private void UpdateLayout( BandPanel objPanel, int nIndex )
		{
			int nVPos = (nIndex <= SelectedBand) ? ButtonHeight * nIndex : ButtonHeight * nIndex + SelectedBandHeight;
			int nHeight = (nIndex <= SelectedBand) ? SelectedBandHeight + ButtonHeight : ButtonHeight;

			objPanel.Location = new Point(0, nVPos);
			objPanel.Size = new Size(ClientRectangle.Width, nHeight);

			objPanel.Controls[0].Location = new Point(0, 0);
			objPanel.Controls[0].Size = new Size(ClientRectangle.Width, ButtonHeight);

			objPanel.Controls[1].Location = new Point(0, ButtonHeight);
			objPanel.Controls[1].Size = new Size(ClientRectangle.Width - m_objPanelMargin.Width, Height - m_objPanelMargin.Height);
		}

		private int m_nButtonHeight;
		private int m_nSelectedBand;
		private int m_nSelectedBandHeight;
		private Size m_objPanelMargin;

		private const int DEFAULT_BUTTONHEIGHT		= 25;
		private const int DEFAULT_PANELMARGIN_WIDTH	= 2;
		private const int DEFAULT_PANELMARGIN_HEIGHT	= 8;
	}

	public abstract class ContentPanel : Panel
	{
		public OutlookBar ParentOutlookBar;

		public ContentPanel()
		{
			Visible = true;
		}
	}

	public class TextPanel : ContentPanel
	{
		public const int DEFAULT_TEXTHEIGHT	= 25;
		Label	m_objSelectedLabel = null;

		public TextPanel()
		{
			BackColor = Color.LightBlue;
			AutoScroll = true;

			SizeChanged += new EventHandler(SizeChangedEvent);
		}

		public void AddItem( string strText, EventHandler OnClickEvent, object objShowingForm )
		{
			int nIndex = Controls.Count;

			Label objLabel		= new Label();
			objLabel.Text		= strText;
			objLabel.Visible	= true;
			objLabel.Location	= new Point( 0, nIndex * DEFAULT_TEXTHEIGHT );
			objLabel.Size		= new Size( this.Size.Width, DEFAULT_TEXTHEIGHT );
			objLabel.TextAlign	= ContentAlignment.MiddleCenter;
			objLabel.Click		+= OnClickEvent;
			objLabel.Click		+= new EventHandler(ChangeColor);
			objLabel.Tag		= objShowingForm;
			Controls.Add( objLabel );
		}

		private void ChangeColor( object objSender, EventArgs eEventArgs )
		{
			if( m_objSelectedLabel != objSender ) {
				if( null != m_objSelectedLabel )
					m_objSelectedLabel.BackColor = Color.Transparent;
				m_objSelectedLabel = (Label)objSender;
				m_objSelectedLabel.BackColor = Color.LightCyan;
			}
		}

		private void SizeChangedEvent(object objSender, EventArgs eEventArgs)
		{
			UpdateLayout( Parent.Width );
		}

		public void UpdateLayout( int nWidth )
		{
			for( int nIndex = 0; nIndex < Controls.Count; ++nIndex ) {
				Label objLabel = (Label)Controls[nIndex];
				objLabel.Location = new Point( 0, nIndex * DEFAULT_TEXTHEIGHT );
				objLabel.Size = new Size( nWidth, DEFAULT_TEXTHEIGHT );
			}
		}
	}
	
}