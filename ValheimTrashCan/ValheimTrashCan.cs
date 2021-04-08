using BepInEx;
using HarmonyLib;
using JotunnLib.Entities;
using JotunnLib.Managers;
using System;
using UnityEngine;
using ValheimTrashCan.Utils;
using ValheimLib;

namespace ValheimTrashCan
{
    [BepInPlugin("dfirst.ValheimTrashCan", "Valheim Trash Can", "1.0.1")]
    [BepInDependency(JotunnLib.JotunnLib.ModGuid)]
    public class ValheimTrashCan : BaseUnityPlugin
    {
        private static readonly string TRASH_PIECE_NAME = "$piece_dfirst_trash";
        private static readonly string TRASH_PIECE_DESC = "$piece_dfirst_trash_description";
        private static readonly string TOKEN_LANGUAGE = "English";
        private readonly Harmony harmony = new Harmony("dfirst.ValheimTrashCan");

        private void Awake()
        {
            PrefabManager.Instance.PrefabRegister += RegisterPrefabs;
            PieceManager.Instance.PieceRegister += RegisterPieces;

            Language.AddToken(TRASH_PIECE_NAME, "Trash Can", TOKEN_LANGUAGE);
            Language.AddToken(TRASH_PIECE_DESC, "A trash can that connected to another dimension. No one knows where the garbage goes.", TOKEN_LANGUAGE);

            harmony.PatchAll();
        }

        private void RegisterPrefabs(object sender, EventArgs e)
        {
            var assetBundle = AssetBundleHelper.GetAssetBundleFromResources("trash");
            var trashAsset = assetBundle.LoadAsset<GameObject>("Assets/CustomItems/Trash/Trash.prefab");

            Container container = trashAsset.GetComponent<Container>();
            container.m_name = TRASH_PIECE_NAME;

            Piece piece = trashAsset.GetComponent<Piece>();
            PieceConfig pieceConfig = new PieceConfig()
            {
                Name = TRASH_PIECE_NAME,
                Description = TRASH_PIECE_DESC,
                Requirements = new PieceRequirementConfig[]
                {
                    new PieceRequirementConfig()
                    {
                        Item = "Wood",
                        Amount = 10,
                        Recover = true
                    },
                    new PieceRequirementConfig()
                    {
                        Item = "FineWood",
                        Amount = 10,
                        Recover = true
                    },
                    new PieceRequirementConfig()
                    {
                        Item = "SurtlingCore",
                        Amount = 1,
                        Recover = true
                    },
                    new PieceRequirementConfig()
                    {
                        Item = "GreydwarfEye",
                        Amount = 5,
                        Recover = true
                    }
                }
            };

            piece.m_name = pieceConfig.Name;
            piece.m_description = pieceConfig.Description;
            piece.m_resources = pieceConfig.GetRequirements();

            AccessTools.Method(typeof(PrefabManager), "RegisterPrefab", new Type[] { typeof(GameObject), typeof(string) }).Invoke(PrefabManager.Instance, new object[] { trashAsset, "Trash" });
        }

        private void RegisterPieces(object sender, EventArgs e)
        {
            PieceManager.Instance.RegisterPiece("Hammer", "Trash");
        }

        [HarmonyPatch(typeof(Container), nameof(Container.Interact))]
        class DustBoxPatch
        {
            static void Prefix(ref string ___m_name, ref Inventory ___m_inventory)
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
