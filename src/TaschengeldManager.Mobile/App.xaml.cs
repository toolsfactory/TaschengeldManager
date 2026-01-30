using Microsoft.Extensions.DependencyInjection;
using TaschengeldManager.Mobile.Services.Feedback;

namespace TaschengeldManager.Mobile;

public partial class App : Application
{
	public App(IGlobalExceptionHandler exceptionHandler)
	{
		InitializeComponent();

		// Initialize global exception handler
		exceptionHandler.Initialize();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}