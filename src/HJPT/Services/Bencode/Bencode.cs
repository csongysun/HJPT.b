using System;
using System.IO;
using System.Text;
using BencodeNET.IO;
using BencodeNET.Objects;

namespace BencodeNET
{
    public static class Bencode
    {
        private const int Int64MaxDigits = 19;

        private static Encoding _defaultEncoding = Encoding.UTF8;

        /// <summary>
        /// Gets or sets the default encoding used to convert strings to and from bytes
        /// when encoding/decoding bencode and no encoding is explicitly specified.
        /// </summary>
        public static Encoding DefaultEncoding
        {
            get { return _defaultEncoding; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "DefaultEncoding may not be set to null");
                _defaultEncoding = value;
            }
        }

        /// <summary>
        /// Decodes the specified bencoded string using the default encoding.
        /// </summary>
        /// <param name="bencodedString">The bencoded string.</param>
        /// <returns>An <see cref="IBObject"/> representing the bencoded string.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IBObject Decode(string bencodedString)
        {
            if (bencodedString == null) throw new ArgumentNullException("bencodedString");

            return Decode(bencodedString, DefaultEncoding);
        }

        /// <summary>
        /// Decodes the specified bencoded string using the specified encoding.
        /// </summary>
        /// <param name="bencodedString">The bencoded string.</param>
        /// <param name="encoding">The encoding used to convert the string to bytes.</param>
        /// <returns>An <see cref="IBObject"/> representing the bencoded string.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IBObject Decode(string bencodedString, Encoding encoding)
        {
            if (bencodedString == null) throw new ArgumentNullException("bencodedString");
            if (encoding == null) throw new ArgumentNullException("encoding");

            using (var ms = new MemoryStream(encoding.GetBytes(bencodedString)))
            {
                return Decode(ms, encoding);
            }
        }

        /// <summary>
        /// Decodes the specified stream using the default encoding.
        /// </summary>
        /// <param name="stream">The stream to decode.</param>
        /// <returns>An <see cref="IBObject"/> representing the bencoded stream.</returns>
        /// <exception cref="ArgumentNullException">stream</exception>
        public static IBObject Decode(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return Decode(stream, DefaultEncoding);
        }

        /// <summary>
        /// Decodes the specified stream using the specified encoding.
        /// </summary>
        /// <param name="stream">The stream to decode.</param>
        /// /// <param name="encoding">The encoding used by <see cref="BString"/> when calling <c>ToString()</c> with no arguments.</param>
        /// <returns>An <see cref="IBObject"/> representing the bencoded stream.</returns>
        /// <exception cref="ArgumentNullException">stream</exception>
        public static IBObject Decode(Stream stream, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (encoding == null) throw new ArgumentNullException("encoding");

            return Decode(new BencodeStream(stream, leaveOpen: true), encoding);
        }

        /// <summary>
        /// Decodes the specified stream using the specified encoding.
        /// </summary>
        /// <param name="stream">The stream to decode.</param>
        /// /// <param name="encoding">The encoding used by <see cref="BString"/> when calling <c>ToString()</c> with no arguments.</param>
        /// <returns>An <see cref="IBObject"/> representing the bencoded stream.</returns>
        /// <exception cref="ArgumentNullException">stream</exception>
        public static IBObject Decode(BencodeStream stream, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (encoding == null) throw new ArgumentNullException("encoding");

            switch (stream.PeekChar())
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9': return DecodeString(stream, encoding);
                case 'i': return DecodeNumber(stream);
                case 'l': return DecodeList(stream, encoding);
                case 'd': return DecodeDictionary(stream, encoding);
            }

            // TODO: Throw BencodeDecodingException because next char was not a valid start of a BObject?
            return null;
        }

        public static BString DecodeString(string bencodedString)
        {
            if (bencodedString == null) throw new ArgumentNullException("bencodedString");

            return DecodeString(bencodedString, DefaultEncoding);
        }

        public static BString DecodeString(string bencodedString, Encoding encoding)
        {
            if (bencodedString == null) throw new ArgumentNullException("bencodedString");
            if (encoding == null) throw new ArgumentNullException("encoding");

            using (var ms = new MemoryStream(encoding.GetBytes(bencodedString)))
            {
                return DecodeString(ms, encoding);
            }
        }

        public static BString DecodeString(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return DecodeString(stream, DefaultEncoding);
        }

        public static BString DecodeString(Stream stream, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (encoding == null) throw new ArgumentNullException("encoding");

            return DecodeString(new BencodeStream(stream, leaveOpen: true), encoding);
        }

        public static BString DecodeString(BencodeStream stream, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (encoding == null) throw new ArgumentNullException("encoding");

            // Minimum valid bencode string is '0:' meaning an empty string
            if (stream.Length < 2)
                throw new InvalidDataException("Minimum valid stream length is 2 (an empty string: '0:')");

            var startPosition = stream.Position;

            var lengthString = new StringBuilder();
            for (var c = stream.ReadChar(); c != ':' && c != default(char); c = stream.ReadChar())
            {
                // Because of memory limitations (~1-2 GB) we know for certain we cannot handle more than 10 digits (10GB)
                if (lengthString.Length >= BString.LengthMaxDigits)
                {
                    throw new InvalidDataException(
                        string.Format("Length of string is more than {0} digits (>10GB) and is not supported (max is ~1-2GB).", BString.LengthMaxDigits));
                }

                lengthString.Append(c);
            }

            long stringLength;
            if (!TryParseLongFast(lengthString.ToString(), out stringLength))
            {
                throw new InvalidDataException(string.Format("Invalid length of string '{0}'", lengthString));
            }

            // Int32.MaxValue is ~2GB and is the absolute maximum that can be handled in memory
            if (stringLength > int.MaxValue)
            {
                throw new InvalidDataException(
                    string.Format("Length of string is {0:N0} but maximum supported length is {1:N0}.", stringLength, int.MaxValue));
            }

            var bytes = stream.Read((int)stringLength);

            // If the two don't match we've reached the end of the stream before reading the expected number of chars
            if (bytes.Length != stringLength)
            {
                throw new InvalidDataException(
                    string.Format("Expected string to be {0:N0} bytes long but could only read {1:N0} bytes.", stringLength, bytes.Length));
            }

            return new BString(bytes, encoding);
        }

        public static BNumber DecodeNumber(string bencodedString)
        {
            if (bencodedString == null) throw new ArgumentNullException("bencodedString");

            using (var ms = new MemoryStream(DefaultEncoding.GetBytes(bencodedString)))
            {
                return DecodeNumber(ms);
            }
        }

        public static BNumber DecodeNumber(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return DecodeNumber(new BencodeStream(stream, leaveOpen: true));
        }

        public static BNumber DecodeNumber(BencodeStream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (stream.Length < 3)
                throw new InvalidDataException("Minimum valid length of stream is 3 ('i0e').");

            var startPosition = stream.Position;

            // Numbers must start with 'i'
            if (stream.ReadChar() != 'i')
                throw new InvalidDataException(string.Format("Must begin with 'i' but began with '{0}'.", stream.ReadPreviousChar()));

            var digits = new StringBuilder();
            char c;
            for (c = stream.ReadChar(); c != 'e' && c != default(char); c = stream.ReadChar())
            {
                digits.Append(c);
            }

            // Last read character should be 'e'
            if (c != 'e')
                throw new InvalidDataException();

            var isNegative = digits[0] == '-';
            var numberOfDigits = isNegative ? digits.Length - 1 : digits.Length;

            // We do not support numbers that cannot be stored as a long (Int64)
            if (numberOfDigits > BNumber.MaxDigits)
            {
                throw new InvalidDataException();

            }

            // We need at least one digit
            if (numberOfDigits < 1)
                throw new InvalidDataException();

            var firstDigit = isNegative ? digits[1] : digits[0];

            // Leading zeros are not valid
            if (firstDigit == '0' && numberOfDigits > 1)
                throw new InvalidDataException();

            // '-0' is not valid either
            if (firstDigit == '0' && numberOfDigits == 1 && isNegative)
                throw new InvalidDataException();

            long number;
            if (!TryParseLongFast(digits.ToString(), out number))
            {
                throw new InvalidDataException();

            }

            return new BNumber(number);
        }

        public static BList DecodeList(string bencodedString)
        {
            return DecodeList(bencodedString, DefaultEncoding);
        }

        public static BList DecodeList(string bencodedString, Encoding encoding)
        {
            if (bencodedString == null) throw new ArgumentNullException("bencodedString");
            if (encoding == null) throw new ArgumentNullException("encoding");

            using (var ms = new MemoryStream(encoding.GetBytes(bencodedString)))
            {
                return DecodeList(ms, encoding);
            }
        }

        public static BList DecodeList(Stream stream)
        {
            return DecodeList(stream, DefaultEncoding);
        }

        public static BList DecodeList(Stream stream, Encoding encoding)
        {
            return DecodeList(new BencodeStream(stream, leaveOpen: true), encoding);
        }

        public static BList DecodeList(BencodeStream stream, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (encoding == null) throw new ArgumentNullException("encoding");

            if (stream.Length < 2)
                throw new InvalidDataException();

            // Lists must start with 'l'
            if (stream.ReadChar() != 'l')
                throw new InvalidDataException();

            var list = new BList();
            // Loop until next character is the end character 'e' or end of stream
            while (stream.Peek() != 'e' && stream.Peek() != -1)
            {
                // Decode next object in stream
                var bObject = Decode(stream, encoding);
                if (bObject == null)
                    throw new InvalidDataException();

                list.Add(bObject);
            }

            if (stream.ReadChar() != 'e')
                throw new InvalidDataException();

            return list;
        }

        public static BDictionary DecodeDictionary(string bencodedString)
        {
            return DecodeDictionary(bencodedString, DefaultEncoding);
        }

        public static BDictionary DecodeDictionary(string bencodedString, Encoding encoding)
        {
            if (bencodedString == null) throw new ArgumentNullException("bencodedString");
            if (encoding == null) throw new ArgumentNullException("encoding");

            using (var ms = new MemoryStream(encoding.GetBytes(bencodedString)))
            {
                return DecodeDictionary(ms, encoding);
            }
        }

        public static BDictionary DecodeDictionary(Stream stream)
        {
            return DecodeDictionary(stream, DefaultEncoding);
        }

        public static BDictionary DecodeDictionary(Stream stream, Encoding encoding)
        {
            return DecodeDictionary(new BencodeStream(stream, leaveOpen: true), encoding);
        }

        public static BDictionary DecodeDictionary(BencodeStream stream, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (encoding == null) throw new ArgumentNullException("encoding");

            var startPosition = stream.Position;

            if (stream.Length < 2)
                throw new InvalidDataException();

            // Dictionaries must start with 'd'
            if (stream.ReadChar() != 'd')
                throw new InvalidDataException();

            var dictionary = new BDictionary();
            // Loop until next character is the end character 'e' or end of stream
            while (stream.Peek() != 'e' && stream.Peek() != -1)
            {
                // Decode next string in stream as the key
                BString key;
                try
                {
                    key = DecodeString(stream, encoding);
                }
                catch (InvalidDataException ex)
                {
                    throw new InvalidDataException();
                }

                // Decode next object in stream as the value
                var value = Decode(stream, encoding);
                if (value == null)
                    throw new InvalidDataException();

                dictionary.Add(key, value);
            }

            if (stream.ReadChar() != 'e')
                throw new InvalidDataException();

            return dictionary;
        }

        public static TorrentFile DecodeTorrentFile(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return DecodeTorrentFile(stream);
            }
        }

        public static TorrentFile DecodeTorrentFile(Stream stream)
        {
            return DecodeTorrentFile(stream, DefaultEncoding);
        }

        public static TorrentFile DecodeTorrentFile(Stream stream, Encoding encoding)
        {
            return DecodeTorrentFile(new BencodeStream(stream, leaveOpen: true), encoding);
        }

        public static TorrentFile DecodeTorrentFile(BencodeStream stream, Encoding encoding)
        {
            var bdictionary = DecodeDictionary(stream, encoding);
            return new TorrentFile(bdictionary);
        }

        /// <summary>
        /// A faster implementation than <see cref="long.TryParse(string, out long)"/>
        /// because we skip some checks that are not needed.
        /// </summary>
        private static bool TryParseLongFast(string value, out long result)
        {
            result = 0;

            if (value == null)
                return false;

            var length = value.Length;

            // Cannot parse empty string
            if (length == 0)
                return false;

            var isNegative = value[0] == '-';
            var startIndex = isNegative ? 1 : 0;

            // Cannot parse just '-'
            if (isNegative && length == 1)
                return false;

            // Cannot parse string longer than long.MaxValue
            if (length - startIndex > Int64MaxDigits)
                return false;

            long parsedLong = 0;
            for (var i = startIndex; i < length; i++)
            {
                var character = value[i];
                if (!char.IsDigit(character))
                    return false;

                var digit = character - '0';

                if (isNegative)
                    parsedLong = 10 * parsedLong - digit;
                else
                    parsedLong = 10 * parsedLong + digit;
            }

            // Negative - should be less than zero
            if (isNegative && parsedLong >= 0)
                return false;

            // Positive - should be equal to or greater than zero
            if (!isNegative && parsedLong < 0)
                return false;

            result = parsedLong;
            return true;
        }
    }
}
