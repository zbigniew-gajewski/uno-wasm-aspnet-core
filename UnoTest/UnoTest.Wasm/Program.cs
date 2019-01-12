using System;
using Windows.UI.Xaml;

namespace UnoTest.Wasm
{
	public class Program
	{
		private static App _app;

		static void Main(string[] args)
		{
			Application.Start(_ => new App());
		}
	}
}
