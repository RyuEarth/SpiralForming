//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Shot : MonoBehaviour
//{
//    public List<int> ShotKnd = new List<int>() { 1, 2, 3, 4, 5 };
//    int iterator;
//    int nextShotKnd;
//    int shotStatus;
//    int shotKndCmb;
//    int shotKndChild;

//    private List<ShotInfo> shotInfoList = new List<ShotInfo>();
//    void IniShotChild(int shotKndChild)
//    {
//        ShotInfo sInfo;
//        switch (shotKndChild)
//        {
//            case 0:
//                sInfo = enterShotChild00();
//                break;
//        }
//        shotInfoList.Add(sInfo);
//    }

//    ShotInfo enterShotChild00()
//    {
//        ShotInfo shotInfo = new ShotInfo();
//        float interval = 0f;
//        interval += Time.deltaTime;
//        if(interval >= 2f)
//        {
//            interval = 0f;
//            for(int i = 0; i < 10; i++)
//            {
//                shotInfo.bulletInfo.Add(new BulletInfo());
//            }
//        }
        
//        for(int i = 0; i < shotInfo.bulletInfo.Count; i++)
//        {
//            if (shotInfo.bulletInfo[i].time == 0)
//            {
                
//            }
//        //    if ((*mvBullet)[i].mCount == 0)
//        //    {
//        //        (*mvBullet)[i].mAngle = PI2 / 10 * i;
//        //        (*mvBullet)[i].mCount = 0;
//        //        (*mvBullet)[i].mFlag = 1;
//        //        (*mvBullet)[i].mTill = 0;
//        //        (*mvBullet)[i].mVec->x = mVec->x;
//        //        (*mvBullet)[i].mVec->y = mVec->y;
//        //        (*mvBullet)[i].mVel = 2.0;
//        //        (*mvBullet)[i].mCol = 2;
//        //        (*mvBullet)[i].mBulletAngle = (*mvBullet)[i].mAngle;
//        //        (*mvBullet)[i].mKnd = 1;
//        //    }
//        //    else if ((*mvBullet)[i].mCount < 60)
//        //    {
//        //        (*mvBullet)[i].mVel += 0.01;
//        //        (*mvBullet)[i].mAngle -= 0.02;
//        //        (*mvBullet)[i].mBulletAngle = (*mvBullet)[i].mAngle;
//        //    }
//        //    else
//        //    {
//        //        (*mvBullet)[i].mVel += 0.01;
//        //        (*mvBullet)[i].mAngle += 0.02;
//        //        (*mvBullet)[i].mBulletAngle = (*mvBullet)[i].mAngle;
//        //    }
//        //(*mvBullet)[i].mCount++;
//        }
//        return shotInfo;
//    } 
//    void enterShotCmb()
//    {
//        for (int i = 0; i < shotKndChildList.size(); i++)
//        {
//            IniShotChild(shotKndChildList(i));
//        }

//        switch (shotKndCmb)
//        {
//            case 0:

//                break;
//        }

//        createShot()
//    }

//    // Start is called before the first frame update
//    void Start()
//    {



//    }

//    // Update is called once per frame
//    void Update()
//    {
//        int finishTime = Time.time +

//        if (Time.time + shotTime > ShotKnd.time)
//        {
//            Time.erapsedTime = 0f;
//            ShotKndList++;
//            ShotKndInitialize();
//        }
//    }

//    //現在のショット情報
//    struct ShotInfo
//    {
//        public int knd;
//        public float finishTime;
//        public float nowTime;
//        public int blltKnd;
//        public int blltNum;
//        public List<BulletInfo> bulletInfo;

//        public ShotInfo(ShotInfo shotInfo)
//        {
//            this.knd = shotInfo.knd;
//            this.nowTime = shotInfo.nowTime;
//            this.blltKnd = shotInfo.blltKnd;
//            this.blltNum = shotInfo.blltNum;
//            this.finishTime = shotInfo.finishTime;
//            this.bulletInfo = new List<BulletInfo>();
//        }
//    }

//    struct BulletInfo
//    {
//        public float time;
//        public float fwAngle;
//        public bool flag;
//        public float till;
//        public Transform transform;
//        public Vector2 vel;
//        public int col;
//        public float blltAngle;
//        public int knd;

//    }
//}
