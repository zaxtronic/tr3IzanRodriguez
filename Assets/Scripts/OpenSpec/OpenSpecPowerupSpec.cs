using System;
using UnityEngine;

namespace OpenSpec
{
    [Serializable]
    public class OpenSpecPowerupSpec
    {
        public PowerupEntry[] powerups;

        [Serializable]
        public class PowerupEntry
        {
            public string id;
            public float x;
            public float y;
            public float healAmount;
            public float respawnSeconds;
            public float radius;
        }

        public static OpenSpecPowerupSpec LoadFromResources()
        {
            var text = Resources.Load<TextAsset>("OpenSpec/powerups");
            if (text == null || string.IsNullOrWhiteSpace(text.text))
            {
                return null;
            }

            try
            {
                return JsonUtility.FromJson<OpenSpecPowerupSpec>(text.text);
            }
            catch (Exception ex)
            {
                Debug.LogError("OpenSpec powerups invalid JSON: " + ex.Message);
                return null;
            }
        }
    }
}
