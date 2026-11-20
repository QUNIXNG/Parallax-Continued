# Parallax Continued
Support development at [Linx's Patreon](https://www.patreon.com/linx_)

## 简体中文本地化（本 Fork 新增）

本仓库是 [Gameslinx/Parallax-Continued](https://github.com/Gameslinx/Parallax-Continued) 的 Fork，
在原版基础上补充了 **简体中文界面**，未改动地形、着色器、散布物等任何渲染逻辑，
配置结构（`Config/`）与默认参数保持与上游一致。

### 汉化范围

* 主设置窗口：地形着色器、散布系统、光照、缩放空间、可视化等全部分组与参数名
* 参数提示（tooltip）：细分质量、散布密度/距离倍率、碰撞级别、阴影步进等说明文字
* 散布物编辑器（Scatter）、地形编辑器（Terrain）、贴图导出器（Exporter）的按钮与标签

### 有意保留英文的部分

* **着色器关键字与属性名**（`AMBIENT_OCCLUSION`、`ADVANCED_BLENDING`、`_MaxTessellation`、`_Color` 等）：
  这些字符串会直接传给 `EnableKeyword` / `SetFloat` / `SetColor`，翻译后着色器将匹配不到
* **配置节点与文件名**（`Scatter-Backup`、`-EXPORTED`、`-Exported.cfg`、`ParallaxGUIScroll` 等）：
  参与导出与存档读写
* **贴图与预制体路径**（`ParallaxContinued/Textures/*`）

### 文件说明

```
Mod Source/GameData/ParallaxContinued/Localization/en-us.cfg   英文原文
Mod Source/GameData/ParallaxContinued/Localization/zh-cn.cfg   简体中文
```

游戏语言设为简体中文时自动生效。

### 构建说明

与上游一致：.NET SDK + `KSPBuildTools` + `Krafs.Publicizer`（见 `Parallax.csproj`）。
新增的 `using KSP.Localization;` 与 `Localizer.Format("#PARALLAX_n")` 只依赖 KSP 自带的 `Assembly-CSharp`。

### 同步上游

改动仅限 `Mod Source/Parallax/GUI/` 下 6 个文件，上游更新后 `git merge upstream/master` 即可，
冲突通常只出现在这些 GUI 文件。

## Installation Instructions - Windows

Make sure you first **uninstall the old Parallax version** if you have it installed.

1. Download and install the required dependencies:
    - Kopernicus: https://github.com/Kopernicus/Kopernicus/releases
    - KSP Burst: https://github.com/KSPModdingLibs/KSPBurst/releases
    
2. Ensure you have downloaded the following files:
    - ParallaxContinued.zip
    - ParallaxContinued_StockTerrainTextures.zip
    - ParallaxContinued_StockPlanetTextures.zip
    - ParallaxContinued_StockScatterTextures.zip
    - Optional lifeless Eve patch - ParallaxContinued_LifelessEvePatch.zip

3. Browse to **ParallaxContinued.zip/GameData** and move **ParallaxContinued** to your GameData folder
4. Browse to **ParallaxContinued_StockTerrainTextures.zip/GameData** and move **Parallax_StockTerrainTextures** to your GameData folder
5. Browse to **ParallaxContinued_StockScatterTextures.zip/GameData** and move **Parallax_StockScatterTextures** to your GameData folder
6. Browse to **ParallaxContinued_StockPlanetTextures.zip/GameData** and move **Parallax_StockPlanetTextures** to your GameData folder
7. If you downloaded the lifeless eve patch, move '**ParallaxContinued_LifelessEvePatch**' from '**Parallax_LifelessEvePatch.zip/GameData**' to your **GameData** folder

8. OPTIONAL - Delete the **Settings.cfg** file in your **Kerbal Space Program** folder. This ensures your planet configuration is clean and any junk from previous planet mods is gone.

9. Launch the game

10. **IMPORTANT - In the main menu, click 'Settings', then 'Graphics', and set 'Terrain Detail' to 'High'**

NOTE: If you use Deferred, you might need to install Deferred manually (not via CKAN) if CKAN reports a conflict!
Any issues? You can reach me on Discord (linked in my Patreon description) or the KSP forums as 'Gameslinx'. I go by 'Linx' anywhere else.

## Installation Instructions - Mac

1. Follow the Windows installation instructions first, but **refer to step 2 for running the game**
2. Run Kerbal Space Program using **CrossOver** with these settings:
   
![hWevPwF](https://github.com/user-attachments/assets/6958cf71-8885-47f5-b90b-a30a8980c8d2)

If you don't follow the above step, Parallax Continued will not work!

## Installation Instructions - Linux

1. Follow the Windows installation instructions first, but **refer to step 2 for running the game**
2. Run Kerbal Space Program using **Proton and use DX11**

## Recommended Mods
Mods that are fully compatible with and enhance the visual experience when installed with Parallax:
 - Deferred (Improves surface reflections)
 - Scatterer (Atmospheric scattering)
 - Environmental Visual Enhancements (EVE) or EVE Volumetrics (Clouds, rain)

## List of Supported Planet Packs
"Author Contacted" means not currently supported, but I've reached out to the author to help get upgrading existing configs started

**"Release 2" configs will work with Release 3**, but the orbit textures won't be improved.
|Planet Pack Name|Compatibility Status|Supports Release|Last Updated
|----|----|----|----
|Stock Planetary System|Fully supported|Public Release|
|Stock Planetary System|Fully supported - no extra configs needed|Public Release
|Sol (People looking for RSS Reborn, look here)|[Download](https://docs.google.com/document/d/1GyQlvDE8JD81fL8GPenKOA3Do4GqlDSx0tWsKy_p208/edit?tab=t.0)|Public Release|Not maintained by me
|Promised Worlds|[Download](https://github.com/PromisedWorlds/PromisedWorlds/releases)|Public Release|Not maintained by me
|Cosmic Serenity|[Download](https://forum.kerbalspaceprogram.com/topic/228453-112x-cosmic-serenity-v211/)|Public Release|Not maintained by me
|OPM - Jthero Configs|[Download](https://forum.kerbalspaceprogram.com/topic/226905-1125-aetherium-021-16-feb-2025-random-assortment-of-parallax-continued-configs/)|Public Release|Not maintained by me
|MPE - Jthero Configs|[Download](https://forum.kerbalspaceprogram.com/topic/226905-1125-aetherium-021-16-feb-2025-random-assortment-of-parallax-continued-configs/)|Public Release|Not maintained by me
|Quack Pack - Jthero Configs|[Download](https://forum.kerbalspaceprogram.com/topic/226905-1125-aetherium-021-16-feb-2025-random-assortment-of-parallax-continued-configs/)|Public Release|Not maintained by me
|Nem - Jthero Configs|[Download](https://forum.kerbalspaceprogram.com/topic/226905-1125-aetherium-021-16-feb-2025-random-assortment-of-parallax-continued-configs/)|Public Release|Not maintained by me
|Beyond Home|Maintainer Contacted
|Kcalbeloh|[Download](https://drive.google.com/file/d/1co_qgylXOP4WqXgu5gvF4qpDZzZp1qVt/view?usp=drive_link)|Public Release - But does not change the orbit textures|09/02/2025
|Celestial Harmony|[Download](https://github.com/ProximaCentauri-star/Celestial-Harmony/releases)|Public Release|Not maintained by me
|Techo's Stock Planet Revamp|Author Contacted
|KSRSS Reborn|Removed, as they are broken| | 
|JNSQ|[Download](https://github.com/yukkine0704/JNSQ_ParallaxScattersContinued/releases)|Public Release|Not maintained by me
|1 Thousand Special|[Download](https://forum.kerbalspaceprogram.com/topic/228426-1-thousand-special-planet-pack-ksp-1125/)|Public Release|Not maintained by me

## License
All Rights Reserved
 - Excludes bingus.dds
