using System.Linq;
using UnityEngine;

namespace PlusModMenu.Core.Unity;

public class Platform
{
	public enum Category
	{
		Unknown,
		Windows,
		MacOS,
		Linux,
		Android,
		IOS,
		WebApp,
		Salty
	}

	public static Category Current
	{
		get
		{
			if (IsWindows)
			{
				return Category.Windows;
			}
			if (IsMacOS)
			{
				return Category.MacOS;
			}
			if (IsLinux)
			{
				return Category.Linux;
			}
			if (IsAndroid)
			{
				return Category.Android;
			}
			if (IsIOS)
			{
				return Category.IOS;
			}
			if (IsWebApp)
			{
				return Category.WebApp;
			}
			if (IsSalty)
			{
				return Category.Salty;
			}
			return Category.Unknown;
		}
	}

	public static bool IsWindows => new RuntimePlatform[9]
	{
		RuntimePlatform.WindowsEditor,
		RuntimePlatform.WindowsPlayer,
		RuntimePlatform.MetroPlayerARM,
		RuntimePlatform.MetroPlayerX64,
		RuntimePlatform.MetroPlayerX86,
		RuntimePlatform.WP8Player,
		RuntimePlatform.MetroPlayerARM,
		RuntimePlatform.MetroPlayerX64,
		RuntimePlatform.MetroPlayerX86
	}.Any((RuntimePlatform r) => Application.platform == r);

	public static bool IsMacOS => Application.platform == RuntimePlatform.OSXPlayer;

	public static bool IsLinux => new RuntimePlatform[2]
	{
		RuntimePlatform.LinuxEditor,
		RuntimePlatform.LinuxPlayer
	}.Any((RuntimePlatform r) => Application.platform == r);

	public static bool IsAndroid => Application.platform == RuntimePlatform.Android;

	public static bool IsIOS => new RuntimePlatform[2]
	{
		RuntimePlatform.IPhonePlayer,
		RuntimePlatform.tvOS
	}.Any((RuntimePlatform r) => Application.platform == r);

	public static bool IsWebApp => new RuntimePlatform[2]
	{
		RuntimePlatform.FlashPlayer,
		RuntimePlatform.WebGLPlayer
	}.Any((RuntimePlatform r) => Application.platform == r);

	public static bool IsSalty => Application.platform == RuntimePlatform.NaCl;
}
