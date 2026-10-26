using System;
using System.Globalization;

namespace PlusModMenu.Api.Settings;

public readonly partial struct UIText : IEquatable<UIText>
{
	private static readonly object[] NoArgs = new object[0];

	private readonly object[] _args;

	public Type ResourceType { get; }

	public string Value { get; }

	public object[] Args => _args ?? NoArgs;

	public bool IsEmpty => string.IsNullOrEmpty(Value);

	private UIText(Type resourceType, string value, object[] args)
	{
		ResourceType = resourceType;
		Value = value;
		_args = args;
	}

	public static UIText Literal(string text)
	{
		return new UIText(null, text, null);
	}

	public static UIText Resource<TResource>(string key, params object[] args)
	{
		return Resource(typeof(TResource), key, args);
	}

	public static UIText Resource(Type resourceType, string key, params object[] args)
	{
		if (resourceType == null)
		{
			throw new ArgumentNullException(nameof(resourceType));
		}
		return new UIText(resourceType, key, args != null && args.Length > 0 ? args : null);
	}

	public static implicit operator UIText(string literal)
	{
		return Literal(literal);
	}

	public string Resolve(Func<Type, string, string> lookup)
	{
		if (IsEmpty)
		{
			return string.Empty;
		}
		var text = ResourceType == null ? Value : lookup(ResourceType, Value) ?? Value;
		if (Args.Length == 0)
		{
			return text;
		}
		try
		{
			return string.Format(CultureInfo.InvariantCulture, text, Args);
		}
		catch (FormatException)
		{
			return text;
		}
	}

	public bool Equals(UIText other)
	{
		if (ResourceType != other.ResourceType || !string.Equals(Value ?? string.Empty, other.Value ?? string.Empty, StringComparison.Ordinal) || Args.Length != other.Args.Length)
		{
			return false;
		}
		for (var i = 0; i < Args.Length; i++)
		{
			if (!Equals(Args[i], other.Args[i]))
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		return obj is UIText other && Equals(other);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			var hash = (ResourceType?.GetHashCode() ?? 0) * 397 ^ (Value ?? string.Empty).GetHashCode();
			foreach (var arg in Args)
			{
				hash = hash * 31 + (arg?.GetHashCode() ?? 0);
			}
			return hash;
		}
	}

	public static bool operator ==(UIText left, UIText right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(UIText left, UIText right)
	{
		return !left.Equals(right);
	}

	public override string ToString()
	{
		return ResourceType == null ? Value ?? string.Empty : ResourceType.Name + ":" + Value;
	}
}
