using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;

namespace Artist.Picasso
{
	public delegate void	SubItemEventHandler( object objSender, SubItemEventArgs eEventArgs );
	public delegate void	SubItemEndEditingEventHandler( object objSender, SubItemEndEditingEventArgs eEventArgs );

	public class SubItemEventArgs : EventArgs
	{
		public SubItemEventArgs( ListViewItem objItem, int nSubItem )
		{
			m_objItem	= objItem;
			m_nSubItemIndex	= nSubItem;
		}

		public int SubItem
		{
			get { return m_nSubItemIndex;	}
		}

		public ListViewItem Item
		{
			get {  return m_objItem;	}
		}

		private int m_nSubItemIndex	= -1;
		private ListViewItem m_objItem	= null;
	}

	public class SubItemEndEditingEventArgs : SubItemEventArgs
	{
		public SubItemEndEditingEventArgs( ListViewItem objItem, int nSubItem, string strDisplay, bool bCancel ) :
			base( objItem, nSubItem )
		{
			m_strText = strDisplay;
			m_bCancel = bCancel;
		}

		public string DisplayText
		{
			get { return m_strText;		}
			set { m_strText = value;	}
		}

		public bool Cancel
		{
			get { return m_bCancel;		}
			set { m_bCancel = value;	}
		}

		private string m_strText	= string.Empty;
		private bool m_bCancel		= true;
	}

	public class ListViewEx : System.Windows.Forms.ListView
	{
		#region Interop structs, imports and constants
		public	struct NMHDR
		{
            public IntPtr	hWndFrom;
            public Int32	nIDFrom;
            public Int32	nCode;
		}

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage( IntPtr hWnd, int nMessage, IntPtr wParam, IntPtr lParam );
		[DllImport("user32.dll", CharSet = CharSet.Ansi)]
		private static extern IntPtr SendMessage( IntPtr hWnd, int nMessage, int nLength, ref int[] Order );
		#endregion

		private System.ComponentModel.Container		m_objComponents = null;

		public event SubItemEventHandler		SubItemClicked;
		public event SubItemEventHandler		SubItemBeginEditing;
		public event SubItemEndEditingEventHandler	SubItemEndEditing;

		//public event EventHandler<SubItemEventArgs>	SubItemClicked;

		public ListViewEx()
		{
			InitializeComponent();

			base.FullRowSelect	= true;
			base.View		= View.Details;
			base.AllowColumnReorder	= true;
			base.LabelEdit		= false;
		}

		public new bool LabelEdit
		{
			get { return false;	}
			set {}
		}

		protected override void Dispose( bool bDisposing )
		{
			if( bDisposing && m_objComponents != null )
				m_objComponents.Dispose();

			base.Dispose();
		}

		private void InitializeComponent()
		{
			m_objComponents = new System.ComponentModel.Container();
		}

		public bool DoubleClickActivation
		{
			get { return m_bDoubleClickActivation;	}
			set { m_bDoubleClickActivation = value;	}
		}

		public int[] GetColumnOrder()
		{
			IntPtr lParam = Marshal.AllocHGlobal( Marshal.SizeOf( typeof(int) ) * Columns.Count );
			IntPtr wParam = new IntPtr( Columns.Count );

			IntPtr lResult = SendMessage( Handle, (int)Win32.ListViewMessage.LVM_GETCOLUMNORDERARRAY, wParam, lParam );
			if( lResult.ToInt32() == 0 ) {
				Marshal.FreeHGlobal(lParam);
				return null;
			}

			int [] arrColumnOrder = new int[Columns.Count];
			Marshal.Copy(lParam, arrColumnOrder, 0, Columns.Count);
			Marshal.FreeHGlobal(lParam);

			return arrColumnOrder;
		}

		public int GetSubItemAt( int nX, int nY, out ListViewItem objItem )
		{
			objItem = this.GetItemAt( nX, nY );

			if( objItem != null ) {
				int [] arrColumnOrder = GetColumnOrder();
				Rectangle rcLVBounds;
				int nSubItemX = 0;

				rcLVBounds = objItem.GetBounds(ItemBoundsPortion.Entire);
				nSubItemX = rcLVBounds.Left;
				for( int nIndex = 0; nIndex < arrColumnOrder.Length; ++nIndex ) {
					ColumnHeader objColHdr = this.Columns[ arrColumnOrder[nIndex] ];
					if( nX < nSubItemX + objColHdr.Width )
						return objColHdr.Index;
					nSubItemX += objColHdr.Width;
				}
			}

			return -1;
		}

		public Rectangle GetSubItemBounds( ListViewItem objItem, int nSubItem )
		{
			int [] arrColumnOrder = GetColumnOrder();

			Rectangle rcSubItem = Rectangle.Empty;
			if( nSubItem >= arrColumnOrder.Length )
				throw new IndexOutOfRangeException( "SubItem " + nSubItem + " out of range" );

			if( objItem == null )
				throw new ArgumentNullException( "Item" );

			Rectangle rcLVBounds = objItem.GetBounds(ItemBoundsPortion.Entire);
			int nSubItemX = rcLVBounds.Left;

			ColumnHeader objColHdr;
			int nIndex = 0;
			for( nIndex = 0; nIndex < arrColumnOrder.Length; ++nIndex ) {
				objColHdr = this.Columns[ arrColumnOrder[nIndex] ];
				if( objColHdr.Index == nSubItem )
					break;
				nSubItemX += objColHdr.Width;
			}
			rcSubItem = new Rectangle( nSubItemX, rcLVBounds.Top, this.Columns[ arrColumnOrder[ nIndex ] ].Width, rcLVBounds.Height );
			return rcSubItem;
		}

		protected override void WndProc(ref Message objMessage)
		{
			switch( objMessage.Msg ) {
				case (int)Win32.Win32Message.WM_VSCROLL :
				case (int)Win32.Win32Message.WM_HSCROLL :
				case (int)Win32.Win32Message.WM_SIZE :
					EndEditing( false );
					break;
				case (int)Win32.Win32Message.WM_NOTIFY :
					NMHDR objNotifyHeader = (NMHDR)Marshal.PtrToStructure( objMessage.LParam, typeof(NMHDR) );
					if( objNotifyHeader.nCode == (Int32)Win32.HEADERCONTROLMESSAGE.HDN_BEGINDRAG ||
						objNotifyHeader.nCode == (Int32)Win32.HEADERCONTROLMESSAGE.HDN_ITEMCHANGINGA ||
						objNotifyHeader.nCode == (Int32)Win32.HEADERCONTROLMESSAGE.HDN_ITEMCHANGINGW )
						EndEditing( false );
					break;
			}

			base.WndProc(ref objMessage);
		}

		#region Initialize editing depending of DoubleClickActivation property
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs eMouseEventArgs)
		{
			base.OnMouseUp(eMouseEventArgs);

			if( DoubleClickActivation )
				return;

			EditSubItemAt( new Point( eMouseEventArgs.X, eMouseEventArgs.Y ) );
		}

		protected override void OnDoubleClick(EventArgs eEventArgs)
		{
			base.OnDoubleClick(eEventArgs);

			if( !DoubleClickActivation )
				return;

			Point ptCursor = this.PointToScreen( Cursor.Position );

			EditSubItemAt( ptCursor );
		}

		private void EditSubItemAt( Point pt )
		{
			ListViewItem objItem;
			int nSubItemIndex = GetSubItemAt( pt.X, pt.Y, out objItem );
			if( nSubItemIndex >= 0 )
				OnSubItemClicked( new SubItemEventArgs( objItem, nSubItemIndex ) );
		}
		#endregion

		#region In-place editing functions
		protected void OnSubItemBeginEditing( SubItemEventArgs eEventArgs )
		{
			if( SubItemBeginEditing != null )
				SubItemBeginEditing( this, eEventArgs );
		}

		protected void OnSubItemEndEditing( SubItemEndEditingEventArgs eEventArgs )
		{
			if( SubItemEndEditing != null )
				SubItemEndEditing( this, eEventArgs );
		}

		protected void OnSubItemClicked( SubItemEventArgs eEventArgs )
		{
			if( SubItemClicked != null )
				SubItemClicked( this, eEventArgs );
		}

		public void StartEditing( Control objControl, ListViewItem objItem, int nSubItem ) 
		{
			SubItemEventArgs objArgs = new SubItemEventArgs( objItem, nSubItem );
			OnSubItemBeginEditing( objArgs );
			
			Rectangle rcSubItem = GetSubItemBounds( objItem, nSubItem );

			if( rcSubItem.X < 0 ) {
				rcSubItem.Width += rcSubItem.X;
				rcSubItem.X = 0;
			}

			if( rcSubItem.X + rcSubItem.Width > this.Width )
				rcSubItem.Width = this.Width - rcSubItem.Left;

			rcSubItem.Offset( Left, Top );

			Point ptOrigin		= new Point( 0, 0 );
			Point ptLVOrigin	= this.Parent.PointToScreen( ptOrigin );
			Point ptCtlOrigin	= objControl.Parent.PointToScreen( ptOrigin );

			rcSubItem.Offset( ptLVOrigin.X - ptCtlOrigin.X, ptLVOrigin.Y - ptCtlOrigin.Y );

			objControl.Bounds	= rcSubItem;
			objControl.Text		= objItem.SubItems[nSubItem].Text;
			objControl.Visible	= true;
			objControl.BringToFront();
			objControl.Focus();

			m_objEditControl		= objControl;
			m_objEditControl.Leave		+= new EventHandler(OnEditControlLeave);
			m_objEditControl.KeyPress	+= new KeyPressEventHandler(OnEditControlKeyPress);

			m_objEditItem		= objItem;
			m_nEditSubItem		= nSubItem;
		}

		private void OnEditControlLeave( object objSender, EventArgs objEventArgs )
		{
			EndEditing( true );
		}

		private void OnEditControlKeyPress( object objSender, System.Windows.Forms.KeyPressEventArgs objKeyEventArgs )
		{
			switch( objKeyEventArgs.KeyChar ) {
				case (char)(int)Keys.Escape :
					EndEditing( false );
					break;
				case (char)(int)Keys.Enter :
					EndEditing( true );
					break;
			}
		}

		public void EndEditing( bool bAcceptChanges )
		{
			if( m_objEditControl == null )
				return;

			SubItemEndEditingEventArgs objEventArgs = new SubItemEndEditingEventArgs(
				m_objEditItem,
				m_nEditSubItem,
				bAcceptChanges ? m_objEditControl.Text : m_objEditItem.SubItems[ m_nEditSubItem ].Text,
				!bAcceptChanges );

			OnSubItemEndEditing( objEventArgs );

			m_objEditItem.SubItems[ m_nEditSubItem ].Text = objEventArgs.DisplayText;

			m_objEditControl.Leave		-= new EventHandler( OnEditControlLeave );
			m_objEditControl.KeyPress	-= new KeyPressEventHandler( OnEditControlKeyPress );
			m_objEditControl.Visible	= false;
			m_objEditControl		= null; // release

			m_objEditItem			= null;
			m_nEditSubItem			= -1;
		}

		//public Control[]	Editors
		//{
		//        get { return m_arrEditors;	}
		//        set {
		//                m_arrEditors = new Control[ Columns.Count ];
		//                int nMinCount = ( Columns.Count > value.Length ) ? value.Length : Columns.Count;

		//                for( int nIndex = 0; nIndex < nMinCount; ++nIndex )
		//                        m_arrEditors[nIndex] = value[nIndex];

		//                if( Columns.Count > value.Length ) {
		//                        for( int nIndex = value.Length; nIndex < Columns.Count; ++nIndex )
		//                                m_arrEditors[nIndex] = new TextBox();
		//                }
		//        }
		//}

		private Control		m_objEditControl;
		private ListViewItem	m_objEditItem;
		private int		m_nEditSubItem;
		//private Control	[]	m_arrEditors = null;
		#endregion

		private bool		m_bDoubleClickActivation = false;
	}
}
