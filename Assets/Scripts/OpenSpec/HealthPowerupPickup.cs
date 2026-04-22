using System.Collections;
using Combat;
using UnityEngine;

namespace OpenSpec
{
    [AddComponentMenu("OpenSpec/Health Powerup Pickup")]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class HealthPowerupPickup : MonoBehaviour
    {
        private string pickupId;
        private float healAmount;
        private float respawnSeconds;
        private bool available = true;
        private SpriteRenderer spriteRenderer;
        private CircleCollider2D trigger;

        public void Configure(string id, float heal, float respawn, float radius)
        {
            pickupId = string.IsNullOrEmpty(id) ? System.Guid.NewGuid().ToString("N") : id;
            healAmount = Mathf.Max(1f, heal);
            respawnSeconds = Mathf.Max(1f, respawn);

            if (trigger == null) trigger = GetComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = Mathf.Clamp(radius, 0.2f, 2f);
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            trigger = GetComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            EnsureHeartSprite();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!available) return;
            if (other == null) return;

            var health = other.GetComponentInParent<Health>();
            if (health == null) return;

            health.Heal(healAmount);
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            available = false;
            spriteRenderer.enabled = false;
            trigger.enabled = false;
            yield return new WaitForSeconds(respawnSeconds);
            available = true;
            spriteRenderer.enabled = true;
            trigger.enabled = true;
        }

        private void EnsureHeartSprite()
        {
            if (spriteRenderer.sprite != null) return;

            var tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    bool inHeart =
                        ((x - 4) * (x - 4) + (y - 11) * (y - 11) <= 10) ||
                        ((x - 11) * (x - 11) + (y - 11) * (y - 11) <= 10) ||
                        (y <= 11 && y >= 2 && x >= (8 - (y - 2)) && x <= (8 + (y - 2)));
                    tex.SetPixel(x, y, inHeart ? new Color(0.95f, 0.2f, 0.3f, 1f) : Color.clear);
                }
            }
            tex.Apply();
            spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 32f);
            spriteRenderer.sortingOrder = 80;
        }
    }
}
