using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Artist
{
	namespace Picasso
	{

		public class ImageButton : Button
		{
			public enum ButtonState
			{
				eNormal = 0,	// button is in the normal state
				eHover,		// button is in the hover state
				ePressed,	// button is in the pressed state
				eDisable,	// button is in the disabled state
				eDefault	// button is in the default state
			};
			public Bitmap[] m_arrobjButtonImages = new Bitmap[4] { null, null, null, null };
			protected ButtonState m_eCurrentState = ButtonState.eNormal;
			protected bool m_bIsCanClick = false;
			protected bool m_bIsHasNoImage = true;

			//private System.ComponentModel.Container component = null;

			public ImageButton()
			{
				InitializeCompoent();
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			private void InitializeCompoent()
			{
			}

			#region Public Graphic Methods
			public void SetBitmaps(ButtonState eState, string strPath, bool bStretch)
			{
				switch( eState ) {
					case ButtonState.eNormal:
					case ButtonState.eHover:
					case ButtonState.ePressed:
					case ButtonState.eDisable:
						m_arrobjButtonImages[(int)eState] = (Bitmap)Bitmap.FromFile(strPath);
						if( bStretch )
							this.Size = new Size(m_arrobjButtonImages[(int)eState].Width, m_arrobjButtonImages[(int)eState].Height);
						m_bIsHasNoImage = false;
						break;
					default:
						m_bIsHasNoImage = true;
						return;
				}

				this.Invalidate();
			}

			public void SetBitmaps(string strNormal, string strHover, string strPressed, string strDisabled, bool bStretchByNormal)
			{
				if( strNormal != null && strNormal != "" )
					m_arrobjButtonImages[(int)ButtonState.eNormal] = (Bitmap)Bitmap.FromFile(strNormal);
				if( strHover != null && strHover != "" )
					m_arrobjButtonImages[(int)ButtonState.eHover] = (Bitmap)Bitmap.FromFile(strHover);
				if( strPressed != null && strPressed != "" )
					m_arrobjButtonImages[(int)ButtonState.ePressed] = (Bitmap)Bitmap.FromFile(strPressed);
				if( strDisabled != null && strDisabled != "" )
					m_arrobjButtonImages[(int)ButtonState.eDisable] = (Bitmap)Bitmap.FromFile(strDisabled);

				if( bStretchByNormal )
					this.Size = new Size(m_arrobjButtonImages[(int)ButtonState.eNormal].Width, m_arrobjButtonImages[(int)ButtonState.eNormal].Height);

				m_bIsHasNoImage = false;

				this.Invalidate();
			}
			#endregion

			#region RGN functions
			public void SetRegion(GraphicsPath objRegion)
			{
				this.Region = new Region(objRegion);
			}

			public void SetRegionByColor(string strImagePath, int nXPos, int nYPos)
			{
				Bitmap objBitmap = (Bitmap)Bitmap.FromFile(strImagePath);
				SetRegionByColor(objBitmap, objBitmap.GetPixel(nXPos, nYPos));
			}

			public void SetRegionByColor(string strImagePath, Color crCut)
			{
				Bitmap objBitmap = (Bitmap)Bitmap.FromFile(strImagePath);
				SetRegionByColor(objBitmap, crCut);
			}

			protected void SetRegionByColor(Bitmap objBitmap, Color crCut)
			{
				GraphicsPath objGrpPath = ImageUtility.GetRegionByColor(objBitmap, crCut);
				SetRegion(objGrpPath);
			}

			public void SetRegionByAlpha(string strImagePath, int nXPos, int nYPos)
			{
				Bitmap objBitmap = (Bitmap)Bitmap.FromFile(strImagePath);
				SetRegionByAlpha(objBitmap, objBitmap.GetPixel(nXPos, nYPos).A);
			}

			public void SetRegionByAlpha(string strImagePath, byte byOpacity)
			{
				Bitmap objBitmap = (Bitmap)Bitmap.FromFile(strImagePath);
				SetRegionByAlpha(objBitmap, byOpacity);
			}

			protected void SetRegionByAlpha(Bitmap objBitmap, byte byOpacity)
			{
				GraphicsPath objGrpPath = ImageUtility.GetRegionByAlpha(objBitmap, byOpacity);
				SetRegion(objGrpPath);
			}
			#endregion


			#region Mouse message handlers
			protected override void OnClick(EventArgs objEventArgs)
			{
				this.Capture = false;
				m_bIsCanClick = false;

				if( this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition)) )
					m_eCurrentState = ButtonState.eHover;
				else
					m_eCurrentState = ButtonState.eNormal;
				
				this.Invalidate();

				base.OnClick(objEventArgs);
			}

			protected override void OnMouseEnter(EventArgs objEventArgs)
			{
				base.OnMouseEnter(objEventArgs);

				m_eCurrentState = ButtonState.eHover;

				this.Invalidate();
			}

			protected override void OnMouseDown(MouseEventArgs objMouseEventArgs)
			{
				base.OnMouseDown(objMouseEventArgs);

				if( objMouseEventArgs.Button == MouseButtons.Left ) {
					m_bIsCanClick = true;
					m_eCurrentState = ButtonState.ePressed;

					this.Invalidate();
				}
			}

			protected override void OnMouseMove(MouseEventArgs objMouseEventArgs)
			{
				base.OnMouseMove(objMouseEventArgs);

				if( ClientRectangle.Contains(objMouseEventArgs.X, objMouseEventArgs.Y) ) {
					if( m_eCurrentState == ButtonState.eHover && this.Capture && !m_bIsCanClick ) {
						m_bIsCanClick = true;
						m_eCurrentState = ButtonState.ePressed;

						this.Invalidate();
					}
				} else {
					if( m_eCurrentState == ButtonState.ePressed ) {
						m_bIsCanClick = false;
						m_eCurrentState = ButtonState.eHover;

						this.Invalidate();
					}
				}
			}

			protected override void OnMouseLeave(EventArgs objEventArgs)
			{
				base.OnMouseLeave(objEventArgs);

				m_eCurrentState = ButtonState.eNormal;

				this.Invalidate();
			}
			#endregion

			#region Windows message handlers
			protected override void OnPaint(PaintEventArgs objPaintEventArgs)
			{
				if( !m_bIsHasNoImage ) {
					this.OnPaintBackground(objPaintEventArgs);

					switch( m_eCurrentState ) {
						case ButtonState.eNormal:
							if( this.Enabled ) {
								//if( this.Focused || this.IsDefault )
								//        OnDrawDefault(objPaintEventArgs.Graphics, objPaintEventArgs.ClipRectangle);
								//else
								OnDrawNormal(objPaintEventArgs.Graphics, objPaintEventArgs.ClipRectangle);
							} else
								OnDrawDisabled(objPaintEventArgs.Graphics, objPaintEventArgs.ClipRectangle);
							break;
						case ButtonState.eHover:
							OnDrawHover(objPaintEventArgs.Graphics, objPaintEventArgs.ClipRectangle);
							break;
						case ButtonState.ePressed:
							OnDrawPressed(objPaintEventArgs.Graphics, objPaintEventArgs.ClipRectangle);
							break;
						default:
							OnDrawDefault(objPaintEventArgs.Graphics, objPaintEventArgs.ClipRectangle);
							break;
					}
				} else
					base.OnPaint(objPaintEventArgs);
			}

			//protected override void OnPaintBackground(PaintEventArgs objPaintEventArgs)
			//{
			//        if( this.Parent != null ) {
			//                //IntPtr hParentDC = Win32.Win32GDI.GetDC(this.Parent.Handle);
			//                //IntPtr hMyDC = objPaintEventArgs.Graphics.GetHdc();

			//                //if( 0 == Win32.Win32GDI.BitBlt(hMyDC, 0, 0, Width, Height, hParentDC, this.Location.X, this.Location.Y, Win32.Win32GDI.SRCCOPY) ) {
			//                //        int a = 0;
			//                //}

			//                //objPaintEventArgs.Graphics.ReleaseHdc(hMyDC);
			//                //Win32.Win32GDI.ReleaseDC(this.Parent.Handle, hParentDC);
			//        }
			//}

			protected override void OnEnabledChanged(EventArgs objEventArgs)
			{
				base.OnEnabledChanged(objEventArgs);
				m_eCurrentState = ButtonState.eNormal;

				this.Invalidate();
			}

			new public void Invalidate()
			{
				if( this.Parent != null ) {
					Rectangle objRectAngle = this.ClientRectangle;
					objRectAngle = this.RectangleToScreen(objRectAngle);
					objRectAngle = this.Parent.RectangleToClient(objRectAngle);
					this.Parent.Invalidate(objRectAngle);
				}
				base.Invalidate();
			}

			#endregion

			#region Custom drawing function (protected)
			protected void OnDrawDefault(Graphics objGrp, Rectangle objClipRectangle)
			{
			}

			protected void OnDrawNormal(Graphics objGrp, Rectangle objClipRectangle)
			{
				Bitmap objImage = m_arrobjButtonImages[(int)ButtonState.eNormal];
				if( objImage == null )
					return;

				DrawImage(objGrp, objImage, objClipRectangle);
			}

			protected void OnDrawHover(Graphics objGrp, Rectangle objClipRectangle)
			{
				Bitmap objImage = m_arrobjButtonImages[(int)ButtonState.eHover];
				if( objImage == null )
					return;

				DrawImage(objGrp, objImage, objClipRectangle);
			}

			protected void OnDrawDisabled(Graphics objGrp, Rectangle objClipRectangle)
			{
				Bitmap objImage = m_arrobjButtonImages[(int)ButtonState.eDisable];
				if( objImage == null )
					return;

				DrawImage(objGrp, objImage, objClipRectangle);
			}

			protected void OnDrawPressed(Graphics objGrp, Rectangle objClipRectangle)
			{
				Bitmap objImage = m_arrobjButtonImages[(int)ButtonState.ePressed];
				if( objImage == null )
					return;

				DrawImage(objGrp, objImage, objClipRectangle);
			}

			protected void DrawImage(Graphics objGrp, Bitmap objImage, Rectangle objClipRectangle)
			{
				Rectangle rcDest = new Rectangle(0, 0, objClipRectangle.Width, objClipRectangle.Height);
				Rectangle rcSrc = new Rectangle(0, 0, objImage.Width, objImage.Height);
				GraphicsUnit objUnits = GraphicsUnit.Pixel;

				Color objColor = ((Bitmap)objImage).GetPixel(0, 0);

				objGrp.DrawImage(objImage, rcSrc, rcSrc, objUnits);
			}
			#endregion
		}
	}
}
