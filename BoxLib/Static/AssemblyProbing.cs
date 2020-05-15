using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BoxLib.Static
{
	public static class AssemblyProbing
	{
		private static readonly Dictionary<string, ResolveEventHandler> ProbingPathSubs =
			new Dictionary<string, ResolveEventHandler>();

		/// <summary>
		/// Loads an assembly when it is needed from the specified directory.
		/// </summary>
		/// <param name="path">The path to probe for assemblies.</param>
		/// <param name="args">The <see cref="EventArgs"/> that were given by the <see cref="AppDomain.AssemblyResolve"/> event.</param>
		/// <returns>The correct assembly that was requested.</returns>
		/// <exception cref="ArgumentNullException">Thrown when the assembly directory is in an invalid location.</exception>
		private static Assembly LoadAssembly(string path, ResolveEventArgs args)
		{
			//Gets the directory of the executing assembly (this application)
			string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			//Gets the path to the requested assembly
			string assemblyPath = Path.Combine(path, new AssemblyName(args.Name).Name + ".dll");

			//Doesn't return anything if the assembly was not found
			if (!File.Exists(assemblyPath)) return null;

			//Loads the requested assembly
			Assembly assembly = Assembly.LoadFrom(assemblyPath);

			return assembly;
		}

		/// <summary>
		/// Add a new path to probe for assemblies.
		/// </summary>
		/// <param name="path">The FULL path to the directory which contains assemblies</param>
		public static void AddProbingPath(string path)
		{
			var method = new ResolveEventHandler((_, args) => LoadAssembly(path, args));
			AppDomain.CurrentDomain.AssemblyResolve += method;

			if(ProbingPathSubs.ContainsKey(path))
				RemoveProbingPath(path);

			ProbingPathSubs.Add(path, method);
		}
		
		/// <summary>
		/// Removes a path to probe for assemblies.
		/// </summary>
		/// <param name="path">The FULL path to the directory which contains assemblies</param>
		public static void RemoveProbingPath(string path)
		{
			if(!ProbingPathSubs.ContainsKey(path))
				return;

			AppDomain.CurrentDomain.AssemblyResolve -= ProbingPathSubs[path];
			ProbingPathSubs.Remove(path);
		}
	}
}
