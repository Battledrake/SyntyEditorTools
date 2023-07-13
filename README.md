Visit https://youtu.be/ljYOAoZYQOw and https://youtu.be/Pg6UZSdqKrYfor for video showcasing of the tool and it's use.

<b>Warning:</b> Only the standard FantasyHero material currently works(You can use it as existing or create a new one from it). This appears to be an issue with Synty's pack and not my tool. All other materials in the standard and custom folders, FantasyHero_01, 02, etc.. appear broken.
<br><b>Update:</b> I've contacted Synty regarding the material issue, and it appears to be something related to URP. They've informed me they are currently working on a solution but the current solution is to upgrade to the URP and switch the shaders. This is only if you're attempting to use any material but the default FantasyHero.

Steps to get up and running:

1. After installing package, drag and drop a Synty Modular Fantasy Hero model into the scene. 
The model can be the base model with no material assignments, the prefab with material assignments, any of the presets, or a custom character you've designed;
as long as you kept the original hierarchy intact.

2. Add the ModularCharacterManager Component to the gameobject. This asset works by searching all the transforms of the object to find the necessary parts, 
so the component can be placed on the model gameobject, or a parent if that's your setup. 
Alternatively, you can just highlight the gameobject, and go into the new menu option ModularCharacter and select SetupWizard. 
This will give you the option to add the component through the custom editor.

3. After the component is added, select whether this is a new character or existing. New means you are setting up the character and resetting all it's parts to a base form. 
Existing means you are setting up a character, but keeping all the currently active parts on and registered. Do this if you want to keep a model looking exactly as it is.

4. Choose whether the model is male or female. If it's a new character, your choice can be changed in the editor. 
If it's an existing, make sure to match the gender with the currently active parts in order to make sure the parts are correctly registered to the right lists.

5. Create duplicate or use existing material. Creating a duplicate will have you select a material that will be duplicated and saved to BattleDrakeStudios/ModularCharacterEditor/Materials.
Existing will have you choose a material that will be shared with any other objects that use that material. 
The material to be selected will need to be one of Synty's or one that uses their shader in order to for the tool to recognize the Shader Properties.

7. You will then be presented with the options to Commit which will initialize the character. 
Commit and Show Editor will initialize and automatically load the custom editor tool for modification. 
Reset will start over from the beginning.

8. Once initialized, your character is ready to go. Any changes can be made with the Modular Editor.
Values for the parts range from -1 to all the parts currently registered under their respective hierarchies;
-1 disables all active parts under that category and is a value used for the editor tool only. Disabling a part through the component requires only calling the DeactivatePart method with the bodypart.
You can swap the gender between male and female at any stage and all parts will remain the same except eyebrows(male forms have more, so the value is reset to avoid an index out of bounds issue).
Colors can be changed and are linked with Unity's Undo system making it easy to reverse any unwanted changes to the color system.

9. While the component is active and initialized, you have access to all the parts and the arrays that store them. 
The parts are arranged into two dictionaries for easy access and manipulation in game.
To activate a part, you can call the method ActivePart in the ModularCharacterManager component class, passing in an enum ModularBodyPart for the part to activate, and the int partID which is the index of it within it's array.
(the number in the Modular Editor is the correct partID you can use unless you wish to deactivate. Don't use -1, that's for the tool only).
The activepart method will automatically deactivate any currently active parts for that particular bodypart, meaning no direct deactivating is required when activating another part. 
If you wish to deactivate a part manually, however, you can do so using the DeactivatePart method, putting in only the ModularBodyPart you wish to deactivate.

ARMOR CREATOR TOOL:

A new tool added to the original pack. This tool allows you to create custom armor pieces to attach to your character like in many popular MMO's. Steps to get it working.

1. Setup a new character from the Synty prefab. Choose the gender you wish as the base(can toggle between in the tool). Use an existing material, select fantasy hero.
(The other materials are not functioning properly. An issue with Synty's pack, not mine). Commit changes.
2. Rename gameobject to "Pf_ArmorCreatorBase" without the quotations.
3. Create a Resources folder in your Assets project folder.
4. Drag and drop the gameobject into the resources folder.
5. Go into BattleDrakeStudios menu, select ModularCharacters > ArmorCreator.
6. Tool is now loaded! Preview window can be manipulated with different mouse buttons. Left Mouse button to rotate back and forth, and spin. Right mouse button to roll.
Holding scroll/middle mouse button will reposition. Scrollwheel will zoom in and out. Holding shift will increase the speed of these functions.
7. Dropdown to select armor parts. If you wish to setup your own custom popup, access the enum class ModularArmorType and set up the different armor selections. Aftewards,
go into the ModularArmorCreator class and scroll to the very bottom. There is a method to setup which parts are associated with each option. Name the cases to match your enum,
then add the parts you want associated with the enum value to its case block.
Afterwards, the tool should automatically populate the options for you. No further configuration needed. Create the item using your settings, and the created modularpart will reflect those changes.
8. After creating the item, you can create an icon using the built in Icon Creator. The tool was also released as a standalone, but i've included in this pack for you, and all setup with the tool.
You can use it to create icons of the modular pieces, or any prefabs you drag and drop into the object field. The tool will work independently of the charactereditor.
9. After created the armorpart, it'll be created as a ModularArmor scriptableobject. Use your existing item structure, or create an item scriptable object from the asset menu I created for you.
10. Drag the modulararmor scriptable into the objectfield in the item.
11. Take an existing initialized character in the scene, and attached the demo EquipmentManager script to it. Make sure to remove the randomizer script if it's still on.
12. Increase the array size to add the items you want, and drag and drop them into the fields.
13. Hit play, and watch your item get automatically equipped! Use the equipmentmanager as a reference to how equipment is activated.

Tips: You'll notice the base always spawns towards the top of the preview view. This is because the preview places the object based on it's privot point, which is at the feet for the modular characters.
To fix this, you can create an empty game object and name it Pf_ArmorCreatorBase. Afterwards, attach the modularfantasy prefab and set it's y position to -1. Run the setup wizard on the parent gameobject.
Drag and drop the prefab into the resources folder. Now your preview object will spawn in the center of the window!

The youtube video goes over all this and can be used as a reference on how to use the tool. Any additional questions or concerns can be directed to me at battledrakecreations@gmail.com
