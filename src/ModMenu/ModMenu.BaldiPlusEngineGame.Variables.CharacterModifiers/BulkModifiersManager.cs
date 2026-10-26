using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Core.Common.Utils;
using ModMenu.BaldiPlusEngineGame.Services;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using ModMenu.Settings;
using ModMenu.Settings.Editors;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;

internal class BulkModifiersManager
{
	public static void CreateBulkModifiers()
	{
		BuildPage();
	}

	private static void BuildPage()
	{
		SettingsRegistry registry = SettingsRegistryService.Registry;
		ISettingsPage page = registry.GetPage(typeof(CharacterModifiersAllMenu));
		foreach (KeyValuePair<MemberInfo, List<(Type, MemberInfo)>> item3 in GetModifierMembersGroupedByInterfaceMember())
		{
			SettingsGroup group = (SettingsGroup)page.AddGroup(item3.Value.First().Item2.GetAttribute<LabelAttribute>().ToUIText());
			foreach (var item4 in item3.Value)
			{
				Type item = item4.Item1;
				MemberInfo item2 = item4.Item2;
				Type type = item2.ReturnType() ?? typeof(void);
				EditorInformation editorControl = EditorInformation.GetEditorControl(type, item.Assembly, item2);
				if (editorControl == null || !typeof(SettingEditor).IsAssignableFrom(editorControl.EditorType) || editorControl.EditorType == typeof(LockEditor))
				{
					continue;
				}
				object modifierInstance = Service.Get<IModMenuService>().Get(item);
				IMetadata metadata;
				if (item2 is PropertyInfo propertyInfo)
				{
					metadata = (PropertyMetadata)Activator.CreateInstance(typeof(PropertyMetadata<>).MakeGenericType(type), modifierInstance, propertyInfo, propertyInfo.GetMethod.Delegate(modifierInstance), propertyInfo.SetMethod.Delegate(modifierInstance));
				}
				else if (item2 is MethodInfo methodInfo)
				{
					metadata = new MethodMetadata(modifierInstance, methodInfo, () =>
					{
						methodInfo.Delegate(modifierInstance).DynamicInvoke();
					});
				}
				else
				{
					continue;
				}
				Type characterType = item.GetInterfaces().First((Type x) => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IModifiersForCharacter<>)).GetGenericArguments()[0];
				SettingEditor.CreateEntry(registry, group, metadata, editorControl.EditorType == typeof(RangeEditor), UIText.Literal(Helper.GetCharacterName(characterType)));
			}
		}
		static Dictionary<Type, Type[]> GetCharacterModifiersGroupedByModifiersInterface()
		{
			List<Type> source = GetModifiersForCharacters();
			Type[] array = (from x in source.SelectMany((Type x) => x.GetInterfaces())
				where x.IsAssignableTo(typeof(ICharacterModifiers))
				select x).Distinct().ToArray();
			Dictionary<Type, Type[]> dictionary = new Dictionary<Type, Type[]>();
			Type[] array2 = array;
			foreach (Type characterModifierInterface in array2)
			{
				dictionary[characterModifierInterface] = source.Where((Type x) => x.GetInterfaces().Contains(characterModifierInterface)).ToArray();
			}
			return dictionary;
		}
		static Dictionary<MemberInfo, List<(Type modifierType, MemberInfo modifierMember)>> GetModifierMembersGroupedByInterfaceMember()
		{
			Dictionary<MemberInfo, List<(Type, MemberInfo)>> dictionary = new Dictionary<MemberInfo, List<(Type, MemberInfo)>>();
			foreach (KeyValuePair<Type, Type[]> item5 in GetCharacterModifiersGroupedByModifiersInterface())
			{
				Type[] value = item5.Value;
				foreach (Type type2 in value)
				{
					InterfaceMap interfaceMap = InterfaceMap.Get(type2, item5.Key);
					MemberInfo[] interfaceMembers = interfaceMap.InterfaceMembers;
					foreach (MemberInfo interfaceMember in interfaceMembers)
					{
						MemberInfo memberInfo = (from x in interfaceMap.TargetMembers
							where x.GetType() == interfaceMember.GetType()
							where x.Name == interfaceMember.Name
							select x).FirstOrDefault((MemberInfo x) => x.HasAttributes(typeof(LabelAttribute), typeof(GroupAttribute)));
						if (!(memberInfo == null))
						{
							if (!dictionary.ContainsKey(interfaceMember))
							{
								dictionary[interfaceMember] = new List<(Type, MemberInfo)>();
							}
							dictionary[interfaceMember].Add((type2, memberInfo));
						}
					}
				}
			}
			return dictionary;
		}
		static List<Type> GetModifiersForCharacters()
		{
			return (from x in Service.Get<PluginService>().AvailablePlugins.Select((IPlugin x) => x.GetType().Assembly).SelectMany(TypeUtils.GetLoadableTypes)
				where !x.IsInterface && !x.IsAbstract
				where x.GetInterfaces().Any((Type y) => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IModifiersForCharacter<>))
				select x).ToList();
		}
	}
}
