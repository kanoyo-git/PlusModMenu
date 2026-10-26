using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using PlusModMenu.Core.Unity;
using MidiPlayerTK;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

internal class VertexGlitchingModifiers : IModifiers
{
	private static readonly int _vertexGlitchSeed = Shader.PropertyToID("_VertexGlitchSeed");

	private static readonly int _vertexGlitchIntensity = Shader.PropertyToID("_VertexGlitchIntensity");

	private static readonly int _tileVertexGlitchSeed = Shader.PropertyToID("_TileVertexGlitchSeed");

	private static readonly int _tileVertexGlitchIntensity = Shader.PropertyToID("_TileVertexGlitchIntensity");

	private static float _glitchVal;

	private bool _overrideVertexGlitching;

	[DataMember]
	[Label<Strings>("OVERRIDE_VERTEX_GLITCHING")]
	[Help<HelpText>("OVERRIDE_VERTEX_GLITCHING_TITLE", "OVERRIDE_VERTEX_GLITCHING_TEXT")]
	[Group<SideMenuGroupOtherModifiersVertexGlitching>]
	public virtual bool OverrideVertexGlitching
	{
		get
		{
			return _overrideVertexGlitching;
		}
		set
		{
			_overrideVertexGlitching = value;
			if (value)
			{
				MusicManager.OnMidiEvent += MidiEvent;
			}
			else
			{
				MusicManager.OnMidiEvent -= MidiEvent;
			}
		}
	}

	[DataMember]
	[Label<Strings>("GLITCH_INTENSITY")]
	[Help<HelpText>("GLITCH_INTENSITY_TITLE", "GLITCH_INTENSITY_TEXT")]
	[Group<SideMenuGroupOtherModifiersVertexGlitching>]
	public virtual float GlitchIntensity { get; set; } = 3f;

	private void MidiEvent(MPTKEvent midiEvent)
	{
		if (_glitchVal <= 0f)
		{
			new TaskManager.SimpleTask(UnGlitch()).Start();
		}
		_glitchVal = 1f;
		Shader.SetGlobalFloat(_vertexGlitchSeed, Random.Range(0f, 1000f));
		Shader.SetGlobalFloat(_tileVertexGlitchSeed, Random.Range(0f, 1000f));
		SetIntensity(_glitchVal * GlitchIntensity);
	}

	private IEnumerator UnGlitch()
	{
		yield return null;
		while (_glitchVal > 0f)
		{
			_glitchVal -= Time.deltaTime * 4f;
			SetIntensity(_glitchVal * GlitchIntensity);
			yield return null;
		}
		_glitchVal = 0f;
		SetIntensity(0f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void SetIntensity(float value)
	{
		Shader.SetGlobalFloat(_vertexGlitchIntensity, value);
		Shader.SetGlobalFloat(_tileVertexGlitchIntensity, value);
	}
}
