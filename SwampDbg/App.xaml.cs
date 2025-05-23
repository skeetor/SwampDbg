namespace SwampDbg;

using Microsoft.Extensions.Configuration;
using SwampDbg.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;

/// <summary>
/// Startup code for SwampDbg
/// </summary>
public partial class App : Application
{
	private static IConfiguration _config = null;

	public App()
		: base()
	{
		Trace.WriteLine("Startuppath: " + Directory.GetCurrentDirectory());
		_config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
					.AddEnvironmentVariables()
					.Build();
	}

	private void ApplicationStartup(object sender, StartupEventArgs e)
	{
		//var generalConfig = builder.GetSection("GeneralConfig").Get<GeneralConfig>();
		var generalConfig = App.GetConfig<GeneralConfig>();

		if (e.Args.Length > 0)
		{
		}

		// Create the startup window
		MainWindow wnd = new MainWindow();
		wnd.Show();

		Trace.WriteLine("Started");
	}

	private void ApplicationUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
	{
		MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
		e.Handled = true;
	}

	/**
	 * Get a configuration entry where the name of the section matches the corresponding class.
	 */
	public static T? GetConfig<T>() => App.GetConfig<T>(typeof(T).Name);

	/**
	 * Get a configuration instance from an arbitrary section name.
	 */
	public static T? GetConfig<T>(string sectionName) => _config.GetSection(sectionName).Get<T>();
}
