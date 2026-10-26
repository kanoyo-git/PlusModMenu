using System;

namespace PlusModMenu.Core.Common;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class CommentAttribute : Attribute
{
	public static void Make(string comment)
	{
	}

	public CommentAttribute(string comment)
	{
	}
}
