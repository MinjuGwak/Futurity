 using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPoolManager<PoolingClass> : OBJPoolParent where PoolingClass : Component
{
    private List<PoolingClass> activedPoolingObjects = new List<PoolingClass>();
	private Stack<PoolingClass> nonActivedObjects =  new Stack<PoolingClass>();

	public List<PoolingClass> ActivePoolingObjects => activedPoolingObjects;

    [SerializeField] private int activeObjCount;

    public ObjectPoolManager(GameObject prefab, GameObject parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        activeObjCount = 0;
    }

	public void SetManager(GameObject prefab, GameObject parent = null)
	{
		this.prefab = prefab;
		this.parent = parent;
		activeObjCount = 0;
	}

    public PoolingClass ActiveObject(Vector3? startPos = null, Quaternion? startRot = null)
    {
		PoolingClass returnValue = null;
		startPos = startPos ?? Vector3.zero;
		startRot = startRot ?? Quaternion.identity;

		// 비활성화된 오브젝트가 남아있다면
		if (nonActivedObjects.Count > 0)
        {
			PoolingClass obj = nonActivedObjects.Pop();

            // 오브젝트가 존재하고 비활성화 상태라면
            if (obj != null)
            {
                // PoolingObject일 경우
                if (obj.GetType() == typeof(PoolingObject))
                {
                    // PoolingObject로 변환
                    PoolingObject poolObj;
                    obj.TryGetComponent(out poolObj);

                    // 변환 성공시
                    if (poolObj != null)
                    {
                        poolObj.ActiveObj(); // Active시 발동되는 작업
                    }
                }
                // 활성화
                obj.gameObject.SetActive(true);
				returnValue = obj;
            }
        }
        else
        {
            // 새 오브젝트 생성
            GameObject newObj = GameObject.Instantiate(prefab, parent == null ? null : parent.transform);

            PoolingObject poolObj;
            newObj.TryGetComponent(out poolObj);

            // PoolingObject일 경우
            if (poolObj != null)
            {
                // 에러 체크로 오류가 아마 발생 안할 듯?
                dynamic poolManager = this;

                poolObj.Initialize(poolManager);
                poolObj.ActiveObj();
            }
            returnValue = newObj.GetComponent<PoolingClass>();
        }
        
        // 성공시 활성화된 오브젝트 플러스
        if(returnValue != null)
        {
            activeObjCount++;
			returnValue.gameObject.transform.position = (Vector3)startPos;
			returnValue.gameObject.transform.rotation = (Quaternion)startRot;
			activedPoolingObjects.Add(returnValue);
		}
        return returnValue;
    }

    public void DeactiveObject(PoolingClass target)
    {
        activeObjCount--;

        PoolingObject poolObj;
        target.TryGetComponent(out poolObj);

        // PoolingObject일 경우
        if (poolObj != null)
        {
            poolObj.DeactiveObj();
        }
        target.gameObject.SetActive(false);
		activedPoolingObjects.Remove(target);
		nonActivedObjects.Push(target);
    }
}