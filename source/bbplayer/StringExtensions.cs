using System;
using System.Text;

namespace iLandMan.Utility
{
    public static class StringExtensions
    {
        /// <summary>
        /// Folds the specified text and the <paramref name="characterToFoldAt"/> point.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="characterToFoldAt">The character to fold at.</param>
        /// <returns></returns>
        public static string Fold( this string text, int characterToFoldAt )
        {
            if ( text.Length <= characterToFoldAt )
            {
                return text;
            }

            return string.Format( "{0}...", text.Substring( 0, characterToFoldAt ) );
        }

        /// <summary>
        /// Adds spaces were appropriate to make PascalCased text presentable. (i.e. PascalCased -> Pascal Cased)
        /// </summary>
        /// <param name="pascalText">The pascal text.</param>
        /// <returns></returns>
        public static string PascalCaseToWord( this string pascalText )
        {
            if ( string.IsNullOrEmpty( pascalText ) )
            {
                return string.Empty;
            }

            var sbText = new StringBuilder( pascalText.Length + 4 );

            char[] chars = pascalText.ToCharArray();

            sbText.Append( chars[0] );

            bool lastWasNumber = Char.IsDigit( chars[0] );
            bool lastWasUpper = Char.IsUpper( chars[0] );

            for ( int i = 1; i < chars.Length; i++ )
            {
                bool spaceAppended = false;
                char c = chars[i];

                if ( Char.IsUpper( c ) && ! lastWasUpper )
                {
                    sbText.Append( ' ' );
                    spaceAppended = true;
                }

                // Account for acronymns
                lastWasUpper = Char.IsUpper( c );
                if ( lastWasUpper && i + 1 < chars.Length && Char.IsLower( chars[i+1] ) && ! spaceAppended )
                {
                    sbText.Append( ' ' );
                    spaceAppended = true;
                }

                if ( !Char.IsDigit( c ) )
                {
                    lastWasNumber = false;
                }
                else if ( !lastWasNumber && ! spaceAppended )
                {
                    sbText.Append( ' ' );
                    spaceAppended = true;
                    lastWasNumber = true;
                }

                sbText.Append( c );
            }

            return sbText.ToString();            
        }

        public static string ToBase64String( this string input )
        {
            return input.ToBase64String( false );
        }

        public static string ToBase64String( this string input, bool removeTrailingEquals )
        {
            byte[] bytes = Encoding.UTF8.GetBytes( input );
            string base64 = Convert.ToBase64String( bytes );            

            if ( removeTrailingEquals )
            {
                base64 = base64.TrimEnd( new[] { '=' } );
            }

            return base64;
        }

        public static string FromBase64String( this string input )
        {
            int paddedLength = ( 4 - ( input.Length % 4 ) ) % 4;
            input = input.PadRight( input.Length + paddedLength, '=' );

            byte[] bytes = Convert.FromBase64String( input );
            return Encoding.UTF8.GetString( bytes );
        }
    }
}