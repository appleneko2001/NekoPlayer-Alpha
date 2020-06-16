using System;
using System.Collections.Generic;
using System.Text;
using ManagedBass;
using System.IO;
using System.Runtime.InteropServices;

namespace NekoPlayer.Core
{
    /// <summary>
    /// An simple bass BASS_FILEPROCS class function implements with FileStream base, can be used on local storage only
    /// </summary>
    public class BassFileStream : FileStream, TagLib.File.IFileAbstraction
    {
        /// <summary>
        /// Create a BassFileStream object, and start FileStream with read-only mode
        /// </summary>
        /// <param name="path">File source</param>
        public BassFileStream(string path) : base(path, FileMode.Open)
        {
            bass_fs = new FileProcedures() { Close = BassFileClose, Length = BassFileLength, Read = BassFileRead, Seek = BassFileSeek };//new BASS_FILEPROCS(BassFileClose, BassFileLength, BassFileRead, BassFileSeek);
        }

        private FileProcedures bass_fs;

        public Stream ReadStream => this;

        public Stream WriteStream => Stream.Null; // read-only stream

        private void BassFileClose(IntPtr user)
        {
            Close();
            Dispose();
        }
        private long BassFileLength(IntPtr user)
        {
            return Length;
        }
        private int BassFileRead(IntPtr buffer, int length, IntPtr user)
        {
            try
            {
                if (CanRead)
                {
                    byte[] data = new byte[length];
                    int bytesread = Read(data, 0, length);
                    Marshal.Copy(data, 0, buffer, bytesread);
                    return bytesread;
                }
                else
                {
                    // Returns empty data.
                    Marshal.Copy(Array.Empty<byte>(), 0, buffer, 0);
                    return 0;
                }
            }
            catch(ObjectDisposedException)
            {
                Marshal.Copy(Array.Empty<byte>(), 0, buffer, 0);
                return 0;
            }
        }
        private bool BassFileSeek(long offset, IntPtr user)
        {
            try
            {
                long pos = Seek(offset, SeekOrigin.Begin);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public FileProcedures GetBassFileController()
        {
            return bass_fs;
        }
        public bool IsDisposed()
        {
            return CanRead;
        }

        public void CloseStream(Stream stream)
        {
        }
    }
}
