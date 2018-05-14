// Jeremy Anderson
// HarmonyPatch_UI_BackgroundMain_BackgroundOnGUI.cs
// 2018-05-10

using System.Collections.Generic;
using Harmony;
using RimWorld;
using Verse;
using UnityEngine;

namespace RimTES
{
    /// <summary>
    /// Select and display a random image from the UI/Background folder on each load.
    /// </summary>
    [HarmonyPatch(typeof(UI_BackgroundMain), "BackgroundOnGUI"), StaticConstructorOnStartup]
    public class HarmonyPatch_UI_BackgroundMain_BackgroundOnGUI
    {
        private static Texture2D selectedTexture = null;

        public static bool Prefix()
        {
            Vector2 TextureSize = new Vector2(2048f, 1280f);

            bool flag = true;
            if (UI.screenWidth > UI.screenHeight * (TextureSize.x / TextureSize.y))
                flag = false;

            Rect position;
            if (flag)
            {
                float height = UI.screenHeight;
                float num = UI.screenHeight * (TextureSize.x / TextureSize.y);
                position = new Rect((UI.screenWidth / 2) - num / 2f, 0f, num, height);
            }
            else
            {
                float width = UI.screenWidth;
                float num2 = UI.screenWidth * (TextureSize.y / TextureSize.x);
                position = new Rect(0f, (UI.screenHeight / 2) - num2 / 2f, width, num2);
            }

            if (selectedTexture == null)
            {
                IEnumerable<Texture2D> BackgroundTextures = ContentFinder<Texture2D>.GetAllInFolder("UI/Background");
                selectedTexture = BackgroundTextures.RandomElement();
            }

            GUI.DrawTexture(position, selectedTexture, ScaleMode.ScaleToFit);

            return false;
        }
    }
}
