namespace Blobber
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Handles player movement through map.
    /// </summary>
    public class Avatar : MonoBehaviour
    {
        private const float MoveForwardDuration = 0.25f;
        private const float RotateDuration = 0.15f;
        private const int TurnsBetweenSeasons = 160;

        private Map map { get { return Gameplay.instance.map; } }

        private Queue<KeyCode> inputBuffer = new Queue<KeyCode>();
        private bool isMoving = false;

        public int turnsSinceSeason = 0;

        public Vector2 mapPosition;

        public static Avatar instance;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (Gameplay.isPaused)
            {
                inputBuffer.Clear();
                return;
            }
            if (inputBuffer.Count > 0 && !isMoving)
            {
                var nextInput = inputBuffer.Dequeue();
                switch (nextInput)
                {
                    case KeyCode.UpArrow:
                        TryMoveForward(false);
                        break;
                    case KeyCode.LeftArrow:
                        RotateLeft();
                        break;
                    case KeyCode.RightArrow:
                        RotateRight();
                        break;
                }
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (isMoving) inputBuffer.Enqueue(KeyCode.UpArrow);
                else TryMoveForward(false);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (isMoving) inputBuffer.Enqueue(KeyCode.LeftArrow);
                else RotateLeft();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (isMoving) inputBuffer.Enqueue(KeyCode.RightArrow);
                else RotateRight();
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                if (!isMoving) TryMoveForward(true);
            }
        }

        public void RotateLeft()
        {
            StartCoroutine(Rotate(-90));
        }

        public void RotateRight()
        {
            StartCoroutine(Rotate(90));
        }

        IEnumerator Rotate(float angle)
        {
            isMoving = true;
            var oldRot = transform.rotation;
            var oldEuler = oldRot.eulerAngles;
            var newRot = Quaternion.Euler(oldEuler.x, oldEuler.y + angle, oldEuler.z);
            float elapsed = 0;
            while (elapsed < RotateDuration)
            {
                transform.rotation = Quaternion.Slerp(oldRot, newRot, elapsed / MoveForwardDuration);
                yield return null;
                elapsed += Time.deltaTime;
            }
            transform.rotation = newRot;
            isMoving = false;
            CheckAhead();
        }

        public void TryMoveForward(bool kicking)
        {
            // Compute new position:
            var newPos = transform.position + transform.forward;
            int x = Mathf.RoundToInt(-newPos.x);
            int y = Mathf.RoundToInt(newPos.z);

            // Get tile in new position, aborting if tile is not open:
            var tile = MapAssets.instance.GetTile(map.GetTileID(x, y));
            if (!tile.open) return;

            // Check for content in new position, kicking kickables and other aborting if content is there.
            var index = map.GetIndex(x, y);
            var tileContentGameObject = Gameplay.instance.contentGameObjects[index];
            if (tileContentGameObject != null)
            {
                var kickable = tileContentGameObject.GetComponentInChildren<Kickable>();
                if (kickable != null)
                {
                    if (kicking)
                    {
                        Effects.Flash(kickable.kickColor, kickable.kickAudioClip);
                        Destroy(tileContentGameObject);
                        Gameplay.instance.contentGameObjects[index] = null;
                    }
                    else
                    {
                        return;
                    }
                }
                else if (tileContentGameObject.GetComponentInChildren<Encounter>() != null)
                {
                    return;
                }
            }
            StartCoroutine(MoveForward(newPos));
        }

        IEnumerator MoveForward(Vector3 newPos)
        {
            // Move:
            isMoving = true;
            var oldPos = transform.position;
            float elapsed = 0;
            while (elapsed < MoveForwardDuration)
            {
                transform.position = Vector3.Lerp(oldPos, newPos, elapsed / MoveForwardDuration);
                yield return null;
                elapsed += Time.deltaTime;
            }
            transform.position = newPos;
            isMoving = false;

            CheckAhead();
        }

        public void CheckAhead()
        {
            // Handle whatever is ahead:
            var aheadPos = transform.position + transform.forward;
            var x = Mathf.RoundToInt(-aheadPos.x);
            var y = Mathf.RoundToInt(aheadPos.z);
            Gameplay.instance.HandleEncounter(x, y);
            if (!Gameplay.instance.combatPanel.gameObject.activeInHierarchy)
            {
                IncrementTurn();
            }
        }

        public void IncrementTurn()
        {
            turnsSinceSeason++;
            if (turnsSinceSeason >= TurnsBetweenSeasons)
            {
                turnsSinceSeason = 0;
                Gameplay.instance.CombineDNA();
            }
        }
    }
}