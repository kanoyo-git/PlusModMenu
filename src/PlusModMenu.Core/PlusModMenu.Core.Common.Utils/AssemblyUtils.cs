using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace PlusModMenu.Core.Common.Utils;

public class AssemblyUtils
{
	public static bool IsAssemblyDebugBuild(Assembly assembly)
	{
		return assembly.GetCustomAttributes(inherit: false).OfType<DebuggableAttribute>().Any((DebuggableAttribute da) => da.IsJITTrackingEnabled);
	}
}
