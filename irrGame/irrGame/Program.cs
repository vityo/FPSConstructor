using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using IrrlichtLime;
using IrrlichtLime.Core;
using IrrlichtLime.Video;
using IrrlichtLime.Scene;
using IrrlichtLime.GUI;
using IrrlichtLime.IO;

//using System.Xml;
using  System.Xml.Linq;

using IrrGame.IrrAi.Interface;
using IrrGame.IrrAi;
using IrrGame.IrrFPS;

namespace IrrGame
{
 //   public enum E_CHARACTER_TYPE
   // {
     //   ECT_CHASING,
      //  ECT_FLEEING,
      //  ECT_SPECTATING
    //}

    

    public class Program
    {
        static public bool mouseDown = false;
        static public bool closed = false;
        static public bool endGame = false;
        static public bool needStartGame = false;

        static public bool playerTurnForward = false;
        static public bool playerTurnBack = false;
        static public bool playerTurnLeft = false;
        static public bool playerTurnRight = false;
        static public bool playerJump = false;
        static public bool playerCrouch = false;

        static public float direction;
        static public float zdirection;
        static public Vector2Df cursorPosRelativeLast;
       // static public bool playerCrouch = false;
       // static public Line3Df ray1 = new Line3Df();    

//         public struct CameraParameters
//         {
//             public Vector3Df position;
//         }
        public struct Game
        {
           // public string InstallPath;
            public string Name;
            public bool FPS;
        }
        public struct Window
        {
            public int Width;
            public int Height;
            public byte BitsPerPixel;
        }

        public struct Interface
        {
            public string WinImagePath;
            public string LoseImagePath;
            public string FontDigitPath;
            public Color ColorHealth;
            public Color ColorProjectiles;
        }

        public struct Graphics
        {
            public byte Antialiasing;
            public DriverType DriverType;
            public bool FullScreen;
            public bool VSync;
        }

        public enum SkyType
        {
            Dome,
            Box
        }

        public class Sky
        {
            public SkyType Type;
        }
        
        public class SkyDome: Sky
        {
            public string TexturePath;
            public int HoriRes;
            public int VertRes;
            public float TexturePercentage;
            public float SpherePercentage;
        }

        public class SkyBox: Sky
        {
            public string TopTexturePath;
            public string BottomTexturePath;
            public string LeftTexturePath;
            public string RightTexturePath;
            public string FrontTexturePath;
            public string BackTexturePath;
        }

        public struct Map
        {
            public string MapPath;
            public int MinimalPolysPerNode;
            public string MapAIPath;
        }

        public struct AnimationSpeed
        {
            public float Death;
            public float Boom;
            public float CrouchAttack;
            public float Attack;
            public float Jump;
            public float CrouchWalk;
            public float Run;
            public float CrouchStand;
            public float Stand;
        }

        public struct Character
        { 
            public int MaxHealth;
            public int MaxAmmo;
            public uint TimeShotDelay;
            public uint TimeFallToDeath;
            public bool RegenerateHealth;
            public int RefillPeriodHealth;
            public bool RegenerateAmmo;
            public int RefillPeriodAmmo;
            public int DrawnHealth;
            public Vector3Df OffsetStand;
            public Vector3Df OffsetCrouch;
            public AnimationSpeed animationSpeed;
        }

        public struct Bots
        {
            public int NumBots;
            public uint TimeJump;
            public uint TimeCrouching;
            public float Range;
            public Vector3Df FieldOfViewDimensions;
            public float MoveSpeed;
            public float AtDestinationThreshold;
            public float JumpUpFactor;
            public float JumpAroundFactor;
            public string ModelPath;
            public string TexturePath;
        }
           
        public struct ShotNode
        {
            public Vector3Df Scale;
            public Vector3Df Position;
            public Vector3Df Rotation;
            public string ModelPath;
            public string TexturePath;
        }

        public struct Player
        {
            public ShotNode shotNode;

            public float MoveSpeed;
            public float RotateSpeed;
            public float JumpSpeed;
            public Vector3Df Position;
            public Vector3Df Target;
            public float FarValue;
            public Vector3Df Scale;
            public Vector3Df EllipsoidRadius;
            public Vector3Df EllipsoidTranslation;
            public Vector3Df GravityPerSecond;
        }



        public struct BillBoard
        {
            public float TextureTimePerFrame;
            public Dimension2Df Dimension;
            public List<string> TexturePaths;
        }

        public struct Projectile
        {
            public Vector3Df StartPositionBotStand;
            public Vector3Df StartPositionBotCrouch;
            public Vector3Df StartPositionPlayerStand;
            public Vector3Df StartPositionPlayerCrouch;
            public float Speed;
            public float MaxDistanceTravelled;
            public BillBoard LiveBillBoard;
            public BillBoard DieBillBoard;
        }
        
        public struct Parameters
        {
            //public IrrlichtCreationParameters irrCrPar;
            public Game game;
            public Window window;
            public Interface interfaces;
            public Graphics graphics;
            public Sky sky;
            public Map map;
            public Character character;
            public Bots bots;
            public Player player;

            public Projectile projectile;
        }

        static bool LoadParametersFromXML(string fileName, ref Parameters parameters)
        {
            //parameters.irrCrPar = new Irrlichtparameters();
            
            try
            {
            
                XDocument xDocument = XDocument.Load(fileName);

                //parameters.game.InstallPath = xDocument.Root.Element("Game").Attribute("InstallPath").Value;
                parameters.game.Name = xDocument.Root.Element("Game").Attribute("Name").Value;

                if (!bool.TryParse(xDocument.Root.Element("Game").Attribute("FPS").Value,
                    out parameters.game.FPS))
                    throw new Exception("Error in Parameters.xml: Game.FPS");

                if(!int.TryParse(xDocument.Root.Element("Window").Attribute("Width").Value, out parameters.window.Width))
                    throw new Exception("Error in Parameters.xml: Window.Width");

                if (parameters.window.Width <= 0)
                    throw new Exception("Error in Parameters.xml: Window.Width");


                if (!int.TryParse(xDocument.Root.Element("Window").Attribute("Height").Value, out parameters.window.Height))
                    throw new Exception("Error in Parameters.xml: Window.Height");

                if (parameters.window.Height <= 0)
                    throw new Exception("Error in Parameters.xml: Window.Height");

                parameters.interfaces.WinImagePath = xDocument.Root.Element("Interface").Attribute("WinImagePath").Value;

                if (!File.Exists(parameters.interfaces.WinImagePath))
                    throw new Exception("Error in Parameters.xml: Interface.WinImagePath");

                parameters.interfaces.LoseImagePath = xDocument.Root.Element("Interface").Attribute("LoseImagePath").Value;

                if (!File.Exists(parameters.interfaces.LoseImagePath))
                    throw new Exception("Error in Parameters.xml: Interface.LoseImagePath");

                parameters.interfaces.FontDigitPath = xDocument.Root.Element("Interface").Attribute("FontDigitPath").Value;

                if (!File.Exists(parameters.interfaces.FontDigitPath))
                    throw new Exception("Error in Parameters.xml: Interface.FontDigitPath");

                if (!Utility.getColourFrom(xDocument.Root.Element("Interface").Attribute("ColorHealth").Value,
                    ref parameters.interfaces.ColorHealth))
                    throw new Exception("Error in Parameters.xml: Interface.ColorHealth");

                if(!Utility.getColourFrom(xDocument.Root.Element("Interface").Attribute("ColorProjectiles").Value,
                    ref parameters.interfaces.ColorProjectiles))
                    throw new Exception("Error in Parameters.xml: Interface.ColorProjectiles");

                if (!byte.TryParse(xDocument.Root.Element("Graphics").Attribute("Antialiasing").Value,
                    out parameters.graphics.Antialiasing))
                    throw new Exception("Error in Parameters.xml: Graphics.Antialiasing");

                parameters.graphics.DriverType = xDocument.Root.Element("Graphics").Attribute("DriverType").Value == "DirectX" ?
                    DriverType.Direct3D9: DriverType.OpenGL;

                if (!bool.TryParse(xDocument.Root.Element("Graphics").Attribute("FullScreen").Value,
                    out parameters.graphics.FullScreen))
                    throw new Exception("Error in Parameters.xml: Graphics.FullScreen");

                if (!bool.TryParse(xDocument.Root.Element("Graphics").Attribute("VSync").Value,
                    out parameters.graphics.VSync))
                    throw new Exception("Error in Parameters.xml: Graphics.VSync");

               SkyType skyType = xDocument.Root.Element("Sky").Attribute("Type").Value == "Dome" ? SkyType.Dome : SkyType.Box;

                //parameters.sky.TexturePaths = new List<string>();

               if (skyType == SkyType.Dome)
                {
                    parameters.sky = new SkyDome();
                    
                    SkyDome skyDome = (SkyDome)parameters.sky;

                    skyDome.Type = SkyType.Dome;

                    if (!int.TryParse(xDocument.Root.Element("Sky").Attribute("HoriRes").Value,
                        out skyDome.HoriRes))
                        throw new Exception("Error in Parameters.xml: Sky.HoriRes");

                    if (skyDome.HoriRes < 3)
                        throw new Exception("Error in Parameters.xml: Sky.HoriRes");

                    if (!int.TryParse(xDocument.Root.Element("Sky").Attribute("VertRes").Value,
                        out skyDome.VertRes))
                        throw new Exception("Error in Parameters.xml: Sky.VertRes");

                    if (skyDome.VertRes < 2)
                        throw new Exception("Error in Parameters.xml: Sky.VertRes");

                    if (!float.TryParse(xDocument.Root.Element("Sky").Attribute("TexturePercentage").Value.Replace('.', ','),
                        out skyDome.TexturePercentage))
                        throw new Exception("Error in Parameters.xml: Sky.TexturePercentage");

                    if (skyDome.TexturePercentage < 0 || skyDome.TexturePercentage > 1)
                        throw new Exception("Error in Parameters.xml: Sky.TexturePercentage");

                    if (!float.TryParse(xDocument.Root.Element("Sky").Attribute("SpherePercentage").Value.Replace('.', ','),
                        out skyDome.SpherePercentage))
                        throw new Exception("Error in Parameters.xml: Sky.SpherePercentage");

                    if (skyDome.SpherePercentage < 0 || skyDome.SpherePercentage > 2)
                        throw new Exception("Error in Parameters.xml: Sky.SpherePercentage");

                    skyDome.TexturePath = xDocument.Root.Element("Sky").Attribute("TexturePath").Value;

                    if (!File.Exists(skyDome.TexturePath))
                        throw new Exception("Error in Parameters.xml: Sky.TexturePath");
                }
                else
                {
                    parameters.sky = new SkyBox();

                    SkyBox skyBox = (SkyBox)parameters.sky;

                    skyBox.Type = SkyType.Box;

                    skyBox.TopTexturePath = xDocument.Root.Element("Sky").Attribute("TopTexturePath").Value;

                    if (!File.Exists(skyBox.TopTexturePath))
                        throw new Exception("Error in Parameters.xml: Sky.TopTexturePath");

                    skyBox.BottomTexturePath = xDocument.Root.Element("Sky").Attribute("BottomTexturePath").Value;

                    if (!File.Exists(skyBox.BottomTexturePath))
                        throw new Exception("Error in Parameters.xml: Sky.BottomTexturePath");

                    skyBox.LeftTexturePath = xDocument.Root.Element("Sky").Attribute("LeftTexturePath").Value;

                    if (!File.Exists(skyBox.LeftTexturePath))
                        throw new Exception("Error in Parameters.xml: Sky.LeftTexturePath");

                    skyBox.RightTexturePath = xDocument.Root.Element("Sky").Attribute("RightTexturePath").Value;

                    if (!File.Exists(skyBox.RightTexturePath))
                        throw new Exception("Error in Parameters.xml: Sky.RightTexturePath");

                    skyBox.FrontTexturePath = xDocument.Root.Element("Sky").Attribute("FrontTexturePath").Value;

                    if (!File.Exists(skyBox.FrontTexturePath))
                        throw new Exception("Error in Parameters.xml: Sky.FrontTexturePath");

                    skyBox.BackTexturePath = xDocument.Root.Element("Sky").Attribute("BackTexturePath").Value;

                    if (!File.Exists(skyBox.BackTexturePath))
                        throw new Exception("Error in Parameters.xml: Sky.BackTexturePath");
                }

                parameters.map.MapPath = xDocument.Root.Element("Map").Attribute("MapPath").Value;

                if (!File.Exists(parameters.map.MapPath))
                    throw new Exception("Error in Parameters.xml: Map.MapPath");

                if (!int.TryParse(xDocument.Root.Element("Map").Attribute("MinimalPolysPerNode").Value,
                    out parameters.map.MinimalPolysPerNode))
                    throw new Exception("Error in Parameters.xml: Map.MinimalPolysPerNode");

                if (parameters.map.MinimalPolysPerNode < 0)
                    throw new Exception("Error in Parameters.xml: Map.MinimalPolysPerNode");

                parameters.map.MapAIPath = xDocument.Root.Element("Map").Attribute("MapAIPath").Value;

                if (!File.Exists(parameters.map.MapAIPath))
                    throw new Exception("Error in Parameters.xml: Map.MapAIPath");

                if (!int.TryParse(xDocument.Root.Element("Character").Attribute("MaxHealth").Value,
                        out parameters.character.MaxHealth))
                    throw new Exception("Error in Parameters.xml: Character.MaxHealth");

                if(parameters.character.MaxHealth<=0)
                    throw new Exception("Error in Parameters.xml: Character.MaxHealth");

                if (!int.TryParse(xDocument.Root.Element("Character").Attribute("MaxAmmo").Value,
                        out parameters.character.MaxAmmo))
                    throw new Exception("Error in Parameters.xml: Character.MaxAmmo");

                if (parameters.character.MaxAmmo <= 0)
                    throw new Exception("Error in Parameters.xml: Character.MaxAmmo");

                if (!uint.TryParse(xDocument.Root.Element("Character").Attribute("TimeShotDelay").Value,
                        out parameters.character.TimeShotDelay))
                    throw new Exception("Error in Parameters.xml: Character.TimeShotDelay");

                if (parameters.character.TimeShotDelay <= 0)
                    throw new Exception("Error in Parameters.xml: Character.TimeShotDelay");

                if (!uint.TryParse(xDocument.Root.Element("Character").Attribute("TimeFallToDeath").Value,
                    out parameters.character.TimeFallToDeath))
                    throw new Exception("Error in Parameters.xml: Character.TimeFallToDeath");

                if (parameters.character.TimeFallToDeath <= 0)
                    throw new Exception("Error in Parameters.xml: Character.TimeFallToDeath");

                if (!bool.TryParse(xDocument.Root.Element("Character").Attribute("RegenerateHealth").Value,
                    out parameters.character.RegenerateHealth))
                    throw new Exception("Error in Parameters.xml: Character.RegenerateHealth");

                if (!int.TryParse(xDocument.Root.Element("Character").Attribute("RefillPeriodHealth").Value,
                    out parameters.character.RefillPeriodHealth))
                    throw new Exception("Error in Parameters.xml: Character.RefillPeriodHealth");

                if (parameters.character.RefillPeriodHealth < 0)
                    throw new Exception("Error in Parameters.xml: Character.RefillPeriodHealth");

                if (!bool.TryParse(xDocument.Root.Element("Character").Attribute("RegenerateAmmo").Value,
                    out parameters.character.RegenerateAmmo))
                    throw new Exception("Error in Parameters.xml: Character.RegenerateAmmo");

                if (!int.TryParse(xDocument.Root.Element("Character").Attribute("RefillPeriodAmmo").Value,
                    out parameters.character.RefillPeriodAmmo))
                    throw new Exception("Error in Parameters.xml: Character.RefillPeriodAmmo");

                if (parameters.character.RefillPeriodAmmo < 0)
                    throw new Exception("Error in Parameters.xml: Character.RefillPeriodHealth");

                if (!int.TryParse(xDocument.Root.Element("Character").Attribute("DrawnHealth").Value,
                    out parameters.character.DrawnHealth))
                    throw new Exception("Error in Parameters.xml: Character.DrawnHealth");

                if (parameters.character.DrawnHealth <= 0)
                    throw new Exception("Error in Parameters.xml: Character.DrawnHealth");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Character").Attribute("OffsetStand").Value,
                    ref parameters.character.OffsetStand))
                    throw new Exception("Error in Parameters.xml: Character.OffsetStand");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Character").Attribute("OffsetCrouch").Value,
                    ref parameters.character.OffsetCrouch))
                    throw new Exception("Error in Parameters.xml: Character.OffsetCrouch");

                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Death").Value.Replace('.', ','), out parameters.character.animationSpeed.Death))
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Death");

                if (parameters.character.animationSpeed.Death < 0)
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Death");

                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Boom").Value.Replace('.', ','), out parameters.character.animationSpeed.Boom))
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Boom");

                if (parameters.character.animationSpeed.Boom < 0)
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Boom");

                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("CrouchAttack").Value.Replace('.', ','), out parameters.character.animationSpeed.CrouchAttack))
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.CrouchAttack");

                if (parameters.character.animationSpeed.CrouchAttack < 0)
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.CrouchAttack");

                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Attack").Value.Replace('.', ','), out parameters.character.animationSpeed.Attack))
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Attack");

                if (parameters.character.animationSpeed.Attack < 0)
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Attack");

                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Jump").Value.Replace('.', ','), out parameters.character.animationSpeed.Jump))
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Jump");

                if (parameters.character.animationSpeed.Jump < 0)
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Jump");

                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("CrouchWalk").Value.Replace('.', ','), out parameters.character.animationSpeed.CrouchWalk))
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.CrouchWalk");

                if (parameters.character.animationSpeed.CrouchWalk < 0)
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.CrouchWalk");

                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Run").Value.Replace('.', ','), out parameters.character.animationSpeed.Run))
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Run");

                if (parameters.character.animationSpeed.Run < 0)
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Run");
               
                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("CrouchStand").Value.Replace('.', ','), out parameters.character.animationSpeed.CrouchStand))
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.CrouchStand");

                if (parameters.character.animationSpeed.CrouchStand < 0)
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.CrouchStand");

                if (!float.TryParse(xDocument.Root.Element("Character").Element("AnimationSpeed").
                    Attribute("Stand").Value.Replace('.', ','), out parameters.character.animationSpeed.Stand))
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Stand");

                if (parameters.character.animationSpeed.Stand < 0)
                    throw new Exception("Error in Parameters.xml: Character.AnimationSpeed.Stand");

                if (!int.TryParse(xDocument.Root.Element("Bots").Attribute("NumBots").Value, out parameters.bots.NumBots))
                    throw new Exception("Error in Parameters.xml: Bots.NumBots");

                if (parameters.bots.NumBots <= 0)
                    throw new Exception("Error in Parameters.xml: Bots.NumBots");

                if (!uint.TryParse(xDocument.Root.Element("Bots").Attribute("TimeJump").Value,
                        out parameters.bots.TimeJump))
                    throw new Exception("Error in Parameters.xml: Bots.TimeJump");

                if (parameters.bots.TimeJump <= 0)
                    throw new Exception("Error in Parameters.xml: Bots.TimeJump");

                if (!uint.TryParse(xDocument.Root.Element("Bots").Attribute("TimeCrouching").Value,
                    out parameters.bots.TimeCrouching))
                    throw new Exception("Error in Parameters.xml: Bots.TimeCrouching");

                if (parameters.bots.TimeCrouching <= 0)
                    throw new Exception("Error in Parameters.xml: Bots.TimeCrouching");

                if (!float.TryParse(xDocument.Root.Element("Bots").Attribute("Range").Value.Replace('.', ','),
                    out parameters.bots.Range))
                    throw new Exception("Error in Parameters.xml: Bots.Range");

                if (parameters.bots.Range < 0)
                    throw new Exception("Error in Parameters.xml: Bots.Range");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Bots").Attribute("FieldOfViewDimensions").Value,
                    ref parameters.bots.FieldOfViewDimensions))
                    throw new Exception("Error in Parameters.xml: Bots.FieldOfViewDimensions");

                if (!float.TryParse(xDocument.Root.Element("Bots").Attribute("MoveSpeed").Value.Replace('.', ','),
                    out parameters.bots.MoveSpeed))
                    throw new Exception("Error in Parameters.xml: Bots.MoveSpeed");

                if (parameters.bots.MoveSpeed < 0)
                    throw new Exception("Error in Parameters.xml: Bots.MoveSpeed");

                if (!float.TryParse(xDocument.Root.Element("Bots").Attribute("AtDestinationThreshold").Value.Replace('.', ','),
                    out parameters.bots.AtDestinationThreshold))
                    throw new Exception("Error in Parameters.xml: Bots.AtDestinationThreshold");

                if (parameters.bots.AtDestinationThreshold < 0)
                    throw new Exception("Error in Parameters.xml: Bots.AtDestinationThreshold");

                if (!float.TryParse(xDocument.Root.Element("Bots").Attribute("JumpUpFactor").Value.Replace('.', ','),
                    out parameters.bots.JumpUpFactor))
                    throw new Exception("Error in Parameters.xml: Bots.JumpUpFactor");

                if (parameters.bots.JumpUpFactor < 0)
                    throw new Exception("Error in Parameters.xml: Bots.JumpUpFactor");

                if (!float.TryParse(xDocument.Root.Element("Bots").Attribute("JumpAroundFactor").Value.Replace('.', ','),
                    out parameters.bots.JumpAroundFactor))
                    throw new Exception("Error in Parameters.xml: Bots.JumpAroundFactor");

                if (parameters.bots.JumpAroundFactor < 0)
                    throw new Exception("Error in Parameters.xml: Bots.JumpAroundFactor");

                parameters.bots.ModelPath = xDocument.Root.Element("Bots").Attribute("ModelPath").Value;

                if (!File.Exists(parameters.bots.ModelPath))
                    throw new Exception("Error in Parameters.xml: Bots.ModelPath");

                parameters.bots.TexturePath = xDocument.Root.Element("Bots").Attribute("TexturePath").Value;

                if (!File.Exists(parameters.bots.TexturePath))
                    throw new Exception("Error in Parameters.xml: Bots.TexturePath");

                if (!float.TryParse(xDocument.Root.Element("Player").Attribute("MoveSpeed").Value.Replace('.', ','),
                    out parameters.player.MoveSpeed))
                    throw new Exception("Error in Parameters.xml: Player.MoveSpeed");

                if (parameters.player.MoveSpeed < 0)
                    throw new Exception("Error in Parameters.xml: Player.MoveSpeed");

                if (!float.TryParse(xDocument.Root.Element("Player").Attribute("RotateSpeed").Value.Replace('.', ','),
                    out parameters.player.RotateSpeed))
                    throw new Exception("Error in Parameters.xml: Player.RotateSpeed");

                if (parameters.player.RotateSpeed <= 0)
                    throw new Exception("Error in Parameters.xml: Player.RotateSpeed");

                if (!float.TryParse(xDocument.Root.Element("Player").Attribute("JumpSpeed").Value.Replace('.', ','),
                    out parameters.player.JumpSpeed))
                    throw new Exception("Error in Parameters.xml: Player.JumpSpeed");

                if (parameters.player.JumpSpeed < 0)
                    throw new Exception("Error in Parameters.xml: Player.JumpSpeed");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("Position").Value,
                    ref parameters.player.Position))
                    throw new Exception("Error in Parameters.xml: Player.Position");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("Target").Value,
                    ref parameters.player.Target))
                    throw new Exception("Error in Parameters.xml: Player.Target");

                if (!float.TryParse(xDocument.Root.Element("Player").Attribute("FarValue").Value.Replace('.', ','),
                        out parameters.player.FarValue))
                    throw new Exception("Error in Parameters.xml: Player.FarValue");

                if (parameters.player.FarValue <= 0)
                    throw new Exception("Error in Parameters.xml: Player.FarValue");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("Scale").Value,
                    ref parameters.player.Scale))
                    throw new Exception("Error in Parameters.xml: Player.Scale");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("EllipsoidRadius").Value,
                    ref parameters.player.EllipsoidRadius))
                    throw new Exception("Error in Parameters.xml: Player.EllipsoidRadius");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("EllipsoidTranslation").Value,
                    ref parameters.player.EllipsoidTranslation))
                    throw new Exception("Error in Parameters.xml: Player.EllipsoidTranslation");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Player").Attribute("GravityPerSecond").Value,
                    ref parameters.player.GravityPerSecond))
                    throw new Exception("Error in Parameters.xml: Player.GravityPerSecond");



                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Player").Element("ShotNode").Attribute("Scale").Value,
                ref parameters.player.shotNode.Scale))
                    throw new Exception("Error in Parameters.xml: Player.ShotNode.Scale");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Player").Element("ShotNode").Attribute("Position").Value,
                    ref parameters.player.shotNode.Position))
                    throw new Exception("Error in Parameters.xml: Player.ShotNode.Position");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Player").Element("ShotNode").Attribute("Rotation").Value,
                    ref parameters.player.shotNode.Rotation))
                    throw new Exception("Error in Parameters.xml: Player.ShotNode.Rotation");

                parameters.player.shotNode.ModelPath = xDocument.Root.Element("Player").Element("ShotNode").Attribute("ModelPath").Value;

                if (!File.Exists(parameters.player.shotNode.ModelPath))
                    throw new Exception("Error in Parameters.xml: Player.ShotNode.ModelPath");

                parameters.player.shotNode.TexturePath = xDocument.Root.Element("Player").Element("ShotNode").Attribute("TexturePath").Value;

                if (!File.Exists(parameters.player.shotNode.TexturePath))
                    throw new Exception("Error in Parameters.xml: Player.ShotNode.TexturePath");
//                 }
//                 else
//                 {
//                     parameters.player = new PlayerTPS(player);
//                     PlayerTPS playerTPS = (PlayerTPS)parameters.player;
// 
//                     playerTPS.MeshPath = xDocument.Root.Element("Player").Attribute("MeshPath").Value;
// 
//                     if (!File.Exists(playerTPS.MeshPath))
//                         throw new Exception("Error in Parameters.xml: Player.MeshPath");
// 
//                     playerTPS.TexturePath = xDocument.Root.Element("Player").Attribute("TexturePath").Value;
// 
//                     if (!File.Exists(playerTPS.TexturePath))
//                         throw new Exception("Error in Parameters.xml: Player.TexturePath");
//                 }


                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Projectile").Attribute("StartPositionBotStand").Value,
                    ref parameters.projectile.StartPositionBotStand))
                    throw new Exception("Error in Parameters.xml: Projectile.StartPositionBotStand");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Projectile").Attribute("StartPositionBotCrouch").Value,
                    ref parameters.projectile.StartPositionBotCrouch))
                    throw new Exception("Error in Parameters.xml: Projectile.StartPositionBotCrouch");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Projectile").Attribute("StartPositionPlayerStand").Value,
                    ref parameters.projectile.StartPositionPlayerStand))
                    throw new Exception("Error in Parameters.xml: Projectile.StartPositionPlayerStand");

                if (!Utility.getVector3dfFrom(xDocument.Root.Element("Projectile").Attribute("StartPositionPlayerCrouch").Value,
                    ref parameters.projectile.StartPositionPlayerCrouch))
                    throw new Exception("Error in Parameters.xml: Projectile.StartPositionPlayerCrouch");

                if (!float.TryParse(xDocument.Root.Element("Projectile").Attribute("Speed").Value.Replace('.', ','),
                        out parameters.projectile.Speed))
                    throw new Exception("Error in Parameters.xml: Projectile.Speed");

                if (parameters.projectile.Speed <= 0)
                    throw new Exception("Error in Parameters.xml: Projectile.Speed");

                if (!float.TryParse(xDocument.Root.Element("Projectile").Attribute("MaxDistanceTravelled").Value.Replace('.', ','),
                        out parameters.projectile.MaxDistanceTravelled))
                    throw new Exception("Error in Parameters.xml: Projectile.MaxDistanceTravelled");

                if (parameters.projectile.MaxDistanceTravelled <= 0)
                    throw new Exception("Error in Parameters.xml: Projectile.MaxDistanceTravelled");


                if (!float.TryParse(xDocument.Root.Element("Projectile").Element("LiveBillBoard").
                    Attribute("TextureTimePerFrame").Value.Replace('.', ','), 
                    out parameters.projectile.LiveBillBoard.TextureTimePerFrame))
                    throw new Exception("Error in Parameters.xml: Projectile.LiveBillBoard.TextureTimePerFrame");

                if(parameters.projectile.LiveBillBoard.TextureTimePerFrame<=0)
                    throw new Exception("Error in Parameters.xml: Projectile.LiveBillBoard.TextureTimePerFrame");

                if (!Utility.getDimension2DfFrom(xDocument.Root.Element("Projectile").Element("LiveBillBoard").
                    Attribute("Dimension").Value,ref parameters.projectile.LiveBillBoard.Dimension))
                    throw new Exception("Error in Parameters.xml: Projectile.LiveBillBoard.Dimension");

                if (parameters.projectile.LiveBillBoard.Dimension.Width <= 0 ||
                    parameters.projectile.LiveBillBoard.Dimension.Height <= 0)
                    throw new Exception("Error in Parameters.xml: Projectile.LiveBillBoard.Dimension");

                parameters.projectile.LiveBillBoard.TexturePaths = new List<string>();

                foreach (XElement elem in xDocument.Root.Element("Projectile").Element("LiveBillBoard").Elements())
                {
                    parameters.projectile.LiveBillBoard.TexturePaths.Add(elem.Attribute("Path").Value);

                    if (!File.Exists(parameters.projectile.LiveBillBoard.
                        TexturePaths[parameters.projectile.LiveBillBoard.TexturePaths.Count-1]))
                        throw new Exception("Error in Parameters.xml: Projectile.LiveBillBoard.Texture.Path");
                }

                if (!float.TryParse(xDocument.Root.Element("Projectile").Element("DieBillBoard").
                    Attribute("TextureTimePerFrame").Value.Replace('.', ','),
                    out parameters.projectile.DieBillBoard.TextureTimePerFrame))
                    throw new Exception("Error in Parameters.xml: Projectile.DieBillBoard.TextureTimePerFrame");

                if (parameters.projectile.DieBillBoard.TextureTimePerFrame <= 0)
                    throw new Exception("Error in Parameters.xml: Projectile.DieBillBoard.TextureTimePerFrame");

                if (!Utility.getDimension2DfFrom(xDocument.Root.Element("Projectile").Element("DieBillBoard").
                    Attribute("Dimension").Value, ref parameters.projectile.DieBillBoard.Dimension))
                    throw new Exception("Error in Parameters.xml: Projectile.DieBillBoard.Dimension");

                if (parameters.projectile.DieBillBoard.Dimension.Width <= 0 ||
                    parameters.projectile.DieBillBoard.Dimension.Height <= 0)
                    throw new Exception("Error in Parameters.xml: Projectile.DieBillBoard.Dimension");

                parameters.projectile.DieBillBoard.TexturePaths = new List<string>();

                foreach (XElement elem in xDocument.Root.Element("Projectile").Element("DieBillBoard").Elements())
                {
                    parameters.projectile.DieBillBoard.TexturePaths.Add(elem.Attribute("Path").Value);

                    if (!File.Exists(parameters.projectile.DieBillBoard.
                        TexturePaths[parameters.projectile.DieBillBoard.TexturePaths.Count - 1]))
                        throw new Exception("Error in Parameters.xml: Projectile.DieBillBoard.Texture.Path");
                }



//                 parameters.projectile.StartPositionBotStand r       public struct Projectile
//         {
//             public Vector3Df StartPositionBotStand;
//             public Vector3Df StartPositionBotCrouch;
//             public float Speed;
//             public float MaxDistanceTravelled;
//         }

                //прогаешь, да прогаешь, не планируешь заранее как выйдет, иначе тормозит процесс


    // 
    // 
    // 
    // 
    // 
    // 
    //             public Game game;
    //             public Sky sky;
    //             public Map map;
    // 
    //             parameters.irrCrPar.AntiAliasing = byte.Parse(xDocument.Root.Element("creation").Attribute("antialiasing").Value);
    //             int driverType = int.Parse(xDocument.Root.Element("creation").Attribute("drivertype").Value);
    //             parameters.irrCrPar.DriverType = (DriverType)driverType;
    //             parameters.irrCrPar.WindowSize = new Dimension2Di(int.Parse(xDocument.Root.Element("creation").Attribute("width").Value),
    //                 int.Parse(xDocument.Root.Element("creation").Attribute("height").Value));
    //             parameters.irrCrPar.BitsPerPixel = byte.Parse(xDocument.Root.Element("creation").Attribute("color").Value);
    //             parameters.irrCrPar.Fullscreen = bool.Parse(xDocument.Root.Element("creation").Attribute("fullscreen").Value);
    //             parameters.irrCrPar.VSync = bool.Parse(xDocument.Root.Element("creation").Attribute("vsync").Value);
    // 
    //             parameters.camera.position = new Vector3Df(float.Parse(xDocument.Root.Element("camera").Element("position").Attribute("x").Value),
    //                 float.Parse(xDocument.Root.Element("camera").Element("position").Attribute("y").Value), float.Parse(xDocument.Root.Element("camera").Element("position").Attribute("z").Value));
    // 
    //             parameters.mapFileName = xDocument.Root.Element("mapfilename").Attribute("filename").Value;
    // 
    //             parameters.skyType = xDocument.Root.Element("sky").Attribute("type").Value=="box"?
    //                 Parameters.SkyType.Box:Parameters.SkyType.Dome;
    // 
    //             if (parameters.skyType == Parameters.SkyType.Box)
    //             {
    //                 parameters.skyFileNames = new string[6];
    //                 parameters.skyFileNames[0] = xDocument.Root.Element("sky").Attribute("top").Value;
    //                 parameters.skyFileNames[1] = xDocument.Root.Element("sky").Attribute("bottom").Value;
    //                 parameters.skyFileNames[2] = xDocument.Root.Element("sky").Attribute("left").Value;
    //                 parameters.skyFileNames[3] = xDocument.Root.Element("sky").Attribute("right").Value;
    //                 parameters.skyFileNames[4] = xDocument.Root.Element("sky").Attribute("front").Value;
    //                 parameters.skyFileNames[5] = xDocument.Root.Element("sky").Attribute("back").Value;
    //             }
    //             else
    //             {
    //                 parameters.skyFileNames = new string[1];
    //                 parameters.skyFileNames[0] = xDocument.Root.Element("sky").Attribute("around").Value;
    //             }

                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
/*class MyEventReceiver : public EventReceiver {
      
	public:
		virtual bool OnEvent(const SEvent& event) {

			if (event.EventType == EET_KEY_INPUT_EVENT && !event.KeyInput.PressedDown) {
				switch(event.KeyInput.Key) {
					case KEY_ESCAPE: {
						// Quit the game
						quitGame = true;
						return true;     
					}
					case KEY_KEY_V: {
						aiDebug = !aiDebug;
						if (aimgr) aimgr->setDebugVisible(aiDebug);
						if (aiDebugText) aiDebugText->setText(aiDebug?L"ON":L"OFF");
						return true;
					} 
					case KEY_KEY_P: {
						// Pause the scene
						paused = !paused;
						if (pausedText) pausedText->setVisible(paused);
						if (paused) { // Freezes animators and other timer dependent things
							if (device) device->getTimer()->stop();
							if (smgr && player && player->getCharacterType() != ECT_SPECTATING) smgr->getActiveCamera()->setInputReceiverEnabled(false);
						} else {// Unfreezes animators and other timer dependent things
							if (device) device->getTimer()->start();
							if (smgr) smgr->getActiveCamera()->setInputReceiverEnabled(true);
						}
						return true;
					} 
					case KEY_KEY_O: {
						occlusion = !occlusion;
						if (aimgr) aimgr->setOcclusionQueryCallback(occlusion?&occlusionQuery:NULL);
						if (occlusionText) occlusionText->setText(occlusion?L"ON":L"OFF");
						return true;     
					}
				}
			} else if (event.EventType == EET_MOUSE_INPUT_EVENT) {
				switch (event.MouseInput.Event) {
					case EMIE_LMOUSE_PRESSED_DOWN:
						mouseDown = true;
						break;  
					case EMIE_LMOUSE_LEFT_UP:
						mouseDown = false;
						break;     
				}     
			} else if (event.EventType == EET_GUI_EVENT) {
				if (event.GUIEvent.EventType == gui::EGET_MESSAGEBOX_OK) {
					messageClosed = true;
				}
			}  
          
			return false;
      
		}
	
}*/

        static bool deviceOnEvent(Event evnt)
        {
            if (evnt.Type == EventType.Mouse &&
                evnt.Mouse.Type == MouseEventType.LeftDown)
            {
                mouseDown = true;
            }
            
            if (evnt.Type == EventType.Mouse &&
                evnt.Mouse.Type == MouseEventType.LeftUp)
            {
                mouseDown = false;
            }
 
             if (evnt.Type == EventType.Key &&
                 !evnt.Key.PressedDown && evnt.Key.Key == KeyCode.Esc )
             {
                 closed = true;
                 // closed == true ? false : true; 
             }

             if (evnt.Type == EventType.Key &&
                !evnt.Key.PressedDown && evnt.Key.Key == KeyCode.Return)
             {
                needStartGame = true;
             }

             if (evnt.Type == EventType.Key && evnt.Key.Key == KeyCode.KeyW)
                 playerTurnForward = evnt.Key.PressedDown;
             
            if (evnt.Type == EventType.Key && evnt.Key.Key == KeyCode.KeyS)
                playerTurnBack = evnt.Key.PressedDown;
             
             if (evnt.Type == EventType.Key &&evnt.Key.Key == KeyCode.KeyA)
                 playerTurnLeft = evnt.Key.PressedDown;


             if (evnt.Type == EventType.Key && evnt.Key.Key == KeyCode.KeyD)
                 playerTurnRight = evnt.Key.PressedDown;


             if (evnt.Type == EventType.Key && evnt.Key.Key == KeyCode.Space)
                 playerJump = evnt.Key.PressedDown;

             if (evnt.Type == EventType.Key && evnt.Key.Key == KeyCode.LShift)
                 playerCrouch = evnt.Key.PressedDown;

// 
//              if (evnt.Type == EventType.Key &&
//                 evnt.Key.PressedDown && evnt.Key.Key == KeyCode.LShift)
//              {
//                  //                  if (endGame)
//                  //                  {
//                 playerCrouch = true;
//                  //                 }
//              }
// 
//              if (evnt.Type == EventType.Key &&
//                  !evnt.Key.PressedDown && evnt.Key.Key == KeyCode.LShift)
//              {
//                  //                  if (endGame)
//                  //                  {
//                  playerCrouch = false;
//                  //                 }
//              }


            return false;

        }

        static private bool StartNewGame(ref VideoDriver driver, ref Parameters parameters, ref IrrlichtDevice device, ref SceneManager smgr, ref CAIManager aimgr,
            ref MetaTriangleSelector metaTriangleSelector, ref SWaypointGroup waypointGroup, ref List<CBotCharacter> lBots, ref CPlayerCharacter player)
        {

            try
            {
            
            //IrrAi

                aimgr.resetAI();
                waypointGroup = aimgr.getWaypointGroupFromIndex(0);


                lBots = new List<CBotCharacter>();









                /*  for (int i = 0; i < 1; ++i)
                  {
                      desc.sceneManager = smgr;
                      desc.AIManager = aimgr;
                      desc.StartWaypointID = -1;
                      desc.WaypointGroupName = waypointGroup.getName();
                      chasingCharacter = new CChasingCharacter(desc, "Media/init2win.md2", "Media/init2winred.jpg", metaTriangleSelector);

                      if (chasingCharacter == null)
                          Console.WriteLine("Failed character1 creation");

                      //characters.Add(character);
                  }

                  for (int i = 0; i < 1; ++i)
                  {
                      desc.sceneManager = smgr;
                      desc.AIManager = aimgr;
                      desc.StartWaypointID = -1;
                      desc.WaypointGroupName = waypointGroup.getName();
                      fleeingCharacter = new CFleeingCharacter(desc, "Media/init2win.md2", "Media/init2winblue.jpg");

                      if (fleeingCharacter == null) 
                          Console.WriteLine("Failed character2 creation");
                
                      //characters.Add(character);
                  }
           


                  //opposite teams

                  //275

                  SEntityGroup chaseTeam = aimgr.createEntityGroup();
                  SEntityGroup fleeTeam = aimgr.createEntityGroup();

      //             for (int i = 0; i < characters.Count; ++i)
      //             {
      //                 if (characters[i].getCharacterType() == E_CHARACTER_TYPE.ECT_CHASING)
      //                 {
                  chaseTeam.addEntity(chasingCharacter.getAIEntity());
                          chasingCharacter.getAIEntity().setEnemyGroup(fleeTeam);
                          chasingCharacter.getAIEntity().setAllyGroup(chaseTeam);
      //                 }
      //                 else
      //                 {
                          fleeTeam.addEntity(fleeingCharacter.getAIEntity());
                          fleeingCharacter.getAIEntity().setEnemyGroup(chaseTeam);
                          fleeingCharacter.getAIEntity().setAllyGroup(fleeTeam);
      //                 }
      //             }


                        */




                CameraSceneNode camera = smgr.AddCameraSceneNode(null, parameters.player.Position,
                    parameters.player.Target , -1, true);
                camera.TargetAndRotationBinding = true;
                // add camera
//                                  KeyMap keyMap = new KeyMap();
//                                  //keyMap.Add(KeyAction.Crouch, KeyCode.KeyX);
//                                  keyMap.Add(KeyAction.StrafeLeft, KeyCode.KeyA);
//                                  keyMap.Add(KeyAction.StrafeRight, KeyCode.KeyD);
//                                  keyMap.Add(KeyAction.MoveForward, KeyCode.KeyW);
//                                  keyMap.Add(KeyAction.MoveBackward, KeyCode.KeyS);
//                                  keyMap.Add(KeyAction.Jump, KeyCode.Space);

                 //CameraSceneNode camera1 = smgr.AddCameraSceneNode(null, new Vector3Df(-50, 450,0),
                   //  new Vector3Df(0, 0, 0), -1, true);
//                 null, 200,0.3f,-1,keyMap,false,7,false,true);
//                                   camera1.Position = parameters.player.Position;
//                                   camera1.Target = parameters.player.Target;
//                                   camera1.FarValue = parameters.player.FarValue;
    
                //camera.TargetAndRotationBinding = true;

//                     Maya(null, parameters.player.RotateSpeed, 1,
//                     parameters.player.MoveSpeed, -1);
//                 camera.TargetAndRotationBinding = true;
//                 //camera.Orthogonal = true;
// //                     smgr.AddCameraSceneNodeFPS(null,parameters.player.RotateSpeed,
// //                     parameters.player.MoveSpeed, -1, keyMap,false, parameters.player.JumpSpeed,false,true);
            


                



                // Create the player
              //  CameraSceneNode cameraTPS = smgr.AddCameraSceneNode(camera,new Vector3Df(0,0,0) , new Vector3Df(0,0,0), -1,false);

                CCharacter.SetCharacterSettings(parameters.character.MaxHealth, parameters.character.MaxAmmo,
                    parameters.character.TimeShotDelay, parameters.character.RegenerateHealth,
                    parameters.character.RefillPeriodHealth,parameters.character.RegenerateAmmo,
                    parameters.character.RefillPeriodAmmo,parameters.character.DrawnHealth,parameters.character.animationSpeed.Death,
                    parameters.character.animationSpeed.Boom, parameters.character.animationSpeed.CrouchAttack,
                    parameters.character.animationSpeed.Attack, parameters.character.animationSpeed.Jump,
                    parameters.character.animationSpeed.CrouchWalk, parameters.character.animationSpeed.Run,
                    parameters.character.animationSpeed.CrouchStand, parameters.character.animationSpeed.Stand,
                    parameters.character.OffsetStand,parameters.character.OffsetCrouch,
                    parameters.character.TimeFallToDeath);

                CCombatNPC.SetCombatNPCSettings(parameters.bots.TimeJump, parameters.bots.TimeCrouching,
                    parameters.character.TimeFallToDeath, parameters.bots.JumpUpFactor,parameters.bots.JumpAroundFactor);

                CCharacter.SCharacterDesc pDesc = new CCharacter.SCharacterDesc();
                pDesc.sceneManager = smgr;
                pDesc.AIManager = aimgr;
                //pDesc.RegenerateHealth = true;

                   player = new CPlayerCharacter(pDesc, camera, metaTriangleSelector,
                        parameters.player.Scale,
                        parameters.player.shotNode.Scale,parameters.player.shotNode.Position,
                        parameters.player.shotNode.Rotation, parameters.player.shotNode.ModelPath,
                        parameters.player.shotNode.TexturePath, parameters.game.FPS,camera.Target,camera.Position,
                        parameters.player.MoveSpeed, parameters.player.EllipsoidRadius,
                           parameters.player.GravityPerSecond, parameters.player.EllipsoidTranslation,
                           parameters.player.RotateSpeed, parameters.player.JumpSpeed);
                //,
                        //camera.Position + new Vector3Df(0, 150, 0));
                //-camera.Target);

//                 player = new CPlayerCharacter(pDesc, camera, metaTriangleSelector,
//                        parameters.player.Scale, parameters.player.Offset, parameters.projectile.StartPositionPlayer,
//                        parameters.player.shotNode.Scale, parameters.player.shotNode.Position,
//                        parameters.player.shotNode.Rotation, parameters.player.shotNode.ModelPath,
//                        parameters.player.shotNode.TexturePath,parameters.player.FPS);
// 
//                 if (parameters.player.FPS)
//                 {
//                     PlayerFPS playerFPS = (PlayerFPS)parameters.player;
// 
//                    
//                 }
//                 else
//                 {
//                     PlayerTPS playerTPS = (PlayerTPS)parameters.player;
// 
//                     player = new CPlayerCharacter(pDesc, camera, metaTriangleSelector,
//                         parameters.player.Scale, parameters.player.Offset, parameters.projectile.StartPositionPlayer,
//                         playerTPS.MeshPath, playerTPS.TexturePath);
//                 }
                //, E_CHARACTER_TYPE.ECT_CHASING);

                if (player == null)
                {
                    throw new Exception("Failed player creation");
    //                 Console.WriteLine("failed player creation");

    //                return false;
                }


                int numBots = parameters.bots.NumBots;
                int waypointCount = waypointGroup.Waypoints.Count;

                if (numBots > waypointCount)
                {
                    throw new Exception("The count of bots is too big. Must be less or equal to waypoints count.");
                    //Console.WriteLine("The count of bots is too big. Must be less or equal to waypoints count.");

                    //return false;
                }

                //npc


                //CNPCharacter character;
                CCharacter.SCharacterDesc desc = new CCharacter.SCharacterDesc();
                desc.RegenerateHealth = true;

                for (int i = 0; i < numBots; ++i)
                {
                    desc.sceneManager = smgr;
                    desc.AIManager = aimgr;

                    bool nextIt = false;
                    do
                    {
                        nextIt = false;

                        desc.StartWaypointID = (new System.Random()).Next() % waypointCount;

                        for (int j = 0; j < i; j++)
                            if (lBots[j].startWaypointID == desc.StartWaypointID)
                                nextIt = true;

                    } while (nextIt);

                    desc.WaypointGroupName = waypointGroup.getName();


                    lBots.Add(new CBotCharacter(desc,parameters.bots.ModelPath,parameters.bots.TexturePath , metaTriangleSelector,
                        parameters.bots.Range, parameters.bots.FieldOfViewDimensions, parameters.bots.MoveSpeed,
                        parameters.bots.AtDestinationThreshold));

                }

                //every bot in own team

                List<SEntityGroup> lBotTeams = new List<SEntityGroup>();

                for (int i = 0; i < numBots; i++)
                {
                    lBotTeams.Add(aimgr.createEntityGroup());
                    lBotTeams[i].addEntity(lBots[i].getAIEntity());
                }

                lBotTeams.Add(aimgr.createEntityGroup());
                lBotTeams[numBots].addEntity(player.getAIEntity());


                for (int i = 0; i <= numBots; i++)
                {
                    for (int j = 0; j <= numBots; j++)
                    {
                        if (i == numBots)
                            if (j == numBots)
                                player.getAIEntity().addAllyGroup(lBotTeams[j]);
                            else
                                player.getAIEntity().addEnemyGroup(lBotTeams[j]);
                        else
                            if (i == j)
                                lBots[i].getAIEntity().addAllyGroup(lBotTeams[j]);
                            else
                                lBots[i].getAIEntity().addEnemyGroup(lBotTeams[j]);
                    }
                }


                


                //CameraFPSSceneNodeAnimator animFPS = new CameraFPSSceneNodeAnimator();


               
                

                

                /*
                SceneNodeAnimator anim = smgr.CreateCollisionResponseAnimator(
                    metaTriangleSelector, camera, parameters.player.EllipsoidRadius,
                    parameters.player.GravityPerSecond, parameters.player.EllipsoidTranslation);

                camera.AddAnimator(anim);
                */

                

                // create skybox and skydome
                driver.SetTextureCreationFlag(TextureCreationFlag.CreateMipMaps, false);

                if (parameters.sky.Type == SkyType.Dome)
                {
                    SkyDome skyBox = (SkyDome)parameters.sky;
                    SceneNode skyDomeSceneNode = smgr.AddSkyDomeSceneNode(driver.GetTexture(skyBox.TexturePath),
                        skyBox.HoriRes, skyBox.VertRes, skyBox.TexturePercentage, skyBox.SpherePercentage);
                }
                else
                {
                    SkyBox skyBox = (SkyBox)parameters.sky;
                    SceneNode skyBoxSceneNode = smgr.AddSkyBoxSceneNode(
                         skyBox.TopTexturePath, skyBox.BottomTexturePath,
                         skyBox.LeftTexturePath, skyBox.RightTexturePath,
                         skyBox.FrontTexturePath, skyBox.BackTexturePath);                    
                }


                CIrrOcclusion.sceneManager = smgr;
                CIrrOcclusion.triangleSelector = metaTriangleSelector;
                aimgr.setOcclusionQueryCallback(CIrrOcclusion.Occlusion);

                CProjectile.SetProjectileSettings(parameters.projectile.StartPositionBotStand,
                    parameters.projectile.StartPositionBotCrouch, parameters.projectile.StartPositionPlayerStand,
                    parameters.projectile.StartPositionPlayerCrouch,
                    parameters.projectile.Speed, parameters.projectile.MaxDistanceTravelled,
                    parameters.projectile.LiveBillBoard.TextureTimePerFrame, parameters.projectile.LiveBillBoard.Dimension,
                    parameters.projectile.LiveBillBoard.TexturePaths,parameters.projectile.DieBillBoard.TextureTimePerFrame, 
                    parameters.projectile.DieBillBoard.Dimension, parameters.projectile.DieBillBoard.TexturePaths);
        
                driver.SetTextureCreationFlag(TextureCreationFlag.CreateMipMaps, true);

                // create event receiver
                //new MyEventReceiver(device, node, skybox, skydome);

                //            skybox.Visible = false;
            //skydome.Visible = true;

            //int lastFPS = -1;

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }

        }

        static public void moveCameraControl(IrrlichtDevice device, CPlayerCharacter player, Parameters parameters)
        {
            Vector2Df changeXYRelative = new Vector2Df(device.CursorControl.RelativePosition.X - cursorPosRelativeLast.X,
                 device.CursorControl.RelativePosition.Y - cursorPosRelativeLast.Y);


            
            player.RotateCamera(changeXYRelative);

            
            device.CursorControl.Position = new Vector2Di(parameters.window.Width / 2, parameters.window.Height / 2);
            cursorPosRelativeLast = new Vector2Df(device.CursorControl.RelativePosition.X, device.CursorControl.RelativePosition.Y);

            

// 
//             CameraSceneNode camera = device.SceneManager.ActiveCamera;
// 
// 
//              Vector3Df cameraPos = camera.AbsolutePosition;
// 
//              float changeX = (cursorPos.X - 0.5f);// *256.0f;
//              float changeY = (cursorPos.Y - 0.5f);// *256.0f;
//              direction += changeX;
//              zdirection -= changeY;
// 
//              if( zdirection <- 90 )
//                  zdirection = -90;
//              else
//                  if( zdirection > 90 )
//                      zdirection = 90;

             //device.CursorControl.RelativePosition = new Vector2Df(0.5f, 0.5f);
            
//              Vector3Df playerPos = playerNode.Position;
// 
//              float xf = playerPos.X - (float)Math.Cos(direction * Math.PI / 180.0f) *64.0f;
//              float yf = playerPos.Y - (float)Math.Sin(zdirection * Math.PI / 180.0f) *64.0f;
//              float zf = playerPos.Z + (float)Math.Sin(direction * Math.PI / 180.0f) *64.0f;
//      
//             // camera.Position = new Vector3Df(xf, yf, zf) ;
//              camera.Target = new Vector3Df(playerPos.X, playerPos.Y+25.0f, playerPos.Z);
//              
//             playerNode.Rotation = new Vector3Df( 0, direction, 0 );
        }
        
        static void Main(string[] args)
        {
            //irrAi
            try
            {

                CAIManager aimgr = null;
                SWaypointGroup waypointGroup = null;
                List<CBotCharacter> lBots = null;
                CPlayerCharacter player = null;
                //CFleeingCharacter fleeingCharacter = null;
                //CChasingCharacter chasingCharacter = null;







                //irrGame
                Parameters parameters = new Parameters();
                if (!LoadParametersFromXML("Parameters.xml", ref parameters))
                    throw new Exception("Error to load parameters from Parameters.xml file. Check the file structure or parameter's values.");

//                 CProjectile.StartPositionBotStand = parameters.projectile.StartPositionBotStand;
//                 CProjectile.StartPositionBotCrouch = parameters.projectile.StartPositionBotCrouch;
//                 CProjectile.Speed = parameters.projectile.Speed;
//                 CProjectile.MaxDistanceTravelled = parameters.projectile.MaxDistanceTravelled;



                IrrlichtCreationParameters irrCrPar = new IrrlichtCreationParameters();
                irrCrPar.AntiAliasing = parameters.graphics.Antialiasing;
                irrCrPar.DriverType = parameters.graphics.DriverType;
                irrCrPar.WindowSize = new Dimension2Di(parameters.window.Width, parameters.window.Height);
                irrCrPar.BitsPerPixel = parameters.window.BitsPerPixel;
                irrCrPar.Fullscreen = parameters.graphics.FullScreen;
                irrCrPar.VSync = parameters.graphics.VSync;

                IrrlichtDevice device = IrrlichtDevice.CreateDevice(irrCrPar);
            
                device.OnEvent += new IrrlichtDevice.EventHandler(deviceOnEvent);
			
                VideoDriver driver = device.VideoDriver;
                SceneManager smgr = device.SceneManager;
                //GUIEnvironment env = device.GUIEnvironment;

                driver.SetTextureCreationFlag(TextureCreationFlag.Always32Bit, true);

                // add irrlicht logo
                //env.AddImage(driver.GetTexture("media/irrlichtlogoalpha2.tga"), new Vector2Di(10));

                // set gui font
                //env.Skin.SetFont(env.GetFont("media/fontlucida.png"));

                // add some help text
                //env.AddStaticText(
                  //  "Press 'W' to change wireframe mode\nPress 'D' to toggle detail map\nPress 'S' to toggle skybox/skydome",
                    //new Recti(10, 421, 250, 475), true, true, null, -1, true);

                string[] mapPath = parameters.map.MapPath.Split(new char[] { '\\', '/','.' });

                if (mapPath[mapPath.Length - 1] == "irr")
                    smgr.LoadScene(parameters.map.MapPath);
                else
                {
                    device.FileSystem.AddFileArchive(parameters.map.MapPath);



                    AnimatedMesh mesh = smgr.GetMesh(mapPath[mapPath.Length - 2] + ".bsp");
             
                    smgr.AddOctreeSceneNode(mesh.GetMesh(0), null, -1, parameters.map.MinimalPolysPerNode);
                }
                //node.Position = new Vector3Df(0, 0, 0);




                //"media/big_fps_scene.irr");

                //selector
                MetaTriangleSelector metaTriangleSelector = smgr.CreateMetaTriangleSelector();

                List<SceneNode> listSceneNode = smgr.GetSceneNodesFromType(SceneNodeType.Any);

                foreach (SceneNode sceneNode in listSceneNode)
                {
                    TriangleSelector triangleSelector = null;
                
                    switch(sceneNode.Type)
                    {
                    case SceneNodeType.Cube: case SceneNodeType.AnimatedMesh:
                        triangleSelector = smgr.CreateTriangleSelectorFromBoundingBox(sceneNode);
                    break;

                    case SceneNodeType.Mesh: case SceneNodeType.Sphere:
                        triangleSelector = smgr.CreateTriangleSelector(((MeshSceneNode)sceneNode).Mesh, sceneNode);
                        break;

                    case SceneNodeType.Terrain:
                        triangleSelector = smgr.CreateTerrainTriangleSelector((TerrainSceneNode)sceneNode);
                        break;

                    case SceneNodeType.Octree:
                        triangleSelector = smgr.CreateOctreeTriangleSelector(((MeshSceneNode)sceneNode).Mesh, sceneNode);
                        break;
                    }

                    if(triangleSelector != null)
                    {
                        metaTriangleSelector.AddTriangleSelector(triangleSelector);
                        triangleSelector.Drop();
                    }


                }








    // 
    // 



    //             // create triangle selector for the terrain
    //             TriangleSelector selector = smgr.CreateOctreeTriangleSelector(node.Mesh, node, 128);
    // 
    //             node.TriangleSelector = selector;
    // 
    // 
    // 
    //             metaTriangleSelector.AddTriangleSelector(selector);
    // 
    //             // create collision response animator and attach it to the camera
    //             selector.Drop();




                aimgr = new CAIManager(device);
                aimgr.loadAI(parameters.map.MapAIPath);
                //"media/big_fps.irrai");
                aimgr.createDebugWaypointMeshes();


                //if(!
                if (!StartNewGame(ref driver, ref parameters, ref device, ref smgr, ref aimgr,
                    ref metaTriangleSelector, ref waypointGroup, ref lBots, ref player))
                    throw new Exception("Error in the parameter's values from Parameters.xml file. Failed to start new game.");
                    //)
    //             {
    //                 
    //                 Console.WriteLine("Failed to start game.");
    //                 return;
    //            }





            





              
            







    //             if (playerType == ECT_FLEEING)
    //             {
    //                 fleeTeam->addEntity(player->getAIEntity());
    //                 player->getAIEntity()->setEnemyGroup(chaseTeam);
    //                 player->getAIEntity()->setAllyGroup(fleeTeam);
    //             }
    //             else if (playerType == ECT_CHASING)
                //{

                //    fleeTeam.addEntity(player.getAIEntity());
                  //  player.getAIEntity().setEnemyGroup(chaseTeam);
                    //player.getAIEntity().setAllyGroup(fleeTeam);
                    //player.getAIEntity().setAllyGroup(chaseTeam);

                //}
  




            
            
                // add terrain scene node
            
             /*   TerrainSceneNode terrain = smgr.AddTerrainSceneNode(
                    "media/terrain-heightmap.bmp",	// heightmap
                    null,								// parent node
                    -1,									// node id
                    new Vector3Df(),					// position
                    new Vector3Df(),					// rotation
                    new Vector3Df(40, 4.4f, 40),		// scale
                    new Color(255, 255, 255),			// vertex color
                    5,									// max LOD
                    TerrainPatchSize._17,				// patch size
                    4);									// smooth factor

                terrain.SetMaterialFlag(MaterialFlag.Lighting, false);
                terrain.SetMaterialTexture(0, driver.GetTexture("media/terrain-texture.jpg"));
                terrain.SetMaterialTexture(1, driver.GetTexture("media/detailmap3.jpg"));
                terrain.SetMaterialType(MaterialType.DetailMap);

                terrain.ScaleTexture(1, 20);
                */

            
            
                //device.FileSystem.AddFileArchive("media/map-20kdm2.pk3"); //1
          

                device.SetWindowCaption(parameters.game.Name);

            
                // Pre-load explosion projectile plasmaball textures
// 	    for (int i = 1 ; i <= 11 ; ++i)
// 		    driver.GetTexture("Media/Explosion/"+i.ToString()+".jpg");
		
                foreach (string texturePath in parameters.projectile.LiveBillBoard.TexturePaths)
                {
                    driver.GetTexture(texturePath);
                }

// 	    for (int i = 1 ; i <= 7 ; ++i) 
// 		    driver.GetTexture("Media/Projectile/portal"+i.ToString()+".jpg");

                foreach (string texturePath in parameters.projectile.DieBillBoard.TexturePaths)
                {
                    driver.GetTexture(texturePath);
                }

// 	    for (int i = 1 ; i <= 6 ; ++i) 
// 		    driver.GetTexture("Media/Plasmaball/"+i.ToString()+".jpg");
    
                // disable mouse cursor
                device.CursorControl.Visible = false;
            


                uint startTime = device.Timer.Time;
	            uint elapsedTime = 0;
                uint approxMaxFrameTime = 100;

                GUIFont font = device.GUIEnvironment.GetFont(parameters.interfaces.FontDigitPath);

                string strHealth = "";
                string strProjectiles = "";

                Dimension2Di d2diHealth = new Dimension2Di();
                Dimension2Di d2diProjectiles = new Dimension2Di();
            
                Texture imageWin = driver.GetTexture(parameters.interfaces.WinImagePath);
                Texture imageLose = driver.GetTexture(parameters.interfaces.LoseImagePath);

                bool botsDead = true;

            
            
            
    //             bool bAn = true;
    //             CollisionResponseSceneNodeAnimator cAnim = smgr.CreateCollisionResponseAnimator(metaTriangleSelector,
    //                                         Bill, Bill.BoundingBox.MaxEdge - Bill.BoundingBox.Center, new Vector3Df(0, -10, 0));

                device.CursorControl.Position = new Vector2Di(parameters.window.Width / 2, parameters.window.Height / 2);
                cursorPosRelativeLast = new Vector2Df(device.CursorControl.RelativePosition.X,device.CursorControl.RelativePosition.Y);
                

                while (device.Run() && !closed)
                {
                
                    if (device.WindowActive)
                    {
                        moveCameraControl(device, player, parameters);







    //                     if (!paused)
    //                     {
                            // Calculate elapsed time

                        if (needStartGame)
                        {

                            foreach (CBotCharacter bot in lBots)
                            {
                                bot.deleteNodes();
                            }
                        
    //                         if(player.getAIEntity().bIsLive)
    //                             player.RemoveGun();

                            player.deleteNodes();

    //                        if(!
                            if(!StartNewGame(ref driver, ref parameters, ref device, ref smgr, ref aimgr,
                               ref metaTriangleSelector, ref waypointGroup, ref lBots, ref player))
                                throw new Exception("Error in the parameter's values from Parameters.xml file. Failed to start new game.");
                            //)
    //                         {
    //                             Console.WriteLine("Failed to start game.");
    //                             return;
    //                         }



                            needStartGame = false;
                            endGame = false;
                        }

//                         if (!endGame)
//                         {
                            elapsedTime = device.Timer.Time - startTime;
                            startTime = device.Timer.Time;


                            if (elapsedTime > approxMaxFrameTime)
                                elapsedTime = approxMaxFrameTime;

                            // Update AIManager
                            aimgr.update(elapsedTime);

                            // Update characters
                            
                            if(!endGame)
                                botsDead = true;

                            foreach (CBotCharacter bot in lBots)
                            {


                            
                                    bot.update(elapsedTime);

                                    if (bot.getAIEntity().bIsLive)
                                    {
                                        if(!endGame)
                                            botsDead = false;
                                    }
                            }


                            if (botsDead && !endGame)
                            {
                                endGame = true;
                            
                            }
    /*
                            // Update characters
                            if (chasingCharacter.bIsLive)
                                chasingCharacter.update(elapsedTime);
                                 //   chasingCharacter = null;

                            if (fleeingCharacter.bIsLive)
                                fleeingCharacter.update(elapsedTime);
                        
                                   // fleeingCharacter = null;


                            //fleeingCharacter.Pos = new Vector3Df(0, 300, 0);

                            //                     		static bool IsKeyDown(KeyCode keyCode)
                            // 		{
                            // 			return KeyIsDown.ContainsKey(keyCode) ? KeyIsDown[keyCode] : false;
                            // 		}

                            //if(MouseMouseEventType. nIsKeyDown()
    */

                            player.SetAnimation(playerTurnForward, playerTurnBack, playerTurnLeft,
                                playerTurnRight, mouseDown, playerCrouch);


                            

                            player.Turn(playerTurnForward, playerTurnBack, playerTurnLeft, playerTurnRight);


                            if (mouseDown)
                            {
                                player.fire();
                            }
                            
                            if (playerJump)
                            {
                                player.Jump();
                            }

                            if (player.update(elapsedTime)) 
                                {
                                    //player = null;
                                    //smgr.anim


                                

                                    endGame = true;
                                }

                                //player.Crouch(playerCrouch);
                                
                        

                        //if(playerCrouch)
                          //  CameraFPSSceneNodeAnimator
                                strHealth = player.getHealth().ToString();
                                strProjectiles = player.getAmmo().ToString();

                                d2diHealth = font.GetDimension(strHealth);

                                d2diProjectiles = font.GetDimension(strProjectiles);

                             if (!player.getAIEntity().bIsLive)
                                endGame = true;



//                             if (endGame&&FPS)
  //                           {
//                                 //player.RemoveGun();
// 
//                                 foreach (CBotCharacter bot in lBots)
//                                 {
//                                
//                                         bot.StopAnimation();
//                                 }
// 
                                 //player.StopAnimation();
                             //}
                    
                       // }

                            driver.BeginScene(true, true, new Color(0));

                            smgr.DrawAll();
                            //env.DrawAll();

                            if (endGame)
                            {
                                Texture textureEnd = botsDead ? imageWin : imageLose;

                                driver.Draw2DImage(textureEnd, new Vector2Di((parameters.window.Width - textureEnd.Size.Width) / 2,
                                    (parameters.window.Height - textureEnd.Size.Height) / 2),
                                    new Recti(0, 0, textureEnd.Size.Width, textureEnd.Size.Height), null, new Color(255, 255, 255), true);
                            }
                            else
                            {
                                font.Draw(strHealth, new Vector2Di(0, parameters.window.Height - d2diHealth.Height),parameters.interfaces.ColorHealth);
                                font.Draw(strProjectiles, new Vector2Di(parameters.window.Width - d2diProjectiles.Width,
                                    parameters.window.Height - d2diProjectiles.Height), parameters.interfaces.ColorProjectiles);

                            }

                            //driver.Draw3DLine(ray1, Color.OpaqueGreen);

                            driver.EndScene();


    //                     }
    //                     else
    //                     {
    //                         driver.BeginScene(true, true, new Color(0));
    // 
    //                         smgr.DrawAll();
    // 
    //                         driver.EndScene();
    //                     }

                    
                        // display frames per second in window title
                        /*int fps = driver.FPS;
                        if (lastFPS != fps)
                        {
                            // also print terrain height of current camera position
                            // we can use camera position because terrain is located at coordinate origin

                            device.SetWindowCaption("Game FPS");
                                  //String.Format(
    //                              "Terrain rendering example - Irrlicht Engine [{0}] fps: {1} Height: {2}",
    //                              driver.Name, fps, terrain.GetHeight(camera.AbsolutePosition.X, camera.AbsolutePosition.Z)));

                            lastFPS = fps;
                        }*/
                    }
                }

                device.Drop();
            
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }

/*        static bool AskUserForDriver(out DriverType driverType)
        {
            driverType = DriverType.Null;

            Console.Write("Please select the driver you want for this example:\n" +
                        " (a) OpenGL\n (b) Direct3D 9.0c\n (c) Direct3D 8.1\n" +
                        " (d) Burning's Software Renderer\n (e) Software Renderer\n" +
                        " (f) NullDevice\n (otherKey) exit\n\n");

            ConsoleKeyInfo i = Console.ReadKey();

            switch (i.Key)
            {
                case ConsoleKey.A: driverType = DriverType.OpenGL; break;
                case ConsoleKey.B: driverType = DriverType.Direct3D9; break;
                case ConsoleKey.C: driverType = DriverType.Direct3D8; break;
                case ConsoleKey.D: driverType = DriverType.BurningsVideo; break;
                case ConsoleKey.E: driverType = DriverType.Software; break;
                case ConsoleKey.F: driverType = DriverType.Null; break;
                default:
                    return false;
            }

            return true;
        }
 */

    }
        
   /* class MyEventReceiver
    {
        public MyEventReceiver(IrrlichtDevice device, SceneNode terrain, SceneNode skybox, SceneNode skydome)
        {
            this.terrain = terrain;
            this.skybox = skybox;
            this.skydome = skydome;
            showBox = true;
            showDebug = false;

             skybox.Visible = true;
             skydome.Visible = false;

            device.OnEvent += new IrrlichtDevice.EventHandler(device_OnEvent);
        }

        bool device_OnEvent(Event e)
        {
            // check if user presses the key 'W', 'P', 'D', 'S' or 'X'
/*            if (e.Type == EventType.Key && e.Key.PressedDown)
            {
                switch (e.Key.Key)
                {
                    case KeyCode.KeyW: // switch wire frame mode
                        terrain.SetMaterialFlag(MaterialFlag.Wireframe, !terrain.GetMaterial(0).Wireframe);
                        terrain.SetMaterialFlag(MaterialFlag.PointCloud, false);
                        return true;

                    case KeyCode.KeyP: // switch point cloud mode
                        terrain.SetMaterialFlag(MaterialFlag.PointCloud, !terrain.GetMaterial(0).PointCloud);
                        terrain.SetMaterialFlag(MaterialFlag.Wireframe, false);
                        return true;

                    case KeyCode.KeyD: // toggle detail map
                        terrain.SetMaterialType(terrain.GetMaterial(0).Type == MaterialType.Solid ?
                            MaterialType.DetailMap : MaterialType.Solid);
                        return true;

                    case KeyCode.KeyS: // toggle skies
                        showBox = !showBox;
                        skybox.Visible = showBox;
                        skydome.Visible = !showBox;
                        return true;

                    case KeyCode.KeyX: // toggle debug information
                        showDebug = !showDebug;
                        terrain.DebugDataVisible = showDebug ? DebugSceneType.BBoxAll : DebugSceneType.Off;
                        return true;
                }
            }
            
            return false;
        }

        SceneNode terrain;
        SceneNode skybox;
        SceneNode skydome;
        bool showBox;
        bool showDebug;
    }
    */
             
}
