using UnityEngine;

public class finalBossAI : MonoBehaviour
{
    [SerializeField] Renderer mModel;
    [SerializeField] Transform headPos;
    [SerializeField] Transform mTurretTop;
    [SerializeField] Transform mShootPos;
    [SerializeField] GameObject mBullet;
    [SerializeField] Transform mBarrel;
  //  [SerializeField] Animator animator;


    [Header("===== Stats =====")]
    [SerializeField][Range(0,1)] float mFaceTargetSpeed;
    [SerializeField][Range(0, 180)] int mFOV;
    [SerializeField][Range(0, 45)] int mShootFOV;
    [SerializeField] float mShootRate;

    [SerializeField] LineRenderer mLaserLine;
    [SerializeField] float mLockOnTime;
    [SerializeField] float mFireDelay;
    [SerializeField] GameObject mExplosionPrefab;

    float mShootTimer;
    Vector3 mPlayerDir;
    float mAngleToPlayer;

    Vector3 mLockedTargetPos;
    bool mIsLockingOn;
    float mLockTimer;

    void Update()
    {
        CanSeePlayer();

        if (!mIsLockingOn)
        {
            RotateTurret(); // track player before lock
        }
        else
        {
            RotateToLockedPosition(); // keep looking at locked point
        }

        if (mIsLockingOn)
        {
            mLockTimer += Time.deltaTime;

            // Keep laser aimed
            mLaserLine.SetPosition(0, mShootPos.position);
            mLaserLine.SetPosition(1, mLockedTargetPos);

            if (mLockTimer >= mFireDelay)
            {
                FireAtLockedPosition();
                mIsLockingOn = false;
                mLaserLine.enabled = false;
                mShootTimer = 0;
            }
        }
    }

    void RotateTurret()
    {
        if (gameManager.instance == null) return;

        Transform player = gameManager.instance.player.transform;

        // -------- Turret Yaw (Horizontal) --------
        Vector3 turretDir = (player.position - mTurretTop.position);
        turretDir.y = 0; // ignore vertical
        if (turretDir != Vector3.zero)
        {
            Quaternion yawRotation = Quaternion.LookRotation(turretDir);
            mTurretTop.rotation = Quaternion.Lerp(mTurretTop.rotation, yawRotation, Time.deltaTime * mFaceTargetSpeed);
        }

        // -------- Barrel Pitch (Vertical) --------
        Vector3 barrelDir = (player.position - mBarrel.position).normalized;
        Quaternion pitchRotation = Quaternion.LookRotation(barrelDir);

        // Only pitch rotation (X axis), keep turret’s current Y
        mBarrel.localRotation = Quaternion.Lerp(mBarrel.localRotation, Quaternion.Euler(pitchRotation.eulerAngles.x, 0, 0), Time.deltaTime * mFaceTargetSpeed);
    }


    bool CanSeePlayer()
    {
        mPlayerDir = gameManager.instance.player.transform.position - headPos.position;
        mAngleToPlayer = Vector3.Angle(new Vector3(mPlayerDir.x, 0, mPlayerDir.z), transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, mPlayerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && mAngleToPlayer <= mFOV)
            {
                mShootTimer += Time.deltaTime;

                if (mAngleToPlayer <= mShootFOV && mShootTimer >= mShootRate)
                {
                    if (mAngleToPlayer <= mShootFOV && mShootTimer >= mShootRate)
                    {
                        TryLockAndShoot();
                    }
                }

                return true;
            }
        }

        return false;
    }

    void Shoot()
    {
      
        CreateBullet();
    }

    public void CreateBullet()
    {
        Instantiate(mBullet, mShootPos.position, mBarrel.rotation);
    }

    void TryLockAndShoot()
    {
        if (!mIsLockingOn)
        {
            mLockedTargetPos = gameManager.instance.player.transform.position;
            mIsLockingOn = true;
            mLockTimer = 0;

            // Show the laser line
            mLaserLine.enabled = true;
            mLaserLine.SetPosition(0, mShootPos.position);
            mLaserLine.SetPosition(1, mLockedTargetPos);
        }
    }

    void FireAtLockedPosition()
    {
      //  animator.SetTrigger("shoot");

        // You can instantiate a bullet OR do an instant explosion
        Instantiate(mExplosionPrefab, mLockedTargetPos, Quaternion.identity);

    }

    void RotateToLockedPosition()
    {
        // -------- Turret Yaw (Horizontal) --------
        Vector3 turretDir = mLockedTargetPos - mTurretTop.position;
        turretDir.y = 0;
        if (turretDir != Vector3.zero)
        {
            Quaternion yawRotation = Quaternion.LookRotation(turretDir);
            mTurretTop.rotation = Quaternion.Lerp(mTurretTop.rotation, yawRotation, Time.deltaTime * mFaceTargetSpeed);
        }

        // -------- Barrel Pitch (Vertical) --------
        Vector3 barrelDir = mLockedTargetPos - mBarrel.position;
        Quaternion pitchRotation = Quaternion.LookRotation(barrelDir);

        mBarrel.localRotation = Quaternion.Lerp(
            mBarrel.localRotation,
            Quaternion.Euler(pitchRotation.eulerAngles.x, 0, 0),
            Time.deltaTime * mFaceTargetSpeed
        );
    }

}
