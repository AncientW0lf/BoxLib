using System;
using System.Globalization;

namespace BoxLib.Scripts
{
	public class LanguageHelper
	{
		public static Type Resources;

		/// <summary>
		/// Changes the currently active language of this project.
		/// </summary>
		/// <param name="culture">The culture to change to.</param>
		public static void ChangeLanguage(string culture, Type resources = null)
		{
			ChangeLanguage(new CultureInfo(culture), resources);
		}

		public static void ChangeLanguage(CultureInfo culture, Type resources = null)
		{
			if(resources == null)
				resources = Resources;

			resources?.GetProperty("Culture")?.SetValue(null, culture);
		}
	}
}
