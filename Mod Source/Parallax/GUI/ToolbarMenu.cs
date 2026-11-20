using KSP.Localization;
using KSP.UI;
using KSP.UI.Screens;
using Parallax.Scaled_System;
using SoftMasking.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Parallax
{
    //
    // Toolbar Menu
    // This is what most players will use to configure Parallax - This directly interfaces with the ParallaxGlobalSettings 
    //
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class ToolbarMenu : MonoBehaviour
    {
        const float maxWindowHeightToWidth = 1.45f;
        private static ScrollableWindow window;
        private static Rect hoverWindow = new Rect(0, 0, 350, 80);
        private static GUIStyle activeButton;
        private static GUIStyle titleLabel;
        private static GUIStyle headerLabel;
        private static ApplicationLauncherButton button;

        static string maxTessellationTooltip = Localizer.Format("#PARALLAX_78");
        static string tessellationEdgeLengthTooltip = Localizer.Format("#PARALLAX_79");
        static string maxTessellationRangeTooltip = Localizer.Format("#PARALLAX_80");
        static string advancedTextureBlendingTooltip = Localizer.Format("#PARALLAX_81");
        static string ambientOcclusionTooltip = Localizer.Format("#PARALLAX_82");

        static string densityMultiplierTooltip = Localizer.Format("#PARALLAX_83");
        static string rangeMultiplierTooltip = Localizer.Format("#PARALLAX_84");
        static string fadeOutStartRangeTooltip = Localizer.Format("#PARALLAX_85");
        static string collisionLevelTooltip = Localizer.Format("#PARALLAX_86");
        static string colliderLookaheadTimeTooltip = Localizer.Format("#PARALLAX_87");
        static string showCollidersTooltip = Localizer.Format("#PARALLAX_88");

        static string lightShadowsTooltip = Localizer.Format("#PARALLAX_89");
        static string lightShadowsQualityTooltip = Localizer.Format("#PARALLAX_90");

        static string scaledSpaceShadowsTooltip = Localizer.Format("#PARALLAX_91");
        static string smoothScaledSpaceShadowsTooltip = Localizer.Format("#PARALLAX_92");
        static string scaledRaymarchedShadowStepCountTooltip = Localizer.Format("#PARALLAX_93");
        static string scaledRaymarchedShadowMaxStarsTooltip = Localizer.Format("#PARALLAX_94");
        static string loadTexturesImmediatelyTooltip = Localizer.Format("#PARALLAX_95");

        static string wireframeTerrainTooltip = Localizer.Format("#PARALLAX_96");
        static string suppressCriticalMessagesTooltip = "Stops critical error and warning popups from appearing. They are still written to the log";
        static string enableDebugVisualizationsTooltip = Localizer.Format("#PARALLAX_97");

        bool showGUI = false;

        bool initialized = false;

        static bool showCollideables = false;

        static bool showDebugSettings = false;
        static string tooltip = "";

        static int bingusCount = 0;
        void Start()
        {
            if (!initialized)
            {
                window = new ScrollableWindow(new Rect(Screen.width / 2 - 450 / 2, Screen.height / 2 - 50, 450, 100), maxWindowHeightToWidth, "ParallaxSettingsScroll");

                // Create on button hover style
                activeButton = new GUIStyle(HighLogic.Skin.button);
                activeButton.normal.textColor = HighLogic.Skin.label.normal.textColor;
                activeButton.hover.textColor = HighLogic.Skin.label.normal.textColor * 1.25f;

                titleLabel = new GUIStyle(HighLogic.Skin.label);
                titleLabel.alignment = TextAnchor.MiddleCenter;
                titleLabel.fontStyle = FontStyle.Bold;

                headerLabel = new GUIStyle(HighLogic.Skin.label);
                headerLabel.fontStyle = FontStyle.Bold;

                // Create toolbar button
                Texture buttonTexture = GameDatabase.Instance.GetTexture("ParallaxContinued/Textures/button", false);
                button = ApplicationLauncher.Instance.AddModApplication(ShowToolbarGUI, HideToolbarGUI, IncrementBingus, Nothing, Nothing, Nothing, ApplicationLauncher.AppScenes.ALWAYS, buttonTexture);

                initialized = true;
            }
        }
        public void ShowToolbarGUI()
        {
            showGUI = true;
        }
        public void HideToolbarGUI()
        {
            showGUI = false;
        }
        
        void Update()
        {
            window?.UpdateInputLock(showGUI);
        }
        void OnGUI()
        {
            if (showGUI)
            {
                window.Draw(GetInstanceID(), DrawWindow, Localizer.Format("#PARALLAX_2"));

                if (!string.IsNullOrEmpty(tooltip))
                {
                    hoverWindow.x = Event.current.mousePosition.x + 22;
                    hoverWindow.y = Event.current.mousePosition.y - hoverWindow.height / 2;
                    hoverWindow = GUILayout.Window(GetInstanceID() + 1, hoverWindow, ShowTooltip, "Tooltip", HighLogic.Skin.window);
                }
            }
        }

        static void ShowTooltip(int windowID)
        {
            GUILayout.BeginVertical();

            GUIStyle skin = new GUIStyle(HighLogic.Skin.label)
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft
            };

            // Measure content size dynamically
            GUIContent content = new GUIContent(tooltip);
            Vector2 size = skin.CalcSize(content);
            float minHeight = skin.CalcHeight(content, size.x); // Ensure multi-line height is considered

            // Adjust window size based on content
            hoverWindow.height = Mathf.Max(minHeight + 10, 30); // Minimum height to avoid excessive shrinking

            GUILayout.Label(tooltip, skin);
            GUILayout.EndVertical();
        }

        static void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();
            ///////////////////////////

            GUILayout.Label(Localizer.Format("#PARALLAX_100"), titleLabel);

            window.BeginScrollView();

            GUILayout.Label(Localizer.Format("#PARALLAX_101"), headerLabel);
            ParamCreator.ChangeMethod terrainCallback = UpdateTerrainMaterials;
            ParamCreator.ChangeMethod terrainKeywordCallback = UpdateTerrainKeywords;

            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_102"),         ref ConfigLoader.parallaxGlobalSettings.terrainGlobalSettings.maxTessellation,             GUIHelperFunctions.FloatField, terrainCallback, maxTessellationTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_103"), ref ConfigLoader.parallaxGlobalSettings.terrainGlobalSettings.tessellationEdgeLength,      GUIHelperFunctions.FloatField, terrainCallback, tessellationEdgeLengthTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_104"),       ref ConfigLoader.parallaxGlobalSettings.terrainGlobalSettings.maxTessellationRange,        GUIHelperFunctions.FloatField, terrainCallback, maxTessellationRangeTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_105"),    ref ConfigLoader.parallaxGlobalSettings.terrainGlobalSettings.advancedTextureBlending,     GUIHelperFunctions.BoolField, terrainKeywordCallback, advancedTextureBlendingTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_106"),        ref ConfigLoader.parallaxGlobalSettings.terrainGlobalSettings.ambientOcclusion,            GUIHelperFunctions.BoolField, terrainKeywordCallback, ambientOcclusionTooltip);
            GUILayout.Space(15);

            GUILayout.Label(Localizer.Format("#PARALLAX_107"), headerLabel);
            ParamCreator.ChangeMethod scatterCallback = UpdateScatterSettings;
            ParamCreator.ChangeMethod colliderCallback = UpdateColliderSettings;

            GUILayout.Label(Localizer.Format("#PARALLAX_108"));
            GUILayout.Label(Localizer.Format("#PARALLAX_109"));
            GUILayout.Space(10);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_110"),                  ref ConfigLoader.parallaxGlobalSettings.scatterGlobalSettings.densityMultiplier,        GUIHelperFunctions.FloatField, scatterCallback, densityMultiplierTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_111"),                    ref ConfigLoader.parallaxGlobalSettings.scatterGlobalSettings.rangeMultiplier,          GUIHelperFunctions.FloatField, scatterCallback, rangeMultiplierTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_112"),                ref ConfigLoader.parallaxGlobalSettings.scatterGlobalSettings.fadeOutStartRange,        GUIHelperFunctions.FloatField, scatterCallback, fadeOutStartRangeTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_113"),  ref ConfigLoader.parallaxGlobalSettings.scatterGlobalSettings.collisionLevel,           GUIHelperFunctions.IntField,   scatterCallback, collisionLevelTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_114"),             ref ConfigLoader.parallaxGlobalSettings.scatterGlobalSettings.colliderLookaheadTime,    GUIHelperFunctions.FloatField, colliderCallback, colliderLookaheadTimeTooltip);
            GUILayout.Space(15);

            GUILayout.Label(Localizer.Format("#PARALLAX_115"), headerLabel);
            ParamCreator.ChangeMethod lightingCallback = UpdateLightingSettings;

            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_116"),         ref ConfigLoader.parallaxGlobalSettings.lightingGlobalSettings.lightShadows,          GUIHelperFunctions.BoolField,   lightingCallback, lightShadowsTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_117"), ref ConfigLoader.parallaxGlobalSettings.lightingGlobalSettings.lightShadowsQuality,   EnumField,                      lightingCallback, lightShadowsQualityTooltip);

            GUILayout.Label(Localizer.Format("#PARALLAX_118"), headerLabel);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_119"), ref showCollideables, GUIHelperFunctions.BoolField, ShowCollideableScatters, showCollidersTooltip);
            GUILayout.Space(15);

            GUILayout.Label(Localizer.Format("#PARALLAX_120"), headerLabel);
            GUILayout.Label(Localizer.Format("#PARALLAX_121"));
            ParamCreator.ChangeMethod scaledShadowCallback = UpdateScaledShadowSettings;
            ParamCreator.ChangeMethod scaledTextureCallback = UpdateScaledTextureSettings;
            ParamCreator.ChangeMethod scaledShadowMaterialCallback = UpdateScaledShadowMaterialSettings;

            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_122"),              ref ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.scaledSpaceShadows, GUIHelperFunctions.BoolField, scaledShadowCallback, scaledSpaceShadowsTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_123"),            ref ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.smoothScaledSpaceShadows, GUIHelperFunctions.BoolField, scaledShadowMaterialCallback, smoothScaledSpaceShadowsTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_124"),    ref ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.scaledRaymarchedShadowStepCount, GUIHelperFunctions.IntField, scaledShadowMaterialCallback, scaledRaymarchedShadowStepCountTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_125"),     ref ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.scaledRaymarchedShadowMaxStars, GUIHelperFunctions.IntField, scaledShadowMaterialCallback, scaledRaymarchedShadowMaxStarsTooltip);
            ParamCreator.CreateParam(Localizer.Format("#PARALLAX_126"),        ref ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.loadTexturesImmediately, GUIHelperFunctions.BoolField, scaledTextureCallback, loadTexturesImmediatelyTooltip);
            GUILayout.Space(15);

            if (SectionButton(Localizer.Format("#PARALLAX_127"), ref showDebugSettings))
            {
                GUISection.Begin();
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_128"),               ref ConfigLoader.parallaxGlobalSettings.debugGlobalSettings.wireframeTerrain,           GUIHelperFunctions.BoolField, UpdateWireframeTerrain, wireframeTerrainTooltip);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_129"),      ref ConfigLoader.parallaxGlobalSettings.debugGlobalSettings.suppressCriticalMessages,   GUIHelperFunctions.BoolField, null, suppressCriticalMessagesTooltip);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_130"),     ref ConfigLoader.parallaxGlobalSettings.debugGlobalSettings.enableDebugVisualizations,  GUIHelperFunctions.BoolField, null, enableDebugVisualizationsTooltip);
                GUISection.End();
            }

            window.EndScrollView();

            // Prevent some stupid settings
            if (ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.scaledRaymarchedShadowStepCount > 256)
            {
                ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.scaledRaymarchedShadowStepCount = 256;
            }
            ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.scaledRaymarchedShadowMaxStars = Mathf.Clamp(ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.scaledRaymarchedShadowMaxStars, 1, RaymarchedShadowsRenderer.MaxShadowStars);

            // Save button
            if (GUILayout.Button(Localizer.Format("#PARALLAX_131"), activeButton))
            {
                ConfigLoader.parallaxGlobalSettings.SaveSettings();
            }

            ///////////////////////////
            GUILayout.EndVertical();

            // Store it so we can set show it in the tooltip window
            tooltip = GUI.tooltip;

            // Must be last or buttons wont work
            UnityEngine.GUI.DragWindow();
        }

        static bool SectionButton(string name, ref bool isOpen)
        {
            if (GUILayout.Button(name, isOpen ? activeButton : HighLogic.Skin.button))
            {
                isOpen = !isOpen;
            }
            return isOpen;
        }

        //
        //  Callbacks
        //

        static void UpdateTerrainMaterials()
        {
            foreach (ParallaxTerrainBody body in ConfigLoader.parallaxTerrainBodies.Values)
            {
                body.parallaxMaterials.SetAll("_MaxTessellation", ConfigLoader.parallaxGlobalSettings.terrainGlobalSettings.maxTessellation);
                body.parallaxMaterials.SetAll("_TessellationEdgeLength", ConfigLoader.parallaxGlobalSettings.terrainGlobalSettings.tessellationEdgeLength);
                body.parallaxMaterials.SetAll("_MaxTessellationRange", ConfigLoader.parallaxGlobalSettings.terrainGlobalSettings.maxTessellationRange);
            }
        }
        static void UpdateTerrainKeywords()
        {
            foreach (TerrainShaderQuadData data in PQSMod_Parallax.terrainQuadData.Values)
            {
                // Advanced blending
                if (ConfigLoader.parallaxGlobalSettings.terrainGlobalSettings.advancedTextureBlending)
                {
                    data.quadMaterial.EnableKeyword("ADVANCED_BLENDING");
                }
                else
                {
                    data.quadMaterial.DisableKeyword("ADVANCED_BLENDING");
                }

                // Ambient Occlusion
                if (ConfigLoader.parallaxGlobalSettings.terrainGlobalSettings.ambientOcclusion)
                {
                    data.quadMaterial.EnableKeyword("AMBIENT_OCCLUSION");
                }
                else
                {
                    data.quadMaterial.DisableKeyword("AMBIENT_OCCLUSION");
                }
            }
        }

        static void UpdateScatterSettings()
        {
            ParallaxDebug.Log("Updating scatter system settings from GUI...");

            // check current pause state
            bool isGamePaused = FlightDriver.Pause;
            
            // Pause to prevent collider issues
            FlightDriver.SetPause(true);

            // Reverse normalisations - we're modifying the range
            foreach (ParallaxScatterBody body in ConfigLoader.parallaxScatterBodies.Values)
            {
                foreach (Scatter scatter in body.fastScatters)
                {
                    ConfigLoader.ReverseNormalisationConversions(scatter);
                }
            }

            // Reperform normalisations - we've modified the range and pop mult, but want to keep lod distances the same and avoid int float conversion inaccuracy
            foreach (ParallaxScatterBody body in ConfigLoader.parallaxScatterBodies.Values)
            {
                foreach (Scatter scatter in body.fastScatters)
                {
                    ConfigLoader.PerformNormalisationConversions(scatter);
                }
            }

            FlightGlobals.currentMainBody.pqsController.RebuildSphere();

            // Restore previous pause state
            FlightDriver.SetPause(isGamePaused);
        }

        static void UpdateWireframeTerrain()
        {
            // The terrain material is picked when each quad is built
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.currentMainBody != null)
            {
                FlightGlobals.currentMainBody.pqsController.RebuildSphere();
            }
        }

        static void UpdateColliderSettings()
        {
            // Nothing needed here for now
            // All that updates is the collider lookahead but that auto updates next frame anyway
        }

        static void UpdateScaledTextureSettings()
        {
            // Nothing needed here for now
        }

        static void UpdateScaledShadowMaterialSettings()
        {
            // Toggle smooth shadows
            foreach (ParallaxScaledBody scaledBody in ConfigLoader.parallaxScaledBodies.Values)
            {
                if (scaledBody.shadowCasterMaterial != null)
                {
                    if (!scaledBody.shadowCasterMaterial.IsKeywordEnabled("BLUE_NOISE") && ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.smoothScaledSpaceShadows)
                    {
                        scaledBody.shadowCasterMaterial.EnableKeyword("BLUE_NOISE");
                    }
                    else if (scaledBody.shadowCasterMaterial.IsKeywordEnabled("BLUE_NOISE") && !ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.smoothScaledSpaceShadows)
                    {
                        scaledBody.shadowCasterMaterial.DisableKeyword("BLUE_NOISE");
                    }
                }
            }

            Shader.SetGlobalInt("_ParallaxScaledShadowStepSize", ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.scaledRaymarchedShadowStepCount);
        }

        static void UpdateScaledShadowSettings()
        {
            // Shadows just turned off
            if (!ConfigLoader.parallaxGlobalSettings.scaledGlobalSettings.scaledSpaceShadows)
            {
                RaymarchedShadowsRenderer.Instance.Disable();
            }
            // Shadows just turned on
            else
            {
                RaymarchedShadowsRenderer.Instance.Enable();
            }
            
            // As for instant texture loading, that'll update whenever a planet is next loaded
        }

        static void ShowCollideableScatters()
        {
            ParallaxGUI.ShowCollideableScatters(showCollideables);
        }
        static void UpdateLightingSettings()
        {
            foreach (Vessel v in FlightGlobals.VesselsLoaded)
            {
                if (v.parts.Count > 0)
                {
                    if (v.parts[0].isKerbalEVA())
                    {
                        KerbalEVA headlamp = v.parts[0].Modules.GetModule<KerbalEVA>();
                        if (headlamp != null)
                        {
                            Light headLight = headlamp.headLamp.GetComponent<Light>();
                            if (headLight != null)
                            {
                                headLight.shadows = ConfigLoader.parallaxGlobalSettings.lightingGlobalSettings.lightShadows ? LightShadows.Soft : LightShadows.None;
                                headLight.shadowResolution = ConfigLoader.parallaxGlobalSettings.lightingGlobalSettings.lightShadowsQuality;
                                headLight.lightShadowCasterMode = ConfigLoader.parallaxGlobalSettings.lightingGlobalSettings.lightShadows ? LightShadowCasterMode.Everything : LightShadowCasterMode.Default;
                            }
                        }
                    }
                }
                foreach (Part p in v.Parts)
                {
                    PartModule lightModule = p.Modules.GetModule("ModuleLight");
                    if (lightModule != null)
                    {
                        ModuleLight moduleLight = lightModule as ModuleLight;
                        foreach (Light light in moduleLight.lights)
                        {
                            light.shadows = ConfigLoader.parallaxGlobalSettings.lightingGlobalSettings.lightShadows ? LightShadows.Soft : LightShadows.None;
                            light.shadowResolution = ConfigLoader.parallaxGlobalSettings.lightingGlobalSettings.lightShadowsQuality;
                            light.lightShadowCasterMode = ConfigLoader.parallaxGlobalSettings.lightingGlobalSettings.lightShadows ? LightShadowCasterMode.Everything : LightShadowCasterMode.Default;

                            light.shadowBias = 0.05f;
                            light.shadowNormalBias = 0.4f;
                        }
                    }
                }
            }
        }

        static void IncrementBingus()
        {
            bingusCount++;
            if (bingusCount % 10 == 0 && bingusCount > 0)
            {
                button.SetTexture(GameDatabase.Instance.GetTexture("ParallaxContinued/Textures/bingus", false));
            }
            else
            {
                button.SetTexture(GameDatabase.Instance.GetTexture("ParallaxContinued/Textures/button", false));
            }
        }

        void Nothing()
        {

        }
        void OnDestroy()
        {
            window?.ReleaseInputLock();

            if (button)
            {
                ApplicationLauncher.Instance.RemoveModApplication(button);
            }
        }

        //
        // Helper Function
        //

        static LightShadowResolution activeEnumFieldLastValue = LightShadowResolution.Medium;
        static int activeEnumField = -1;
        static string activeEnumFieldString = "";

        public static LightShadowResolution EnumField(LightShadowResolution value, out bool valueWasChanged)
        {
            valueWasChanged = false;

            // Get rect and control for this enum field for identification
            Rect pos = GUILayoutUtility.GetRect(new GUIContent(value.ToString()), HighLogic.Skin.label, new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.MinWidth(140) });
            int enumFieldID = GUIUtility.GetControlID("EnumField".GetHashCode(), FocusType.Keyboard, pos) + 1;
            if (enumFieldID == 0)
                return value;

            bool recorded = activeEnumField == enumFieldID;
            bool active = enumFieldID == GUIUtility.keyboardControl;

            if (active && recorded && !activeEnumFieldLastValue.Equals(value))
            { // Value has been modified externally
                activeEnumFieldLastValue = value;
                activeEnumFieldString = value.ToString();
            }

            // Get stored string for the text field if this one is recorded
            string str = recorded ? activeEnumFieldString : value.ToString();

            // pass it in the text field
            string strValue = GUI.TextField(pos, str, HighLogic.Skin.textField);

            // Update stored value if this one is recorded
            if (recorded)
                activeEnumFieldString = strValue;

            // Try Parse if value got changed. If the string could not be parsed, ignore it and keep last value
            bool parsed = true;
            if (strValue != value.ToString())
            {
                LightShadowResolution newValue;
                parsed = TryParseEnum(strValue, out newValue);
                if (parsed)
                {
                    value = activeEnumFieldLastValue = newValue;
                    valueWasChanged = true;
                }
            }

            if (active && !recorded)
            { // Gained focus this frame
                activeEnumField = enumFieldID;
                activeEnumFieldString = strValue;
                activeEnumFieldLastValue = value;
            }
            else if (!active && recorded)
            { // Lost focus this frame
                activeEnumField = -1;
                if (!parsed)
                    value = TryParseEnum(strValue, out LightShadowResolution forcedValue) ? forcedValue : value;
            }

            return value;
        }
        private static bool TryParseEnum(string str, out LightShadowResolution result)
        {
            return Enum.TryParse(str, true, out result) && Enum.IsDefined(typeof(LightShadowResolution), result);
        }
    }
}
