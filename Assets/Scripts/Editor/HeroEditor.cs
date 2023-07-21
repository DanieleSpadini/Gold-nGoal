using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Core;
using Character;
using static HeroEditor.GeneralSetting;
using Ability;

public class HeroEditor : EditorWindow
{
    Texture2D headerTexture;
    Texture2D speedTexture;
    Texture2D kickPowerTexture;
    Texture2D actionRadiusTexture;

    Texture2D heroButtonTexture;
    Texture2D heroPrefabTexture;

    Color headerSectionColor = new Color(13f / 255f, 32f / 255, 44f / 255f, 1f);
    Color speedSectionColor = new Color(242f /255f, 238f/255f, 105f/255f , 1f);
    Color kickPowerSectionColor = new Color(246f / 255f, 143f / 255, 70f / 255f, 1f);
    Color actionRadiusSectionColor = new Color(199f / 255f, 48f / 255, 128f / 255f, 1f);

    Color heroSectionColor = new Color(13f / 255f, 32f / 255, 44f / 255f, 1f);
    Color heroPrefabColor = new Color(106f /255f,90f/255f,205f/255f,0.6f);

    Rect headerSection;
    Rect speedSection;
    Rect kickPowerSection;
    Rect actionRadiusSection;

    Rect heroButtonSection;
    Rect heroPrefabSection;

    static ActionRadiusDefinition actionRadiusDefinition;
    static SpeedDefinition speedDefinition;
    static KickPowerDefinition kickPowerDefinition;

    public static ActionRadiusDefinition ActionRadiusDefinition { get { return actionRadiusDefinition; } }
    public static SpeedDefinition SpeedDefinition { get { return speedDefinition; } }
    public static KickPowerDefinition KickPowerDefinition { get { return kickPowerDefinition; } }

    [MenuItem("Window/HeroEditor")]
    protected static void ShowWindow()
    {
        HeroEditor window = (HeroEditor)GetWindow(typeof(HeroEditor));
        window.minSize = new Vector2(600, 300);
        window.Show();
    }
    private void OnEnable()
    {
        InitTextures();
        InitDefinitions();
    }
    public static void InitDefinitions()
    {
        speedDefinition =(SpeedDefinition)ScriptableObject.CreateInstance(typeof(SpeedDefinition));
        actionRadiusDefinition =(ActionRadiusDefinition)ScriptableObject.CreateInstance(typeof(ActionRadiusDefinition));
        kickPowerDefinition =(KickPowerDefinition)ScriptableObject.CreateInstance(typeof(KickPowerDefinition));
    }
    private void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        DrawAttributeKickPower();
        DrawAttributeActionRadius();
        DrawAttributeSpeed();
        DrawBoarButton();
        DrawDwarfButton();
        DrawElfButton();
        DrawHumanButton();
        DrawOrcButton();
        DrawGoblinButton();

        switch (heroTypes)
        {
            case GeneralSetting.HeroTypes.BOAR: DrawBoarPrefab(); break;
            case GeneralSetting.HeroTypes.DWARF: DrawDwarfPrefab(); break;
            case GeneralSetting.HeroTypes.ELF: DrawElfPrefab(); break;
            case GeneralSetting.HeroTypes.GOBLIN: DrawGoblinPrefab(); break;
            case GeneralSetting.HeroTypes.HUMAN: DrawHumanPrefab(); break;
            case GeneralSetting.HeroTypes.ORC: DrawOrcPrefab(); break;  
        }
    }
    void InitTextures()
    {
        headerTexture = new Texture2D(1, 1);
        headerTexture.SetPixel(0, 0, headerSectionColor);
        headerTexture.Apply();

        speedTexture = new Texture2D(1, 1);
        speedTexture.SetPixel(0, 0, speedSectionColor);
        speedTexture.Apply();

        kickPowerTexture = new Texture2D(1, 1);
        kickPowerTexture.SetPixel(0, 0, kickPowerSectionColor);
        kickPowerTexture.Apply();

        actionRadiusTexture = new Texture2D(1, 1);
        actionRadiusTexture.SetPixel(0, 0, actionRadiusSectionColor);
        actionRadiusTexture.Apply();

        heroButtonTexture = new Texture2D(1, 1);
        heroButtonTexture.SetPixel(0, 0, heroSectionColor);
        heroButtonTexture.Apply();

        heroPrefabTexture = new Texture2D(1, 1);
        heroPrefabTexture.SetPixel(0,0,heroPrefabColor);
        heroPrefabTexture.Apply();
    }

    void DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 50f;

        speedSection.x = 0;
        speedSection.y = 50;
        speedSection.width = Screen.width/3f;
        speedSection.height = 50 ;

        kickPowerSection.x = Screen.width/3f;
        kickPowerSection.y = 50;
        kickPowerSection.width = Screen.width / 3f;
        kickPowerSection.height = 50;

        actionRadiusSection.x = Screen.width/3f * 2;
        actionRadiusSection.y = 50;
        actionRadiusSection.width = Screen.width / 3f;
        actionRadiusSection.height = 50;

        heroButtonSection.x = 0;
        heroButtonSection.y = 80;
        heroButtonSection.width = Screen.width/5f;
        heroButtonSection.height = Screen.height;

        heroPrefabSection.x = Screen.width / 5f;
        heroPrefabSection.y = 100;
        heroPrefabSection.width = Screen.width- Screen.width / 5f;
        heroPrefabSection.height = Screen.height;

        GUI.DrawTexture(heroButtonSection, heroButtonTexture);
        GUI.DrawTexture(heroPrefabSection, heroPrefabTexture);

        GUI.DrawTexture(headerSection,headerTexture);
        GUI.DrawTexture(speedSection, speedTexture);
        GUI.DrawTexture(kickPowerSection, kickPowerTexture);
        GUI.DrawTexture(actionRadiusSection, actionRadiusTexture);
    }

    #region Attributes
    void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);

        GUILayout.Label("AttributeDesigner");

        GUILayout.EndArea();
    }
    void DrawAttributeSpeed()
    {
        GUILayout.BeginArea(speedSection);

        GUILayout.Label("SpeedSection");

        if (GUILayout.Button("Create!", GUILayout.Height(40)))
        {
            GeneralSetting.OpenWindow(GeneralSetting.SettingTypes.SPEED);
        }
        GUILayout.EndArea();
    }
    void DrawAttributeKickPower()
    {
        GUILayout.BeginArea(kickPowerSection);

        GUILayout.Label("KickPowerSction");

        if (GUILayout.Button("Create!", GUILayout.Height(40)))
        {
            GeneralSetting.OpenWindow(GeneralSetting.SettingTypes.KICKPOWER);
        }
        GUILayout.EndArea();
    }
    void DrawAttributeActionRadius()
    {
        GUILayout.BeginArea(actionRadiusSection);

        GUILayout.Label("ActionRadiusSection");

        if (GUILayout.Button("Create!", GUILayout.Height(40)))
        {
            GeneralSetting.OpenWindow(GeneralSetting.SettingTypes.ACTIONRADIUS);
        }

        GUILayout.EndArea();
    }
    #endregion

    #region HeroButtons
    void DrawBoarButton()
    {
        heroButtonSection.y = heroButtonSection.y + 40;
        GUILayout.BeginArea(heroButtonSection);

        GUILayout.Label("Boar");

        if (GUILayout.Button("Modify Prefab", GUILayout.Height(40)))
        {

            heroTypes = HeroTypes.BOAR;
        }
        heroButtonSection.y = heroButtonSection.y + 10;
        GUILayout.EndArea();
    }
    void DrawDwarfButton()
    {
        heroButtonSection.y = heroButtonSection.y + 50;
        GUILayout.BeginArea(heroButtonSection);

        GUILayout.Label("Dwarf");

        if (GUILayout.Button("Modify Prefab!", GUILayout.Height(40)))
        {
            heroTypes = HeroTypes.DWARF;
        }

        heroButtonSection.y = heroButtonSection.y + 10;
        GUILayout.EndArea();
    }
    void DrawElfButton()
    {
        heroButtonSection.y = heroButtonSection.y + 50;
        GUILayout.BeginArea(heroButtonSection);

        GUILayout.Label("Elf");

        if (GUILayout.Button("Modify Prefab!", GUILayout.Height(40)))
        {
            heroTypes = HeroTypes.ELF;
        }

        heroButtonSection.y = heroButtonSection.y + 10;
        GUILayout.EndArea();
    }
    void DrawGoblinButton()
    {
        heroButtonSection.y = heroButtonSection.y + 50;
        GUILayout.BeginArea(heroButtonSection);

        GUILayout.Label("Goblin");

        if (GUILayout.Button("Modify Prefab!", GUILayout.Height(40)))
        {
            heroTypes = HeroTypes.GOBLIN;
        }

        heroButtonSection.y = heroButtonSection.y + 10;
        GUILayout.EndArea();
    }
    void DrawHumanButton()
    {
        heroButtonSection.y = heroButtonSection.y + 50;
        GUILayout.BeginArea(heroButtonSection);

        GUILayout.Label("Human");

        if (GUILayout.Button("Modify Prefab!", GUILayout.Height(40)))
        {
            heroTypes = HeroTypes.HUMAN;
        }

        heroButtonSection.y = heroButtonSection.y + 10;
        GUILayout.EndArea();
    }
    void DrawOrcButton()
    {
        heroButtonSection.y = heroButtonSection.y + 50;
        GUILayout.BeginArea(heroButtonSection);

        GUILayout.Label("Orc");

        if (GUILayout.Button("Modify Prefab!", GUILayout.Height(40)))
        {
            heroTypes = HeroTypes.ORC;
        }

        heroButtonSection.y = heroButtonSection.y + 10;
        GUILayout.EndArea();
    }
    #endregion

    #region Prefabs
    void DrawBoarPrefab()
    {
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        Boar boar = (Boar)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Character/Boar.prefab",typeof(Boar));
        
        GUILayout.BeginArea(heroPrefabSection);
        
        GUILayout.Label("Boar",style);

        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
        GUILayout.Label("KickPower");
        boar.KickPowerDefinition = (KickPowerDefinition)EditorGUILayout.ObjectField(boar.KickPowerDefinition, typeof(KickPowerDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        boar.SpeedDefinition = (SpeedDefinition)EditorGUILayout.ObjectField(boar.SpeedDefinition, typeof(SpeedDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ActionRadius");
        boar.ActionRadiusDefinition = (ActionRadiusDefinition)EditorGUILayout.ObjectField(boar.ActionRadiusDefinition, typeof(ActionRadiusDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("AbilityDefinition");
        boar.ActiveAbilityDefinition = (ModifierDefinition)EditorGUILayout.ObjectField(boar.ActiveAbilityDefinition, typeof(ModifierDefinition), true);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("CooldownTimer");
        boar.CooldownTimeActiveAbility = (float)EditorGUILayout.FloatField(boar.CooldownTimeActiveAbility);
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }
    void DrawDwarfPrefab()
    {
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        Dwarf dwarf = (Dwarf)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Character/Dwarf.prefab",typeof(Dwarf));
        GUILayout.BeginArea(heroPrefabSection);

        GUILayout.Label("Dwarf",style);

        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
        GUILayout.Label("KickPower");
        dwarf.KickPowerDefinition = (KickPowerDefinition)EditorGUILayout.ObjectField(dwarf.KickPowerDefinition, typeof(KickPowerDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        dwarf.SpeedDefinition = (SpeedDefinition)EditorGUILayout.ObjectField(dwarf.SpeedDefinition, typeof(SpeedDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ActionRadius");
        dwarf.ActionRadiusDefinition = (ActionRadiusDefinition)EditorGUILayout.ObjectField(dwarf.ActionRadiusDefinition, typeof(ActionRadiusDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("AbilityDefinition");
        dwarf.ActiveAbilityDefinition = (ModifierDefinition)EditorGUILayout.ObjectField(dwarf.ActiveAbilityDefinition, typeof(ModifierDefinition), true);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("CooldownTimer");
        dwarf.CooldownTimeActiveAbility = (float)EditorGUILayout.FloatField(dwarf.CooldownTimeActiveAbility);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    void DrawElfPrefab()
    {
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        Elf elf = (Elf)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Character/Elf.prefab",typeof(Elf));

        GUILayout.BeginArea(heroPrefabSection);

        GUILayout.Label("Elf", style);


        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
        GUILayout.Label("KickPower");
        elf.KickPowerDefinition = (KickPowerDefinition)EditorGUILayout.ObjectField(elf.KickPowerDefinition, typeof(KickPowerDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        elf.SpeedDefinition = (SpeedDefinition)EditorGUILayout.ObjectField(elf.SpeedDefinition, typeof(SpeedDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ActionRadius");
        elf.ActionRadiusDefinition = (ActionRadiusDefinition)EditorGUILayout.ObjectField(elf.ActionRadiusDefinition, typeof(ActionRadiusDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("AbilityDefinition");
        elf.ActiveAbilityDefinition = (ModifierDefinition)EditorGUILayout.ObjectField(elf.ActiveAbilityDefinition, typeof(ModifierDefinition), true);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("CooldownTimer");
        elf.CooldownTimeActiveAbility = (float)EditorGUILayout.FloatField(elf.CooldownTimeActiveAbility);
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }
    void DrawGoblinPrefab()
    {
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        Goblin goblin = (Goblin)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Character/Goblin.prefab",typeof(Goblin));


        GUILayout.BeginArea(heroPrefabSection);

        GUILayout.Label("Goblin",style);

        GUILayout.Space(30);


        GUILayout.BeginHorizontal();
        GUILayout.Label("KickPower");
        goblin.KickPowerDefinition = (KickPowerDefinition)EditorGUILayout.ObjectField(goblin.KickPowerDefinition, typeof(KickPowerDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        goblin.SpeedDefinition = (SpeedDefinition)EditorGUILayout.ObjectField(goblin.SpeedDefinition, typeof(SpeedDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ActionRadius");
        goblin.ActionRadiusDefinition = (ActionRadiusDefinition)EditorGUILayout.ObjectField(goblin.ActionRadiusDefinition, typeof(ActionRadiusDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("AbilityDefinition");
        goblin.ActiveAbilityDefinition = (ModifierDefinition)EditorGUILayout.ObjectField(goblin.ActiveAbilityDefinition, typeof(ModifierDefinition), true);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("CooldownTimer");
        goblin.CooldownTimeActiveAbility = (float)EditorGUILayout.FloatField(goblin.CooldownTimeActiveAbility);
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }
    void DrawHumanPrefab()
    {
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        Human human = (Human)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Character/Human.prefab", typeof(Human));

        GUILayout.BeginArea(heroPrefabSection);
        GUILayout.Label("Human",style);

        GUILayout.Space(30);


        GUILayout.BeginHorizontal();
        GUILayout.Label("KickPower");
        human.KickPowerDefinition = (KickPowerDefinition)EditorGUILayout.ObjectField(human.KickPowerDefinition, typeof(KickPowerDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        human.SpeedDefinition = (SpeedDefinition)EditorGUILayout.ObjectField(human.SpeedDefinition, typeof(SpeedDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ActionRadius");
        human.ActionRadiusDefinition = (ActionRadiusDefinition)EditorGUILayout.ObjectField(human.ActionRadiusDefinition, typeof(ActionRadiusDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("AbilityDefinition");
        human.ActiveAbilityDefinition = (ModifierDefinition)EditorGUILayout.ObjectField(human.ActiveAbilityDefinition, typeof(ModifierDefinition), true);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("CooldownTimer");
        human.CooldownTimeActiveAbility = (float)EditorGUILayout.FloatField(human.CooldownTimeActiveAbility);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    void DrawOrcPrefab()
    {
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        Orc orc = (Orc)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Character/Orc.prefab", typeof(Orc));

        GUILayout.BeginArea(heroPrefabSection);

        GUILayout.Label("Orc",style);
        GUILayout.Space(30);


        GUILayout.BeginHorizontal();
        GUILayout.Label("KickPower");
        orc.KickPowerDefinition = (KickPowerDefinition)EditorGUILayout.ObjectField(orc.KickPowerDefinition, typeof(KickPowerDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed");
        orc.SpeedDefinition = (SpeedDefinition)EditorGUILayout.ObjectField(orc.SpeedDefinition, typeof(SpeedDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ActionRadius");
        orc.ActionRadiusDefinition = (ActionRadiusDefinition)EditorGUILayout.ObjectField(orc.ActionRadiusDefinition, typeof(ActionRadiusDefinition), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("AbilityDefinition");
        orc.ActiveAbilityDefinition = (ModifierDefinition)EditorGUILayout.ObjectField(orc.ActiveAbilityDefinition, typeof(ModifierDefinition), true);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("CooldownTimer");
        orc.CooldownTimeActiveAbility = (float)EditorGUILayout.FloatField(orc.CooldownTimeActiveAbility);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    #endregion
    public class GeneralSetting : EditorWindow
    {
        public enum SettingTypes
        {
            SPEED,
            ACTIONRADIUS,
            KICKPOWER
        }
        public enum HeroTypes
        {
            BOAR,
            DWARF,
            ELF,
            GOBLIN,
            HUMAN,
            ORC
        }
        public static HeroTypes heroTypes;
        static SettingTypes dataSetting;
        static GeneralSetting window;

        string newNameSpeed;
        string newNameActionRadius;
        string newNameKickPower;
        public static void OpenWindow(SettingTypes setting)
        {
            dataSetting = setting;
            window = (GeneralSetting)GetWindow(typeof(GeneralSetting));
            window.minSize = new Vector2(250, 200);
            window.Show();
        }
        private void OnGUI()
        {
            switch (dataSetting)
            {
                case SettingTypes.SPEED:
                    DrawSpeedSettings((SpeedDefinition)HeroEditor.SpeedDefinition);
                    break;
                case SettingTypes.KICKPOWER:
                    DrawKickPowerSettings((KickPowerDefinition)HeroEditor.KickPowerDefinition);
                    break;
                case SettingTypes.ACTIONRADIUS:
                    DrawActionRadiusSettings((ActionRadiusDefinition)HeroEditor.ActionRadiusDefinition);
                    break;
            }
            
        }
        void DrawSpeedSettings(SpeedDefinition speed)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Name of property");
            newNameSpeed = EditorGUILayout.TextField(newNameSpeed);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Speed");
            speed.Speed = EditorGUILayout.FloatField(speed.Speed);
            EditorGUILayout.EndHorizontal();

            if(speed.Speed == 0)
            {
                EditorGUILayout.HelpBox("You cant leave the speed to 0!!",MessageType.Warning);
            }
            else if(GUILayout.Button("Finish and Save", GUILayout.Height(20)))
            {
                SaveAttributeData();
                window.Close();
            }
            
        }
        void DrawActionRadiusSettings(ActionRadiusDefinition actionRadius)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Name of property");
            newNameActionRadius = EditorGUILayout.TextField(newNameActionRadius);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("ActionRadius");
            actionRadius.Radius = EditorGUILayout.FloatField(actionRadius.Radius);
            EditorGUILayout.EndHorizontal();

            if (actionRadius.Radius == 0)
            {
                EditorGUILayout.HelpBox("You cant leave the radius to 0!!", MessageType.Warning);
            }
            else if (GUILayout.Button("Finish and Save", GUILayout.Height(20)))
            {
                SaveAttributeData();
                window.Close();
            }
        }
        void DrawKickPowerSettings(KickPowerDefinition kickPowerDefinition)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Name of property");
            newNameKickPower = EditorGUILayout.TextField(newNameKickPower);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Power");
            kickPowerDefinition.Power = EditorGUILayout.FloatField(kickPowerDefinition.Power);
            EditorGUILayout.EndHorizontal();


            if (kickPowerDefinition.Power == 0)
            {
                EditorGUILayout.HelpBox("You cant leave the power to 0!!", MessageType.Warning);
            }
            else if (GUILayout.Button("Finish and Save", GUILayout.Height(20)))
            {
                SaveAttributeData();
                window.Close();
            }
        }

        void SaveAttributeData()
        {
            
            string speedDataPath = "Assets/ScriptableObjects/Attribute/Speed/";
            string actionRadiusDataPath = "Assets/ScriptableObjects/Attribute/ActionRadius/";
            string kickPowerDataPath = "Assets/ScriptableObjects/Attribute/KickPower/";

            switch (dataSetting)
            {
                case SettingTypes.SPEED:
                    speedDataPath += newNameSpeed + ".asset";

                    AssetDatabase.CreateAsset(HeroEditor.SpeedDefinition, speedDataPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;

                case SettingTypes.ACTIONRADIUS:
                    actionRadiusDataPath += newNameActionRadius + ".asset";

                    AssetDatabase.CreateAsset(HeroEditor.ActionRadiusDefinition, actionRadiusDataPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;

                case SettingTypes.KICKPOWER:
                    kickPowerDataPath += newNameKickPower + ".asset";

                    AssetDatabase.CreateAsset(HeroEditor.KickPowerDefinition, kickPowerDataPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
            }
        }
    }
}
