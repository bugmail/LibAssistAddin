using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;

using Artist.Picasso.Win32;

namespace Artist
{
	namespace Picasso
	{
		public class ImageForm : Form
		{
			#region Member variables
			#endregion

			public ImageForm()
			{
				FormBorderStyle = FormBorderStyle.None;
			}

			#region Public graphic methods
			public void SetBackgroundImage(string strImagePath, bool bStretch)
			{
				BackgroundImage = new Bitmap(strImagePath);

				if( bStretch )
					this.Size = new Size(BackgroundImage.Width, BackgroundImage.Height);
			}

			public void SetBackgroundImage(Image objImage, bool bStretch)
			{
				BackgroundImage = new Bitmap(objImage);
				if( bStretch )
					this.Size = new Size(BackgroundImage.Width, BackgroundImage.Height);
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

			#region Windows message handlers
			protected override void OnPaint(PaintEventArgs objPaintEventArgs)
			{
				base.OnPaint(objPaintEventArgs);
			}

			protected override void OnPaintBackground(PaintEventArgs objPaintEventArgs)
			{
				if( BackgroundImage != null ) {
					Rectangle rcDest = new Rectangle(0, 0, objPaintEventArgs.ClipRectangle.Width, objPaintEventArgs.ClipRectangle.Height);
					Rectangle rcSrc = new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height);

					GraphicsUnit objUnits = GraphicsUnit.Pixel;

					Win32GDI.Point ptThisFormWindowLocation = new Win32GDI.Point(0, 0);
					Point ptThisFormClientLocation = new Point(0, 0);
					Win32GDI.ClientToScreen(Handle, ref ptThisFormWindowLocation);
					//objPaintEventArgs.Graphics.CopyFromScreen(ptThisFormWindowLocation.x, ptThisFormWindowLocation.y, 0, 0, new Size(Size.Width, Size.Height), CopyPixelOperation.SourceCopy);
					objPaintEventArgs.Graphics.DrawImage(BackgroundImage, rcDest, rcSrc, objUnits);
				}
				//base.OnPaintBackground(e);
			}

			protected override void OnMouseMove(MouseEventArgs objMouseEventArgs)
			{
				if( objMouseEventArgs.Button == MouseButtons.Left ) {
					Point ptCurrent = new Point(objMouseEventArgs.X, objMouseEventArgs.Y);
					IntPtr lParam = new IntPtr();
					lParam = (IntPtr)(((UInt16)objMouseEventArgs.X << 16) | ((UInt16)objMouseEventArgs.Y));
					Message.Create(Handle, (int)Win32Message.WM_MOVE, IntPtr.Zero, lParam);
				} else
					base.OnMouseMove(objMouseEventArgs);
			}

			#endregion
		}
	}
}