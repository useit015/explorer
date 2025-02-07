using UnityEngine;

namespace DCL.Configuration
{
    public static class BuilderInWorldSettings
    {
        public static string BASE_URL_CATALOG = "https://builder-api.decentraland.org/v1/storage/contents/";
        public static string BASE_URL_ASSETS_PACK = "https://builder-api.decentraland.org/v1/assetPacks";
        public static string BASE_URL_ASSETS_PACK_CONTENT = "https://builder-api.decentraland.org/v1/storage/assetPacks/";

        public static int SELECTION_LAYER = LayerMask.NameToLayer("Selection");
        public static int DEFAULT_LAYER = LayerMask.NameToLayer("Default");
        public static int COLLIDER_SELECTION_LAYER = LayerMask.NameToLayer("OnBuilderPointerClick");

        public const string VOXEL_ASSETS_PACK_ID = "b51e5e7c-c56b-4ad9-b9d2-1dc1c6546169";
        public const string FLOOR_CATEGORY = "ground";

        public const string CATALOG_ASSET_PACK_TITLE = "Asset Packs";
        public const string VOXEL_TAG = "Voxel";
        public const string CUSTOM_LAND = "CUSTOM LAND";

        public const int RETRY_AMOUNTS = 3;

        //Kernel Report
        public const string STATE_EVENT_NAME = "stateEvent";
        public const string SCENE_EVENT_NAME = "SceneEvent";
        public static float ENTITY_POSITION_REPORTING_DELAY = 0.1f; // In seconds

        //Floor Scene Object
        public const string FLOOR_ID = "da1fed3c954172146414a66adfa134f7a5e1cb49c902713481bf2fe94180c2cf";
        public const string FLOOR_MODEL = "FloorBaseGrass_01/FloorBaseGrass_01.glb";
        public const string FLOOR_NAME = "Floor";

        public const string FLOOR_GLTF_KEY = "FloorBaseGrass_01/FloorBaseGrass_01.glb";
        public const string FLOOR_GLTF_VALUE = "QmSyvWnb5nKCaGHw9oHLSkwywvS5NYpj6vgb8L121kWveS";

        public const string FLOOR_TEXTURE_KEY = "FloorBaseGrass_01/Floor_Grass01.png.png";
        public const string FLOOR_TEXTURE_VALUE = "QmT1WfQPMBVhgwyxV5SfcfWivZ6hqMCT74nxdKXwyZBiXb";

        //Collectables
        public const string ASSETS_COLLECTIBLES = "Collectibles";
        public const string COLLECTIBLE_MODEL_PROTOCOL = "ethereum://";

        //Gizmos
        public const string TRANSLATE_GIZMO_NAME = "MOVE";
        public const string ROTATE_GIZMO_NAME = "ROTATE";
        public const string SCLAE_GIZMO_NAME = "SCALE";

        public const float GIZMOS_RELATIVE_SCALE_RATIO = 0.06f;
    }

    public static class ApplicationSettings
    {
        public static string version = "1.0";
    }

    public static class EnvironmentSettings
    {
        public static bool RUNNING_TESTS = false;
        public static bool DEBUG = true;
        public static readonly Vector3 MORDOR = new Vector3(10000, 10000, 10000);
        public static readonly int MORDOR_SCALAR = 10000;
    }

    public static class InputSettings
    {
        public static KeyCode PrimaryButtonKeyCode = KeyCode.E;
        public static KeyCode SecondaryButtonKeyCode = KeyCode.F;
    }

    public static class PlayerSettings
    {
        public static float POSITION_REPORTING_DELAY = 0.1f; // In seconds
        public static float WORLD_REPOSITION_MINIMUM_DISTANCE = 100f;
    }

    public static class ParcelSettings
    {
        public static float DEBUG_FLOOR_HEIGHT = -0.1f;
        public static float PARCEL_SIZE = 16f;
        public static float PARCEL_BOUNDARIES_THRESHOLD = 0.01f;
        public static float UNLOAD_DISTANCE = PARCEL_SIZE * 12f;
        public static bool VISUAL_LOADING_ENABLED = true;
    }

    public static class TestSettings
    {
        public static int VISUAL_TESTS_APPROVED_AFFINITY = 95;
        public static float VISUAL_TESTS_PIXELS_CHECK_THRESHOLD = 5.0f;
        public static int VISUAL_TESTS_SNAPSHOT_WIDTH = 1280;
        public static int VISUAL_TESTS_SNAPSHOT_HEIGHT = 720;
    }

    public static class AssetManagerSettings
    {
        //When library item count gets above this threshold, unused items will get pruned on Get() method.
        public static int LIBRARY_CLEANUP_THRESHOLD = 10;
    }

    public static class MessageThrottlingSettings
    {
        public static float SIXTY_FPS_TIME = 1.0f / 60.0f;
        public static float GLOBAL_FRAME_THROTTLING_TIME = SIXTY_FPS_TIME / 8.0f;
        public static float LOAD_PARCEL_SCENES_THROTTLING_TIME = SIXTY_FPS_TIME / 4.0f;
    }

    public static class UISettings
    {
        public static float RESERVED_CANVAS_TOP_PERCENTAGE = 10f;
    }

    public static class NFTDataFetchingSettings
    {
        public static UnityEngine.Vector2
            NORMALIZED_DIMENSIONS =
                new UnityEngine.Vector2(512f, 512f); // The image dimensions that correspond to Vector3.One scale

        public static string DAR_API_URL = "https://schema.decentraland.org/dar";
    }

    public static class PhysicsLayers
    {
        public static int defaultLayer = LayerMask.NameToLayer("Default");
        public static int onPointerEventLayer = LayerMask.NameToLayer("OnPointerEvent");
        public static int characterLayer = LayerMask.NameToLayer("CharacterController");
        public static int characterOnlyLayer = LayerMask.NameToLayer("CharacterOnly");
        public static LayerMask physicsCastLayerMask = 1 << onPointerEventLayer;

        public static LayerMask physicsCastLayerMaskWithoutCharacter = (physicsCastLayerMask | (1 << defaultLayer))
                                                                       & ~(1 << characterLayer)
                                                                       & ~(1 << characterOnlyLayer);

        public static int friendsHUDPlayerMenu = LayerMask.NameToLayer("FriendsHUDPlayerMenu");
        public static int playerInfoCardMenu = LayerMask.NameToLayer("PlayerInfoCardMenu");
    }
}