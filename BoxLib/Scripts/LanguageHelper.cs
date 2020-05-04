using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace BoxLib.Scripts
{
	/// <summary>
	/// This class can help you setup multi-language support for your application.
	/// It can get and change languages of satellite assemblies.
	/// </summary>
	public class LanguageHelper
	{
		/// <summary>
		/// The resource assembly containing all languages.
		/// </summary>
		private readonly Type _resources;

		/// <summary>
		/// The full path to the embedded text file containing a list of supported languages.
		/// </summary>
		[Obsolete]
		private readonly string _langListName;

		/// <summary>
		/// The <see cref="CultureInfo"/> to use in place of <see cref="CultureInfo.InvariantCulture"/> when
		/// retrieving a list of supported languages.
		/// </summary>
		private readonly CultureInfo _invariantCulture;

		/// <summary>
		/// Initializes a new <see cref="LanguageHelper"/> with a link to the specified <see cref="resources"/> class.
		/// </summary>
		/// <param name="resources">The resource assembly containing all languages.</param>
		public LanguageHelper(Type resources)
		{
			_resources = resources;
		}

		/// <summary>
		/// Initializes a new <see cref="LanguageHelper"/> with a link to the specified <see cref="resources"/> class.
		/// </summary>
		/// <param name="resources">The resource assembly containing all languages.</param>
		/// <param name="neutralLang">The <see cref="CultureInfo"/> to use in place of
		/// <see cref="CultureInfo.InvariantCulture"/> when retrieving a list of supported languages.</param>
		public LanguageHelper(Type resources, CultureInfo neutralLang)
		{
			_resources = resources;
			_invariantCulture = neutralLang;
		}

		/// <summary>
		/// Initializes a new <see cref="LanguageHelper"/> with a link to the specified <see cref="resources"/> class.
		/// </summary>
		/// <param name="resources">The resource assembly containing all languages.</param>
		/// <param name="langListName">The full path to the embedded text file containing a list of supported languages.</param>
		[Obsolete("Not intuitive. Please use one of the new constructors instead.")]
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

		/// <summary>
		/// Retrieves a list of all supported languages of a project.
		/// </summary>
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
						if(_invariantCulture != null && culture.Equals(CultureInfo.InvariantCulture))
							culture = _invariantCulture;

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
		public void ChangeLanguage(string culture)
		{
			ChangeLanguage(new CultureInfo(culture));
		}

		/// <summary>
		/// Changes the currently active language of this project.
		/// </summary>
		/// <param name="culture">The culture to change to.</param>
		/// <param name="resources">The class that stores all resource files.</param>
		public void ChangeLanguage(CultureInfo culture)
		{
			_resources?.GetProperty("Culture")?.SetValue(null, culture);
		}
	}
}
