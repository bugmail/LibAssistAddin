using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;

namespace Artist
{
	namespace Picasso
	{
		public class NumericTextBox : TextBox
		{
			protected bool m_bIsAllowSpace = false;

			public bool AllowSpace
			{
				get { return m_bIsAllowSpace; }
				set { m_bIsAllowSpace = value; }
			}

			protected override void OnKeyPress(KeyPressEventArgs objKeyPressEvent)
			{
				NumberFormatInfo objNumberFormat = CultureInfo.CurrentCulture.NumberFormat;
				string strDecimalSeparator = objNumberFormat.NumberDecimalSeparator;
				string strGroupSeparator = objNumberFormat.NumberGroupSeparator;
				string strNegativeSign = objNumberFormat.NegativeSign;

				string strKeyInput = objKeyPressEvent.KeyChar.ToString();

				if( char.IsDigit(objKeyPressEvent.KeyChar) || char.IsControl(objKeyPressEvent.KeyChar) ) {
					if( objKeyPressEvent.KeyChar == ' ' ) {
						if( !AllowSpace )
							objKeyPressEvent.Handled = true;
					}
					base.OnKeyPress(objKeyPressEvent);
				} else
					objKeyPressEvent.Handled = true;
			}
		}
	}
}