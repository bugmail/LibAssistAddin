using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Artist
{
	namespace Picasso
	{
		public class ImageUtility
		{
			public ImageUtility() { }

			#region RGN functions
			public static GraphicsPath GetRegionByColor(string strGraphicFilePath, Color crCut)
			{
				Bitmap objBitmap = new Bitmap(strGraphicFilePath);
				return GetRegionByColor(objBitmap, crCut);
			}

			public static GraphicsPath GetRegionByColor(string strGraphicFilePath, int nXPos, int nYPos)
			{
				Bitmap objBitmap = new Bitmap(strGraphicFilePath);
				return GetRegionByColor(objBitmap, objBitmap.GetPixel(nXPos, nYPos));
			}

			public static GraphicsPath GetRegionByColor(Bitmap objBitmap, int nXPos, int nYPos)
			{
				return GetRegionByColor(objBitmap, objBitmap.GetPixel(nXPos, nYPos));
			}

			public static GraphicsPath GetRegionByColor(Bitmap objBitmap, Color crCut)
			{
				int nWidth = objBitmap.Width;
				int nHeight = objBitmap.Height;
				Rectangle objRectangle = new Rectangle();
				GraphicsPath objGrpPath = new GraphicsPath();
				for( int nY = 0; nY < nHeight; ++nY ) {
					objRectangle.X = objRectangle.Width = -1;
					objRectangle.Y = nY;
					objRectangle.Height = 1;
					for( int nX = 0; nX < nWidth; ++nX ) {
						if( crCut != objBitmap.GetPixel(nX, nY) ) {
							if( objRectangle.X == -1 )
								objRectangle.X = nX;

							objRectangle.Width = nX - objRectangle.X;

							if( nX == (nWidth - 1) ) {
								if( (objRectangle.Width != 0 && objRectangle.Width != -1) ) {
									objGrpPath.AddRectangle(objRectangle);
									objRectangle.X = objRectangle.Width = -1;
								}
							}
						} else {
							if( (objRectangle.Width != 0 && objRectangle.Width != -1) ) {
								objGrpPath.AddRectangle(objRectangle);
								objRectangle.X = objRectangle.Width = -1;
							}
						}
					}
				}
				return objGrpPath;
			}

			public static GraphicsPath GetRegionByAlpha(string strGraphicFilePath, byte byOpacity)
			{
				Bitmap objBitmap = new Bitmap(strGraphicFilePath);
				return GetRegionByAlpha(objBitmap, byOpacity);
			}

			public static GraphicsPath GetRegionByAlpha(string strGraphicFilePath, int nXPos, int nYPos)
			{
				Bitmap objBitmap = new Bitmap(strGraphicFilePath);
				return GetRegionByAlpha(objBitmap, objBitmap.GetPixel(nXPos, nYPos).A);
			}

			public static GraphicsPath GetRegionByAlpha(Bitmap objBitmap, int nXPos, int nYPos)
			{
				return GetRegionByAlpha(objBitmap, objBitmap.GetPixel(nXPos, nYPos).A);
			}

			public static GraphicsPath GetRegionByAlpha(Bitmap objBitmap, byte byOpacity)
			{
				int nWidth = objBitmap.Width;
				int nHeight = objBitmap.Height;
				Rectangle objRectangle = new Rectangle();
				GraphicsPath objGrpPath = new GraphicsPath();
				for( int nY = 0; nY < nHeight; ++nY ) {
					objRectangle.X = objRectangle.Width = -1;
					objRectangle.Y = nY;
					objRectangle.Height = 1;
					for( int nX = 0; nX < nWidth; ++nX ) {
						if( byOpacity < objBitmap.GetPixel(nX, nY).A ) {
							if( objRectangle.X == -1 )
								objRectangle.X = nX;

							objRectangle.Width = nX - objRectangle.X;

							if( nX == (nWidth - 1) ) {
								if( (objRectangle.Width != 0 && objRectangle.Width != -1) ) {
									objGrpPath.AddRectangle(objRectangle);
									objRectangle.X = objRectangle.Width = -1;
								}
							}
						} else {
							if( (objRectangle.Width != 0 && objRectangle.Width != -1) ) {
								objGrpPath.AddRectangle(objRectangle);
								objRectangle.X = objRectangle.Width = -1;
							}
						}
					}
				}
				return objGrpPath;
			}
			#endregion
		}
	}
}