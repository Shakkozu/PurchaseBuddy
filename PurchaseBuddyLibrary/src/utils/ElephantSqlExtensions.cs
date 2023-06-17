namespace PurchaseBuddyLibrary.src.utils;
public static class ElephantSqlExtensions
{
	public static string ToConnectionString(this string? uriString)
	{
		if (string.IsNullOrEmpty(uriString))
			return string.Empty;

		var uri = new Uri(uriString);
		var db = uri.AbsolutePath.Trim('/');
		var user = uri.UserInfo.Split(':')[0];
		var passwd = uri.UserInfo.Split(':')[1];
		var port = uri.Port > 0 ? uri.Port : 5432;
		var connStr = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4};MaxPoolSize=5;",
			uri.Host, db, user, passwd, port);
		return connStr;
	}
}
