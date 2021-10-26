using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WsaPartner.APKViewer;

namespace WsaPartner.Services
{
	public class WindowsCmdPath : ICmdPathProvider
	{
		public const string FOLDER_EXTERNAL_TOOL = "CMDTools";
		public const string EXE_AAPT = "aapt.exe";
		public const string JAR_APKSIGNER = "apksigner.jar";
		public const string EXE_ADB = "adb.exe";
		public const string BAT_BUNDLETOOL = "bundletool.bat";

		public string GetAAPTPath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_EXTERNAL_TOOL, EXE_AAPT);
		}

		public string GetAPKSignerPath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_EXTERNAL_TOOL, JAR_APKSIGNER);
		}

		public string GetADBPath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_EXTERNAL_TOOL, EXE_ADB);
		}

		public string GetBundleToolPath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FOLDER_EXTERNAL_TOOL, BAT_BUNDLETOOL);
		}
	}
}
