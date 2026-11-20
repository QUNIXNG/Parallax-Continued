using KSP.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSPTextureLoader;
using UnityEngine;

namespace Parallax
{
    public partial class ParallaxGUI
    {
        static void ScatterHeader()
        {
            GUIStyle labelSkin = HighLogic.Skin.label;
            labelSkin.alignment = TextAnchor.MiddleCenter;

            // Show current scatter
            Scatter scatter = GetScatter();
            GUILayout.Label(Localizer.Format("#PARALLAX_30") + scatter.scatterName, labelSkin);

            // Align correctly
            labelSkin.alignment = TextAnchor.MiddleLeft;
        }
        static void ScatterMenu()
        {
            Scatter scatter = scatters[currentScatterIndex];

            ProcessDistributionParams(scatter);
            ProcessDistributionNoiseParams(scatter);
            ProcessBiomeBlacklist(scatter);
            ProcessBaseMaterialParams(scatter);

            ProcessDebug(scatter);
            ProcessSaveButton(scatter);
        }
        static Scatter GetScatter()
        {
            // Advance scatter
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous Scatter", HighLogic.Skin.button, GUILayout.Width(214)))
            {
                ChangeScatter(DecrementScatterIndex());
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Next Scatter", HighLogic.Skin.button, GUILayout.Width(214)))
            {
                ChangeScatter(IncrementScatterIndex());
            }
            GUILayout.EndHorizontal();
            return scatters[currentScatterIndex];
        }
        static void ChangeScatter(int newIndex)
        {
            if (debugShowFaceOrientation)
            {
                ShowFaceOrientation(false, scatters[currentScatterIndex]);
                ShowFaceOrientation(true, scatters[newIndex]);
            }
            currentScatterIndex = newIndex;
        }
        static void ProcessDistributionParams(Scatter scatter)
        {
            if (scatter.isShared) { return; }
            if (GUILayout.Button(Localizer.Format("#PARALLAX_31"), GetButtonColor(showDistribution)))
            {
                showDistribution = !showDistribution;
            }
            if (showDistribution)
            {
                GUISection.Begin();
                // Callback method that regenerates scatters when these values are changed
                ParamCreator.ChangeMethod callback = scatter.ReinitializeDistribution;

                GUILayout.Label("Distribution Params: ", HighLogic.Skin.label);

                // General params
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_32"), ref scatter.distributionParams.seed, GUIHelperFunctions.FloatField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_33"), ref scatter.distributionParams.spawnChance, GUIHelperFunctions.FloatField, callback);
                // These edit the config values, which the global settings then scale
                ParamCreator.ChangeMethod globalSettingsCallback = () =>
                {
                    ConfigLoader.ApplyGlobalScatterSettings(scatter);
                    callback();
                };
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_34"), ref scatter.distributionParams.originalRange, GUIHelperFunctions.FloatField, globalSettingsCallback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_35"), ref scatter.distributionParams.originalPopulationMultiplier, GUIHelperFunctions.IntField, globalSettingsCallback);

                // Scale params
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_36"), ref scatter.distributionParams.minScale, GUIHelperFunctions.Vector3Field, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_37"), ref scatter.distributionParams.maxScale, GUIHelperFunctions.Vector3Field, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_38"), ref scatter.distributionParams.scaleRandomness, GUIHelperFunctions.FloatField, callback);

                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_39"), ref scatter.distributionParams.noiseCutoff, GUIHelperFunctions.FloatField, callback);

                // Steep params
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_40"), ref scatter.distributionParams.steepPower, GUIHelperFunctions.FloatField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_41"), ref scatter.distributionParams.steepContrast, GUIHelperFunctions.FloatField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_42"), ref scatter.distributionParams.steepMidpoint, GUIHelperFunctions.FloatField, callback);

                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_43"), ref scatter.distributionParams.maxNormalDeviance, GUIHelperFunctions.FloatField, callback);

                // Altitude params
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_44"), ref scatter.distributionParams.minAltitude, GUIHelperFunctions.FloatField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_45"), ref scatter.distributionParams.maxAltitude, GUIHelperFunctions.FloatField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_46"), ref scatter.distributionParams.altitudeFadeRange, GUIHelperFunctions.FloatField, callback);

                // Mesh params
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_47"), ref scatter.distributionParams.alignToTerrainNormal, GUIHelperFunctions.IntField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_48"), ref scatter.distributionParams.coloredByTerrain, GUIHelperFunctions.BoolField, callback);

                GUILayout.Label("LOD Params: ", HighLogic.Skin.label);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_49"), ref scatter.distributionParams.lod1.range, GUIHelperFunctions.FloatField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_50"), ref scatter.distributionParams.lod2.range, GUIHelperFunctions.FloatField, callback);
                GUISection.End();
            }
        }
        static void ProcessBiomeBlacklist(Scatter scatter)
        {
            if (scatter.isShared) { return; }
            CBAttributeMapSO biomeMap = FlightGlobals.currentMainBody.BiomeMap;
            if (biomeMap == null || biomeMap.Attributes == null)
            {
                return;
            }

            if (GUILayout.Button(Localizer.Format("#PARALLAX_51"), GetButtonColor(showBiomeBlacklist)))
            {
                showBiomeBlacklist = !showBiomeBlacklist;
            }
            if (!showBiomeBlacklist)
            {
                return;
            }

            GUISection.Begin();
            CBAttributeMapSO.MapAttribute[] attributes = biomeMap.Attributes;
            List<string> blacklistedBiomes = scatter.biomeBlacklistParams.blacklistedBiomes;
            bool blacklistChanged = false;

            GUILayout.Label("Biome Blacklist: ", HighLogic.Skin.label);
            for (int i = 0; i < blacklistedBiomes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#PARALLAX_52") + (i + 1));

                List<string> options = GetSelectableBiomes(attributes, blacklistedBiomes, blacklistedBiomes[i]);
                string biome = GUIHelperFunctions.DropdownField(blacklistedBiomes[i], options, scatter.scatterName + "-" + i, out bool biomeChanged);
                if (biomeChanged)
                {
                    blacklistedBiomes[i] = biome;
                    blacklistChanged = true;
                }

                if (GUIHelperFunctions.CompactButton("X", 24))
                {
                    blacklistedBiomes.RemoveAt(i);
                    GUIHelperFunctions.CloseDropdown();
                    blacklistChanged = true;
                    i--;
                }
                GUILayout.EndHorizontal();
            }

            List<string> remainingBiomes = GetSelectableBiomes(attributes, blacklistedBiomes, null);
            if (remainingBiomes.Count > 0 && GUILayout.Button("Add Blacklisted Biome", HighLogic.Skin.button))
            {
                blacklistedBiomes.Add(remainingBiomes[0]);
                blacklistChanged = true;
            }
            GUISection.End();

            if (blacklistChanged)
            {
                scatter.biomeBlacklistParams.fastBlacklistedBiomes.Clear();
                scatter.biomeBlacklistParams.fastBlacklistedBiomes.UnionWith(blacklistedBiomes);
                BiomeLoader.UpdateBiomeMask(scatter, attributes);
                scatter.ReinitializeDistribution();
            }
        }
        static List<string> GetSelectableBiomes(CBAttributeMapSO.MapAttribute[] attributes, List<string> blacklistedBiomes, string currentBiome)
        {
            List<string> biomes = new List<string>();
            for (int i = 0; i < Mathf.Min(attributes.Length, 64); i++)
            {
                string name = attributes[i].name;
                if (name == currentBiome || !blacklistedBiomes.Contains(name))
                {
                    biomes.Add(name);
                }
            }
            return biomes;
        }
        static void ProcessBaseMaterialParams(Scatter scatter)
        {
            if (GUILayout.Button(Localizer.Format("#PARALLAX_53"), GetButtonColor(showMaterial)))
            {
                showMaterial = !showMaterial;
            }
            if (showMaterial)
            {
                GUISection.Begin();
                ScatterRenderer renderer = ScatterManager.Instance.fastScatterRenderers[scatter.scatterName];

                if (GUILayout.Button(Localizer.Format("#PARALLAX_54"), GetButtonColor(showLOD0)))
                {
                    showLOD0 = !showLOD0;
                }
                if (showLOD0)
                {
                    GUISection.Begin();
                    ProcessMaterialParams(scatter,
                        scatter.materialParams,
                        renderer,
                        renderer.instancedMaterialLOD0,
                        renderer.SetLOD0MaterialParams,
                        ref showLOD0Keywords,
                        true);
                    GUISection.End();
                }

                if (GUILayout.Button(Localizer.Format("#PARALLAX_55"), GetButtonColor(showLOD1)))
                {
                    showLOD1 = !showLOD1;
                }
                if (showLOD1)
                {
                    GUISection.Begin();
                    ProcessMaterialParams(scatter,
                        scatter.distributionParams.lod1.materialOverride,
                        renderer,
                        renderer.instancedMaterialLOD1,
                        renderer.SetLOD1MaterialParams,
                        ref showLOD1Keywords,
                        false);
                    GUISection.End();
                }

                if (GUILayout.Button(Localizer.Format("#PARALLAX_56"), GetButtonColor(showLOD2)))
                {
                    showLOD2 = !showLOD2;
                }
                if (showLOD2)
                {
                    GUISection.Begin();
                    ProcessMaterialParams(scatter,
                        scatter.distributionParams.lod2.materialOverride,
                        renderer,
                        renderer.instancedMaterialLOD2,
                        renderer.SetLOD2MaterialParams,
                        ref showLOD2Keywords,
                        false);
                    GUISection.End();
                }

                if (ReloadTexturesButton())
                {
                    BeginTextureReload(ReloadScatterTexturesFromDisk(FlightGlobals.currentMainBody.name));
                }

                GUISection.End();
            }
        }
        static IEnumerator ReloadScatterTexturesFromDisk(string planetName)
        {
            if (!ConfigLoader.parallaxScatterBodies.TryGetValue(planetName, out ParallaxScatterBody body))
            {
                yield break;
            }

            body.UnloadTextures();

            yield return WaitForTextureLoaderCleanup();

            if (FlightGlobals.currentMainBody == null || FlightGlobals.currentMainBody.name != planetName)
            {
                yield break;
            }

            foreach (ScatterRenderer renderer in ScatterManager.Instance.activeScatterRenderers)
            {
                renderer.SetLOD0MaterialParams();
                renderer.SetLOD1MaterialParams();
                renderer.SetLOD2MaterialParams();
            }

            foreach (TextureHandle handle in new List<TextureHandle>(body.loadedTextures.Values))
            {
                if (!handle.IsComplete)
                {
                    yield return handle;
                }
            }

            // Lets the renderers' texture assignment coroutines finish before unlocking
            yield return null;
        }
        static void ProcessDistributionNoiseParams(Scatter scatter)
        {
            if (scatter.isShared) { return; }
            if (GUILayout.Button(Localizer.Format("#PARALLAX_57"), GetButtonColor(showDistributionNoise)))
            {
                showDistributionNoise = !showDistributionNoise;
            }
            if (showDistributionNoise)
            {
                GUISection.Begin();
                // Callback method that regenerates scatters when these values are changed
                ParamCreator.ChangeMethod callback = scatter.ReinitializeDistribution;

                GUILayout.Label("Distribution Params: ", HighLogic.Skin.label);

                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_58"), ref scatter.noiseParams.noiseType, GUIHelperFunctions.EnumField, callback);

                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_32"), ref scatter.noiseParams.seed, GUIHelperFunctions.IntField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_59"), ref scatter.noiseParams.frequency, GUIHelperFunctions.FloatField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_60"), ref scatter.noiseParams.lacunarity, GUIHelperFunctions.FloatField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_61"), ref scatter.noiseParams.octaves, GUIHelperFunctions.IntField, callback);
                ParamCreator.CreateParam(Localizer.Format("#PARALLAX_62"), ref scatter.noiseParams.inverted, GUIHelperFunctions.BoolField, callback);
                GUISection.End();
            }
        }
        // Very ugly method which used to be quite elegant until I needed to parse keywords while keeping LOD material overrides consistent
        static void ProcessMaterialParams(Scatter scatter, in MaterialParams materialParams, ScatterRenderer renderer, Material material, ParamCreator.ChangeMethod callback, ref bool keywordBool, bool isBaseMaterial)
        {
            GUILayout.Label("Material Params: ", HighLogic.Skin.label);
            ShaderProperties properties = materialParams.shaderProperties;

            List<(ShaderProperties properties, ParamCreator.ChangeMethod callback)> inheritors = null;
            if (isBaseMaterial)
            {
                inheritors = new List<(ShaderProperties properties, ParamCreator.ChangeMethod callback)>();
                if (scatter.distributionParams.lod1.materialOverride.shader == materialParams.shader)
                {
                    inheritors.Add((scatter.distributionParams.lod1.materialOverride.shaderProperties, renderer.SetLOD1MaterialParams));
                }
                if (scatter.distributionParams.lod2.materialOverride.shader == materialParams.shader)
                {
                    inheritors.Add((scatter.distributionParams.lod2.materialOverride.shaderProperties, renderer.SetLOD2MaterialParams));
                }
            }

            ProcessTextureParams(properties.shaderTextures, (key, oldValue, value) =>
            {
                if (IsValidTexturePath(value))
                {
                    callback();
                }
                PropagateToInheritors(x => x.shaderTextures, key, oldValue, value, inheritors);
            });

            ProcessGenericMaterialParams(materialParams, callback, inheritors: inheritors);

            // Process keywords
            // This requires reading configs which can be slow, since we don't store keywords anywhere except ones that are already enabled
            if (GUILayout.Button(Localizer.Format("#PARALLAX_63"), GetButtonColor(keywordBool)))
            {
                keywordBool = !keywordBool;
            }
            if (keywordBool)
            {
                GUISection.Begin();
                GUILayout.Label("Keywords", HighLogic.Skin.label);
                if (wrappedLabel == null)
                {
                    wrappedLabel = new GUIStyle(UnityEngine.GUI.skin.label) { wordWrap = true };
                }
                GUILayout.Label(Localizer.Format("#PARALLAX_64"), wrappedLabel);
                // Get shader name
                string shaderName = materialParams.shader;

                // The linq incident
                // Gets "ParallaxShader" node
                UrlDir.UrlConfig shaderBank = GameDatabase.Instance.GetConfigs("ParallaxScatterShaderProperties").FirstOrDefault();
                ConfigNode config = shaderBank.config.GetNodes("ParallaxShader").Where(x => x.GetValue("name") == shaderName).FirstOrDefault();
                ConfigNode keywordsNode = config.GetNode("Keywords");
                foreach (ConfigNode node in keywordsNode.GetNodes())
                {
                    // These are our keywords
                    string keywordName = node.name;
                    bool enabled = materialParams.shaderKeywords.Contains(keywordName);
                    bool wasChanged = ParamCreator.CreateParam(keywordName, ref enabled, GUIHelperFunctions.BoolField);
                    if (wasChanged)
                    {
                        SetKeyword(materialParams, null, material, shaderBank, keywordsNode, keywordName, enabled);
                        callback();

                        // LODs follow the base material's keyword unless they had it set differently
                        if (isBaseMaterial && FollowsBaseKeyword(scatter.distributionParams.lod1.materialOverride, materialParams.shader, keywordName, enabled))
                        {
                            SetKeyword(scatter.distributionParams.lod1.materialOverride, materialParams.shaderProperties, renderer.instancedMaterialLOD1, shaderBank, keywordsNode, keywordName, enabled);
                            renderer.SetLOD1MaterialParams();
                        }
                        if (isBaseMaterial && FollowsBaseKeyword(scatter.distributionParams.lod2.materialOverride, materialParams.shader, keywordName, enabled))
                        {
                            SetKeyword(scatter.distributionParams.lod2.materialOverride, materialParams.shaderProperties, renderer.instancedMaterialLOD2, shaderBank, keywordsNode, keywordName, enabled);
                            renderer.SetLOD2MaterialParams();
                        }
                    }
                }
                GUISection.End();
            }
        }
        static bool FollowsBaseKeyword(in MaterialParams lodMaterialParams, string baseShader, string keyword, bool enabled)
        {
            return lodMaterialParams.shader == baseShader && lodMaterialParams.shaderKeywords.Contains(keyword) != enabled;
        }
        static void SetKeyword(in MaterialParams materialParams, ShaderProperties fallbackProperties, Material material, UrlDir.UrlConfig shaderBank, ConfigNode keywordsNode, string keyword, bool enabled)
        {
            List<string> keywords = materialParams.shaderKeywords;
            if (enabled)
            {
                // The config loader drops a keyword when the keyword superseding it is also enabled, so only allow one of them
                string supersededBy = keywordsNode.GetNode(keyword).GetValue("supersededBy");
                keywords.RemoveAll(x => x == supersededBy || keywordsNode.GetNode(x)?.GetValue("supersededBy") == keyword);
                if (!keywords.Contains(keyword))
                {
                    keywords.Add(keyword);
                }
            }
            else
            {
                keywords.Remove(keyword);
            }

            // Rebuild from the template so properties shared between keywords are kept, and removed values come back if the keyword is re-enabled
            ShaderProperties template = ConfigLoader.LookupTemplateConfig(shaderBank, materialParams.shader, new List<string>(keywords));
            ShaderProperties properties = materialParams.shaderProperties;
            if (!removedKeywordProperties.TryGetValue(properties, out ShaderProperties removed))
            {
                removed = new ShaderProperties();
                removedKeywordProperties.Add(properties, removed);
            }

            SyncKeywordProperties(properties.shaderTextures, template.shaderTextures, removed.shaderTextures, fallbackProperties?.shaderTextures);
            SyncKeywordProperties(properties.shaderFloats, template.shaderFloats, removed.shaderFloats, fallbackProperties?.shaderFloats);
            SyncKeywordProperties(properties.shaderVectors, template.shaderVectors, removed.shaderVectors, fallbackProperties?.shaderVectors);
            SyncKeywordProperties(properties.shaderColors, template.shaderColors, removed.shaderColors, fallbackProperties?.shaderColors);
            SyncKeywordProperties(properties.shaderInts, template.shaderInts, removed.shaderInts, fallbackProperties?.shaderInts);

            foreach (ConfigNode node in keywordsNode.GetNodes())
            {
                if (keywords.Contains(node.name))
                {
                    material.EnableKeyword(node.name);
                }
                else
                {
                    material.DisableKeyword(node.name);
                }
            }
        }
        static void SyncKeywordProperties<T>(Dictionary<string, T> properties, Dictionary<string, T> template, Dictionary<string, T> removed, Dictionary<string, T> fallback)
        {
            foreach (string key in properties.Keys.ToList())
            {
                if (!template.ContainsKey(key))
                {
                    removed[key] = properties[key];
                    properties.Remove(key);
                }
            }
            foreach (KeyValuePair<string, T> pair in template)
            {
                if (properties.ContainsKey(pair.Key))
                {
                    continue;
                }
                if (removed.TryGetValue(pair.Key, out T value) || (fallback != null && fallback.TryGetValue(pair.Key, out value)))
                {
                    properties.Add(pair.Key, value);
                }
                else
                {
                    properties.Add(pair.Key, pair.Value);
                }
            }
        }
        static void ProcessDebug(Scatter scatter)
        {
            if (GUILayout.Button(Localizer.Format("#PARALLAX_66"), GetButtonColor(showDebug)))
            {
                showDebug = !showDebug;
            }
            if (showDebug)
            {
                GUISection.Begin();
                GUILayout.Label("Debug Settings", HighLogic.Skin.label);
                if (ParamCreator.CreateParam(Localizer.Format("#PARALLAX_67"), ref debugShowFaceOrientation, GUIHelperFunctions.BoolField))
                {
                    ShowFaceOrientation(debugShowFaceOrientation, scatter);
                }
                if (ParamCreator.CreateParam(Localizer.Format("#PARALLAX_68"), ref debugShowCollideables, GUIHelperFunctions.BoolField))
                {
                    ShowCollideableScatters(debugShowCollideables);
                }
                if (GUILayout.Button("Log Performance Stats", HighLogic.Skin.button))
                {
                    LogPerformanceStats();
                }
                GUISection.End();
            }
        }
        static void ShowFaceOrientation(bool enabled, Scatter scatter)
        {
            ScatterRenderer renderer = ScatterManager.Instance.fastScatterRenderers[scatter.scatterName];
            if (enabled)
            {
                renderer.instancedMaterialLOD0.EnableKeyword("DEBUG_FACE_ORIENTATION");
                renderer.instancedMaterialLOD1.EnableKeyword("DEBUG_FACE_ORIENTATION");
                renderer.instancedMaterialLOD2.EnableKeyword("DEBUG_FACE_ORIENTATION");
            }
            else
            {
                renderer.instancedMaterialLOD0.DisableKeyword("DEBUG_FACE_ORIENTATION");
                renderer.instancedMaterialLOD1.DisableKeyword("DEBUG_FACE_ORIENTATION");
                renderer.instancedMaterialLOD2.DisableKeyword("DEBUG_FACE_ORIENTATION");
            }
        }
        public static void ShowCollideableScatters(bool enabled)
        {
            if (enabled)
            {
                foreach (Scatter scatter in scatters)
                {
                    if (scatter.renderer.instancedMaterialLOD0.HasProperty("_FresnelPower"))
                    {
                        scatter.renderer.instancedMaterialLOD0.SetFloat("_FresnelPower", 1);
                    }
                    if (scatter.renderer.instancedMaterialLOD1.HasProperty("_FresnelPower"))
                    {
                        scatter.renderer.instancedMaterialLOD1.SetFloat("_FresnelPower", 1);
                    }
                    if (scatter.renderer.instancedMaterialLOD2.HasProperty("_FresnelPower"))
                    {
                        scatter.renderer.instancedMaterialLOD2.SetFloat("_FresnelPower", 1);
                    }

                    Color col = scatter.collideable ? new Color(0.2f, 1.0f, 0.2f) : new Color(1.0f, 0.2f, 0.2f);
                    if (scatter.renderer.instancedMaterialLOD0.HasProperty("_FresnelColor"))
                    {
                        scatter.renderer.instancedMaterialLOD0.SetColor("_FresnelColor", col);
                    }
                    if (scatter.renderer.instancedMaterialLOD1.HasProperty("_FresnelColor"))
                    {
                        scatter.renderer.instancedMaterialLOD1.SetColor("_FresnelColor", col);
                    }
                    if (scatter.renderer.instancedMaterialLOD2.HasProperty("_FresnelColor"))
                    {
                        scatter.renderer.instancedMaterialLOD2.SetColor("_FresnelColor", col);
                    }

                    scatter.renderer.instancedMaterialLOD0.SetColor("_Color", col);
                    scatter.renderer.instancedMaterialLOD1.SetColor("_Color", col);
                    scatter.renderer.instancedMaterialLOD2.SetColor("_Color", col);
                }
            }
            else
            {
                foreach (Scatter scatter in scatters)
                {
                    scatter.renderer.SetLOD0MaterialParams();
                    scatter.renderer.SetLOD1MaterialParams();
                    scatter.renderer.SetLOD2MaterialParams();
                }
            }
        }
        static void LogPerformanceStats()
        {
            // Process renderer
            int numTris = 0;
            foreach (Scatter scatter in scatters)
            {
                ScatterRenderer scatterRenderer = scatter.renderer;

                numTris += scatterRenderer.LogStats();
            }

            ParallaxDebug.Log("");
            ParallaxDebug.Log("Total number of triangles being rendered right now by Parallax: " + numTris);
            ParallaxDebug.Log("Performance logging complete");

            // 0 = total usage, 1 = potential min usage, 2 = wasted
            int[] stats = new int[3];
            // Now process scatters on quads
            foreach (ScatterSystemQuadData quad in ScatterComponent.scatterQuadData.Values)
            {
                int[] quadStats = quad.GetMemoryUsage();
                for (int i = 0; i < quadStats.Length; i++)
                {
                    stats[i] += quadStats[i];
                }
            }

            ParallaxDebug.Log("");
            ParallaxDebug.Log("Total memory used by the scatter system quads: " + stats[0]);
            ParallaxDebug.Log("Potential minimum usage by the scatter system quads: " + stats[1]);
            ParallaxDebug.Log("Memory wasted by the scatter system quads: " + stats[2]);
        }
        static void RemoveKeywordValues(ShaderProperties shaderProperties, ConfigNode keywordNode)
        {
            ConfigNode[] allTypeNodes = keywordNode.GetNodes();
            foreach (ConfigNode node in allTypeNodes)
            {
                string[] keys = node.GetValues("name");
                foreach (string key in keys)
                {
                    // Blindly just remove them all. Shader properties can't share the same name, so this is safe
                    shaderProperties.shaderTextures.Remove(key);
                    shaderProperties.shaderFloats.Remove(key);
                    shaderProperties.shaderInts.Remove(key);
                    shaderProperties.shaderVectors.Remove(key);
                    shaderProperties.shaderColors.Remove(key);
                }
            }
        }
        static int IncrementScatterIndex()
        {
            int newIndex = currentScatterIndex + 1;
            if (newIndex >= scatters.Length)
            {
                newIndex = 0;
            }
            return newIndex;
        }
        static int DecrementScatterIndex()
        {
            int newIndex = currentScatterIndex - 1;
            if (newIndex < 0)
            {
                newIndex = scatters.Length - 1;
            }
            return newIndex;
        }
    }
}
