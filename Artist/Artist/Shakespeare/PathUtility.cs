using System;
using System.IO;
using Microsoft.Win32;
using System.Text;

namespace Artist
{
	namespace Shakespeare
	{
		public class PathUtility
		{
			public PathUtility()
			{
			}

			public static bool MatchFileName(string strPattern, string strFileName, bool bCaseSensitive)
			{
				StringBuilder objPattern = new StringBuilder(strPattern);
				StringBuilder objFileName = new StringBuilder(Path.GetFileName(strFileName));

				int nFileNameIndex  = 0;
                int nPatternIndex   = 0;
				for( nPatternIndex = 0; nPatternIndex < objPattern.Length && nFileNameIndex < objFileName.Length; ++nPatternIndex ) {
					switch( objPattern[nPatternIndex] ) {
						case '*':
							++nPatternIndex;
							switch( objPattern[nPatternIndex] ) {
								case '/' :
								case '\\':
									do {
										if( objFileName[nFileNameIndex] == m_chDirectoryDelimiter )
											break;
									} while( '\0' != objFileName[++nFileNameIndex] );

									if( objFileName[nFileNameIndex] == '\0' )
										return false;
									++nFileNameIndex;
									break;
								case '*':
								case '?':
									break;
								case '\"':
								case '<':
								case '>':
								case '|':
									return false;
								case '.':
									nFileNameIndex = FindCharacter(objFileName, '.', nFileNameIndex);
									if( -1 == nFileNameIndex )
										return false;

									++nFileNameIndex;
									break;
								default:
									if( bCaseSensitive )
										nFileNameIndex = FindCharacter(objFileName, objPattern[nPatternIndex], nFileNameIndex);
									else
										nFileNameIndex = FindCharacterCaseInsensitive(objFileName, objPattern[nPatternIndex], nFileNameIndex);

									if( -1 == nFileNameIndex )
										return false;
									++nFileNameIndex;
									break;
							}
							break;
						case '?':
							break;
						case '/':
						case '\\':
							break;
						case '\"':
						case '<':
						case '>':
						case '|':
							return false;
						default:
							if( bCaseSensitive ) {
								if( objPattern[nPatternIndex] != objFileName[nFileNameIndex] )
									return false;
							} else {
								if( char.ToUpper(objPattern[nPatternIndex]) != char.ToUpper(objFileName[nFileNameIndex]) )
									return false;
								++nFileNameIndex;
							}
							break;
					}
				}

				if( nFileNameIndex == objFileName.Length && nPatternIndex == objPattern.Length )
					return true;
				else
					return false;
			}


			public static char DirectoryDelimiter
			{
				get { return m_chDirectoryDelimiter;	}
				set { m_chDirectoryDelimiter = value;	}
			}
			protected static char m_chDirectoryDelimiter = '\0';

			private static int FindCharacter(StringBuilder objBuilder, char chFindit, int nStartIndex)
			{
				int nFoundIndex = nStartIndex;
				for( ; nStartIndex < objBuilder.Length; ++nStartIndex )
					if( objBuilder[nStartIndex] == chFindit )
						return nStartIndex;

				return -1;
			}

			private static int FindCharacterCaseInsensitive(StringBuilder objBuilder, char chFindit, int nStartIndex)
			{
				int nFoundIndex = nStartIndex;
				chFindit = char.ToUpper(chFindit);
				for( ; nStartIndex < objBuilder.Length; ++nStartIndex )
					if( char.ToUpper(objBuilder[nStartIndex]) == chFindit )
						return nStartIndex;

				return -1;
			}
		};
	};
};