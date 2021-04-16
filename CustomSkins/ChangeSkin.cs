using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using BepInEx;
using AI;
using GameData;
using System.Reflection.Emit;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YanLib;

namespace CustomSkins
{
	public class TurnToSkin
	{
		//点击事件
		class ClickAction : MonoBehaviour, IPointerClickHandler
		{
			int _clothId;
			public void OnPointerClick(PointerEventData eventData)
			{
				Button okbtn = YesOrNoWindow.instance.yesOrNoWindow.Find("YesButton").GetComponent<Button>();
				okbtn.onClick.AddListener(ClickYes);
				Button nobtn = YesOrNoWindow.instance.yesOrNoWindow.Find("NoButton").GetComponent<Button>();
				nobtn.onClick.AddListener(ClickNo);
				YesOrNoWindow.instance.windowMask.SetActive(true);
				YesOrNoWindow.instance.SetYesOrNoWindow(1997052201, "改变形象", "是否将本衣着转化为一个拥有随机特殊立绘的皮肤（不影响属性）？", true, true);
			}
			public void SetParam(int clothId)
			{
				_clothId = clothId;
			}
			private void ClickYes()//在这里做正事
			{
				int id = OnClick.instance.ID;
				if (id == 1997052201)
				{
					TurnToSpecialSkin(_clothId);
					//YanLib.DataManipulator.Actor.DumpActorData(DateFile.instance.MianActorID(), false, false, @"C:\Users\Slimoon\Desktop\MainActor");
				/*	Dictionary<int, Dictionary<int, string>> itempower = DateFile.instance.itemPowerDate;
					foreach (KeyValuePair<int, Dictionary<int, string>> power in itempower)
					{
						foreach (KeyValuePair<int, string> pow in power.Value)
						{
							Console.WriteLine("Key1={0}, Key2={1}, Content={2}", power.Key, pow.Key, pow.Value);
						}
					}*/
				}
				RemoveBind();
			}

			private void ClickNo()
			{
				RemoveBind();
			}
			private static void RemoveBind()
			{
				Button okbtn = YesOrNoWindow.instance.yesOrNoWindow.Find("YesButton").GetComponent<Button>();
				okbtn.onClick.RemoveAllListeners();
				Button nobtn = YesOrNoWindow.instance.yesOrNoWindow.Find("NoButton").GetComponent<Button>();
				nobtn.onClick.RemoveAllListeners();
			}
		}
		//预先设置点击事件
		[HarmonyPatch(typeof(SetItem), "SetActorMenuItemIcon")]
		public static class EasyRefine_SetActorMenuItemIcon_Patch
		{
			static void Postfix(SetItem __instance, int itemId)
			{
				AddClickAction(__instance.gameObject, itemId);
			}
		}
		[HarmonyPatch(typeof(SetItem), "SetActorEquipIcon")]
		public static class EasyRefine_SetActorEquipIcon_Patch
		{
			static void Postfix(SetItem __instance, int itemId)
			{
				AddClickAction(__instance.gameObject, itemId);
			}
		}

		[HarmonyPatch(typeof(DateFile), "SetItemName")]//显示物品当前ID和类别ID，测试用
		public static class SetItemName_Patch
		{
			public static void Postfix(ref string __result, int id)
			{
				if(int.Parse(DateFile.instance.GetItemDate(id, 15, true)) != 0 && int.Parse(DateFile.instance.GetItemDate(id, 34, true)) != 0)
				__result = __result + "\n立绘ID："+ DateFile.instance.GetItemDate(id, 34, true);

			}

		}
		static void AddClickAction(GameObject icon, int itemId)
		{
			//判断是不是衣着
			bool flag1 = int.Parse(DateFile.instance.GetItemDate(itemId, 15, true)) ==0;
			if (flag1)
			{
				GameObject.Destroy(icon.GetComponent<ClickAction>());
				return;
			}
		/*	else
			{
				bool flag2 = Items.GetItemProperty(itemId, 901).Equals(Items.GetItemProperty(itemId, 902));
				if (flag2)
					return;
			}*/

			//添加相应处理Component,注入参数
			var clickActions = icon.GetComponents<ClickAction>();
			if (clickActions.Length >= 1)//避免重复添加
			{
				clickActions[0].SetParam(itemId);
			}
			else
			{
				var actionstub = icon.AddComponent<ClickAction>();
				actionstub.SetParam(itemId);
			}
		}

		public static void TurnToSpecialSkin(int _clothId)
		{
			string appPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			appPath = Path.Combine(appPath, "big");
			//List<string> SkinsBig = null;
			var SkinsBig = Directory.GetFiles(appPath, "*.png");
			int len = SkinsBig.Count();
			for (int i = 0; i < len; i++)
			{
				SkinsBig[i] = SkinsBig[i].Replace(appPath + @"\", "");
				SkinsBig[i] = SkinsBig[i].Replace(".png", "");
				//Debug.Log(SkinsBig[i]);
			}
			if (len > 0)
			{
				string randSkin = SkinsBig[UnityEngine.Random.Range(0, len)];
				//Items.SetItemProperty(_clothId, 902, randSkin);//衣物的property只有三个，901耐久，902耐久上限，999类别id
				//Items.SetItemProperty(_clothId, 901, randSkin);
				//if (int.Parse(DateFile.instance.GetItemDate(_clothId, 34)) == 0)
					DateFile.instance.ChangItemDate(_clothId, 34, int.Parse(randSkin) * 10, true);
				//else
					//DateFile.instance.ChangItemDate(_clothId, 34, 10, false);//茄子真有够闲
				//DateFile.instance.ChangItemDate(_clothId, 504, 407 * 10, true);
				//Debug.Log(DateFile.instance.GetItemDate(_clothId, 504, true, -1));
			}
		}
	}
}
