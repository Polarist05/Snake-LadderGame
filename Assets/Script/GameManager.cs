using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static bool pauseFlag=false;
    public const int START_POSITION=0;
    public const float PARTICLE_DELAY = .8f;
    public const float ROLLING_TIME = .2f;
    [Header("Player Movement")]
    public float moveDuration = .5f;
    public AnimationCurve moveAnimation;
    [Header("Dice")]
    public Dice dice;
    public AnimationCurve diceCurve;
    public float rollingHeight;
    [Range(0, 1)]
    public float threashold = .5f;
    [Range(1, 20)]
    public float diceSpeed = 5;
    public LayerMask diceMask;
    public GameObject diceBase;
    [Header("Pawn")]
    public GameObject pawnPrefab;
    public PawnColorTable pawnColorTable;
    [Header("Board")]
    public List<GameAction> actions = new List<GameAction>();
    public LayerMask boardMask;
    public Board board;
    [Header("Automata")]
    public AutomataManager automataManager;
    [Header("UI")]
    public GameView view;
    [Header("Particle")]
    public ParticleSystem goodParticle;
    public ParticleSystem badParticle;
    public ParticleSystem confettiParticle;
    [Header("Sound")]
    public SoundManager soundManager;
    [Header("Table")]
    public EventModelTable gameEventTable;
    public MapModelTable mapTable;
    public TextureTable textureTable;

    #region DLLImport
    [DllImport("__Internal")]
    public static extern void ShowMessage(string str);
    [DllImport("__Internal")]
    public static extern void OpenSummarize(string str);
    #endregion

    internal List<Pawn> pawns;
    internal int _currentTurn = 0;
    internal int currentTurn { 
        get => _currentTurn;
        set { 
            if (_currentTurn != value)
            {
                _currentTurn = value;
                UpdateDiceColor();
            }
        } 
    }
    internal Pawn currentPawn => pawns[currentTurn];
    private int _hoverIndex;
    internal int hoverIndex {
        get => _hoverIndex;
        set {
            if (_hoverIndex != value)
            {
                if (value >= 0)
                {
                    var evt = actions.Find(evt => (evt.position == value));
                    if (evt != null)
                    {
                        view.inspectorElement.setAction(true, evt);
                    }
                    else
                    {
                        view.inspectorElement.setAction(false);
                    }
                }
                else
                {
                    view.inspectorElement.setAction(false);
                }
                _hoverIndex = value;
            }
        }
    }
    internal GameState _state;
    internal GameState _nextState;

    private RecieveData _recieveData = new RecieveData() { code = "AFEQYY488", pawnColor = new string[] { "red", "green", "blue", "yellow", "purple", "white" }, map = "map05" };
    public RecieveData recieveData => _recieveData;
    public static GameManager instance;

    private int playerCount => _recieveData.pawnColor.Length;
    
    
    private void Start()
    {
        #if UNITY_EDITOR
        setNextState(new StartState(this));
        //startReplay(GameUtility.sampleString);
        #endif
    }

    private void OnEnable()
    {
        instance = this;
        #if UNITY_EDITOR
        if(_state== null)
            setNextState(new WaitForRollState(this));
        #endif
    }
    void Update()
    {
        SetState();
        _state?.Update();
        if (Input.GetMouseButtonDown(0))
            soundManager.Play(SoundEffectType.click);
    }

    public void CheckBoardRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance: 50, layerMask: boardMask))
        {
            var index = board.getIndex(hit.point);
            if (index != -1)
            {
                hoverIndex = index;
            }
        }
        else
        {
            hoverIndex= -1;
        }
    }

    #region Setup
    public void ResetGame()
    {

        view.endGameElement.style.display = DisplayStyle.None;
        view.rollLabel.style.display = DisplayStyle.None;
        view.inspectorElement.style.display = DisplayStyle.None;
        var map = mapTable.maps.Find(map => (map.Name == _recieveData.map));
        actions.Clear();
        setMap(map);
        actions.AddRange(map.nonEventAction);
        setColor();
        currentTurn = 0;
        _ = UpdatePosition(START_POSITION, false);
    }
    public void setMap(MapModel map)
    {
        if (map != null && map.material != null)
            board.GetComponent<MeshRenderer>().material = map.material;
    }
    public void setColor()
    {
        for (int i = 0; i < pawns?.Count; i++)
            Destroy(board.transform.GetChild(i).gameObject);
        pawns = new List<Pawn>(capacity: playerCount);
        for (int i = 0; i < playerCount; i++)
        {
            Material material = pawnColorTable.datas.Find(x => (x.Name == _recieveData.pawnColor[i])).material;
            pawns.Add(Instantiate(original: pawnPrefab, parent: board.transform).GetComponentInChildren<Pawn>());
            pawns[i].SetMaterial(material);
            pawns[i].currentPosition = START_POSITION;
        }
        UpdateDiceColor();
    }
    public List<GameAction> GenerateRandomActions()
    {
        var map = mapTable.maps.Find(map => (map.Name == _recieveData.map));
        if (map != null)
        {
            System.Random random = new System.Random();
            actions.AddRange(map.eventPostion.Select(i => {
                var model = GameManager.instance.gameEventTable.events;
                return new GameAction(model[random.Next(model.Count)], i);
            }));
        }
        return actions;
    }
    public string GenerateSetupLanguage()
    {
        var events = actions.Where(action => action.type == GameActionType.Event)
            .Select(evt => {
                switch (evt.eventType)
                {
                    case GameEventType.Forward:
                        return $"P{evt.position}F{evt.distance}";
                    case GameEventType.Backword:
                        return $"P{evt.position}B{evt.distance}";
                    case GameEventType.Stop:
                        return $"P{evt.position}S{evt.distance}";
                    default:
                        return $"";
                }

            });
        string language=
            "setup : {\r\n" +
            $"\tcode : {_recieveData.code},\r\n" +
            $"\tmap : {_recieveData.map},\r\n" +
            $"\tcolors : {AutomataManager.FromListToString(list: _recieveData.pawnColor.ToList())},\r\n" +
            $"\tevents : {AutomataManager.FromListToString(list: events.ToList())}\r\n" +
            "}";

        return language;
    }
    #endregion
    
    #region Change State
    public void setNextState(GameState state)
    {
        _nextState = state;
        if (state is RollingState)
        {
            view.inspectorElement.setAction(false);
        }
    }
    private void SetState()
    {
        if (_nextState != null)
        {
            _state = _nextState;
            _nextState = null;
            _state.Start();
        }
    }
    private void UpdateDiceColor()
    {
        dice.GetComponent<MeshRenderer>().material = pawns[currentTurn].GetMaterial();
        dice.GetComponent<MeshRenderer>().materials[1].color = Color.white;
    }
    public async Task UpdatePosition(int position, bool isAnimation = true)
    {
        var arr = pawns.Where(predicate: pond => (pond.currentPosition == position)).ToList();
        var postion = board.getPositionAt(position);
        List<Task> tasks = new List<Task>();
        switch (arr.Count)
        {
            case 0: break;
            case 1:
                tasks.Add(arr[0].Move(postion, moveDuration, moveAnimation));
                break;
            case 2:
                tasks.Add(arr[0].Move(postion + new Vector3(0, 0, .25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[1].Move(postion + new Vector3(0, 0, -.25f), moveDuration, moveAnimation, isAnimation));
                break;
            case 3:
                tasks.Add(arr[0].Move(postion + new Vector3(0, 0, .25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[1].Move(postion + new Vector3(.25f, 0, -.25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[2].Move(postion + new Vector3(-.25f, 0, -.25f), moveDuration, moveAnimation, isAnimation));
                break;
            case 4:
                tasks.Add(arr[0].Move(postion + new Vector3(.25f, 0, .25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[1].Move(postion + new Vector3(.25f, 0, -.25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[2].Move(postion + new Vector3(-.25f, 0, -.25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[3].Move(postion + new Vector3(-.25f, 0, .25f), moveDuration, moveAnimation, isAnimation));
                break;
            case 5:
                tasks.Add(arr[0].Move(postion + new Vector3(.25f, 0, .25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[1].Move(postion + new Vector3(.25f, 0, -.25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[2].Move(postion + new Vector3(-.25f, 0, -.25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[3].Move(postion + new Vector3(-.25f, 0, .25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[4].Move(postion, moveDuration, moveAnimation, isAnimation));
                break;
            case 6:
                tasks.Add(arr[0].Move(postion + new Vector3(.25f, 0, .25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[1].Move(postion + new Vector3(.25f, 0, -.25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[2].Move(postion + new Vector3(-.25f, 0, -.25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[3].Move(postion + new Vector3(-.25f, 0, .25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[4].Move(postion + new Vector3(0, 0, .25f), moveDuration, moveAnimation, isAnimation));
                tasks.Add(arr[5].Move(postion + new Vector3(0, 0, -.25f), moveDuration, moveAnimation, isAnimation));
                break;
        }
        await Task.WhenAll(tasks);
    }
    internal void IncreaseSkipTurn(int playerIndex,int amount = 1)
    {
        if(playerIndex<playerCount)
            pawns[playerIndex].skipTurnCount += amount;
    }
    internal void MoveToNextTurn()
    {
        var nextTurn = currentTurn;
        nextTurn = (++nextTurn== pawns.Count) ? 0 : nextTurn;
        while (pawns[nextTurn].skipTurnCount != 0)
        {
            pawns[nextTurn].skipTurnCount--;
            nextTurn = (++nextTurn == pawns.Count) ? 0 : nextTurn;
        }
        currentTurn = nextTurn;
    }
    #endregion

    #region Particle
    internal void showGoodParticleAt(Pawn pawn)
    {
        goodParticle.transform.position = board.getPositionAt(pawn.currentPosition);
        goodParticle.Play();
        soundManager.Play(SoundEffectType.goodEvent);
    }
    internal void showBadParticleAt(Pawn pawn)
    {
        badParticle.transform.position = board.getPositionAt(pawn.currentPosition);
        badParticle.Play();
        soundManager.Play(SoundEffectType.badEvent);
    }
    #endregion

    #region Plug In
    [ContextMenu(nameof(startReplay))]
    public void startReplay(string replayString)
    {
        var simpLanguage = GameUtility.SimpliflyString(replayString);
        string pattern = @"^setup :{code : ([a-z0-9]+),map : ([a-z,0-9]+),colors :\[([a-z,0-9]+)\],events :\[([a-z,0-9]+)\]},(walks :\[.*\])$ ";
        pattern = GameUtility.SimpliflyString(pattern);
        var match = Regex.Match(simpLanguage, pattern);
        if (match.Success)
        {
            var code = match.Groups[1].Value;
            var map = match.Groups[2].Value;
            var colors = match.Groups[3].Value.Split(',');
            var events = match.Groups[4].Value.Split(',').Select(x =>
            {
                var match2 = Regex.Match(x, @"p([0-9][0-9]|[0-9])(f|b|s)([0-9])");
                int pos = Int32.Parse(match2.Groups[1].Value);
                string t = match2.Groups[2].Value;
                int num = Int32.Parse(match2.Groups[3].Value);
                var action = new GameAction() { position = pos, distance = num, type = GameActionType.Event };
                switch (t)
                {
                    case "f":
                        action.eventType = GameEventType.Forward;
                        break;
                    case "b":
                        action.eventType = GameEventType.Backword;
                        break;
                    case "s":
                        action.eventType = GameEventType.Stop;
                        break;
                }
                return action;
            }).ToList();
            var automataString = Regex.Match(input: replayString, pattern: @"walks(.|\n)*]$",RegexOptions.Multiline).Value;
            
            recieveData.code = code;
            recieveData.map = map;
            recieveData.pawnColor = colors;
            ResetGame();
            actions.AddRange(events);
            setNextState(new ReplayState(this,automataString));
            
        }
        else
        {
            Debug.Log("Not Match");
        }
    }
    public void sendToUnity(string jsonStr)
    {
        _recieveData =JsonUtility.FromJson(jsonStr, type: typeof(RecieveData)) as RecieveData;
        setNextState(new StartState(this));
    }
    public void sendToJson()
    {
        int[] getReward()
        {
            int getMaxIndex(List<int> list)
            {
                int max = 0, maxIndex = -1;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] > max)
                    {
                        max = list[i];
                        maxIndex = i;
                    }
                }
                return maxIndex;
            }
            int[] arr ={
                getMaxIndex(pawns.Select(x => x.totalDistance).ToList()),
                getMaxIndex(pawns.Select(x => x.totalDecrement).ToList()),
                getMaxIndex(pawns.Select(x => x.snakeCount).ToList()),
                getMaxIndex(pawns.Select(x => x.eventCount).ToList()),
                getMaxIndex(pawns.Select(x => x.sameDiceCount).ToList()),
            };
            return arr;
        }
        var transmitData = new TransmitData() { 
            code = _recieveData.code,
            stringRepay = automataManager.AllString,
            walkDistance = pawns.Select(x => x.currentPosition).ToArray(),
            reward = getReward(),
            
        };
        string jsonStr = JsonUtility.ToJson(transmitData);
        ShowMessage(jsonStr);
    }
    public void Summarize()
    {
        OpenSummarize(recieveData.code);
    }
    public class RecieveData{
        public string code;
        public string[] pawnColor;
        public string map;
    }
    public class TransmitData
    {
        public string code = "CODEEEE";
        public string stringRepay = "REPLAYYYY";
        public int[] walkDistance = { 0, 1, 2, 3, 6, 5 };
        public int[] reward = { 5, 3, 5, 3, 5, 3, 5, 3, 5, 3 };
    }
    #endregion
}
