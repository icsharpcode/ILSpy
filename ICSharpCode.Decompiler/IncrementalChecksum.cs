using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ICSharpCode.Decompiler
{
	public class IncrementalChecksum: IDisposable
    {
        IncrementalHash _hash;
        IncrementalHash calchash;
        TextWriter logger = new StringWriter();

        /// <summary>
        /// Can temporarily enable or disable hash calculation
        /// </summary>
        public bool Enabled {
            get {
                return calchash != null;
            }

            set {
                if (value)
                {
                    calchash = _hash;
                }
                else
                {
                    calchash = null;
                }
            }
        }

        public void EnableChecksumCalculation(HashAlgorithmName hashName)
        {
            _hash = calchash = IncrementalHash.CreateHash(hashName);
        }

        public void EnableChecksumLog(string assemblyFileName)
        {
            string log = assemblyFileName + ".log.txt";
            logger = new StreamWriter(log, false, Encoding.UTF8);
            logger.WriteLine($"Checksumming file '{assemblyFileName}'...");
        }

        /// <summary>
        /// Adds string data to hash
        /// </summary>
        public void AppendString(string s)
        {
            if (calchash == null)
            {
                return;
            }

            logger.WriteLine($"- AppendString: '{s}'");
            calchash.AppendData(Encoding.UTF8.GetBytes(s));
        }

        const int binaryBytesToShow = 6;

        /// <summary>
        /// Adds stream data to hash
        /// </summary>
        public void AppendStreamData(string fileName, Stream s)
        {
            if (calchash == null)
            {
                return;
            }

            const int bufferSize = 4096;
            byte[] buf = new byte[bufferSize];
            var pos = s.Position;
            bool firstChunk = true;
            string bufTag = "<empty>";
            int toDump;

            while (true)
            {
                int readed = s.Read(buf, 0, bufferSize);
                if (readed <= 0)
                {
                    if (!firstChunk)
                    {
                        bufTag += " ... " + ByteArrayToString(buf, buf.Length - binaryBytesToShow, binaryBytesToShow);
                    }
                    break;
                }

                if (firstChunk)
                {
                    firstChunk = false;
                    toDump = binaryBytesToShow;
                    if (toDump > readed)
                    {
                        toDump = readed;
                    }
                    
                    bufTag = ByteArrayToString(buf, 0, toDump);
                }
                
                calchash.AppendData(buf, 0, readed);
            }

            AppendString(fileName);
            logger.WriteLine($"- AppendStreamData: '{bufTag}' (length: {s.Position})");
            s.Seek(pos, SeekOrigin.Begin);
        }

        /// <summary>
        /// Gets hash value as a string
        /// </summary>
        /// <returns></returns>
        public string GetHashString()
        {
            var hashv = calchash.GetHashAndReset();
            string s = ByteArrayToString(hashv);
            logger.WriteLine($"Resulting checksum: {s}");
            return s;
        }

        public static string ByteArrayToString(byte[] ba, int offset = 0, int sizeToProcess = -1)
        {
            if (sizeToProcess == -1)
            {
                sizeToProcess = ba.Length - offset;
            }

            StringBuilder hex = new StringBuilder(sizeToProcess * 3);
            for (int i = 0; i < sizeToProcess; i++)
            {
                int off = i + offset;
                if (off >= ba.Length)	// Flip over buffer
                {
                    off -= ba.Length;
                }
                hex.AppendFormat("{0:X2} ", ba[off]);
            }

            var s = hex.ToString();
            if (s.Length == 0)
            {
                return s;
            }

            return s.Substring(0, s.Length - 1);
        }

        public void AppendBinaryData(byte[] bytes)
        {
            calchash.AppendData(bytes);
        }

        public void AppendAndLogBinaryData(byte[] bytes)
        {
            string bufTag;
            if (bytes.Length > binaryBytesToShow * 2)
            {
                bufTag = ByteArrayToString(bytes, 0, binaryBytesToShow) + " ... " +
                    ByteArrayToString(bytes, bytes.Length - binaryBytesToShow, binaryBytesToShow);
            }
            else
            {
                bufTag = ByteArrayToString(bytes, 0, bytes.Length);
            }

            logger.WriteLine($"- AppendAndLogBinaryData: '{bufTag}' (length: {bytes.Length})");

            calchash.AppendData(bytes);
        }

        public void Dispose()
        {
            logger.Close();
        }
    }
}
