
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WsaPartner.APKViewer.Utility;

namespace WsaPartner.APKViewer.Decoders
{
	public class DefaultAPKDecoder: IFileDecoder
	{
		private ICmdPathProvider pathProvider;
		private Uri targetFilePath;
		private PackageDataModel dataModel;

		private string _iconName;

		private List<string> _iconPath;

		public event Action decodeProgressCallbackEvent;

		public DefaultAPKDecoder(ICmdPathProvider newPathProvider)
		{
			dataModel = null;
			pathProvider = newPathProvider;
		}

		public void SetFilePath(Uri fileUri)
		{
			targetFilePath = fileUri;
		}

		public async Task Decode()
		{
			Debug.WriteLine("DefaultAPKDecoder.Decode() decode start.");
			dataModel = new PackageDataModel();

			Debug.WriteLine("DefaultAPKDecoder.Decode() Decode_Badging start.");
			await Decode_Badging();
			decodeProgressCallbackEvent?.Invoke();
			Debug.WriteLine("DefaultAPKDecoder.Decode() Decode_Icon start.");
			
			await Decode_LargeIcon();

			await Decode_LargeIconName();

			await Decode_IconPath();
			//await Decode_Icon();
			decodeProgressCallbackEvent?.Invoke();
			Debug.WriteLine("DefaultAPKDecoder.Decode() Decode_Signature start.");
			await Decode_Signature();
			decodeProgressCallbackEvent?.Invoke();
			Debug.WriteLine("DefaultAPKDecoder.Decode() Decode_Hash start.");
			Decode_Hash();

			Debug.WriteLine("DefaultAPKDecoder.Decode() decode finish.");
			decodeProgressCallbackEvent?.Invoke();
		}

		private async Task Decode_Badging()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = pathProvider.GetAAPTPath(),
				Arguments = " d badging \"" + targetFilePath.OriginalString + "\""
			};
			Debug.WriteLine("DefaultAPKDecoder.Decode_Badging(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi);

			dataModel.RawDumpBadging = processResult;
			DesktopCMDAAPTUtil.ReadBadging(dataModel, dataModel.RawDumpBadging);
		}

		private async Task Decode_LargeIcon()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = pathProvider.GetAAPTPath(),
				Arguments = $"dump xmltree \"{targetFilePath.OriginalString}\" \"AndroidManifest.xml\""
			};
			Debug.WriteLine("DefaultAPKDecoder.Decode_Badging(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi);

			string[] arrayStr = Regex.Split(processResult, "\r\n");

			var listStr = arrayStr.ToList();

			 _iconName = listStr.Where(s=>s.Contains("android:icon")).FirstOrDefault().Split('@')[1];

			//dataModel.RawDumpBadging = processResult;
			//DesktopCMDAAPTUtil.ReadBadging(dataModel, dataModel.RawDumpBadging);
		}

		private async Task Decode_LargeIconName()
		{
			ProcessStartInfo psi = new ProcessStartInfo()
			{
				FileName = pathProvider.GetAAPTPath(),
				Arguments = $"dump --values resources \"{targetFilePath.OriginalString}\""
			};
			Debug.WriteLine("DefaultAPKDecoder.Decode_Badging(), path=" + targetFilePath.OriginalString);
			string processResult = await ProcessExecuter.ExecuteProcess(psi);

			string[] arrayStr = Regex.Split(processResult, "\r\n");

			var listStr = arrayStr.ToList();

			var iconList = new List<string>();

            if (listStr.Any())
            {
				var configNames = Enum.GetNames(typeof(Configs)).Reverse();

				const char seperator = '\"';

				foreach (var itemStr in listStr)
                {
					if(Regex.IsMatch(itemStr, $"^\\s*resource\\s{_iconName}"))
                    {
						var index = listStr.IndexOf(itemStr);

						 var resValue = listStr[index + 1];

						 var config = configNames.FirstOrDefault(c => itemStr.Contains(c));

						if (Regex.IsMatch(resValue, @"^\s*\((string\d*)\)*"))
						{
							// Resource value is icon url
							var iconName = resValue.Split(seperator)
								.FirstOrDefault(n => n.Contains("/"));
							iconList.Add(iconName);
							break;
						}
						if (Regex.IsMatch(resValue, @"^\s*\((reference)\)*"))
						{
							var iconID = resValue.Trim().Split(' ')[1];
							iconList.Add(iconID);
							break;
						}
					}
                }
            }
			_iconPath = iconList;
			//dataModel.RawDumpBadging = processResult;
			//DesktopCMDAAPTUtil.ReadBadging(dataModel, dataModel.RawDumpBadging);
		}

		private async Task Decode_Icon()
		{
			dataModel.MaxIconContent = await FileUtil.ZipExtractData(targetFilePath, dataModel.MaxIconZipEntry);
		}

		private async Task Decode_IconPath()
		{
			dataModel.IconPath = await FileUtil.ZipExtractDataIconPath(targetFilePath,_iconPath.FirstOrDefault());
		}

		private async Task Decode_Signature()
		{
			await DesktopJavaUtil.TestJavaExist();

			if (!DesktopJavaUtil.javaExist)
			{
				dataModel.Signature = "Java is not found, can't read apk signature.";
				return;
			}

			ProcessStartInfo psiAPKSigner = new ProcessStartInfo
			{
				FileName = "java",
				Arguments = "-jar " + pathProvider.GetAPKSignerPath() +
					" verify --verbose --print-certs" +
					" \"" + targetFilePath.OriginalString + "\"",
			};
			string processResult = await ProcessExecuter.ExecuteProcess(psiAPKSigner, false, false);

			dataModel.RawDumpSignature = processResult;
			DesktopCMDAPKSignerUtil.ReadAPKSignature(dataModel, processResult);
		}

		private void Decode_Hash()
		{
			FileUtil.CalculateSHA1(targetFilePath, dataModel);
		}

		public PackageDataModel GetDataModel()
		{
			return dataModel;
		}

	}
}