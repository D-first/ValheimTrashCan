using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;
using ValheimTrashCan.Utils;

namespace ValheimTrashCan
{
    [BepInPlugin("dfirst.ValheimTrashCan", "Valheim Trash Can", "1.1.0")]
    [BepInDependency(Main.ModGuid)]
    public class ValheimTrashCan : BaseUnityPlugin
    {
        private static readonly string TRASH_PIECE_NAME = "$piece_dfirst_trash";
        private static readonly string TRASH_PIECE_DESC = "$piece_dfirst_trash_description";
        private readonly Harmony harmony = new Harmony("dfirst.ValheimTrashCan");

        private void Awake()
        {
            AddPiece();
            harmony.PatchAll();
        }

        private void AddPiece()
        {
            var assetBundle = AssetBundleHelper.GetAssetBundleFromResources("trashcan");
            var trashAsset = assetBundle.LoadAsset<GameObject>("Assets/Pieces/TrashCan/TrashCan.prefab");

            Texture2D mainTex = AssetUtils.LoadTexture("ValheimTrashCan/Assets/Textures/main_texture.png");
            Texture2D normalTex = AssetUtils.LoadTexture("ValheimTrashCan/Assets/Textures/normalmap_texture.png");
            Renderer trashcanRenderer = trashAsset.transform.Find("New/woodtrash").gameObject.GetComponent<Renderer>();
            if(mainTex != null)
            {
                trashcanRenderer.material.mainTexture = mainTex;
            }
            if(normalTex != null)
            {
                trashcanRenderer.material.SetTexture("_BumpMap", normalTex);
            }

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
