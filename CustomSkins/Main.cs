using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using BepInEx;
using AI;
using GameData;
using System.Linq;
using System.Reflection.Emit;
using System.IO;

namespace CustomSkins
{
    [BepInPlugin("Slimoon.CustomSkins", "CustomSkins", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        void Start()
        {
            new Harmony("Slimoon.CustomSkins").PatchAll();
            //    LoadSprite("a_s.png");
            //    LoadSprite("a.png");
        }

        public static Sprite LoadSprite(string imageName, bool small)
        {
            string appPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (small)
			{
                appPath = Path.Combine(appPath, "small");
			}
			else
			{
                appPath = Path.Combine(appPath, "big");
            }
            string path = Path.Combine(appPath, imageName);
            if (!File.Exists(path))
            {
            //    Console.WriteLine($"图片路径[{path}]不存在");
                return null;
            }
            var fileData = File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            //Console.WriteLine($"成功加载图片[{path}]\n大小为{texture.width}*{texture.height}");
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100);
        }

        [HarmonyPatch(typeof(ActorFace), "UpdateFace", new Type[]{typeof(int), typeof(int), typeof(int), typeof(int), typeof(int[]), typeof(int[]), typeof(int), typeof(bool), typeof(bool)})]
        public static class UpdateFace_Patch
        {
            public static bool Prefix(int actorId, ActorFace __instance)
			{
                int SpecialclotheIndex = 0;
                int itemid = int.Parse(DateFile.instance.GetActorDate(actorId, 305, true));
                //Debug.Log(itemid.ToString());//进入游戏的主界面也会用到这个函数，此时游戏数据未加载所以itemid为0，淦哦，用这一点来屏蔽主界面时候的补丁运行
                if (itemid != 0)
                {
                    SpecialclotheIndex = int.Parse(DateFile.instance.GetItemDate(itemid, 34, true, -1));//902
                    if (SpecialclotheIndex>100)

                    {
                        __instance.ageImage.gameObject.SetActive(false);
                        __instance.nose.gameObject.SetActive(false);
                        __instance.faceOther.gameObject.SetActive(false);
                        __instance.eye.gameObject.SetActive(false);
                        __instance.eyePupil.gameObject.SetActive(false);
                        __instance.eyebrows.gameObject.SetActive(false);
                        __instance.mouth.gameObject.SetActive(false);
                        __instance.beard.gameObject.SetActive(false);
                        __instance.hair1.gameObject.SetActive(false);
                        __instance.hair2.gameObject.SetActive(false);
                        __instance.hairOther.gameObject.SetActive(false);
                        __instance.clothes.gameObject.SetActive(false);
                        __instance.clothesColor.gameObject.SetActive(false);
                        __instance.body.gameObject.SetActive(true);
                        bool flag2 = __instance.smallSize;
                        if (flag2)
                        {
                            var temp = LoadSprite(SpecialclotheIndex.ToString() + ".png", true);
                            if (temp == null) temp = LoadSprite(SpecialclotheIndex.ToString() + ".png", false);
                            if (temp == null) return true;
                            __instance.body.sprite = temp;
                        }
                        else
                        {
                            var temp = LoadSprite(SpecialclotheIndex.ToString() + ".png", false);
                            if (temp == null) return true;
                            __instance.body.sprite = temp;
                        }
                        __instance.body.color = new Color(1f, 1f, 1f, 1f);
                        //int id = DateFile.instance.MianActorID();
                        //int itemid = int.Parse(DateFile.instance.GetActorDate(id, 305, true));
                        //Debug.Log(@"it's id is "+itemid.ToString());//+" "+DateFile.instance.GetItemDate(itemid, 999, true, -1)
                        return false;
                    }
                    else return true;
                }
                else return true;
                
                
            }
		}
    }
}
            //获取包含当前执行的代码的程序集的加载文件的完整路径，本mod就用这个了
            //测试位置：D:\SteamLibrary\steamapps\common\The Scroll Of Taiwu\BepInEx\plugins\CustomSkins
            //var appPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //Console.WriteLine(appPath);

            //获取模块的完整路径
            //string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            //Console.WriteLine(path);

            //获取和设置当前目录(该进程从中启动的目录)的完全限定目录。
            //var dicPath = System.Environment.CurrentDirectory;
            //Console.WriteLine(dicPath);

            //获取程序的基目录
            //string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            //Console.WriteLine(basePath);

            //获取和设置包括该应用程序的目录的名称
            //string domainPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //Console.WriteLine(domainPath);