using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelect : MonoBehaviour
{
    public GameObject controlsText;

    [Header ("Cars")]
    public GameObject _fullGripCar, _HalfGripCar, _DriftyRearCar, _heavyRaceSpec;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _fullGripCar.SetActive(true);
            _HalfGripCar.SetActive(false);
            _DriftyRearCar.SetActive(false);
            _heavyRaceSpec.SetActive(false);
            Debug.Log("Selected Full Grip Car");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _fullGripCar.SetActive(false);
            _HalfGripCar.SetActive(true);
            _DriftyRearCar.SetActive(false);
            _heavyRaceSpec.SetActive(false);
            Debug.Log("Selected Half Grip Car");
        }


        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _fullGripCar.SetActive(false);
            _HalfGripCar.SetActive(false);
            _DriftyRearCar.SetActive(true);
            _heavyRaceSpec.SetActive(false);
            Debug.Log("Selected Drifty Rear Car");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _fullGripCar.SetActive(false);
            _HalfGripCar.SetActive(false);
            _DriftyRearCar.SetActive(false);
            _heavyRaceSpec.SetActive(true);
            Debug.Log("Selected Heavy Race Spec");
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!controlsText.activeSelf)
            {
                controlsText.SetActive(true);
            }
            else
            {
                controlsText.SetActive(false);
            }
        }
    }
}
