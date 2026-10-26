using System.Globalization;
using System.Resources;

namespace ModMenu.Settings;

public sealed class LocalizedResourceManager : ResourceManager
{
	private readonly ResourceManager _primary;

	private readonly ResourceManager _fallback;

	public LocalizedResourceManager(ResourceManager primary, ResourceManager fallback)
	{
		_primary = primary;
		_fallback = fallback;
	}

	public override string GetString(string name)
	{
		return GetString(name, null);
	}

	public override string GetString(string name, CultureInfo culture)
	{
		return Lookup(_primary, name, culture) ?? _fallback.GetString(name, culture);
	}

	public override object GetObject(string name)
	{
		return GetObject(name, null);
	}

	public override object GetObject(string name, CultureInfo culture)
	{
		return LookupObject(_primary, name, culture) ?? _fallback.GetObject(name, culture);
	}

	private static string Lookup(ResourceManager manager, string name, CultureInfo culture)
	{
		try
		{
			return manager.GetString(name, culture);
		}
		catch (MissingManifestResourceException)
		{
			return null;
		}
	}

	private static object LookupObject(ResourceManager manager, string name, CultureInfo culture)
	{
		try
		{
			return manager.GetObject(name, culture);
		}
		catch (MissingManifestResourceException)
		{
			return null;
		}
	}
}
