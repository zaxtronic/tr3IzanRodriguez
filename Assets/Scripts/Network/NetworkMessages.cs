using System;
using UnityEngine;

namespace Network
{
    [Serializable]
    public class BaseMsg { public string type; }

    [Serializable]
    public class JoinMsg : BaseMsg
    {
        public string gameId;
        public string userId;
        public string name;
    }

    [Serializable]
    public class MoveMsg : BaseMsg
    {
        public string userId;
        public float x;
        public float y;
        public Vector2 dir;
    }

    [Serializable]
    public class ActionMsg : BaseMsg
    {
        public string userId;
        public string action;
        public string data;
    }

    [Serializable]
    public class PlayerStateData
    {
        public float x;
        public float y;
        public string name;
        public Vector2 dir;
    }

    [Serializable]
    public class SnapshotPlayer
    {
        public string userId;
        public PlayerStateData state;
    }

    [Serializable]
    public class SnapshotMsg : BaseMsg
    {
        public SnapshotPlayer[] players;
    }

    [Serializable]
    public class PlayerJoinedMsg : BaseMsg
    {
        public string userId;
        public PlayerStateData state;
    }

    [Serializable]
    public class PlayerLeftMsg : BaseMsg
    {
        public string userId;
    }
}
