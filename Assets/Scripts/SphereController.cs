using UnityEngine;

public class SphereController : MonoBehaviour
{
    public float speed = 10f;
    public float jumpForce = 20f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;

    // Private variables
    private Rigidbody rb;
    private Renderer sphereRenderer;
    private bool isGrounded = true;
    private int currentLevel = 0;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereRenderer = GetComponent<Renderer>();
        mainCamera = Camera.main;
        ChangeColor();
    }

    void Update()
    {
        HandleMovement();
        HandleJumping();
        HandleShooting();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0.0f, vertical);

        rb.MovePosition(transform.position + movement * speed * Time.deltaTime);

        if (movement != Vector3.zero)
        {
            transform.forward = movement;
        }
    }

    void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(100f);

            ShootProjectile(targetPoint);
        }
    }

    void ShootProjectile(Vector3 targetPoint)
    {
        if (projectilePrefab != null)
        {
            Vector3 spawnPosition = transform.position + transform.forward * 1f + Vector3.up * 0.5f;
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            Vector3 direction = (targetPoint - spawnPosition).normalized;
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

            if (projectileRb != null)
            {
                projectileRb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
            }

            Destroy(projectile, 3f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;

            // Change both sphere and platform colors
            int newLevel = Mathf.RoundToInt(collision.transform.position.y);
            if (newLevel != currentLevel)
            {
                currentLevel = newLevel;
                ChangeColor();

                // Change platform color
                Renderer platformRenderer = collision.gameObject.GetComponent<Renderer>();
                if (platformRenderer != null)
                {
                    Color randomPlatformColor = new Color(Random.value, Random.value, Random.value);
                    platformRenderer.material.color = randomPlatformColor;
                }
            }
        }
    }

    void ChangeColor()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        sphereRenderer.material.color = randomColor;
    }
}