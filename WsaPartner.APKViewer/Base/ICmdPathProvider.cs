namespace WsaPartner.APKViewer
{
	public interface ICmdPathProvider
	{
		string GetAAPTPath();
		string GetAPKSignerPath();
		string GetADBPath();
		string GetBundleToolPath();

	}
}