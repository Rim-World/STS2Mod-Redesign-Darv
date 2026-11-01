English | [中文](README.zh.md)

# If you like this mod, welcome to check out my other mods. They might be helpful to you.

## 1\. Design Goals

After the beta 0.111.0 update, Darv would occasionally offer three options that were almost worthless or overly punishing.

This mod has three design goals:

1. Reduce the chance of Darv offering three useless options at once;
2. Bring in more fun and powerful Boss relics from Slay the Spire 1;
3. Categorize Darv's relics: all Option 2 relics come with additional cost (Snecko Eye?); Option 3 keeps the vanilla 50% Dusty Tome; Option 1 contains all the remaining relics.

## 2\. Main Features

This mod redesigns the three options of Ancient Darv, and only affects the five vanilla characters.

**Option 1**: Runic Pyramid is checked first at a 20% chance; if it misses, the chance is split evenly among Astrolabe / Black Star / Empty Cage / Pandora's Box / Calling Bell.

**Option 2**: Runic Pyramid is checked first at a 20% chance (only if Option 1 did not roll the Pyramid); if it misses, the chance is split evenly among Sozu / Ectoplasm / Philosopher's Stone / Velvet Choker / Snecko Eye.

**Option 3**: Dusty Tome at 50% / character relic pool at 50% (evenly random within the pool).

Runic Pyramid never appears in both Option 1 and Option 2 at the same time, with a combined chance of about 36%.

The character relic pool is as follows:
- Ironclad: Runic Cube, Mark of Pain (25% each)
- Silent: Hovering Kite, Wrist Blade (25% each)
- Defect: Nuclear Battery, Inserter (25% each)
- Regent: Ancient Lunar Pastry, Ancient Mini Regent, Ancient Orange Dough (~16.7% each)
- Necrobinder: Ancient Bookmark, Ancient Big Hat, Ancient Ivory Tile (~16.7% each)

The six relics for the Ironclad, Silent and Defect all come from Slay the Spire 1. The six new Ancient relics for the Regent/Necrobinder are upgraded versions of their six Rare relics, with the following effects:

- Ancient Lunar Pastry: At the end of your turn, gain 2 Stars.
- Ancient Mini Regent: The first time you gain and spend Stars each turn, gain 1 Strength.
- Ancient Orange Dough: At the start of each combat, add 3 random upgraded Colorless cards to your hand.
- Ancient Bookmark: At the end of your turn, a random Retained card costs 1 less for the rest of this combat.
- Ancient Big Hat: At the start of each combat, add 3 random upgraded Ethereal cards to your hand.
- Ancient Ivory Tile: Whenever you play a card that actually costs 2 or more energy, gain 1 Energy.

## 3\. Other Features & Details

You can configure this mod on the RitsuLib settings page; settings can only be saved by clicking the "Save and Quit the Game" button at the very bottom.

1. You can toggle each character individually; when disabled, all three Darv options for that character return to the vanilla effects and probabilities.
2. You can swap the Ironclad's Mark of Pain with the Defect's Nuclear Battery. This option exists because the Ironclad's status-card system in STS1 was ported to the Defect in STS2.
3. You can set whether Snecko Eye appears in a character's Darv Option 2. This option exists because Snecko Eye clearly fits the Regent very poorly: his expensive cards are expensive in star cost rather than energy cost.

**Mutual-exclusion rule**: For the Regent and Necrobinder, once you own a certain Rare relic, its Ancient counterpart no longer appears in Darv's Option 3; conversely, once you obtain an Ancient relic, its Rare counterpart will no longer reappear from chests, shops, combat rewards, or events for the rest of the run.

If you own all three Rare relics, Darv's Option 3 becomes 100% Dusty Tome. (This scenario is almost impossible in practice.)

## 4\. Impact (Based on My Understanding of the Game)

Runic Pyramid's overall chance is raised to 36%, compared to 31.25% in the 0.107.x and about 25% in the 0.111.0;

Originally, Sozu and Ectoplasm could only appear in Act 2. They can now also appear in Act 3 , with their negative effects significantly reduced.

The six relics ported from STS1 mostly perform significantly better in STS2 than in STS1, and the six newly designed Ancient relics are extremely versatile, so it is no longer possible to be offered three useless options.

Darv may still not be the most powerful Ancient, but now he at least reaches the average level of the Ancients.

Save-file impact: past run records are unaffected; in new runs, once this mod is disabled, the relics it adds are no longer available and the run data is handled by the normal game mechanics.

## 5\. Language & Environment & Version

This Mod only supports Chinese（zhs、zht）and English（eng）,with other languages showing English。

This mod depends on the base mod RitsuLib (minimum version 0.5.14).

This mod supports the default 0.107.x branch and public beta 0.111.0 and above (interfaces with virtual-method signature differences between the two versions are adapted at runtime via optional patches + reflection, with identical effects).

In theory, as long as there are no major API updates or changes to game content, this mod should automatically adapt to new versions without any modification.

## 6\. Implementation & Compatibility

This mod is implemented on the RitsuLib patcher framework (Harmony patches): it replaces Darv's option generation, the enumeration and display of the collection subcategory, and the settings page and saving.

The art assets of the six STS1 relics are cropped to fit STS2; the six custom Ancient relics reuse official assets.

Known limitations: multiplayer compatibility has not been carefully tested; characters other than the five vanilla ones (mod characters and future new characters) automatically fall back to the vanilla Darv logic.

Compatibility is not guaranteed with pirated copies, modified versions, mobile versions, outdated versions, niche mods, or secondary mods.

## 7\. Credits

This mod was made with Deepseek Harness (WebUI) + DeepSeek V4 Flash (0731 GA). Thanks to Saint Liang.

Thanks to the Slay the Spire 2 team for their efforts to make modding easier for players.

Thanks to the BaseLib and RitsuLib developers for their open-source implementations and documentation.

## 8\. Open-Source Repository & Contact

Open-source repository: https://github.com/Rim-World/STS2Mod-Redesign-Darv

Feel free to leave your comments and feedback in the comment section.
