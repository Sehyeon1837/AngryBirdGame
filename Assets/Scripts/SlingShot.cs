using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.Serialization;

public class SlingShot : MonoBehaviour
{
    public LineRenderer[] lineRenderers;
    public Transform[] stripPositions;
    public Transform center;
    public Transform idlePosition;
    
    public Vector3 currentPosition;
    public float maxLength;
    public float bottomBoundary;
    
    public float Power = 100.0f;
    public float Mass = 10.0f;
    
    private Vector2 startDragPosition; // 드래그 시작 위치
    private Vector2 endDragPosition;   // 드래그 끝 위치
    
    public float birdPositionOffset;
    public GameObject birdPrefab;
    
    Rigidbody2D birdRigidBody;
    Collider2D birdCollider;

    private bool isMouseDown;
    
    public AudioClip shootSound;
    private AudioSource audioSource;
    
    void Start()
    {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);
        
        audioSource = GetComponent<AudioSource>();
        CreateBird();
    }

    void CreateBird()
    {
        birdRigidBody = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = birdRigidBody.GetComponent<Collider2D>();
        birdCollider.enabled = false;
        birdRigidBody.isKinematic = true;
        
        ResetStrips();
    }

    void Update()
    {
        if (isMouseDown)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;
            
            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            currentPosition = center.position + Vector3.ClampMagnitude(
                currentPosition - center.position, maxLength); // 최대 strip 길이 제한

            currentPosition = ClampBoundary(currentPosition);
            
            SetStrips(currentPosition);

            if (birdCollider)
            {
                birdCollider.enabled = true;
            }
            
            endDragPosition = birdRigidBody.position;
            Vector2 dragDirection = endDragPosition - startDragPosition; // 방향 계산
            float distance = dragDirection.magnitude; // 드래그 거리
            Vector2 force = dragDirection.normalized * distance * Power * -1;

            UpdateTrajectory(force);
            
            foreach (var o in Objects)
            {
                o.SetActive(false);
            }
        
            List<Vector2> trajectorys2 = PredictTrajectory(force, Mass);

            if (Objects.Count == trajectorys2.Count)
            {
                for (var index = 0; index < trajectorys2.Count; index++)
                {
                    var trajectory = trajectorys2[index];
                    Objects[index].SetActive(true);
                    Objects[index].transform.position = trajectory;
                }
            }
        }
        else
        {
            ResetStrips();
        }
    }

    // 영역(Collision) 안에서만 체크
    private void OnMouseDown()
    {
        isMouseDown = true;
        startDragPosition = birdRigidBody.position; // Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
        endDragPosition = birdRigidBody.position;
        Shoot();
        currentPosition = idlePosition.position;
        
        foreach (var o in Objects)
        {
            Destroy(o, 1f);
        }
            
        Objects.Clear();
    }
    
    void Shoot()
    {
        birdRigidBody.isKinematic = false;
        birdRigidBody.mass = Mass;
        Vector2 dragDirection = endDragPosition - startDragPosition; // 방향 계산
        float distance = dragDirection.magnitude; // 드래그 거리
        Vector2 force = dragDirection.normalized * distance * Power * -1; // x가 - 방향으로 이동해서 -1 곱하기
        birdRigidBody.AddForce(force, ForceMode2D.Impulse);
        
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        birdRigidBody = null; // 비우기
        birdCollider = null;
        Invoke("CreateBird", 2);
    }

    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }

    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if (birdCollider)
        {
            Vector3 dir = position - center.position;
            dir.Normalize();
            birdRigidBody.transform.position = position + dir * birdPositionOffset;
            birdRigidBody.transform.right = -dir;
        }
    }

    Vector3 ClampBoundary(Vector3 vector) // 특정 범위를 벗어나는 값을 자동으로 조정
    {
        vector.y = Mathf.Clamp(vector.y, bottomBoundary, 1000);
        return vector;
    }
    
    // 궤도
    public int maxStep = 6;
    public float timeStep = 0.1f;
    
    public GameObject TrajectoryObj;
    
    public List<GameObject> Objects = new List<GameObject>();

    List<Vector2> PredictTrajectory(Vector2 force, float mass)
    {
        List<Vector2> trajectory = new List<Vector2>();
        
        Vector2 position = transform.position;
        Vector2 velocity = force / mass;

        trajectory.Add(position);

        for (int i = 1; i <= maxStep; i++)
        {
            float timeElapsed = timeStep * i;
            // 등가속도 운동
            trajectory.Add(position + 
                           velocity * timeElapsed + 
                           Physics2D.gravity * (0.5f * timeElapsed * timeElapsed));

            if (CheckCollision(trajectory[i - 1], trajectory[i], out Vector2 hitPoint))
            {
                trajectory[i] = hitPoint;
                break;
            }
        }

        return trajectory;
    }
    
    private bool CheckCollision(Vector2 start, Vector2 end, out Vector2 hitPoint)
    {
        hitPoint = end;
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        
        // 3D -> 2D
        if (Physics.Raycast(start, direction.normalized, out RaycastHit hit, distance, 1 << LayerMask.NameToLayer("Default")))
        {
            hitPoint = hit.point;
            return true;
        }
        
        return false;
    }

    private void UpdateTrajectory(Vector2 force)
    {
        List<Vector2> trajectorys = PredictTrajectory(force, Mass);

        foreach (var o in Objects)
        {
            Destroy(o);
        }
            
        Objects.Clear();
            
        foreach (var trajectory in trajectorys)
        {
            var go = Instantiate(TrajectoryObj, trajectory, Quaternion.identity);
            Objects.Add(go);
        }
        
        Objects[0].SetActive(false);
    }
}
