using UnityEngine;

namespace Network
{
    [AddComponentMenu("Network/Remote Player View")]
    public class RemotePlayerView : MonoBehaviour
    {
        private static Sprite defaultSprite;
        private SpriteRenderer spriteRenderer;
        private TextMesh nameText;
        private Vector3 targetPosition;
        [SerializeField] private float positionSmooth = 12f;

        public string UserId { get; private set; }

        public void Initialize(string userId, string displayName, Color color)
        {
            UserId = userId;
            EnsureSpriteRenderer();
            spriteRenderer.color = color;
            transform.localScale = Vector3.one;
            EnsureNameText();
            nameText.text = displayName;
            targetPosition = transform.position;
        }

        public void ApplyState(float x, float y, string displayName, string dir = null)
        {
            SetNetworkState(x, y, dir);
            if (!string.IsNullOrEmpty(displayName))
            {
                EnsureNameText();
                nameText.text = displayName;
            }
        }

        public void SetNetworkState(float x, float y, string dir = null)
        {
            targetPosition = new Vector3(x, y, 0f);
            ApplyFacing(dir);
        }

        public void SetPosition(float x, float y)
        {
            SetNetworkState(x, y);
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, UnityEngine.Time.deltaTime * positionSmooth);
        }

        private void EnsureSpriteRenderer()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            if (defaultSprite == null)
            {
                var tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
                var pixels = new Color[16 * 16];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
                tex.SetPixels(pixels);
                tex.Apply();
                defaultSprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 32f);
            }

            spriteRenderer.sprite = defaultSprite;
            spriteRenderer.sortingOrder = 50;
        }

        private void EnsureNameText()
        {
            if (nameText != null) return;

            var child = new GameObject("Name");
            child.transform.SetParent(transform);
            child.transform.localPosition = new Vector3(0f, 0.6f, 0f);

            nameText = child.AddComponent<TextMesh>();
            nameText.text = "Player";
            nameText.characterSize = 0.15f;
            nameText.alignment = TextAlignment.Center;
            nameText.anchor = TextAnchor.MiddleCenter;
            nameText.color = Color.white;
        }

        private void ApplyFacing(string dir)
        {
            if (string.IsNullOrEmpty(dir)) return;

            if (dir == "left")
            {
                if (spriteRenderer != null) spriteRenderer.flipX = true;
                return;
            }

            if (dir == "right")
            {
                if (spriteRenderer != null) spriteRenderer.flipX = false;
            }
        }
    }
}
