using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =nameof(EventModelTable),menuName =nameof(EventModelTable))]
public class EventModelTable : ScriptableObject
{
    public List<EventModel> events;

}
