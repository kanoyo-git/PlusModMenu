using System.Collections;
using System.Runtime.Serialization;
using PlusModMenu.Core.Unity;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

[DataContract]
internal class ClownVomitModifiers : IModifiers
{
	private static readonly int _colorGlitching = Shader.PropertyToID("_ColorGlitching");

	private static readonly int _colorGlitchVal = Shader.PropertyToID("_ColorGlitchVal");

	private static readonly int _colorGlitchPercent = Shader.PropertyToID("_ColorGlitchPercent");

	private static readonly int _spriteColorGlitching = Shader.PropertyToID("_SpriteColorGlitching");

	private static readonly int _spriteColorGlitchVal = Shader.PropertyToID("_SpriteColorGlitchVal");

	private static readonly int _spriteColorGlitchPercent = Shader.PropertyToID("_SpriteColorGlitchPercent");

	private static readonly TaskManager.SimpleTask _environmentRoutine = new TaskManager.SimpleTask(ClownVomitEnvironment);

	private static readonly TaskManager.SimpleTask _spritesRoutine = new TaskManager.SimpleTask(ClownVomitSprites);

	private bool _environmentEnabled;

	private bool _spritesEnabled;

	private float _swapTime = 0.5f;

	[DataMember]
	[Label<Strings>("CLOWN_VOMIT_ON_ENVIRONMENT")]
	[Help<HelpText>("CLOWN_VOMIT_ON_ENVIRONMENT_TITLE", "CLOWN_VOMIT_ON_ENVIRONMENT_TEXT")]
	[Group<SideMenuGroupOtherModifiersClownVomit>]
	public virtual bool EnvironmentEnabled
	{
		get
		{
			return _environmentEnabled;
		}
		set
		{
			if (value)
			{
				Shader.SetGlobalInt(_colorGlitching, 1);
				_environmentRoutine.Reset();
				_environmentRoutine.Start();
			}
			else
			{
				Shader.SetGlobalInt(_colorGlitching, 0);
				_environmentRoutine.Stop();
			}
			_environmentEnabled = value;
		}
	}

	[DataMember]
	[Label<Strings>("CLOWN_VOMIT_ON_SPRITES")]
	[Help<HelpText>("CLOWN_VOMIT_ON_SPRITES_TITLE", "CLOWN_VOMIT_ON_SPRITES_TEXT")]
	[Group<SideMenuGroupOtherModifiersClownVomit>]
	public virtual bool SpritesEnabled
	{
		get
		{
			return _spritesEnabled;
		}
		set
		{
			if (value)
			{
				Shader.SetGlobalInt(_spriteColorGlitching, 1);
				_spritesRoutine.Reset();
				_spritesRoutine.Start();
			}
			else
			{
				Shader.SetGlobalInt(_spriteColorGlitching, 0);
				_spritesRoutine.Stop();
			}
			_spritesEnabled = value;
		}
	}

	[DataMember]
	[Label<Strings>("SWAP_TIME")]
	[Help<HelpText>("SWAP_TIME_TITLE", "SWAP_TIME_TEXT")]
	[Group<SideMenuGroupOtherModifiersClownVomit>]
	public virtual float SwapTime
	{
		get
		{
			return _swapTime;
		}
		set
		{
			_swapTime = value;
			if (EnvironmentEnabled)
			{
				EnvironmentEnabled = false;
				EnvironmentEnabled = true;
			}
			if (SpritesEnabled)
			{
				SpritesEnabled = false;
				SpritesEnabled = true;
			}
		}
	}

	private static IEnumerator ClownVomitEnvironment()
	{
		ClownVomitModifiers clownVomit = Service.Get<IModMenuService>().Get<ClownVomitModifiers>();
		float swapTime = clownVomit.SwapTime;
		while (true)
		{
			swapTime -= Time.deltaTime;
			if (swapTime <= 0f)
			{
				Shader.SetGlobalFloat(_colorGlitchPercent, Random.value);
				Shader.SetGlobalFloat(_colorGlitchVal, Random.Range(0, 4096));
				swapTime = clownVomit.SwapTime;
			}
			yield return null;
		}
	}

	private static IEnumerator ClownVomitSprites()
	{
		ClownVomitModifiers clownVomit = Service.Get<IModMenuService>().Get<ClownVomitModifiers>();
		float swapTime = clownVomit.SwapTime;
		while (true)
		{
			swapTime -= Time.deltaTime;
			if (swapTime <= 0f)
			{
				Shader.SetGlobalFloat(_spriteColorGlitchPercent, Random.value);
				Shader.SetGlobalFloat(_spriteColorGlitchVal, Random.Range(0f, 1f));
				swapTime = clownVomit.SwapTime;
			}
			yield return null;
		}
	}
}
