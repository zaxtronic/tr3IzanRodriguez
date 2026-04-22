using Event.Events;
using Referencing.Scriptable_Assets;
using UnityEngine;
using World;

namespace GameActions.Actions
{
    [CreateAssetMenu(menuName = "Actions/Warp Player")]
    public class ActionWarpPlayer : ScriptableAsset
    {
        [SerializeField]
        private WarpEvent warpRequest;

        public void Execute(WarpLocation targetLocation)
        {
            warpRequest.Invoke(targetLocation);
        }
    }
}
