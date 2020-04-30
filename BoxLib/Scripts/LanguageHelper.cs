using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace BoxLib.Scripts
{
	public class LanguageHelper
	{
		private readonly Type _resources;

		private readonly string _langListName;

		public LanguageHelper(Type resources, string langListName = null)
		{
			_resources = resources;
			_langListName = langListName;
		}

		/// <summary>
		/// Retrieves a list of all supported languages of a project.
		/// </summary>
		[Obsolete("Not intuitive. Please use GetSupportedLanguages instead.")]
		public string[] GetLanguages()
		{
			try
			{
				//Opens the file with a list of languages
				using Stream languageFile = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{_resources.Name}.{_langListName}.txt");
				using var reader = new StreamReader(languageFile ?? throw new InvalidOperationException());

				//Returns the splitted string with languages
				return reader.ReadToEnd().Split(new []{"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);
			}
			catch(InvalidOperationException)
			{
				return null;
			}
		}

		public CultureInfo[] GetSupportedLanguages()
		{
			var supported = new List<CultureInfo>();

			// Pass the class name of your resources as a parameter e.g. MyResources for MyResources.resx
			var rm = new ResourceManager(_resources);

			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			for(int i = 0; i < cultures.Length; i++)
			{
				CultureInfo culture = cultures[i];
				try
				{
					ResourceSet rs = rm.GetResourceSet(culture, true, false);

					if(rs != null)
					{
						supported.Add(culture);
					}
				}
				catch(CultureNotFoundException)
				{
					//Not supported / Invalid
				}
			}

			return supported.ToArray();
		}

		/// <summary>
		/// Changes the currently active language of this project.
		/// </summary>
		/// <param name="culture">The culture to change to.</param>
		/// <param name="resources">The class that stores all resource files.</param>
		public void ChangeLanguage(string culture, Type resources = null)
		{
			ChangeLanguage(new CultureInfo(culture), resources);
		}

		/// <summary>
		/// Changes the currently active language of this project.
		/// </summary>
		/// <param name="culture">The culture to change to.</param>
		/// <param name="resources">The class that stores all resource files.</param>
		public void ChangeLanguage(CultureInfo culture, Type resources = null)
		{
			if(resources == null)
				resources = _resources;

			resources?.GetProperty("Culture")?.SetValue(null, culture);
		}
	}
}
