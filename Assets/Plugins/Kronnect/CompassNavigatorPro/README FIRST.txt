************************************
*      COMPASS NAVIGATOR PRO 2     *
*    (C) Copyright 2023 Kronnect   * 
*            README FILE           *
************************************


How to use this asset
---------------------

Thanks for purchasing Compass Navigator Pro!

Using Compass Navigator Pro 2 is very easy! Please take a moment to read the Quick Start Guide located in the Documentation folder.



Help & Support Forum
--------------------

Check the Documentation folder for detailed instructions.
Have any question or issue?
* Support-Web: https://kronnect.com/support
* Support-Discord: https://discord.gg/EH2GMaM
* Email: contact@kronnect.com
* Twitter: @Kronnect



Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Compass Navigator Pro be eventually available on the Asset Store.



Other Cool Assets!
------------------

Check our other assets on the Asset Store publisher page:
https://assetstore.unity.com/publishers/15018




Version history
---------------

Version 1.9
- Minimap: camera setup optimizations for URP/HDRP
- Minimap: added "Clamp To World Edges" option for non-maximized mode
- Minimap: added "Captured Depth" option

Version 1.8
- Minimap: added "Radar Range" option to inspector
- Minimap: added option to display ring interval distance or radar range in the minimap

Version 1.7.1
- API: added StartCircleAnimation() method to CompassProPOI
- [Fix] Fixed visibility of view cone outline in radar mode

Version 1.7:
- MiniMap: added View Cone Fall-Off option
- MiniMap: added View Cone Outline option
- MiniMap: added "Allow Rotation" option when mini-map mode is set to UI Texture
- [Fix] Fixes for mini-map view cone

Version 1.6:
- MiniMap: added "Background Opaque" option
- MiniMap: added "Clamp To World Edges" option
- [Fix] Fixes an issue that moved the player icon on mini-map out of position when returning from maximized mode

Version 1.5:
- Radar: added option to enable/disable pulse effect
- MiniMap: added options to enable/disable user drag in normal or maximized modes
- API: added OnPOIOnScreen / OnPOIOffScreen events triggered when indicators become on or off screen

Version 1.4:
- Optimized mini-map world-mapped texture mode which now runs without camera

Version 1.3:
- Added "Orientation" option to MiniMap (camera or follow)
- Improved drag & clamping in maximized mode

Version 1.2.4:
- Internal fixes and minor improvements

Version 1.2.2:
- Added support for RPG Builder integration
- API: CompassProPOI class is now partial to allow extensions
- [Fix] Fixed text reveal pool limit issue

Version 1.2.1:
- Added "Play AudioClip When Visited" option to POI inspector
- [Fix] Fixed 'Visited Distance Max' issue

Version 1.2:
- Compass bar: added icon distance text
- Compass bar: new options for half-winds / ticks on the Compass Bar
- MiniMap: added view cone
- MiniMap: added circle of interest
- MiniMap: added option: when an icon appears, a circle animation is shown (can be disabled in the POI component)
- MiniMap: added radar mode with pulse effect and accurate rings distance
- MiniMap: added ability to pan map around with mouse (right click on MiniMap to center)
- MiniMap: added cardinals
- MiniMap: added tint color option
- New feature: off-screen POI indicators (can be enabled/disabled globally in the Compass Navigator inspector and per POI)
- New POI inspector with better organized settings
- Additional options for visual indicators (they now uses a customizable prefab, near fade distance, ...)
- Revamped inspector with full undo support
- Demo scene: added code example for adding / removing POIs clicking on the minimap
- API: added events OnPOIRegister / OnPOIUnregister / OnPOIEnterCircle / OnPOIExitCircle





