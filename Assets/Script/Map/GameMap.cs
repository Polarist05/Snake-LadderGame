using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMap 
{
    public MapModel _model;
    public MapModel model {
        get => _model;
        set {
            actions.Clear();
            if (value != null)
            {
                System.Random random = new System.Random();
                actions.AddRange(_model.nonEventAction);
                actions.AddRange(value.eventPostion.Select(i => {
                    var model = GameManager.instance.gameEventTable.events;
                    return new GameAction(model[random.Next(model.Count)], i);
                }));
            }
            _model = value;
        }
    }
    private List<GameAction> actions = new List<GameAction>();

}
