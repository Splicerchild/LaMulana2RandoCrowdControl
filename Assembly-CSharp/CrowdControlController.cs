using System;
using System.Collections.Generic;
using CrowdControl.Common;
using CrowdControl.Games.Packs;
using ConnectorType = CrowdControl.Common.ConnectorType;

public class LM2Rando : SimpleTCPPack
{
    public override string Host { get; } = "0.0.0.0";

    public override ushort Port { get; } = 43384;

    public LM2Rando(IPlayer player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new Game(42, "La-Mulana 2 Randomizer", "LM2Rando", "PC", ConnectorType.SimpleTCPConnector);

    public override List<Effect> Effects => new List<Effect>
    {
        //General Effects
        // new Effect("1-Hit KO (30 seconds)", "ohko"),
        new Effect("Trip", "trip"),
        // new Effect("Give Health", "give_health",new[]{"amount100"}),
        // new Effect("Upgrade a Flamethrower to a LAMThrower (1 minute)", "lamthrower"),

        // //Toggle items
        // new Effect("Give Items","giveitems",ItemKind.Folder), //New folder for third batch
        // new Effect("Give a Medkit", "give_medkit", "giveitems"), //Moved into new folder for third batch
        
                
        // //Add/Remove Augs
        // new Effect("Add/Upgrade Augmentations","addaugs",ItemKind.Folder),
        // new Effect("Remove/Downgrade Augmentations","remaugs",ItemKind.Folder),

        // new Effect("Add/Upgrade Aqualung", "add_augaqualung", "addaugs"),

        // new Effect("Remove/Downgrade Aqualung", "rem_augaqualung", "remaugs"),

        
        // //Weapons
        // new Effect("Give Weapons","giveweapon",ItemKind.Folder),

        // new Effect("Give Flamethrower", "give_weaponflamethrower", "giveweapon"),
            
        // //Ammo
        // new Effect("Give Ammo","giveammo",ItemKind.Folder),

        // new Effect("Give 10mm Ammo (Pistols)", "give_ammo10mm",new[]{"amount100"},"giveammo"), //New for second Crowd Control batch
    };

    //Slider ranges need to be defined
    public override List<ItemType> ItemTypes => new List<ItemType>(new[]
    {
        new ItemType("Amount","amount100",ItemType.Subtype.Slider, "{\"min\":1,\"max\":100}")
    });
}