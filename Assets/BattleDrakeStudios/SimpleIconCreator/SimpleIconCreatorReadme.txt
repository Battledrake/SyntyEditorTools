Visit https://youtu.be/_NKHUgD7KAk for a video showcasing of the tool.

Features:
Editor tool to create icons. Don't have to hit play!
Previewscene to view object, rotate, or reposition.
Select Icon Resolution.
Colored or Transparent backgrounds supported.

Steps to use:
- After installing package, go to BattleDrakeStudios/SimpleIconCreator.
- Drag and drop a prefab from your project files into the object field(Scene objects are not supported.) Can also click on the icon to show list of all objects and select from there.
- Set icon resolution, transparency, background color, and icon name.
New Feature: Can now set background texture by dragging and dropping the desired texture into the object field just below transparency button!
Notice: Editor Window has to be at least twice the size of the preview window's size in order to avoid grey rendering. Resize if you notice your texture is being cutoff.
- Press Create Icon when finished. Icon will save to BattleDrakeStudios/SimpleIconCreator/Icons.

Controls:
- Left mouse button dragged left to right rotates object on the y axis(yaw), relative to the object.
- Left mouse button dragged up and down rotates object on the x axis(pitch), relative to the world.
- Right mouse button dragged left to right rotates object on the z axis(roll), relative to the world.
- Middle mouse button, pressed and dragged, moves the object.
- Scrollwheel zooms in and out.
- Holding shift will increase the speed at which several of these actions are performed.

Notes:
- Don't forget to select image and set TextureType to Sprite and UI after creating images.

- You may notice a bit of choppiness/pixelation when creating a transparent image.
Extra steps were taken to provide the best quality transparent images unity could provide, but the way textures are created prevents sharp pixels.
When texture sampling, the pixel sized gaps between the object being rendered and the background, create a pixel bleeding effect due to the smoothing effect of unity's linear sampling.
When left on, the object would have an outline of the background color and was a complete eyesore. To prevent this, transparent images are captured using the point filtermode. 
This results in images with no background color bleeding, but also means the edges aren't smoothed, leaving them looking blocky when zoomed in.
Non transparent icons have the standard filtermode applied.

Support: BattleDrakeStudios@Gmail.com