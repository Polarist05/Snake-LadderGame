using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameView : MonoBehaviour
{
    public static GameView instance;
    public Texture2D plus;
    public Texture2D minus;
    public Texture2D stop;
    public Texture2D pause;
    public Texture2D resume;
    public Font font;
    public readonly Color purple = new Color32(r: 63, g: 56, b: 112, a: byte.MaxValue);
    public Material graphMaterial;
    public TMP_FontAsset graphFont;
    private UIDocument document;
    private VisualElement _root;

    public InpectorElement inspectorElement;
    public Label rollLabel;
    public EndGameElement endGameElement;
    private ButtonMenu buttonMenu;
    const float margin = 10f;
    public StyleSheet ussUtility;
    [Header("End Game Eleement")]
    public VisualTreeAsset endGameUXML;
    public StyleSheet endGameUSS;

    [Header("Graph Button Eleement")]
    public VisualTreeAsset buttonMenuUXML;
    public StyleSheet buttonMenuUSS;
    private void Awake()
    {
        document=GetComponent<UIDocument>();
        instance= this;
    }
    
    private void OnEnable()
    {
        instance ??= this;
        _root = document.rootVisualElement;
        _root.style.unityFontDefinition = new StyleFontDefinition(font);
        _root.style.color = Color.white;
        _root.style.fontSize = 24;
        _root.style.unityTextAlign = TextAnchor.MiddleCenter;
        inspectorElement = new InpectorElement();
        rollLabel = new Label("ROLL IT")
        {
            style =
            {
                position= Position.Absolute,
                width = new StyleLength(new Length(100,LengthUnit.Percent)),
                bottom = new StyleLength(new Length(1,LengthUnit.Percent)),
                fontSize = new StyleLength(new Length(40,LengthUnit.Percent)),
                unityTextOutlineColor= Color.black,
                unityTextOutlineWidth= 2,

            }
        };
        endGameElement = new EndGameElement();
        buttonMenu= new ButtonMenu();

        _root.Add(inspectorElement);
        _root.Add(rollLabel);
        _root.Add(buttonMenu);
        _root.Add(endGameElement);
    }
    void Update()
    {
        var position =Input.mousePosition;
        inspectorElement.style.bottom = position.y;
        inspectorElement.style.left = position.x;
    }

    public class InpectorElement : VisualElement
    {
        private float height = 40f;
        private VisualElement icon;
        private Label distanceLabel;

        public InpectorElement( )
        {
            float iconSize = height - 15;
            icon = new VisualElement()
            {
                style = {
                    borderBottomLeftRadius = height/2,
                    borderBottomRightRadius = height / 2,
                    borderTopLeftRadius = height / 2,
                    borderTopRightRadius = height / 2,
                    width= iconSize,
                    height= iconSize,
                    alignSelf = Align.Center,
                }
            };
            distanceLabel = new Label(text: " 3")
            {
                style =
                {
                    alignContent = Align.Center,
                    unityTextAlign = TextAnchor.MiddleCenter,
                }
            };
            Add(icon);
            Add(distanceLabel);
            setStyle();
        }
        private void setStyle()
        {
            style.flexDirection = FlexDirection.Row;
            style.height = height;
            style.position = Position.Absolute;
            style.backgroundColor = instance.purple;
            style.paddingBottom = margin;
            style.paddingLeft = margin;
            style.paddingRight = margin;
            style.paddingTop = margin;
            style.borderBottomLeftRadius = height / 2;
            style.borderBottomRightRadius = height / 2;
            style.borderTopLeftRadius = height / 2;
            style.borderTopRightRadius = height / 2;
            style.display = DisplayStyle.None;
        }

        public void setAction(bool visible, GameAction evt = null)
        {
            this.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (!visible)
                return;
            switch (evt.type)
            {
                case GameActionType.None:
                    break;
                case GameActionType.Event:
                    switch (evt.eventType)
                    {
                        case GameEventType.Forward:
                            icon.style.backgroundImage = GameManager.instance.textureTable.plus;
                            break;
                        case GameEventType.Backword:
                            icon.style.backgroundImage = GameManager.instance.textureTable.minus;
                            break;
                        case GameEventType.Stop:
                            icon.style.backgroundImage = GameManager.instance.textureTable.stop;
                            break;
                    }
                    break;
                case GameActionType.Ladder:
                    icon.style.backgroundImage = GameManager.instance.textureTable.ladder;
                    break;
                case GameActionType.Snake:
                    icon.style.backgroundImage = GameManager.instance.textureTable.snake;
                    break;
                default:
                    break;
            }
            distanceLabel.text = evt.distance.ToString();
        }
    }
    public class EndGameElement:VisualElement
    {
        public Button restartBtn;
        public Label label;
        public EndGameElement()
        {
            this.Add(instance.endGameUXML.CloneTree().Q<VisualElement>(name:"root"));
            this.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            this.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            this.style.position = Position.Absolute;
            this.style.display = DisplayStyle.None;
            this.styleSheets.Add(instance.endGameUSS);
            this.styleSheets.Add(instance.ussUtility);
            restartBtn = this.Q<Button>(name: "button");
            label = this.Q<Label>(name: "label");
            
        }
    }
    public class ButtonMenu : VisualElement
    {
        private bool displayFlag = false;
        private bool stopFlag = false;
        private Button graphBtn;
        private Button pauseBtn;
        private VisualElement pauseWindow;

        private void graphButtonCallback()
        {
            displayFlag= !displayFlag;
            if (displayFlag)
            {
                _ = AnimationUtility.FadeIn(instance.graphMaterial);
                _ = AnimationUtility.FadeIn(instance.graphFont);
            }
            else
            {
                _= AnimationUtility.FadeOut(instance.graphMaterial);
                _= AnimationUtility.FadeOut(instance.graphFont);
            }
        }
        private void pauseButtonCallback() 
        {
            GameManager.pauseFlag = !GameManager.pauseFlag;
            if (GameManager.pauseFlag)
            {
                pauseWindow.visible = true;
                graphBtn.visible = false;
                pauseBtn.Children().ElementAt(0).style.backgroundImage = new StyleBackground(instance.resume);
                Time.timeScale = 0;

            }
            else
            {
                pauseWindow.visible = false;
                graphBtn.visible = true;
                pauseBtn.Children().ElementAt(0).style.backgroundImage = new StyleBackground(instance.pause);
                Time.timeScale = 1;
            }
        }
        public ButtonMenu()
        {
            instance.graphMaterial.color = new Color(1, 1, 1, 0);
            var initialFaceColor = instance.graphFont.material.GetColor("_FaceColor");
            var initialOutlineColor = instance.graphFont.material.GetColor("_OutlineColor");
            instance.graphFont.material.SetColor("_FaceColor", new Color(initialFaceColor.r, initialFaceColor.g, initialFaceColor.b, 0));
            instance.graphFont.material.SetColor("_OutlineColor", new Color(initialOutlineColor.r, initialOutlineColor.g, initialOutlineColor.b, 0));
            var buttonMenu=instance.buttonMenuUXML.CloneTree().Q<VisualElement>();
            buttonMenu.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            buttonMenu.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            this.Add(buttonMenu);
            this.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            this.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            this.style.position = Position.Absolute;
            this.styleSheets.Add(instance.buttonMenuUSS);
            graphBtn = this.Q<Button>(name: "GraphBtn");
            graphBtn.clicked += graphButtonCallback;
            pauseBtn = this.Q<Button>(name: "PauseButton");
            pauseBtn.clicked += pauseButtonCallback;
            pauseWindow = this.Q<VisualElement>(name: "PauseWindow");
            pauseWindow.visible = false;
        }
    }
}
