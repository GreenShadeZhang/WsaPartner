﻿using System.Collections.Generic;

namespace WsaPartner.APKViewer
{
    public class PackageDataModel
    {
        public string AppNameResourceEntry { get; set; } = "";
        public string AppName { get; set; } = "";
        public Dictionary<string, string> AppNameLangDict { get; set; } = new Dictionary<string, string>();
        public string PackageName { get; set; } = "";
        public string VersionString { get; set; } = "";
        public string VersionCode { get; set; } = "";
        public string MinSDKCode { get; set; } = "";
        public string MaxSDKCode { get; set; } = "";
        public List<string> ScreenSize { get; set; } = new List<string>();
        public List<string> Densities { get; set; } = new List<string>();
        public List<string> Permissions { get; set; } = new List<string>();
        public List<string> Feature_Require { get; set; } = new List<string>();
        public List<string> Feature_NotRequire { get; set; } = new List<string>();
        public List<string> Architecture { get; set; } = new List<string>();
        public string OpenGLVersion { get; set; } = "";
        public string Signature { get; set; } = "";
        public string SHA1Hash { get; set; } = "";
        public string AppIconResourceEntry { get; set; } = "";
        public string MaxIconZipEntry { get; set; } = "";
        public byte[] MaxIconContent { get; set; }

        public string RawDumpBadging { get; set; } = "";
        public string RawDumpSignature { get; set; } = "";
        public string IconPath { get; set; } = "ms-appx:///Assets/SmallTile.scale-200.png";
    }
}
