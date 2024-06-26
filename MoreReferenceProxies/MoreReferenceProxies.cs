﻿using HarmonyLib;
using FrooxEngine;
using Elements.Core;
using FrooxEngine.UIX;
using ResoniteModLoader;

namespace MoreReferenceProxies
{
	public class MoreReferenceProxies : ResoniteMod
	{
		public override string Name => "MoreReferenceProxies";
		public override string Author => "eia485, TheJebForge";
		public override string Version => "1.2.1";
		public override string Link => "https://github.com/TheJebForge/ResoniteMoreReferenceProxies/";
		public override void OnEngineInit()
		{
			Harmony harmony = new Harmony("net.eia485.MoreReferenceProxies");
			harmony.PatchAll();
		}

		[HarmonyPatch]
		class MoreReferenceProxiesPatch
		{
			[HarmonyPostfix]
			[HarmonyPatch(typeof(SyncMemberEditorBuilder), "BuildBag")]
			public static void BuildBagPostFix(ISyncBag bag, UIBuilder ui)
			{
				BuildProxy(bag, ui);
			}
			
			[HarmonyPostfix]
			[HarmonyPatch(typeof(SyncMemberEditorBuilder), "BuildList")]
			public static void BuildListPostFix(ISyncList list, UIBuilder ui)
			{
				BuildProxy(list, ui);
			}

			[HarmonyPostfix]
			[HarmonyPatch(typeof(UserInspectorItem), "RebuildUser")]
			public static void RebuildUserPostFix(UserInspectorItem __instance, SyncRef<User> ____user)
			{
				__instance.Slot[0].AttachComponent<ReferenceProxySource>().Reference.Target = ____user.Target;
				__instance.Slot[1][1][0].AttachComponent<ReferenceProxySource>().Reference.Target = (WorkerBag<UserComponent>)AccessTools.Field(typeof(User), "componentBag").GetValue(____user.Target);
				for (int i = 0; i < ____user.Target.StreamGroupManager.Groups.Count; i++)
				{
					__instance.Slot[1][1][i + 1].AttachComponent<ReferenceProxySource>().Reference.Target = (StreamBag)AccessTools.Field(typeof(User), "streamBag").GetValue(____user.Target);
				}
			}
		}
		private static void BuildProxy(IWorldElement target, UIBuilder ui)
		{
			Slot textSlot = ui.Current[0];
			textSlot.AttachComponent<ReferenceProxySource>().Reference.Target = target;
			InteractionElement.ColorDriver colorDriver = textSlot.AttachComponent<Button>().ColorDrivers.Add();
			Text text = textSlot.GetComponent<Text>();
			colorDriver.ColorDrive.Target = text.Color;
			colorDriver.NormalColor.Value = ui.Style.TextColor;
			colorDriver.HighlightColor.Value = RadiantUI_Constants.LABEL_COLOR;
			colorDriver.PressColor.Value = RadiantUI_Constants.HEADING_COLOR;
		}
	}
}