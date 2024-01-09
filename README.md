# HappyCRappy

An app for making CR a happier process.

This app takes snapshots of your Conflict Resolution patches and the mods that they're overriding. If you update one of the overridden mods, you can compare to the previous snapshot and see if any of the records have changed, and get a quick idea of what, if anything, you need to update in your CR patch. This is *not* a Mator Smash replacement that tries to automatically resolve conflicts for your whole load order. I made this app to make it easier to update Wabbajack lists with custom mods and CR patches added on top, so I could quickly identify which records (if any) would need updated conflict resolution. You believe that happy crappy?

**Usage**

_Setting up the app_

Install HappyCRappy the way you would any other Mod Manager-launched tool. I have mine extracted to MO2/tools/HappyCRappy. Launch the app using your mod manager.

![HappyCRappy_MYb3sN0AVW](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/9d5b7afe-7683-49e4-971b-6e9e63a1c178)

Once launched, go to the Settings menu and makes sure your environment settings are correct for the game you are managing. If you have a portable installation, make sure the Game Data Directory is pointing to the correct location. If you are using rootbuilder, it should point to the root directory rather than the real data directory. The program should display a green "Environment is valid" message if everything is set up correctly.

The Snapshot directory is where the mod snapshots will be stored. By default this will be a Snapshots subdirectory under your installation folder, but you can select a different folder if needed.

Snapshots can be saved in JSON or YAML format. This is purely up to your preference and what you find easier to look at. If you select one format and then you decide that you prefer another, you can toggle the display format at any time. However, to always ensure correct display results, I recommend displaying snapshots in the the format in which they were taken. 

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/4c671fb6-9568-4ad5-b155-2250bc0c3b42)

You can now select Conflict Resolution patches to track. Just type your plugin name into the mod browser and click on it when it pops up. 

Additional settings:
- Use Deep Cacheing: Speeds up the conflict tracking by cacheing records as they're tracked. Uses more memory if enabled. Disable if you run out of memory using the app.
- Warm Up Cache On Startup: When launching the app, it will immediately go through your selected CR mods and start loading their records to cache. Disable if you run out of memory using the app.
- Handle Mismatched Record Types: Allow the app to load a record if its FormID changes between overrides (for example if in one mod XX123456 is an NPC and in another mod XX123456 has been recycled as an Armor). This should happen rarely if ever, but will slow down the app if it does occur.

_Using the app_

To understand what the app is doing, let's lay out a test case. Assume that in your load order you have the Modpocalypse NPC appearance replacer, and also SOS (Songs of Skyrim), which adds songs to some NPCs. You have gone through and made a Conflict Resolution patch for the two mods:

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/3a7af2ff-5ff1-46e9-99d1-69d1f39e15a3)

To track this CR patch, simply add "Mikael - Modpocalypse - SOS Patch.esp" to the tracked mod list:
![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/228f917b-8153-4ffd-bdef-fb93e7e97762)
Then navigate to the Snapshots menu and click the green "Take Snapshot" button. You should see a confirmation pop up that a snapshot has been taken.

Now let's say Songs of Skyrim updates, and Mikael gets a couple new songs:

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/5e2c3cd1-c5cc-42ac-ba3a-0bdbc8b7841e)

In this simple example there's only one record in the CR patch so we could just immediately look at it in SSEedit, but stretch your imagination to envision a large CR patch with tens of hundreds of records. We want to know what, if anything, has changed since before the update. To do this, launch Happy CRappy, go to the Snapshots menu, and select the most recent snapshot date from the "Compare to Snapshot" dropdown, and select "Mikael - Modpocalypse - SOS Patch.esp" from the mod selection dropdown. Note: once you have selected the mod, there will be a slight delay as its data is loaded. The delay will be more than slight if "Show potential conflicts" is checked (more on this later).

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/8cd30f6f-1be8-4833-aaf7-f198d16101e8)

You can now browse the records as you would in xEdit. Any records with red borders have a change/conflict. Records that have no changes have a white border. If "Hide non-conflicting records" is selected, these white bordered records will be hidden since you don't need to worry about updating them.

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/4f7b9aae-afd3-4fac-a7d8-1648db2e113c)

At the top you can see the load order of mods touching this record in the snapshot vs. in your current load order. In this case there have been no changes since the snapshot was taken.

Below the load order, you can see the details of the mods touching the record. Again, ones that have not changed since the snapshot was taken have a white border. Ones that have changed since the snapshot was taken (e.g. the ones you might need to include in your updated CR patch) have a red border. Click on the red one to show the mod details:

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/ab47db09-4777-43cb-bebc-29bbc1c8e235)

You can now scroll down to find changes between the snapshot and the current mod state:

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/43e8501f-1d93-4ee3-81fe-c0a8aa89ab13)

Alternatively, you can right click on either "Snapshot" or "Current" and select "Collapse unchanged sections" to just show the differences:

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/69681d90-7fd0-417a-a8d9-161f82e31feb)

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/27c200ee-0e56-428e-89f8-fff6862ee69d)

You now can see that Songs of Skyrim has had two items added, so this is what you need to do to update the CR patch.

_Potential Conflicts_

In addition to showing mods that have updated, Happy CRappy can also try to predict conflicts that need CR. The algorithm for this is as follows:

1. Look through all records that are touched by the CR patch(es)
2. For each of those records, look at all mods that touch these records
3. From the mods in (2), remove those which serve as masters for other mods in this list
4. For each of those mods, look through all _other_ records that are not already included in the CR patch
5. Notify user if the records aren't identical.

For example, let's say that Songs of Skyrim updates to give Lynly some songs as well:

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/6b60a540-2b8a-41e1-8f19-cb6e50e96dd8)

Lynly isn't in our current CR patch, but if you activate "Show potential conflicts", an entry for her will appear highlighted in purple. Note: "Show potential conflicts" does a lot of work in the background comparing all records in the overridden mods against their own overrides, so you will see the program hang for a few seconds as all of this is calculated.

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/81917cd4-6f73-40c8-8cea-733e4c6fd338)

Clicking on Lynly's entry, you will find pairwise comparisons of all non-mastered mods that contain her NPC record, as well as their differences:

![image](https://github.com/Synthesis-Collective/HappyCRappy/assets/63175798/cc978399-ebd6-44d5-89b6-4c084d03eae6)

**Future plans**

If enough people find this app useful, I will optimize a few things that currently need to be optimized. I think I have the "Show potential conflicts" algorithm doing more work in the background than it needs to and I could probably optimize it to run faster. Likewise, having to wait for the program to unfreeze while records are loaded from disk is always an annoying experience. I don't mind it too much myself, and there's only so much UI work I can do before I get bored, but if enough people like this app I'll circle back to it. 

**Acknowledgements**

Huge thanks to Noggog for making this app possible by providing both the Mutagen framework and more recently the serialization API (made for Spriggit but available for me to use in this program, without which I wouldn't have been able to write this).
