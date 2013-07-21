using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Artist
{
	namespace Picasso
	{
		namespace Win32
		{
			public enum ListViewMessage
			{
				LVM_FIRST		= 0x1000,
				LVM_GETCOLUMNORDERARRAY	= (LVM_FIRST + 59),
			}

			public enum Win32Message
			{
				WM_NULL			= 0x00000000,
				WM_CREATE		= 0x00000001,
				WM_DESTROY		= 0x00000002,
				WM_MOVE			= 0x00000003,
				WM_SIZE			= 0x00000005,
				WM_ACTIVATE 		= 0x00000006,
				WM_SETFOCUS 		= 0x00000007,
				WM_KILLFOCUS		= 0x00000008,
				WM_ENABLE		= 0x0000000A,
				WM_SETREDRAW		= 0x0000000B,
				WM_SETTEXT		= 0x0000000C,
				WM_GETTEXT		= 0x0000000D,
				WM_GETTEXTLENGTH	= 0x0000000E,
				WM_PAINT		= 0x0000000F,
				WM_CLOSE		= 0x00000010,
				WM_QUERYENDSESSION	= 0x00000011,
				WM_QUERYOPEN		= 0x00000013,
				WM_ENDSESSION		= 0x00000016,
				WM_NOTIFY		= 0x0000004E,
				WM_HSCROLL		= 0x00000114,
				WM_VSCROLL		= 0x00000115,
				WM_GRAPHNOTIFY		= 0x00008001
			}

			public enum HEADERCONTROLMESSAGE
			{
				HDN_FIRST		= (int)-300,
				HDN_BEGINDRAG		= (int)-310,
				HDN_ITEMCHANGINGA	= (int)HDN_FIRST - 0,
				HDN_ITEMCHANGINGW	= (int)-320
			}

			public class Win32GDI
			{
				public Win32GDI() { }

				public enum Bool
				{
					False = 0,
					True
				};


				[StructLayout(LayoutKind.Sequential)]
				public struct Point
				{
					public Int32 x;
					public Int32 y;

					public Point(Int32 x, Int32 y) { this.x = x; this.y = y; }
				}


				[StructLayout(LayoutKind.Sequential)]
				public struct Size
				{
					public Int32 cx;
					public Int32 cy;

					public Size(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
				}


				[StructLayout(LayoutKind.Sequential, Pack = 1)]
				struct ARGB
				{
					public byte Blue;
					public byte Green;
					public byte Red;
					public byte Alpha;
				}


				[StructLayout(LayoutKind.Sequential, Pack = 1)]
				public struct BLENDFUNCTION
				{
					public byte BlendOp;
					public byte BlendFlags;
					public byte SourceConstantAlpha;
					public byte AlphaFormat;
				}


				public const Int32 ULW_COLORKEY = 0x00000001;
				public const Int32 ULW_ALPHA = 0x00000002;
				public const Int32 ULW_OPAQUE = 0x00000004;

				public const byte AC_SRC_OVER = 0x00;
				public const byte AC_SRC_ALPHA = 0x01;


				[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
				public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

				[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
				public static extern IntPtr GetDC(IntPtr hWnd);

				[DllImport("user32.dll", ExactSpelling = true)]
				public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

				[DllImport("user32.dll", ExactSpelling = true)]
				public static extern int ClientToScreen(IntPtr hWnd, ref Point point);

				[DllImport("user32.dll", ExactSpelling = true)]
				public static extern int ScreenToClient(IntPtr hWnd, ref Point point);

				[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
				public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

				[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
				public static extern Bool DeleteDC(IntPtr hdc);

				[DllImport("gdi32.dll", ExactSpelling = true)]
				public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

				[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
				public static extern Bool DeleteObject(IntPtr hObject);

				public const UInt32 SRCCOPY	= 0x00CC0020; // dest = source                   */
				public const UInt32 SRCPAINT	= 0x00EE0086; // dest = source OR dest           */
				public const UInt32 SRCAND	= 0x008800C6; // dest = source AND dest          */
				public const UInt32 SRCINVERT	= 0x00660046; // dest = source XOR dest          */
				public const UInt32 SRCERASE	= 0x00440328; // dest = source AND (NOT dest )   */
				public const UInt32 NOTSRCCOPY	= 0x00330008; // dest = (NOT source)             */
				public const UInt32 NOTSRCERASE	= 0x001100A6; // dest = (NOT src) AND (NOT dest) */
				public const UInt32 MERGECOPY	= 0x00C000CA; // dest = (source AND pattern)     */
				public const UInt32 MERGEPAINT	= 0x00BB0226; // dest = (NOT source) OR dest     */
				public const UInt32 PATCOPY	= 0x00F00021; // dest = pattern                  */
				public const UInt32 PATPAINT	= 0x00FB0A09; // dest = DPSnoo                   */
				public const UInt32 PATINVERT 	= 0x005A0049; // dest = pattern XOR dest         */
				public const UInt32 DSTINVERT 	= 0x00550009; // dest = (NOT dest)               */
				public const UInt32 BLACKNESS 	= 0x00000042; // dest = BLACK                    */
				public const UInt32 WHITENESS 	= 0x00FF0062; // dest = WHITE                    */

				[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
				public static extern Bool BitBlt(IntPtr hdcDest, Int32 nXDest, Int32 nYDest, Int32 nWidth, Int32 nHeight, IntPtr hdcSrc, Int32 nXSrc, Int32 nYSrc, UInt32 dwRop);

			}
		}
	}
}
