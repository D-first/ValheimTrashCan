using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using ValheimTrashCan.Utils;

namespace ValheimTrashCan
{
    [BepInPlugin("dfirst.ValheimTrashCan", "Valheim Trash Can", "1.0.1")]
    [BepInDependency(Main.ModGuid)]
    public class ValheimTrashCan : BaseUnityPlugin
    {
        private static readonly string TRASH_PIECE_NAME = "$piece_dfirst_trash";
        private static readonly string TRASH_PIECE_DESC = "$piece_dfirst_trash_description";
        private static readonly string TOKEN_LANGUAGE = "English";
        private readonly Harmony harmony = new Harmony("dfirst.ValheimTrashCan");

        private void Awake()
        {
            RegisterPrefabs();
            LocalizationManager.Instance.AddLocalization(new LocalizationConfig(TOKEN_LANGUAGE)
            {
                Translations =
                {
                    { TRASH_PIECE_NAME.Trim('$'), "Trash Can" },
                    { TRASH_PIECE_DESC.Trim('$'), "A trash can that connected to another dimension. No one knows where the garbage goes." }
                }
            });

            harmony.PatchAll();
        }

        private void RegisterPrefabs()
        {
            var assetBundle = AssetBundleHelper.GetAssetBundleFromResources("trash");
            var trashAsset = assetBundle.LoadAsset<GameObject>("Assets/Pieces/TrashCan/TrashCan.prefab");

            CustomPiece piece = new CustomPiece(trashAsset,
                new PieceConfig
                {
                    PieceTable = "Hammer",
                    Description = TRASH_PIECE_DESC,
                    Requirements = new[]
                   {
                       new RequirementConfig()
                       {
                           Item = "FineWood",
                           Amount = 10,
                           Recover = true
                       },
                       new RequirementConfig()
                       {
                           Item = "SurtlingCore",
                           Amount = 1,
                           Recover = true
                       },
                       new RequirementConfig()
                       {
                           Item = "GreydwarfEye",
                           Amount = 5,
                           Recover = true
                       }
                   }
                });
            piece.FixReference = true;
            PieceManager.Instance.AddPiece(piece);
            assetBundle.Unload(false);

        }

        [HarmonyPatch(typeof(Container), nameof(Container.Interact))]
        class ContainerInteractPatch
        {
            static void Prefix(string ___m_name, Inventory ___m_inventory)
            {
                if (___m_name != TRASH_PIECE_NAME)
                {
                    return;
                }

                ___m_inventory.RemoveAll();
            }
        }
    }
}
