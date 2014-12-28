﻿using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.InteropServices;

namespace Files.Models
{
    public class File
    {
        public File(string path)
        {
            var fileInfo = new FileInfo(path);

            Name = System.IO.Path.GetFileName(path);
            Path = path;
            Size = fileInfo.Length;
            Type = GetMimeTypeFromFile(path);
            Uploaded = fileInfo.CreationTimeUtc;
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string Type { get; set; }

        [Display(Name = "Uploaded (UTC)")]
        [DisplayFormat(DataFormatString = "{0:F}")]
        public DateTime Uploaded { get; set; }

        public static string FormatSize(long size)
        {
            // File size format code credit: http://stackoverflow.com/a/4975942
            string[] suf = {"B", "KB", "MB", "GB", "TB", "PB", "EB"}; // Longs run out around EB
            if (size == 0)
                return "0" + suf[0];

            var bytes = Math.Abs(size);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes/Math.Pow(1024, place), 1);

            return (Math.Sign(size)*num) + " " + suf[place];
        }

        public static string GetMimeTypeFromFile(string path)
        {
            // Get mime from file credit: http://stackoverflow.com/a/22475295
            const int maxContent = 256;

            var buffer = new byte[maxContent];
            using (var fs = new FileStream(path, FileMode.Open))
            {
                if (fs.Length >= maxContent)
                    fs.Read(buffer, 0, maxContent);
                else
                    fs.Read(buffer, 0, (int) fs.Length);
            }

            var mimeTypePtr = IntPtr.Zero;
            try
            {
                var result = FindMimeFromData(IntPtr.Zero, null, buffer, maxContent, null, 0, out mimeTypePtr, 0);
                if (result != 0)
                {
                    Marshal.FreeCoTaskMem(mimeTypePtr);
                    throw Marshal.GetExceptionForHR(result);
                }

                var mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);

                return mime;
            }
            catch (Exception e)
            {
                if (mimeTypePtr != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(mimeTypePtr);

                return "unknown";
            }
        }

        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        private static extern int FindMimeFromData(IntPtr pBC,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
            int cbSize,
            [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
            int dwMimeFlags,
            out IntPtr ppwzMimeOut,
            int dwReserved);
    }
}