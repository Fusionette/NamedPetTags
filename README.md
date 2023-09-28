# Named Pet Tags

A Valheim mod to change the behavior of tamed creatures by putting tags on their names.

## Installation

- Extract `NamedPetTags.dll` file to your `plugins` folder.

## Features

- When naming a tamed creature, you can one or many of the the following tags:
- `<pet>` prevents the creature from taking damage entirely.
- `<bed>` doesn't prevent it to take damage, but teleports it to the player's bed if the damage would be fatal.
- `<spay>` prevents a creature from breeding or laying eggs.
- `<follow=n>` enables (1) or disables (0) the ability of the creature to follow the player when interacted with; doesn't work on Hens.
- `<h=n>` alters hue of the creature's material, if possible; doesn't work on Hens.
- `<s=n>` alters saturation of the creature's material, if possible; doesn't work on Hens.
- `<v=n>` alters value of the creature's material, if possible; doesn't work on Hens.
- Pet, Bed and Spayed will appear in the creature's hover text to indicate those tags are active.
- When both Pet and Bed tags are present, the creature will be sent to bed if it takes fatal damage in a single hit, for example from the Butcher's Knife.
- Works on Valheim 0.217.14 (Hildir's Request)

## Examples

- Name a boar `<bed><spay><follow=1>Mr. Tusks<s=100><h=340><v=25>` to color it a deep red, make it go to bed when it would take fatal damage, never breeed, and follow the player when interacted with.
- Name a wolf `<v=-15><s=50><h=200>Good Boy<follow=0><pet>` to color it dark blue, prevent it from taking any damage, and not follow the player when interacted with.

## Changelog

v0.9.5
- Tameable name length increased to 100 characters.
- Added spay, follow, bed, hue, saturation and value tags.

v0.9.0
- Initial working prototype.
