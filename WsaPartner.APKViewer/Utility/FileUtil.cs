﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WsaPartner.APKViewer.Utility
{
    public static class FileUtil
    {
        public static void CalculateSHA1(Uri fileUri, PackageDataModel dataModel)
        {
            using (SHA1 sha = SHA1.Create())
            {
                using (FileStream stream = File.OpenRead(fileUri.OriginalString))
                {
                    dataModel.SHA1Hash = System.BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
                }
            }
        }

        public static async Task<byte[]> ZipExtractData(Uri fileUri, string zipEntry)
        {
            if (string.IsNullOrEmpty(zipEntry))
                return null;

            Debug.WriteLine("FileUtil.ZipExtractData() entry=" + zipEntry);

            using (ZipArchive za = ZipFile.Open(fileUri.OriginalString, ZipArchiveMode.Read))
            {
                ZipArchiveEntry iconEntry = null;
                try
                {
                    iconEntry = za.GetEntry(zipEntry);
                }
                catch (Exception)
                {
                    //do nothing
                }
                Debug.WriteLine("FileUtil.ZipExtractData() zip entry get. " + iconEntry.FullName);

                if (iconEntry != null)
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), "AAPToolTempImage.png");

                    iconEntry.ExtractToFile(tempPath, true);

                    using (Stream s = iconEntry.Open())
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Task copyTask = s.CopyToAsync(ms);
                        await copyTask;
                        return ms.ToArray();
                    }
                }
            }

            return null;
        }

        public static Task<string> ZipExtractDataIconPath(Uri fileUri, string zipEntry)
        {
            if (string.IsNullOrEmpty(zipEntry))
                return null;

            Debug.WriteLine("FileUtil.ZipExtractData() entry=" + zipEntry);

            using (ZipArchive za = ZipFile.Open(fileUri.OriginalString, ZipArchiveMode.Read))
            {
                ZipArchiveEntry iconEntry = null;
                try
                {
                    iconEntry = za.GetEntry(zipEntry);
                }
                catch (Exception)
                {
                    //do nothing
                }
                Debug.WriteLine("FileUtil.ZipExtractData() zip entry get. " + iconEntry.FullName);

                if (iconEntry != null)
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), "WsaTempImage.png");

                    iconEntry.ExtractToFile(tempPath, true);
                    return Task.FromResult(tempPath);
                    //using (Stream s = iconEntry.Open())
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //	Task copyTask = s.CopyToAsync(ms);
                    //	await copyTask;
                    //	return ms.ToArray();
                    //}
                }
            }

            return null;
        }
    }
}