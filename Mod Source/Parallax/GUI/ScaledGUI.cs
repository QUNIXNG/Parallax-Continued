using KSP.Localization;
using Kopernicus.Configuration;
using Parallax.Scaled_System;
using Parallax.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Parallax
{
    public partial class ParallaxGUI
    {
        static bool showExporter = false;

        static float currentMinAltitude = -100000;
        static float currentMaxAltitude = -100000;
        static string currentMinMaxBody = "";
        static void ScaledMenu(ParallaxScaledBody body)
        {
            GUILayout.Label("Scaled Shader Properties ( " + body.planetName + "):", HighLogic.Skin.label);
            GUILayout.Space(15);
            ProcessBaseScaledMaterial(body);
        }
        static void ProcessBaseScaledMaterial(ParallaxScaledBody body)
        {
            ParamCreator.ChangeMethod callback = body.UpdateBaseMaterialParamsFromGUI;

            ProcessTextureParams(body.scaledMaterialParams.shaderProperties.shaderTextures, (key, oldValue, value) =>
            {
                if (IsValidTexturePath(value))
                {
                    body.ReloadTexture(key);
                }
            });
            if (ReloadTexturesButton())
            {
                BeginTextureReload(ReloadBodyTexturesFromDisk(body.planetName));
            }

            ProcessGenericMaterialParams(body.scaledMaterialParams, callback, true, body.scaledMaterial, "ParallaxScaledShaderProperties");
        }
        static void TextureExporterMenu()
        {
            if (PlanetariumCamera.fetch.target.type != MapObject.ObjectType.CelestialBody)
            {
                return;
            }

            string planetName = PlanetariumCamera.fetch.target.gameObject.name;
            CelestialBody body = FlightGlobals.GetBodyByName(planetName);

            if (GUILayout.Button(Localizer.Format("#PARALLAX_21"), GetButtonColor(showExporter)))
            {
                showExporter = !showExporter;
            }
            if (showExporter)
            {
                GUISection.Begin();
                // Options
                GUILayout.Label("Planet Texture Exporter Options:", HighLogic.Skin.label);

                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_22"), ref exportOptions.horizontalResolution, GUIHelperFunctions.IntField, (ParamCreator.ChangeMethod)null);
                GUILayout.Space(10);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_23"), ref exportOptions.exportHeight, GUIHelperFunctions.BoolField, (ParamCreator.ChangeMethod)null);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_24"), ref exportOptions.exportColor, GUIHelperFunctions.BoolField, (ParamCreator.ChangeMethod)null);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_25"), ref exportOptions.exportNormal, GUIHelperFunctions.BoolField, (ParamCreator.ChangeMethod)null);
                GUILayout.Space(10);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_26"), ref exportOptions.multithread, GUIHelperFunctions.BoolField, (ParamCreator.ChangeMethod)null);
                GUILayout.Space(10);
                if (currentMinMaxBody != planetName)
                {
                    GUILayout.Label(Localizer.Format("#PARALLAX_27"));
                }
                else
                {
                    GUILayout.Label(Localizer.Format("#PARALLAX_28") + currentMinAltitude.ToString("F2"));
                    GUILayout.Label(Localizer.Format("#PARALLAX_29") + currentMaxAltitude.ToString("F2"));
                }
                
                GUILayout.Space(10);
                if (GUILayout.Button("Export", HighLogic.Skin.button))
                {
                    Coroutine co = ScaledManager.Instance.StartCoroutine(TextureExporter.GenerateTextures(exportOptions, body));
                }
                if (GUILayout.Button("Export Entire System", HighLogic.Skin.button))
                {
                    GenerateEntireSystem();
                }
                GUISection.End();
            }
        }
        public static void SetMinMaxAltitudeLabels(string body, float min, float max)
        {
            currentMinMaxBody = body;
            currentMinAltitude = min;
            currentMaxAltitude = max;
        }
        public static void GenerateEntireSystem()
        {
            Coroutine co = ScaledManager.Instance.StartCoroutine(TextureExporter.GenerateTextures(exportOptions, FlightGlobals.Bodies.ToArray()));
        }
    }
}
