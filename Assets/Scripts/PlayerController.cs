using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    [SerializeField] private Rigidbody rb;
    [SerializeField] private new AudioSource audio;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpForce = 7f;
    
    private Vector3 input;
    private bool canCollide = true;
    
    private void Update() {
        GatherInput();
        if(Input.GetButtonDown("Jump") && IsGrounded() && !PauseMenu.isPaused) {
            Jump();
        }
    }

    private void FixedUpdate() {
        Move();
        FixPosition();
    }

    private void GatherInput() {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void Move() {
        if (PauseMenu.isPaused) return;
        Transform t = transform;
        Vector3 result = t.position + speed * Time.deltaTime * (t.forward * input.z + t.right * input.x);
        rb.MovePosition(result);
        canCollide = true;
    }

    private void FixPosition() {
        Transform t = transform;
        Vector3 pos = t.position;
        float maxZ = 6.2f;
        float minZ = -6.2f;
        if (pos.z > maxZ) {
            t.position = new Vector3(pos.x, pos.y, maxZ);
        } else if (t.position.z < minZ) {
            t.position = new Vector3(pos.x, pos.y, minZ);
        }
    }
    
    private void Jump() {
        rb.velocity = Vector3.up * jumpForce;
    }
    
    private bool IsGrounded() {
        Transform t = transform;
        Vector3 pos = t.position;
        Vector3 scale = t.localScale;
        float height = scale.y;
        float width = scale.x;
        return Physics.Raycast(pos, Vector3.down, 0.1f + height / 2)
            || Physics.Raycast(pos + Vector3.right * height / 2, Vector3.down, 0.1f + height / 2)
            || Physics.Raycast(pos - Vector3.right * width / 2, Vector3.down, 0.1f + height / 2)
            || Physics.Raycast(pos + Vector3.forward * width / 2, Vector3.down, 0.1f + height / 2)
            || Physics.Raycast(pos - Vector3.forward * width / 2, Vector3.down, 0.1f + height / 2);
            
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Coin")) {
            audio.clip = Resources.Load<AudioClip>("Audio/CoinSound");
            audio.Play();
            ScoreController.score += 1;
            Destroy(other.gameObject);
        } else if(other.gameObject.layer == LayerMask.NameToLayer("Spike") && canCollide) {
            audio.clip = Resources.Load<AudioClip>("Audio/DeathSound");
            audio.Play();
            rb.transform.position = new Vector3(-5, 1, 0);
            ScoreController.score = 0;
            ScoreController.deaths += 1;
            GameController.currentLevel.RegenerateCoins();
            canCollide = false;
        }
    }
}