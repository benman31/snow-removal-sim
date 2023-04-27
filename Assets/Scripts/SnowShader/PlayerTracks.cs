/*
 * Author: Benjamin Enman, 97377
 * Based loosley on the guide by PeerPlay: https://youtu.be/h_OjBBWoJ-Q
 */

using UnityEngine;

public class PlayerTracks : MonoBehaviour
{
    // The distance at which the player should chage to the other foot
    private const float STRIDE_DISTANCE = 0.075f;
    // The distance the foot should "drag" on the ground before being "lifted"
    private const float FOOT_UP_DISTANCE = 0.05f;

    private RenderTexture splatMap;
    public Shader drawShader;

    private Material brushMaterial;
    private Material splatMaterial;

    public GameObject terrain;
    public Transform playerTransform;
    public Transform leftFoot;
    public Transform rightFoot;

    private Vector3 prevPos;
    private float distanceSquared;
    private bool hasMoved;
    private bool leftFootDown =  true;
    private bool rightFootDown = false;
    private bool footUp = false;

    RaycastHit groundHit;
    int layerMask;

    [Range(1, 500)]
    public float brushSize;
    [Range(0, 1)]
    public float brushStrength;

    private bool playerIsGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Ground");
        brushMaterial = new Material(drawShader);
        brushMaterial.SetVector("_Color", Color.red);

        splatMaterial = WorldGenerator.snowMat;
        splatMaterial.SetTexture("_Splat", splatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat));

        prevPos.x = playerTransform.position.x;
        prevPos.y = playerTransform.position.y;
        prevPos.z = playerTransform.position.z;

        hasMoved = false;

        PlayerMovement.OnPlayerGrounded += this.HandlePlayerGrounded; 
    }

    private void OnDestroy()
    {
        PlayerMovement.OnPlayerGrounded -= this.HandlePlayerGrounded;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevPos.x == playerTransform.position.x && prevPos.y == playerTransform.position.y && prevPos.z == playerTransform.position.z)
        {
            hasMoved = false;
            leftFootDown = true;
            rightFootDown = true;
            distanceSquared = 0;
        }
        else if (!hasMoved)
        {
            hasMoved = true;
            leftFootDown = !rightFootDown;

        }

        if (distanceSquared > STRIDE_DISTANCE)
        {
            leftFootDown = !leftFootDown;
            rightFootDown = !rightFootDown;
            distanceSquared = 0;
            footUp = false;
        } 
        else if (distanceSquared > FOOT_UP_DISTANCE)
        {
            footUp = true;
        }

        if (hasMoved && playerIsGrounded && !footUp)
        { 
            if (leftFootDown && Physics.Raycast(leftFoot.position, Vector3.down, out groundHit, 5f, layerMask))
            {
                brushMaterial.SetVector("_Coordinate", new Vector4(groundHit.textureCoord.x, groundHit.textureCoord.y, 0, 0));
                brushMaterial.SetFloat("_Strength", brushStrength);
                brushMaterial.SetFloat("_Size", brushSize);
                RenderTexture temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(splatMap, temp);
                Graphics.Blit(temp, splatMap, brushMaterial);
                RenderTexture.ReleaseTemporary(temp);
            }
            if (rightFootDown && Physics.Raycast(rightFoot.position, Vector3.down, out groundHit, 5f, layerMask))
            {
                brushMaterial.SetVector("_Coordinate", new Vector4(groundHit.textureCoord.x, groundHit.textureCoord.y, 0, 0));
                brushMaterial.SetFloat("_Strength", brushStrength);
                brushMaterial.SetFloat("_Size", brushSize);
                RenderTexture temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(splatMap, temp);
                Graphics.Blit(temp, splatMap, brushMaterial);
                RenderTexture.ReleaseTemporary(temp);
            }
        }

        this.distanceSquared += Vector3.SqrMagnitude(playerTransform.position - prevPos);
        prevPos.Set(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);
    }

    private void HandlePlayerGrounded(bool playerGrounded)
    {
        this.playerIsGrounded = playerGrounded;
    }
}
