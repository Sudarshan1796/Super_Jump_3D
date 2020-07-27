using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    public Transform lookAt;
    public Transform moveTo;
    public float chaseTime = 0.5f;
    public float lookAtHeight = 6f;
    public float moveToHeight = 25f;

    Vector3 mVelocity;
    Vector3 tempLookAtPos;
    Vector3 tempMoveToPos;
    Vector3 mRollVelocity;

#if UNITY_EDITOR
    //void Update()
    //{
    //    if (!Application.isPlaying)
    //    {
    //        if (moveTo)
    //        {
    //            tempMoveToPos.x = moveTo.position.x;
    //            tempMoveToPos.y = moveTo.position.y;//moveToHeight;
    //            tempMoveToPos.z = moveTo.position.z;
    //            transform.position = tempMoveToPos;
    //        }
    //        if (lookAt)
    //        {
    //            tempLookAtPos.x = lookAt.position.x;
    //            tempLookAtPos.y = lookAt.position.y;//lookAtHeight;
    //            tempLookAtPos.z = lookAt.position.z;
    //            transform.LookAt(tempLookAtPos);
    //        }
    //    }
    //}
#endif

    void LateUpdate()
    {
        if (moveTo)
        {
            tempMoveToPos.x = moveTo.position.x;
            tempMoveToPos.y = moveToHeight;
            tempMoveToPos.z = moveTo.position.z;
            transform.position = Vector3.SmoothDamp(transform.position, tempMoveToPos, ref mVelocity, chaseTime);
        }
        if (lookAt)
        {
            tempLookAtPos.x = lookAt.position.x;
            tempLookAtPos.y = lookAtHeight;
            tempLookAtPos.z = lookAt.position.z;
            transform.LookAt(tempLookAtPos);
        }
    }
}
